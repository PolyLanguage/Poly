# Poly
Poly is Object-Oriented Static-typed and Strong-structured programming language

# Syntax
Basic hello world code:
```c#
import: 'System';

namespace: 'MyProgram';
class program
{
    <entry>
    method int main(string[] args)
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
class ferrari : car
{
    //adds additional code to inherited class
    ctor(string _name) += 
    {
        self.price = 250000;
    }
    
    //overrides code of method
    method reset_name() =
    {
        self.name = 'ferrari';
    }
}
```
