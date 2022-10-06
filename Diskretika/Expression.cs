public abstract class Expression
{
    public abstract bool Evaluate();
}

public class Variable : Expression
{
    public readonly char Name;
    public bool Value { get; set; }
    
    public Variable(char name)
    {
        Name = name;
    }
    
    public override bool Evaluate()
    {
        return Value;
    }

    public override string ToString()
    {
        return $"{Name}:" + (Value ? 1 : 0);
    }
}

public class UnaryOperation : Expression
{
    private readonly Expression _expression;

    private readonly Func<bool, bool> _function; 
    
    public UnaryOperation(Expression expression, Func<bool, bool> function)
    {
        _expression = expression;
        _function = function;
    }

    public override bool Evaluate()
    {
        return _function(_expression.Evaluate());
    } 
    
    public override string ToString()
    {
        return $"!{_expression}";
    }
}

public class BinaryOperation : Expression
{
    private readonly Expression _left;
    private readonly Expression _right;

    private readonly Func<bool, bool, bool> _function;

    public BinaryOperation(Expression left, Expression right, Func<bool, bool, bool> function)
    {
        _left = left;
        _right = right;
        _function = function;
    }
    
    public override bool Evaluate()
    {
        return _function(_left.Evaluate(), _right.Evaluate());
    }

    public override string ToString()
    {
        return $"({_left} => {_right})";
    }
}