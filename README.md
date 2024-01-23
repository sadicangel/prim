# Prim

### Variables
```ts
immutable: str = "text";
muttable: i32 = 29;
```

### Functions
```ts
add: (a: i32, b: i32) => i32 = a + b;
print: (name: str) => void = {
    greet: str = "Hello, " + name + "!";
    print(greet);  
}
```

### Types
```ts
Point2D: type = {
    x: i32 = 0;
    y: i32 = 0;

    add: (other: Point2D) -> Point2D = {
        x += other.x;
        y += other.y;
    }
}
```

### Built-In Types
| name  | description                               |
| ----: | :---------------------------------------- |
| any   | top type                                  |
| never | bottom type                               |
| void  | void type (no return)                     |
| type  | represents a type                         |
| bool  | boolean true or false                     |
| i#    | signed # bit integer (8, 16, 32, 64)      |
| u#    | unsigned # bit integer (8, 16, 32, 64)    |
| f32   | IEEE 754 single precision                 |
| f64   | IEEE 754 double precision                 |
| str   | sequence of code units (string)           |