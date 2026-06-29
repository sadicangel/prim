using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Semantic;

internal sealed class Binder(IScope? scope = null) : IDiagnosticReporter
{
    private readonly Dictionary<Symbol, INode> _bindings = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<DeclarationSyntax, Symbol> _declarations = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<SyntaxTree> _declaredTrees = [];
    private readonly Dictionary<SyntaxTree, ModuleSymbol> _treeModules = [];
    private readonly List<Diagnostic> _diagnostics = [];

    private IScope _scope = scope ?? new ModuleScope(SymbolFactory.CreateGlobalModule());

    public ModuleSymbol GlobalModule => _scope.Module.Global;

    public void CacheBinding(Symbol symbol, INode node) => _bindings[symbol] = node;

    public IEnumerable<Diagnostic> GetDiagnostics() => _diagnostics;

    public void Report(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);

    public ImmutableArray<DeclarationNode> Bind(ImmutableArray<SyntaxTree> syntaxTrees)
    {
        Declare(syntaxTrees);

        var declarations = ImmutableArray.CreateBuilder<DeclarationNode>();
        foreach (var syntaxTree in syntaxTrees)
            declarations.AddRange(BindDeclared(syntaxTree));
        return declarations.ToImmutable();
    }

    public ImmutableArray<DeclarationNode> Bind(SyntaxTree syntaxTree) => Bind([syntaxTree]);

    private void Declare(ImmutableArray<SyntaxTree> syntaxTrees)
    {
        var pending = syntaxTrees.Where(tree => !_declaredTrees.Contains(tree)).ToImmutableArray();
        if (pending.Length == 0) return;

        foreach (var syntaxTree in pending)
            ResolveModule(syntaxTree);

        foreach (var syntaxTree in pending)
            DeclareTypeHeaders(syntaxTree);

        foreach (var syntaxTree in pending)
            DeclareTypeMembers(syntaxTree);

        foreach (var syntaxTree in pending)
            DeclareVariableHeaders(syntaxTree);

        foreach (var syntaxTree in pending)
            _declaredTrees.Add(syntaxTree);
    }

    private void ResolveModule(SyntaxTree syntaxTree)
    {
        var module = GlobalModule;
        var declarations = syntaxTree.CompilationUnit.Declarations;

        for (var i = 0; i < declarations.Count; i++)
        {
            var declaration = declarations[i];
            if (IsModuleDeclaration(declaration))
            {
                if (i == 0)
                {
                    var resolvedModule = ResolveOrDeclareModulePath(declaration);
                    if (resolvedModule is not null)
                    {
                        module = resolvedModule;
                        _declarations[declaration] = module;
                    }
                }
                else
                {
                    Report(Diagnostic.InvalidModuleDeclaration(declaration.Name.SourceSpan));
                }

                continue;
            }

            if (declaration.Name is QualifiedNameSyntax)
                Report(Diagnostic.InvalidQualifiedDeclarationName(declaration.Name.SourceSpan));
        }

        _treeModules[syntaxTree] = module;
    }

    private ModuleSymbol? ResolveOrDeclareModulePath(GlobalDeclarationSyntax syntax)
    {
        var module = GlobalModule;
        var pathParts = new List<string>();

        foreach (var partText in syntax.Name.Name)
        {
            var part = new NameString(partText);
            pathParts.Add(partText);
            var existing = module.Lookup(part);
            if (existing is ModuleSymbol existingModule)
            {
                module = existingModule;
                continue;
            }

            if (existing is not null)
            {
                Report(Diagnostic.InvalidModulePath(syntax.Name.SourceSpan, string.Join(SyntaxFacts.NameSeparator, pathParts)));
                return null;
            }

            var childModule = new ModuleSymbol(part, module, syntax);
            if (!module.Declare(childModule))
            {
                Report(Diagnostic.SymbolRedeclaration(syntax.Name.SourceSpan, part.ToString()));
                return null;
            }

            module = childModule;
        }

        return module;
    }

    private void DeclareTypeHeaders(SyntaxTree syntaxTree)
    {
        var module = _treeModules[syntaxTree];
        foreach (var declaration in syntaxTree.CompilationUnit.Declarations)
        {
            if (IsModuleDeclaration(declaration) || declaration.Name is QualifiedNameSyntax)
                continue;
            if (declaration.Initializer is not TypeInitializerExpressionSyntax)
                continue;

            var type = new StructTypeSymbol(declaration.Name.Name.Name, module, declaration);
            if (!module.Declare(type))
            {
                Report(Diagnostic.SymbolRedeclaration(declaration.Name.SourceSpan, type.Name.ToString()));
                continue;
            }

            _declarations[declaration] = type;
        }
    }

