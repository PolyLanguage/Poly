# Poly
Poly is Object-Oriented Static-typed and Strong-structured programming language

# Syntax
Basic hello world code:
```c#
namespace: 'MyProgram';

class Program
{
    method Main(array<string> args)
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
    method Main(array<string> args)
    {
        car mycar = new car('bmw');
        print(mycar.name);
    }
}
```

# Progress
- [x] Namespace (namespace 'name';)
- [ ] Import (import 'name';)
- [x] Variable declaration (type name = value;)
- [x] Variable assigning (name = value;)
- [x] Return (return value;)
- [x] Conditions
    - [x] If (if(cond) {})
    - [ ] ElseIf (else if(cond) {})
    - [ ] Else (else {})
- [x] Loops
    - [ ] While ... Do, Do ... While
    - [ ] For
    - [ ] Foreach
    - [x] Repeat
- [x] Class declaration (class name {})
- [ ] Static classes (static class name {})
- [ ] Class fields (field name;)
- [ ] Class methods (method type name(args){})
- [x] Method declaration (method type name(args) {})
- [ ] System library
    - [x] print(value)
    - [x] printin(value)
    - [x] read()
    - [x] gettype(value)
    - [x] tostring()
    - [ ] toint()
    - [ ] toreal()
    - [ ] tobool()
    - [ ] toarray()
    - [ ] time()
    - [ ] string.length
    - [ ] array.length
    - [ ] array.find
    - [ ] array.map
    - [ ] array.reduce
- [x] Expressions
    - [x] +, -, *, /, % (Arithemtic)
    - [x] &&, || (Boolean)
    - [x] ==, !=, <, >, <=, >= (Boolean)
    - [ ] ? (true ? true : false)
    - [ ] ?? (true ?? true)
    - [x] Number type (123)
    - [x] Real type (1.0)
    - [x] Boolean type (true/false)
    - [x] String type ('abc')
        - [ ] String formatting ($'hello ${abc}')
    - [x] Array type ([])
    - [x] T of types (array<array<int>>)
    - [ ] Object type ({})
    - [ ] Lambda type(() => {})
    - [x] Method calls (abc())
    - [x] Array calls (abc[0+1])
    - [ ] Dot expressions (abc.cdf.ghj)
    - [ ] Instance creation
