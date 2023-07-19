# Prim

### Immutable variables
```
let a: str = "text";
```

### Mutable variables
```
var a: i32 = 29;
```

### Functions
```
let add: (a: i32, b: i32) => i32 = a + b;
```

```
let f: (name: str) => void = {
    writeLine(name);  
}
```

### Built-In Types
| name         | description               |
| -----------: | :------------------------ |
| any   | top type                  |
| never | bottom type               |
| void  | void type (no return)     |
| type  | represents a type         |
||
| bool  | boolean true or false     |
||
| i8    | signed 8 bit integer      |
| i16   | signed 16 bit integer     |
| i32   | signed 32 bit integer     |
| i64   | signed 64 bit integer     |
||
| u8    | unsigned 8 bit integer    |
| u16   | unsigned 16 bit integer   |
| u32   | unsigned 32 bit integer   |
| u64   | unsigned 64 bit integer   |
|||
| f32   | IEEE 754 single precision |
| f64   | IEEE 754 double precision |
|||
| str   | sequence of code units    |