    private void DeclareTypeMembers(SyntaxTree syntaxTree)
    {
        foreach (var declaration in syntaxTree.CompilationUnit.Declarations)
        {
            if (!_declarations.TryGetValue(declaration, out var symbol) ||
                symbol is not StructTypeSymbol type ||
                declaration.Initializer is not TypeInitializerExpressionSyntax initializer)
            {
                continue;
            }

            foreach (var propertySyntax in initializer.Properties)
            {
                var propertyType = propertySyntax.Type is not null
                    ? BindTypeInModule(type.ContainingModule, propertySyntax.Type)
                    : type.ContainingModule.UnknownType;
                var property = new PropertySymbol(
                    propertySyntax.Name.Name.Name,
                    propertyType,
                    type,
                    propertySyntax.IsStatic,
                    propertySyntax.IsReadOnly,
                    propertySyntax);

                if (!type.Declare(property))
                {
                    Report(Diagnostic.SymbolRedeclaration(propertySyntax.Name.SourceSpan, property.Name.ToString()));
                    continue;
                }

                _declarations[propertySyntax] = property;
            }
        }
    }

    private void DeclareVariableHeaders(SyntaxTree syntaxTree)
    {
        var module = _treeModules[syntaxTree];
        foreach (var declaration in syntaxTree.CompilationUnit.Declarations)
        {
            if (IsModuleDeclaration(declaration) || declaration.Name is QualifiedNameSyntax || declaration.Initializer is TypeInitializerExpressionSyntax)
                continue;

            var variableType = declaration.Type is not null
                ? BindTypeInModule(module, declaration.Type)
                : module.UnknownType;
            var variable = new VariableSymbol(
                declaration.Name.Name.Name,
                variableType,
                module,
                declaration.IsStatic,
                declaration.IsReadOnly,
                declaration);

            if (!module.Declare(variable))
            {
                Report(Diagnostic.SymbolRedeclaration(declaration.Name.SourceSpan, variable.Name.ToString()));
                continue;
            }

            _declarations[declaration] = variable;
        }
    }

    private ImmutableArray<DeclarationNode> BindDeclared(SyntaxTree syntaxTree)
    {
        var declarations = ImmutableArray.CreateBuilder<DeclarationNode>();
        using (PushScope(new ModuleScope(_treeModules[syntaxTree])))
        {
            foreach (var syntax in syntaxTree.CompilationUnit.Declarations)
            {
                if (_declarations.ContainsKey(syntax))
                    declarations.Add(BindGlobalDeclaration(syntax));
            }
        }

        return declarations.ToImmutable();
    }

    private TypeSymbol BindTypeInModule(ModuleSymbol module, TypeSyntax syntax)
    {
        using (PushScope(new ModuleScope(module)))
            return BindType(syntax);
    }

    private static bool IsModuleDeclaration(GlobalDeclarationSyntax syntax) => syntax.Initializer is ModuleInitializerExpressionSyntax;

    public INode Bind(SyntaxNode syntax) => syntax switch
    {
        DeclarationSyntax declaration => BindDeclaration(declaration),
        StatementSyntax statement => BindStatement(statement),
        ExpressionSyntax expression => BindExpression(expression),
        _ => throw new InvalidOperationException($"Unexpected syntax '{syntax?.GetType().Name}'")
    };

    public IStatementNode BindStatement(StatementSyntax syntax) => syntax switch
    {
        DeclarationSyntax declaration => BindDeclaration(declaration),
        ExpressionStatementSyntax statement => BindExpressionStatement(statement),
        EmptyStatementSyntax statement => BindEmptyStatement(statement),
        _ => throw new InvalidOperationException($"Unexpected statement '{syntax?.GetType().Name}'")
    };

    public DeclarationNode BindDeclaration(DeclarationSyntax syntax) => syntax switch
    {
        GlobalDeclarationSyntax declaration => BindGlobalDeclaration(declaration),
        LocalDeclarationSyntax declaration => BindLocalDeclaration(declaration),
        _ => throw new InvalidOperationException($"Unexpected declaration '{syntax?.GetType().Name}'")
    };

    public IExpressionNode BindExpression(ExpressionSyntax syntax) => syntax switch
    {
        BlockExpressionSyntax expression => BindBlockExpression(expression),
        IfElseExpressionSyntax expression => BindIfElseExpression(expression),
        WhileExpressionSyntax expression => BindWhileExpression(expression),
        ContinueExpressionSyntax expression => BindContinueExpression(expression),
        BreakExpressionSyntax expression => BindBreakExpression(expression),
        ReturnExpressionSyntax expression => BindReturnExpression(expression),
        ModuleInitializerExpressionSyntax expression => BindModuleInitializerExpression(expression),
        TypeInitializerExpressionSyntax expression => BindTypeInitializerExpression(expression),
        ObjectInitializerExpressionSyntax expression => BindObjectInitializerExpression(expression),
        ArrayInitializerExpressionSyntax expression => BindArrayInitializerExpression(expression),
        PropertyInitializerExpressionSyntax expression => BindPropertyInitializerExpression(expression),
        ElementAccessExpressionSyntax expression => BindElementAccessExpression(expression),
        MemberAccessExpressionSyntax expression => BindMemberAccessExpression(expression),
        ConversionExpressionSyntax expression => BindConversionExpression(expression),
        LambdaExpressionSyntax expression => BindLambdaExpression(expression),
        InvocationExpressionSyntax expression => BindInvocationExpression(expression),
        GroupExpressionSyntax expression => BindGroupExpression(expression),
        UnaryExpressionSyntax expression => BindUnaryExpression(expression),
        BinaryExpressionSyntax expression => BindBinaryExpression(expression),
        AssignmentExpressionSyntax expression => BindAssignmentExpression(expression),
        NameExpressionSyntax expression => BindNameExpression(expression),
        LiteralExpressionSyntax expression => BindLiteralExpression(expression),
        _ => throw new InvalidOperationException($"Unexpected expression '{syntax?.GetType().Name}'")
    };

