# Prim

<style>
t { color: #4EC9B0 } /* type */
f { color: #DCDCAA } /* function */
v { color: #9CDCFE } /* variable */
k { color: #869CD6 } /* keyword */
c { color: #D8A0DF } /* control keyword */
a { color: #9A9A9A } /* argument / parameter */
s { color: #FFD68f; text-decoration: none } /* string */
n { color: #B5CEA8 } /* number */
</style>


### Immutable variables
> <k>let</k> <v>a</v>: <t>str</t> = <s>"text"</s>;

### Mutable variables
> <k>var</k> <v>a</v>: <t>i32</t> = <n>29</n>;

### Functions
> <k>let</k> <v>add</v>: (<a>a</a>: <t>i32</t>, <a>b</a>: <t>i32</t>) => <t>i32</t> = <a>a</a> + <a>b</a>;

> <k>let</k> <v>f</v>: (<a>name</a>: <t>str</t>) => <t>void</t> = {  
&nbsp;&nbsp;&nbsp;&nbsp;<f>writeLine</f>(<a>name</a>);  
};

### Builtin Types
| name         | description               |
| -----------: | :------------------------ |
| <t>any</t>   | top type                  |
| <t>never</t> | bottom type               |
| <t>void</t>  | void type (no return)     |
| <t>type</t>  | represents a type         |
||
| <t>bool</t>  | boolean true or false     |
||
| <t>i8</t>    | signed 8 bit integer      |
| <t>i16</t>   | signed 16 bit integer     |
| <t>i32</t>   | signed 32 bit integer     |
| <t>i64</t>   | signed 64 bit integer     |
||
| <t>u8</t>    | unsigned 8 bit integer    |
| <t>u16</t>   | unsigned 16 bit integer   |
| <t>u32</t>   | unsigned 32 bit integer   |
| <t>u64</t>   | unsigned 64 bit integer   |
|||
| <t>f32</t>   | IEEE 754 single precision |
| <t>f64</t>   | IEEE 754 double precision |
|||
| <t>str</t>   | sequence of code units    |