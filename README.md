# pling
A smol programming language

```
pling REPL v.1.0
> ($e $s $f s:e ($e f 1+::e<(e _)(..)?) $_ _) $for
> (::1=;2=|(1-)(:1-fib;2-fib+)?) $fib
> (fib disp .) 1 15 for
0
1
1
2
3
5
8
13
21
34
55
89
144
233
```

## General Idea: add arguments to the stack and apply functions to them

```
> 4 4 + disp
8
```
1. add 2 4s to the evaluation stack
2. run add function which pops them both off and puts on an 8
3. run disp function, which displays the value on the top of the stack: 8

## Special Functions:
- `:` duplicate value on the evaluation stack
- `.` pop value off the evaluation stack
- `;` swap the top two values on the evaluation stack
- `?` perform an if statement of the form `condition op1 op2 ?`
- `$` preceding an identifier marks it as a variable definition, it pops off the top value on the evaluation stack and sets that as its value
- `()` parenthesis delay evaluation (contents are only evaluated when necessary)
- `#t` `#f` are true and false, like in scheme (lisp-ish language)
- `=` `<` `>` are for comparisons
- `+` `-` `*` `/` `%` are for general math 
that's pretty much it.

## Built-in Functions:
- `disp` displays value on the top of the stack
- `stack` displays all values on the stack
- `run` takes in a string and runs it as if it is code

Function examples:
- Factorial
```
> (:1=1(:1-fact)?*) $fact
> 5 fact disp
120
```
- FizzBuzz
```
> (:0=(disp)(::1-fizzbuzz.:15%0=('fizzbuzz' disp..)(:5%0=('buzz' disp..)(:3%0=('fizz' disp..)(disp.)?)?)?)?) $fizzbuzz
```
- Fibbonacci
```
> (::1=;2=|(1-)(:1-fib;2-fib+)?) $fib
```
- Recursive for-loop
```
> ($e $s $f s:e ($e f 1+::e<(e _)(..)?) $_ _) $for
> (disp .) 4 10 for
4
5
6
7
8
9
```

have fun recursing!