    public IExpressionNode BindConversion(IExpressionNode expression, TypeSymbol type)
    {
        if (expression.Type.IsNever || type.IsUnknown || type.IsAny || expression.Type == type) return expression;
        if (type.IsNever) return CreateNeverExpression(expression.Syntax);
        return expression;
    }

    public DeclarationNode BindGlobalDeclaration(GlobalDeclarationSyntax syntax)
    {
        if (!_declarations.TryGetValue(syntax, out var symbol))
            throw new InvalidOperationException($"Global declaration '{syntax.Name.Name}' was not declared before binding.");

        var initializer = symbol switch
        {
            ModuleSymbol => BindModuleInitializerExpression((ModuleInitializerExpressionSyntax)syntax.Initializer),
            StructTypeSymbol type => BindTypeInitializerExpression((TypeInitializerExpressionSyntax)syntax.Initializer, type),
            VariableSymbol variable => BindInitializer(syntax.Initializer, variable) ?? CreateLiteralUnitExpression(syntax),
            _ => CreateNeverExpression(syntax)
        };

        CacheBinding(symbol, initializer);
        return new DeclarationNode(syntax, symbol, initializer);
    }

    public DeclarationNode BindLocalDeclaration(LocalDeclarationSyntax syntax)
    {
        if (syntax.Initializer is ModuleInitializerExpressionSyntax or TypeInitializerExpressionSyntax)
        {
            Report(Diagnostic.InvalidLocationForTypeDefinition(syntax.Initializer.SourceSpan));
            return new DeclarationNode(syntax, new VariableSymbol(syntax.Name.Name, _scope.Module.NeverType, _scope.Module, syntax.IsStatic, syntax.IsReadOnly, syntax), CreateNeverExpression(syntax));
        }

        var type = syntax.Type is not null ? BindType(syntax.Type) : _scope.Module.UnknownType;
        var symbol = new VariableSymbol(syntax.Name.Name, type, _scope.Module, syntax.IsStatic, syntax.IsReadOnly, syntax);
        if (!_scope.Declare(symbol)) Report(Diagnostic.SymbolRedeclaration(syntax.Name.SourceSpan, symbol.Name.ToString()));
        var initializer = BindInitializer(syntax.Initializer, symbol);

        return new DeclarationNode(syntax, symbol, initializer);
    }

    private DeclarationNode BindPropertyDeclaration(LocalDeclarationSyntax syntax, PropertySymbol property)
    {
        var initializer = BindInitializer(syntax.Initializer, property);
        return new DeclarationNode(syntax, property, initializer);
    }

    private IExpressionNode? BindInitializer(ExpressionSyntax? syntax, Symbol symbol)
    {
        if (syntax is null) return null;

        var expression = BindExpressionWithTarget(syntax, symbol.Type);
        if (symbol.Type.IsUnknown && !expression.Type.IsUnknown && !expression.Type.IsNever)
            SetType(symbol, expression.Type);

        CacheBinding(symbol, expression);
        return expression;
    }

    private IExpressionNode BindExpressionWithTarget(ExpressionSyntax syntax, TypeSymbol targetType)
    {
        if (syntax is LambdaExpressionSyntax && targetType is LambdaTypeSymbol lambdaType)
        {
            using (PushScope(new LambdaScope(_scope, lambdaType)))
                return BindExpression(syntax);
        }

        if (syntax is LambdaExpressionSyntax && targetType.IsUnknown)
        {
            Report(Diagnostic.InvalidImplicitType(syntax.SourceSpan, targetType.Name.ToString()));
            return CreateNeverExpression(syntax);
        }

        return BindConversion(BindExpression(syntax), targetType);
    }

    public IStatementNode BindExpressionStatement(ExpressionStatementSyntax syntax)
    {
        var expression = BindExpression(syntax.Expression);
        return new ExpressionStatementNode(syntax, expression);
    }

    public IStatementNode BindEmptyStatement(EmptyStatementSyntax syntax) =>
        new ExpressionStatementNode(syntax, new NopExpressionNode(syntax, _scope.Module.UnitType));

    public IExpressionNode BindBlockExpression(BlockExpressionSyntax syntax)
    {
        using (PushScope(new BlockScope(_scope)))
        {
            var items = syntax.Items.Select(Bind).ToImmutableArray();
            var type = (items.LastOrDefault() as IExpressionNode)?.Type ?? _scope.Module.UnitType;
            return new BlockExpressionNode(syntax, type, items);
        }
    }

