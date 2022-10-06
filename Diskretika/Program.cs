using System.Text;

class Program
{
    private static void PrintTooltip()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Добро пожаловать в преобразователь формул в СКНФ.\n");
        Console.ForegroundColor = ConsoleColor.White;
        
        Console.WriteLine("Синтаксис написания формул:\n");
        
        Console.WriteLine("Переменные - обычные латинские буквы, от a до z. Регистр имеет значение!\n");
        
        Console.WriteLine(@"Операция ""И"" - '&', '*'");
        Console.WriteLine(@"Операция ""ИЛИ"" - '|', '+'");
        Console.WriteLine(@"Операция ""НЕ"" - '!'");
        Console.WriteLine(@"Операция ""ИМПЛИКАЦИЯ"" - '>' или '=>'");
        Console.WriteLine(@"Операция ""ЭКВИВАЛЕНЦИЯ"" - '~'");
        Console.WriteLine(@"Операция ""XOR"" - '^'");
        
        Console.WriteLine("\nПримеры формул:");

        Console.WriteLine("(a & b | c)\na*b*c\n(b|(a>c))\na  |b | (a & c)\n");
    }
    
    private static void Main()
    {
        PrintTooltip();

        char userInput = ' ';
        
        do
        {
            Console.Write("Введите свою формулу >> ");
        
            string formula = Console.ReadLine() ?? throw new ArgumentNullException();
        
            ExpressionParser parser = new ExpressionParser(formula);
        
            Console.WriteLine(ToPcnf(parser.Expression, parser.Variables));
            
            Console.Write("Чтобы выйти из программы, введите e, вывести подсказку опять - h, чтобы продолжить, нажмите любую клавишу: ");

            userInput = Console.ReadKey().KeyChar;
            
            Console.Clear();

            if (userInput == 'h')
                PrintTooltip();
        } while (userInput != 'e');
    }

    private static List<bool> Evaluate(Expression exp, List<Variable> variables, int varIndex = 0, List<bool>? truthTable = null)
    {
        truthTable ??= new List<bool>();
        
        if (varIndex == variables.Count)
        {
            truthTable.Add(exp.Evaluate());

            return null; // Костыль чтобы завершить метод досрочно
        }
        
        variables[varIndex].Value = false;
        Evaluate(exp, variables, varIndex + 1, truthTable);
        
        variables[varIndex].Value = true;
        Evaluate(exp, variables, varIndex + 1, truthTable);

        return truthTable;
    }
    
    private static string ToPcnf(Expression expression, List<Variable> variables)
    {
        var list = Evaluate(expression, variables);

        StringBuilder builder = new StringBuilder();
        
        for (int i = 0; i < Math.Pow(2, variables.Count); i++)
        {
            if (list[i])
                continue;

            builder.Append('(');
            
            for (int k = variables.Count, varIndex = 0; k >= 1; k--, varIndex++)
            {
                builder.Append(i % Math.Pow(2, k) >= Math.Pow(2, variables.Count) / Math.Pow(2, varIndex + 1) ? '!' : "");
                builder.Append(variables[varIndex].Name);
                builder.Append('+');
            }

            builder.Remove(builder.Length - 1, 1);
            builder.Append(")*");
        }

        if (builder.Length > 0)
        {
            builder.Remove(builder.Length - 1, 1);
            return "СКНФ для вашей формулы: " + builder;
        }

        return "Ваша формула тождественно истинна. У неё нет СКНФ";
    }
}