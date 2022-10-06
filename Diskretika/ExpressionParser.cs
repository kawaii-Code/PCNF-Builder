public class ExpressionParser
{
    public Expression Expression { get; }
    public List<Variable> Variables { get; }
    
    public ExpressionParser(string formula)
    {
        Variables = new List<Variable>();
        Expression = ToBinaryExpression(formula, 0);
    }
    
    #region Helper structures
    private static readonly Dictionary<char, Func<bool, bool, bool>> FunctionByOperation = new()
    {
        ['*'] = (a, b) => a & b,
        ['&'] = (a, b) => a & b,
        ['|'] = (a, b) => a | b,
        ['+'] = (a, b) => a | b,
        ['>'] = (a, b) => !a | b,
        ['~'] = (a, b) => a & b | !a & !b,
        ['^'] = (a, b) => !(a & b | !a & !b),
    };

    private static readonly char[] BinaryOperations = new[]
    {
        '&', '*', '|', '+', 
        '>', '~', '^',
    };
    
    private static readonly Dictionary<char, int> PriorityByOperation = new()
    {
        ['&'] = 2, ['*'] = 2, ['|'] = 1, ['+'] = 1,
        ['>'] = 0, ['~'] = 0, ['^'] = 0,
    };
    #endregion
    
    private Expression ToBinaryExpression(string formula, int from = 0, Expression? leftPrev = null)
    {
        for (int i = from; i < formula.Length; i++)
        {
            if (formula[i] == '(')
                i = SkipParentheses(formula, i);
            
            if (!BinaryOperations.Contains(formula[i]))
                continue;

            int nextOpIndex = FindNextBinOperIndex(formula, i + 1);
            
            if (nextOpIndex != -1)
            {
                if (PriorityByOperation[formula[i]] < PriorityByOperation[formula[nextOpIndex]])
                {
                    return new BinaryOperation(ToUnaryExpression(formula, from), ToBinaryExpression(formula, i + 1), FunctionByOperation[formula[i]]); 
                }
                
                Expression left = new BinaryOperation(
                    leftPrev ?? ToUnaryExpression(formula, from),
                    ToUnaryExpression(formula, i+1),
                    FunctionByOperation[formula[i]]);

                return ToBinaryExpression(formula, nextOpIndex, left);
            }
            
            return new BinaryOperation(leftPrev ?? ToUnaryExpression(formula, from), ToUnaryExpression(formula, i + 1), FunctionByOperation[formula[i]]);
        }

        return ToUnaryExpression(formula, from);
    }
    
    private Expression ToUnaryExpression(string formula, int from)
    {
        for (int i = from; i < formula.Length; i++)
        {
            if (!char.IsLetter(formula[i]) && formula[i] != '!' && formula[i] != '(')
                continue;
            
            if (formula[i] == '!')
            {
                return new UnaryOperation(
                    ToUnaryExpression(formula, i + 1), (a) => !a);
            }

            if (formula[i] == '(')
            {
                return ToBinaryExpression(formula, i + 1);
            }
            
            if (Variables.Select(v => v.Name).Contains(formula[i]))
            {
                return Variables.First(v => v.Name == formula[i]);
            }
            
            Variable var = new Variable(formula[i]);
            Variables.Add(var);
            return var;
        }

        throw new ArgumentException($"The input formula {formula} is incorrect");
    }

    #region Hepler functions
    private static int FindNextBinOperIndex(string formula, int from)
    {
        for (int i = from; i < formula.Length; i++)
        {
            if (formula[i] == '(')
                i = SkipParentheses(formula, i);
            
            if (formula[i] == ')')
                return -1;
            
            if (BinaryOperations.Contains(formula[i]))
            {
                return i;
            }
        }

        return -1;
    }
    
    private static int SkipParentheses(string formula, int i)
    {
        int countOpen = 1;
        int countClosed = 0;

        do
        {
            i++;

            if (formula[i] == '(')
                countOpen++;
            else if (formula[i] == ')')
                countClosed++;
        } while (countClosed < countOpen);

        return Math.Min(formula.Length - 1,  i + 1);
    }
    #endregion
}