    public IExpressionNode BindIfElseExpression(IfElseExpressionSyntax syntax)
    {
        var condition = BindConversion(BindExpression(syntax.Condition), _scope.Module.BoolType);
        if (condition.Type.IsNever) return CreateNeverExpression(syntax);

        var then = BindExpression(syntax.Then);
        var @else = syntax.ElseClause is null ? null : BindExpression(syntax.ElseClause.Else);
        var type = new TypeSymbolBuilder(_scope.Module).Add(then.Type, @else?.Type ?? _scope.Module.UnitType).Build();
        return new IfElseExpressionNode(syntax, type, condition, then, @else);
    }

    public IExpressionNode BindWhileExpression(WhileExpressionSyntax syntax)
    {
        var condition = BindConversion(BindExpression(syntax.Condition), _scope.Module.BoolType);
        if (condition.Type.IsNever) return CreateNeverExpression(syntax);
        using (PushScope(new LoopScope(_scope)))
        {
            var body = BindExpression(syntax.Body);
            return new WhileExpressionNode(syntax, condition, body);
        }
    }

    private T? GetEnclosingScope<T>() where T : class, IScope
    {
        for (var current = _scope; current is not null; current = current.Parent)
            if (current is T scope)
                return scope;

        return null;
    }

    public IExpressionNode BindContinueExpression(ContinueExpressionSyntax syntax)
    {
        if (GetEnclosingScope<LoopScope>() is null)
        {
            Report(Diagnostic.InvalidBreakOrContinue(syntax.SourceSpan));
            return CreateNeverExpression(syntax);
        }

        var expression = syntax.Expression is not null ? BindExpression(syntax.Expression) : CreateLiteralUnitExpression();
        return new ContinueExpressionNode(syntax, expression);
    }

    public IExpressionNode BindBreakExpression(BreakExpressionSyntax syntax)
    {
        if (GetEnclosingScope<LoopScope>() is null)
        {
            Report(Diagnostic.InvalidBreakOrContinue(syntax.SourceSpan));
            return CreateNeverExpression(syntax);
        }

        var expression = syntax.Expression is not null ? BindExpression(syntax.Expression) : CreateLiteralUnitExpression();
        return new BreakExpressionNode(syntax, expression);
    }

    public IExpressionNode BindReturnExpression(ReturnExpressionSyntax syntax)
    {
        var lambdaScope = GetEnclosingScope<LambdaScope>();
        if (lambdaScope is null)
        {
            Report(Diagnostic.InvalidReturn(syntax.SourceSpan));
            return CreateNeverExpression(syntax);
        }

        var expression = syntax.Expression is not null ? BindExpression(syntax.Expression) : CreateLiteralUnitExpression();
        expression = BindConversion(expression, lambdaScope.LambdaType.ReturnType);
        return new ReturnExpressionNode(syntax, expression);
    }

    public IExpressionNode BindModuleInitializerExpression(ModuleInitializerExpressionSyntax syntax) =>
        new ModuleInitializerExpressionNode(syntax, _scope.Module.ModuleType);

    public IExpressionNode BindTypeInitializerExpression(TypeInitializerExpressionSyntax syntax) =>
        new TypeInitializerExpressionNode(syntax, _scope.Module.TypeType, []);

    private IExpressionNode BindTypeInitializerExpression(TypeInitializerExpressionSyntax syntax, StructTypeSymbol type)
    {
        var properties = ImmutableArray.CreateBuilder<DeclarationNode>();
        foreach (var propertySyntax in syntax.Properties)
        {
            if (_declarations.TryGetValue(propertySyntax, out var property) && property is PropertySymbol propertySymbol)
                properties.Add(BindPropertyDeclaration(propertySyntax, propertySymbol));
        }

        return new TypeInitializerExpressionNode(syntax, _scope.Module.TypeType, properties.ToImmutable());
    }

    public IExpressionNode BindObjectInitializerExpression(ObjectInitializerExpressionSyntax syntax)
    {
        var expression = BindExpression(syntax.TypeName);
        if (expression is not TypeReferenceNode { Symbol: StructTypeSymbol } typeReference)
        {
            Report(Diagnostic.UndefinedType(syntax.TypeName.SourceSpan, syntax.TypeName.ToString() ?? string.Empty));
            return CreateNeverExpression(syntax);
        }

        using (PushScope(new InstanceScope(_scope, typeReference.Symbol)))
        {
            var properties = ImmutableArray.CreateBuilder<PropertyInitializerExpressionNode>();
            foreach (var propertySyntax in syntax.Properties)
            {
                if (BindPropertyInitializerExpression(propertySyntax) is not PropertyInitializerExpressionNode property)
                    return CreateNeverExpression(syntax);
                properties.Add(property);
            }
            return new ObjectInitializerExpressionNode(syntax, typeReference, properties.ToImmutable());
        }
    }

