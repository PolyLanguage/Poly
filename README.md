# Poly
Poly is Object-Oriented Static-typed and Strong-structured programming language

# Syntax
Basic hello world code:
```c#
namespace: 'MyProgram';

class Program
{
    method Main(string[] args)
    {
        print('hello world !');
    }
}
```
Basic Object-oriented code:
```c#
namespace: 'MyProgram';

class car
{
    string name;
    int price;
    
    ctor(string _name)
    {
        self.name = _name;
    }
    
    method reset_name()
    {
        self.name = '';
    }
}

class Program
{
    method Main(string[] args)
    {
        car mycar = new car('bmw');
        print(mycar.name);
    }
}
```

# Progress
- [x] Namespace
- [ ] Import
- [x] Variable declaration
- [x] Variable assigning
- [x] Return
- [x] Conditions
    - [x] If
    - [ ] ElseIf
    - [ ] Else
    - [ ] While ... Do, Do ... While
    - [ ] For
    - [ ] Foreach
- [x] Class declaration
- [ ] Static classes
- [ ] Class fields
- [ ] Class methods
- [x] Method declaration
- [ ] System library
    - [x] print(value)
    - [x] printin(value)
    - [ ] isnull(value)
    - [ ] memory.delete(varName)
- [x] Expressions
    - [x] +, -, *, /, %
    - [x] &&, ||
    - [x] ==, !=, <, >, <=, >=
    - [ ] ? (true ? true : false)
    - [ ] ?? (true ?? true)
    - [x] Number type (123)
    - [x] Real type (1.0)
    - [x] String type ('abc')
        - [ ] String formatting ($'hello ${abc}')
    - [x] Array type ([])
    - [ ] Object type ({})
    - [ ] Lambda type(() => {})
    - [x] Method calls (abc())
    - [x] Array calls (abc[0+1])
    - [ ] Instance calls (abc.cdf.ghj)
    - [ ] Instance creation
