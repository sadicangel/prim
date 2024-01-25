# Prim

### Variables
```rust
immutable: str = "text";
mutable: mutable i32 = 29;
```

### Functions
```ts
add: (a: i32, b: i32) -> i32 = a + b;
print: (name: str) -> unit = {
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