    public IExpressionNode BindPropertyInitializerExpression(PropertyInitializerExpressionSyntax syntax)
    {
        var lookup = _scope.Lookup(syntax.PropertyName.Name);
        if (lookup.Kind == LookupResultKind.Ambiguous)
            return ReportAmbiguousSymbol(syntax, syntax.PropertyName.Name);

        if (lookup.Symbol is not PropertySymbol property)
        {
            Report(Diagnostic.UndefinedTypeMember(syntax.PropertyName.SourceSpan, _scope.Module.Name.ToString(), syntax.PropertyName.Name.ToString()));
            return CreateNeverExpression(syntax);
        }
        if (property.IsStatic)
        {
            Report(Diagnostic.InvalidAssignment(syntax.SourceSpan));
            return CreateNeverExpression(syntax);
        }
        if (property.IsReadOnly)
        {
            Report(Diagnostic.ReadOnlyAssignment(syntax.SourceSpan, property.Name.ToString()));
            return CreateNeverExpression(syntax);
        }
        var value = BindConversion(BindExpression(syntax.PropertyValue), property.Type);
        if (value.Type.IsNever)
            return CreateNeverExpression(syntax);
        return new PropertyInitializerExpressionNode(syntax, property, value);
    }

    public IExpressionNode BindArrayInitializerExpression(ArrayInitializerExpressionSyntax syntax)
    {
        var elements = syntax.Elements.Select(BindExpression).ToImmutableArray();
        var elementType = new TypeSymbolBuilder(_scope.Module).AddRange(elements.Select(x => x.Type)).Build();
        var type = new ArrayTypeSymbol(elementType, elements.Length, syntax);
        return new ArrayInitializerExpressionNode(syntax, type, elements);
    }

    public IExpressionNode BindElementAccessExpression(ElementAccessExpressionSyntax syntax)
    {
        var receiver = BindExpression(syntax.Receiver);
        var index = BindExpression(syntax.Index);

        if (receiver.Type is ArrayTypeSymbol arrayType)
        {
            index = BindConversion(index, _scope.Module.I32Type);
            if (index.Type.IsNever) return CreateNeverExpression(syntax);
            var indexer = new IndexerSymbol(OperatorKind.Index.GetName(_scope.Module.I32Type), arrayType.ElementType, [_scope.Module.I32Type], arrayType, false, false, syntax);
            return new ElementReferenceNode(syntax, indexer, receiver, index);
        }

        var operatorName = OperatorKind.Index.GetName(index.Type);
        var declaredIndexer = receiver.Type.Lookup<IndexerSymbol>(operatorName);
        if (declaredIndexer is null)
        {
            Report(Diagnostic.UndefinedIndexOperator(syntax.SourceSpan, receiver.Type.Name.ToString()));
            return CreateNeverExpression(syntax);
        }

        return new ElementReferenceNode(syntax, declaredIndexer, receiver, index);
    }

    public IExpressionNode BindMemberAccessExpression(MemberAccessExpressionSyntax syntax)
    {
        if (BindQualifiedMemberAccessExpression(syntax) is { } qualifiedExpression)
            return qualifiedExpression;

        var receiver = BindExpression(syntax.Receiver);
        var memberName = syntax.MemberName.Name;
        var member = LookupMember(receiver, memberName);

        if (member is null)
        {
            Report(Diagnostic.UndefinedTypeMember(syntax.MemberName.SourceSpan, receiver.Type.Name.ToString(), memberName.ToString()));
            return CreateNeverExpression(syntax);
        }

        return BindSymbol(syntax, member, receiver);
    }

    private IExpressionNode? BindQualifiedMemberAccessExpression(MemberAccessExpressionSyntax syntax)
    {
        if (!TryGetMemberAccessNameParts(syntax, out var parts))
            return null;

        var firstLookup = _scope.Lookup(parts[0].Name);
        if (firstLookup.Kind == LookupResultKind.Found && IsValueReceiverSymbol(firstLookup.Symbol))
            return null;

        var qualifiedName = new NameString(parts.Select(part => part.Name.FullName));
        var lookup = _scope.Lookup(qualifiedName);
        return lookup.Kind switch
        {
            LookupResultKind.NotFound => null,
            LookupResultKind.Ambiguous => ReportAmbiguousSymbol(syntax, qualifiedName),
            LookupResultKind.Found => BindSymbol(syntax, lookup.Symbol!),
            _ => throw new InvalidOperationException($"Unknown lookup result '{lookup.Kind}'.")
        };
    }

    private IExpressionNode ReportAmbiguousSymbol(SyntaxNode syntax, NameString name)
    {
        Report(Diagnostic.AmbiguousSymbol(syntax.SourceSpan, name.ToString()));
        return CreateNeverExpression(syntax);
    }

    private static Symbol? LookupMember(IExpressionNode receiver, NameString memberName) => receiver switch
    {
        ModuleReferenceNode module => module.Symbol.Lookup(memberName),
        TypeReferenceNode type => type.Symbol.Lookup(memberName),
        _ => receiver.Type.Lookup<MemberSymbol>(memberName)
    };

    private static bool IsValueReceiverSymbol(Symbol? symbol) => symbol switch
    {
        VariableSymbol => true,
        MemberSymbol { IsStatic: false } => true,
        _ => false
    };

