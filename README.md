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

### Functions
- Functions are defined with a signature that includes argument names, types, and a return type.
- The syntax uses -> to indicate the return type.
- Functions can either have a block of code with a return statement or a single expression body.
```ts
sum: (a: i32, b: i32) -> i32 = { return a + b; } // block body with
double: (x: i32) -> i32 = x * 2; // single-expression body
```
- Functions as first-class values, meaning they can be assigned to variables, be passed as arguments to other functions or returned from them.

### Structs
- The language supports user-defined structured types (struct) for grouping related fields.
- Struct fields are defined with explicit types, and instances are created using literal syntax.
```ts
Point2D: struct = { x: f32; y: f32; } // defining a 2D point structure
point := Point2D { .x = 0, .y = 1 }; // creating an instance with literal field assignments
```
### Arrays
- Arrays are fixed-length. This is specified using the syntax `[Type: size]`, meaning the array has a predefined length that cannot be changed after initialization.
- Arrays are initialized using square brackets ([]) containing a list of values.
```ts
array : [i32: 2] = [1, 2]; //  array of two i32 values, initialized with the values 1 and 2.
array := ["str1", "str2", "str3"]; // type inferred from the assigned value - [str: 3]
```
- The type and length are both part of the array's signature. For example, `[i32: 2]` and `[i32: 3]` are considered different types and cannot be used interchangeably.

- Since arrays have fixed lengths, the compiler can enforce bounds checking both at compile-time and runtime.

### Pointers  
> _TODO_

## Previous Versions

- [v0.1](https://github.com/sadicangel/prim/tree/v0.1)
- [v0.2](https://github.com/sadicangel/prim/tree/v0.2)

## References
- [Crafting Interpreters](https://craftinginterpreters.com/)
- [Building a Compiler](https://www.youtube.com/playlist?list=PLRAdsfhKI4OWNOSfS7EUu5GRAVmze1t2y)