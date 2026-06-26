# Prim

<img src=icon.png width=128></img>  
Prim is a (re)learning experience about programming languages and compilers.
The language attempts a balance between readability and strict type safety.

Language Design Overview:
- Statically Typed: Types are checked at compile-time, and types must either be declared or inferred.
- Strong Typing: Variables are bound to specific types, and implicit type conversion is likely minimal.
- Modern Syntax: Use of concise function definitions (like lambdas) and inferred types.
- Structured Types: Supports defining complex types via structs, allowing for object-like behavior.

Compiler includes the typical stages of static compilation:
- Parsing source files into syntax trees.
- Performing binding (semantic analysis) on the parsed syntax.
- Collecting and reporting diagnostics.
- Handling incremental compilation by reusing information from previous compilations.

The compiler is written in C# and there is also a REPL (running on a C# interpreter for the language).

## Key Features
### Type System:

- The language uses explicit typing (e.g., i32 for 32-bit integers, f32 for 32-bit floats) and supports type inference where the type is deduced from the assigned value.
- Primitive types include:
  - signed integers: i8, i16, i32 ,i64, isz
  - unsigned integers: u8, u16, u32 ,u64, usz
  - floating-point numbers: f16, f32, f64
  - strings: str
  - booleans: bool
  - other: any, never, unit
  - unions
  - options
  - arrays
- Types are known and enforced at compile time.

### Unified Declaration and Assignment Syntax:
- The pattern for declaring and assigning variables, functions, and structs is uniform, following the structure:
```ts
name : type = value;
```
- `type` can be inferred by the compiler:
```ts
name := "this is a string" // type "str" is inferred for name
```

### Variables:
- Variables can be declared with explicit types or inferred types.
```ts
a : i32 = 29;  // explicit type i32 assigned to a
b := "a string"; // type inferred from the assigned value - str
```

#### Mutability:
```ts
a :: "cannot reassign a"; // immutable
b := "can reassign b"; // mutable
```

#### Functions
- Functions are just variables with a lamda type.
```ts
sum : (i32, i32) -> i32 = (a, b) -> { return a + b; } // block body
double : (i32) -> i32 = x -> x * 2; // single-expression body
double := (x: i32) -> x * 2; // type inference (**not yet supported**)
```
- Functions as first-class values, meaning they can be assigned to variables, be passed as arguments to other functions or returned from them.

### Arrays
- Arrays are fixed-length. This is specified using the syntax `Type[size]`, meaning the array has a predefined length that cannot be changed after initialization.
- Arrays are initialized using square brackets ([]) containing a list of values.
```ts
array := [1, 2]; // array of two i32 values, initialized with the values 1 and 2. 
// inferred type is i32[2] though this is assignable to i32[] (**not yet supported**)
```
When length is supported in arrays we can have:
- The type and length are both part of the array's signature. For example, `i32[2]` and `i32[3]` are considered different types and cannot be used interchangeably.
- Since arrays have fixed lengths, the compiler can enforce bounds checking both at compile-time and runtime.

### Pointers  
```ts
a : i32 = 42; // a pointer to the variable b of type i32
ptr : i32* = &a;
ptr := &a; // type inference
b := *ptr; // dereferencing the pointer to get the value of a*
```

### Unions
- A union represents a value type that can be one of multiple types
- Appending `?` is short hand for a union of that type with the unit type.
```ts
i32OrStr: i32 | str = "value";
i32OrStr = 29;

a : i32? = null;
a = 29;
```

### Structs
- The language supports user-defined structured types for grouping related fields.
```ts
Point2D :: type { x: f32; y: f32; } // defining a 2D point structure
point := Point2D { x = 0, y = 1 }; // creating an instance with literal field assignments
```
### Modules
- Definitions.
- Imports (**not yet supported**)
```ts
math :: module;
Point2D :: type { x: f32; y: f32; } // defining a 2D point structure
point := Point2D { x = 0, y = 1 }; // creating an instance with literal field assignments
```
In another file it would look like this:
```ts
import math // as M
point := math.Point2D { x = 0, y = 1 }; 
```

### Control flow
- Most constructs in the language are expressions.

#### if-else
```ts
a :: if (x > 0) "greater than 0" else "less than or equal to 0"; // type of a is str
b :: if (x > 0) "greater than 0"; // type of b is str | unit
```

#### while
```ts
h := 0;
a :: while (h <= 10) {
  if (flipCoin() == "tails") break false;
  h += 1;
  continue true;
}
```

## TODO
- Drop parenthesis on if and while. We can most likely know where the expression ends.
- Add for
- Add defer

## Previous Versions

- [v0.1](https://github.com/sadicangel/prim/tree/v0.1)
- [v0.2](https://github.com/sadicangel/prim/tree/v0.2)
- [v0.3](https://github.com/sadicangel/prim/tree/v0.3)

## References
- [Crafting Interpreters](https://craftinginterpreters.com/)
- [Building a Compiler](https://www.youtube.com/playlist?list=PLRAdsfhKI4OWNOSfS7EUu5GRAVmze1t2y)