    private static bool TryGetMemberAccessNameParts(
        MemberAccessExpressionSyntax syntax,
        out List<(NameString Name, SourceSpan SourceSpan)> parts)
    {
        parts = [];
        var current = (ExpressionSyntax)syntax;
        while (current is MemberAccessExpressionSyntax memberAccess)
        {
            parts.Add((memberAccess.MemberName.Name, memberAccess.MemberName.SourceSpan));
            current = memberAccess.Receiver;
        }

        if (current is not NameExpressionSyntax { Name: SimpleNameSyntax firstName })
            return false;

        parts.Add((firstName.Name, firstName.SourceSpan));
        parts.Reverse();
        return parts.Count > 1;
    }

    public IExpressionNode BindConversionExpression(ConversionExpressionSyntax syntax)
    {
        var expression = BindExpression(syntax.Expression);
        var type = BindType(syntax.Type);
        return BindConversion(expression, type);
    }

    public IExpressionNode BindLambdaExpression(LambdaExpressionSyntax syntax)
    {
        if (_scope is not LambdaScope scope)
            throw new InvalidOperationException("Tried to bind lambda in wrong scope");

        if (scope.LambdaType.ParameterTypes.Length != syntax.Parameters.Count)
        {
            Report(
                Diagnostic.InvalidParameterCount(
                    SourceSpan.Union(syntax.ParenthesisOpenToken.SourceSpan, syntax.ParenthesisCloseToken.SourceSpan),
                    scope.LambdaType.ParameterTypes.Length,
                    syntax.Parameters.Count));

            return CreateNeverExpression(syntax);
        }

        var parameters = ImmutableArray.CreateBuilder<VariableSymbol>();
        foreach (var (parameterSyntax, parameterType) in syntax.Parameters.Zip(scope.LambdaType.ParameterTypes))
        {
            var parameter = new VariableSymbol(parameterSyntax.Name, parameterType, _scope.Module, false, true, parameterSyntax);
            if (!_scope.Declare(parameter))
            {
                Report(Diagnostic.SymbolRedeclaration(parameterSyntax.SourceSpan, parameter.Name.ToString()));
                return CreateNeverExpression(syntax);
            }
            parameters.Add(parameter);
        }

        var body = BindConversion(BindExpression(syntax.Body), scope.LambdaType.ReturnType);
        return new LambdaExpressionNode(syntax, scope.LambdaType, parameters.ToImmutable(), body);
    }

    public IExpressionNode BindInvocationExpression(InvocationExpressionSyntax syntax)
    {
        var callee = BindExpression(syntax.Callee);
        if (callee.Type is not LambdaTypeSymbol lambdaType)
        {
            Report(Diagnostic.UndefinedInvocationOperator(syntax.Callee.SourceSpan, callee.Type.Name.ToString()));
            return CreateNeverExpression(syntax);
        }

        if (lambdaType.ParameterTypes.Length != syntax.Arguments.Count)
        {
            Report(Diagnostic.ArgumentCountMismatch(syntax.SourceSpan, syntax.Arguments.Count));
            return CreateNeverExpression(syntax);
        }

        var arguments = ImmutableArray.CreateBuilder<IExpressionNode>();
        foreach (var (argumentSyntax, argumentType) in syntax.Arguments.Zip(lambdaType.ParameterTypes))
        {
            var argument = BindConversion(BindExpression(argumentSyntax), argumentType);
            if (argument.Type.IsNever) return CreateNeverExpression(syntax);
            arguments.Add(argument);
        }

        return new InvocationExpressionNode(syntax, lambdaType.ReturnType, callee, arguments.ToImmutable());
    }

    public IExpressionNode BindGroupExpression(GroupExpressionSyntax syntax) => BindExpression(syntax.Expression);

    public IExpressionNode BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var operand = BindExpression(syntax.Operand);
        var operatorKind = syntax.OperatorToken.Kind.GetOperatorKind(1);
        var operatorName = operatorKind.GetName(operand.Type);
        var @operator = operand.Type.Lookup<OperatorSymbol>(operatorName);
        if (@operator is null)
        {
            Report(Diagnostic.UndefinedUnaryOperator(syntax.OperatorToken, operand.Type.Name.ToString()));
            return CreateNeverExpression(syntax);
        }

