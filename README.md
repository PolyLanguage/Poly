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