        return new UnaryExpressionNode(syntax, @operator, operand);
    }

    public IExpressionNode BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var left = BindExpression(syntax.Left);
        var right = BindExpression(syntax.Right);
        var operatorKind = syntax.OperatorToken.Kind.GetOperatorKind(2);
        var operatorName = operatorKind.GetName(left.Type, right.Type);
        var @operator = left.Type.Lookup<OperatorSymbol>(operatorName) ?? right.Type.Lookup<OperatorSymbol>(operatorName);
        if (@operator is null)
        {
            Report(Diagnostic.UndefinedBinaryOperator(syntax.OperatorToken, left.Type.Name.ToString(), right.Type.Name.ToString()));
            return CreateNeverExpression(syntax);
        }

        return new BinaryExpressionNode(syntax, left, @operator, right);
    }

    public IExpressionNode BindAssignmentExpression(AssignmentExpressionSyntax syntax)
    {
        var expression = BindExpression(syntax.Reference);
        if (expression is not IReferenceNode reference)
        {
            Report(Diagnostic.InvalidAssignment(syntax.Reference.SourceSpan));
            return CreateNeverExpression(syntax);
        }
        if (reference.IsReadOnly)
        {
            Report(Diagnostic.ReadOnlyAssignment(syntax.Reference.SourceSpan, reference.Symbol.Name.ToString()));
            return CreateNeverExpression(syntax);
        }

        var value = BindConversion(BindExpression(syntax.Value), reference.Type);
        if (value.Type.IsNever) return CreateNeverExpression(syntax);
        return new AssignmentExpressionNode(syntax, reference, value);
    }

    public IExpressionNode BindNameExpression(NameExpressionSyntax syntax)
    {
        var lookup = _scope.Lookup(syntax.Name.Name);
        switch (lookup.Kind)
        {
            case LookupResultKind.NotFound:
                Report(Diagnostic.UndefinedSymbol(syntax.SourceSpan, syntax.Name.Name.ToString()));
                return CreateNeverExpression(syntax);
            case LookupResultKind.Ambiguous:
                return ReportAmbiguousSymbol(syntax, syntax.Name.Name);
            case LookupResultKind.Found:
                return BindSymbol(syntax, lookup.Symbol!);
            default:
                throw new InvalidOperationException($"Unknown lookup result '{lookup.Kind}'.");
        }
    }

    public IExpressionNode BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        return syntax.Kind switch
        {
            SyntaxKind.I8LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.I8Type, syntax.InstanceValue),
            SyntaxKind.U8LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.U8Type, syntax.InstanceValue),
            SyntaxKind.I16LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.I16Type, syntax.InstanceValue),
            SyntaxKind.U16LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.U16Type, syntax.InstanceValue),
            SyntaxKind.I32LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.I32Type, syntax.InstanceValue),
            SyntaxKind.U32LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.U32Type, syntax.InstanceValue),
            SyntaxKind.I64LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.I64Type, syntax.InstanceValue),
            SyntaxKind.U64LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.U64Type, syntax.InstanceValue),
            SyntaxKind.F16LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.F16Type, syntax.InstanceValue),
            SyntaxKind.F32LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.F32Type, syntax.InstanceValue),
            SyntaxKind.F64LiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.F64Type, syntax.InstanceValue),
            SyntaxKind.StrLiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.StrType, syntax.InstanceValue),
            SyntaxKind.TrueLiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.BoolType, syntax.InstanceValue),
            SyntaxKind.FalseLiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.BoolType, syntax.InstanceValue),
            SyntaxKind.NullLiteralExpression => new LiteralExpressionNode(syntax, _scope.Module.UnitType, syntax.InstanceValue),
            _ => CreateNeverExpression(syntax)
        };
    }

    public TypeSymbol BindType(TypeSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.AnyType: return _scope.Module.AnyType;
            case SyntaxKind.ErrType: return _scope.Module.ErrType;
            case SyntaxKind.UnknownType: return _scope.Module.UnknownType;
            case SyntaxKind.NeverType: return _scope.Module.NeverType;
            case SyntaxKind.UnitType: return _scope.Module.UnitType;
            case SyntaxKind.TypeType: return _scope.Module.TypeType;
            case SyntaxKind.StrType: return _scope.Module.StrType;
            case SyntaxKind.BoolType: return _scope.Module.BoolType;
            case SyntaxKind.I8Type: return _scope.Module.I8Type;
            case SyntaxKind.I16Type: return _scope.Module.I16Type;
            case SyntaxKind.I32Type: return _scope.Module.I32Type;
            case SyntaxKind.I64Type: return _scope.Module.I64Type;
            case SyntaxKind.IszType: return _scope.Module.IszType;
            case SyntaxKind.U8Type: return _scope.Module.U8Type;
            case SyntaxKind.U16Type: return _scope.Module.U16Type;
            case SyntaxKind.U32Type: return _scope.Module.U32Type;
            case SyntaxKind.U64Type: return _scope.Module.U64Type;
            case SyntaxKind.UszType: return _scope.Module.UszType;
            case SyntaxKind.F16Type: return _scope.Module.F16Type;
            case SyntaxKind.F32Type: return _scope.Module.F32Type;
            case SyntaxKind.F64Type: return _scope.Module.F64Type;
            case SyntaxKind.ArrayType:
                var arrayTypeSyntax = (ArrayTypeSyntax)syntax;
                var elementType = BindType(arrayTypeSyntax.ElementType);
                if (elementType.IsNever) return elementType;
                switch (arrayTypeSyntax.Length)
                {
                    case null:
                        return new ArrayTypeSymbol(elementType, null, syntax);
                    case LiteralExpressionSyntax { Kind: SyntaxKind.I64LiteralExpression } literal:
                        return new ArrayTypeSymbol(elementType, (long)literal.InstanceValue, syntax);
                    case LiteralExpressionSyntax { Kind: SyntaxKind.I32LiteralExpression } literal:
                        return new ArrayTypeSymbol(elementType, (int)literal.InstanceValue, syntax);
                    default:
                        Report(Diagnostic.InvalidArrayLength(arrayTypeSyntax.Length.SourceSpan));
                        return _scope.Module.NeverType;
                }
            case SyntaxKind.LambdaType:
                var lambdaTypeSyntax = (LambdaTypeSyntax)syntax;
                var parameterTypes = lambdaTypeSyntax.ParameterTypes.Select(BindType).ToImmutableArray();
                if (parameterTypes.Any(x => x.IsNever)) return _scope.Module.NeverType;
                var returnType = BindType(lambdaTypeSyntax.ReturnType);
                if (returnType.IsNever) return returnType;
                return new LambdaTypeSymbol(parameterTypes, returnType, syntax);
            case SyntaxKind.NamedType:
                var namedTypeSyntax = (NamedTypeSyntax)syntax;
                var lookup = _scope.Lookup(namedTypeSyntax.Name.Name);
                if (lookup.Kind == LookupResultKind.Ambiguous)
                {
                    Report(Diagnostic.AmbiguousSymbol(namedTypeSyntax.Name.SourceSpan, namedTypeSyntax.Name.Name.ToString()));
                    return _scope.Module.NeverType;
                }

                if (lookup.Symbol is not TypeSymbol namedType)
                {
                    Report(Diagnostic.UndefinedType(namedTypeSyntax.Name.SourceSpan, namedTypeSyntax.Name.Name.ToString()));
                    return _scope.Module.NeverType;
                }
                return namedType;
            case SyntaxKind.MaybeType:
                var maybeTypeSyntax = (MaybeTypeSyntax)syntax;
                var underlyingType = BindType(maybeTypeSyntax.UnderlyingType);
                if (underlyingType.IsNever) return underlyingType;
                return new UnionTypeSymbol([underlyingType, _scope.Module.UnitType], syntax);
            case SyntaxKind.PointerType:
                var pointerTypeSyntax = (PointerTypeSyntax)syntax;
                var baseType = BindType(pointerTypeSyntax.ElementType);
                if (baseType.IsNever) return baseType;
                return new PointerTypeSymbol(baseType, syntax);
            case SyntaxKind.UnionType:
                var unionTypeSyntax = (UnionTypeSyntax)syntax;
                var types = unionTypeSyntax.Types.Select(BindType).ToImmutableArray();
                var union = new TypeSymbolBuilder(_scope.Module.Global).AddRange(types).Build();
                if (union.IsNever) return union;
                if (union is not UnionTypeSymbol)
                {
                    Report(Diagnostic.InvalidExpressionType(syntax.SourceSpan, nameof(UnionTypeSymbol), union.Name.ToString()));
                    return _scope.Module.NeverType;
                }
                return union;
            default:
                return _scope.Module.NeverType;
        }
    }

    private IExpressionNode BindSymbol(SyntaxNode syntax, Symbol symbol, IExpressionNode? receiver = null)
    {
        return symbol switch
        {
            ModuleSymbol module => new ModuleReferenceNode(syntax, module),
            TypeSymbol type => new TypeReferenceNode(syntax, type),
            VariableSymbol variable => new VariableReferenceNode(syntax, variable),
            MemberSymbol member => BindMemberSymbol(syntax, member, receiver),
            _ => CreateNeverExpression(syntax)
        };

        IExpressionNode BindMemberSymbol(SyntaxNode syntax, MemberSymbol member, IExpressionNode? receiver)
        {
            if (member.IsStatic)
            {
                if (receiver is not null and not TypeReferenceNode and not ModuleReferenceNode)
                {
                    Report(Diagnostic.InvalidAssignment(syntax.SourceSpan));
                    return CreateNeverExpression(syntax);
                }

                receiver ??= new TypeReferenceNode(member.ContainingSymbol.Syntax ?? SyntaxToken.CreateSynthetic(SyntaxKind.TypeKeyword), member.ContainingSymbol.Type);
                return new MemberReferenceNode(syntax, member, receiver);
            }

            if (receiver is null or TypeReferenceNode or ModuleReferenceNode)
            {
                Report(Diagnostic.UndefinedTypeMember(syntax.SourceSpan, member.ContainingSymbol.Name.ToString(), member.Name.ToString()));
                return CreateNeverExpression(syntax);
            }

            return new MemberReferenceNode(syntax, member, receiver);
        }
    }

    public NeverExpressionNode CreateNeverExpression(SyntaxNode syntax) => new(syntax, _scope.Module.NeverType);

    public LiteralExpressionNode CreateLiteralUnitExpression(SyntaxNode? syntax = null) =>
        new(syntax ?? SyntaxToken.CreateSynthetic(SyntaxKind.NullLiteralExpression), _scope.Module.UnitType, Unit.Value);

    private ScopeFrame PushScope(IScope scope) => new(this, scope);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = $"set_{nameof(Symbol.Type)}")]
    private static extern void SetType(Symbol symbol, TypeSymbol type);

    private readonly ref struct ScopeFrame : IDisposable
    {
        private readonly Binder _binder;
        private readonly IScope _oldScope;

        public ScopeFrame(Binder binder, IScope newScope)
        {
            _binder = binder;
            _oldScope = binder._scope;
            _binder._scope = newScope;
        }

        public void Dispose() => _binder._scope = _oldScope;
    }
}
