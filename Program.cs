using System;

namespace Program
{
	public static class Program
	{
		public static void Main(string[] args)
		{
            Interpreter interpreter = new Interpreter();
            interpreter.AddOperator(new Operator() { symbol = "+", priority = 1, function = (a, b) => a + b });
            interpreter.AddOperator(new Operator() { symbol = "-", priority = 1, function = (a, b) => a - b });
            interpreter.AddOperator(new Operator() { symbol = "*", priority = 2, function = (a, b) => a * b });
            interpreter.AddOperator(new Operator() { symbol = "/", priority = 2, function = (a, b) => a / b });
            interpreter.AddOperator(new Operator() { symbol = "^", priority = 3, function = (a, b) => Math.Pow(a, b) });

            if(args.Contains("-test"))
            {
                string[][] expresstions = new string[][] {
                    new string[] { "2+2", (2+2).ToString() },
                        new string[] { "3.5 + 4.2 * 2 / (1.7 - 5) ^ 2 ^ -3", (3.5 + 4.2 * 2 / Math.Pow(Math.Pow((1.7 - 5), 2), -3)).ToString() },
                        new string[] { "-12 + (24 - 8) / 4", (-12 + (24 - 8) / 4).ToString() },
                        new string[] { "(0.5 + 2.3) * (5 - 3.3)", ((0.5 + 2.3) * (5 - 3.3)).ToString() },
                        new string[] { "-3 ^ 2 + 15 / (3 + 2)",  (Math.Pow(-3, 2) + 15 / (3 + 2)).ToString() },
                        new string[] { "100 * (2.5 - 1.1) ^ 3", (100 * Math.Pow((2.5 - 1.1), 3)).ToString() },
                        new string[] { "1.5e2 + 3.2e-1 * 2/(6 - 2.4e3) ^ 2", (1.5e2 + 3.2e-1 * 2/Math.Pow((6 - 2.4e3), 2)).ToString() },
                        new string[] { "-1.2e3+(3.4e2 - 8e1) / 4", (-1.2e3+(3.4e2 - 8e1) / 4).ToString() },
                        new string[] { "(5e-1+2.3) * (5e1-33e-1)", ((5e-1+2.3) * (5e1-33e-1)).ToString() },
                        new string[] { "3e2^2-150/(3+2.5e1)", (3e2*3e2-150/(3+2.5e1)).ToString() },
                        new string[] { "2.5e3*(2.05-1.1e1)^3", (2.5e3*Math.Pow((2.05-1.1e1), 3)).ToString() },
                        new string[] { "(2+2", "NaN" },
                        new string[] { "2+2)", "NaN" },
                        new string[] { "2(+2", "NaN" },
                        new string[] { "2(2 +", "NaN" },
                        new string[] { "2+2=", "NaN" },
                        new string[] { "98 - 625 + 36", (98 - 625 + 36).ToString() },
                        new string[] { "98- 625 + 36", (98 - 625 + 36).ToString() },
                        new string[] { "98-625 + 36", (98 - 625 + 36).ToString() },
                        new string[] { "98 - 625+ 36", (98 - 625 + 36).ToString() },
                        new string[] { "98 - 625+36", (98 - 625 + 36).ToString() },
                };


                System.Console.WriteLine($"{"Expression",-40} | {"Computed result", -20} | {"Right result", -20}");
                System.Console.WriteLine("----------------------------------------------------------------------------------------------------");
                foreach(var exp in expresstions)
                    System.Console.WriteLine($"{exp[0], -40} | {interpreter.ExecuteInfix(exp[0]), -20} | {exp[1], -20}");
            }

            while(true)
            {
                System.Console.Write("Enter expression: ");
                var exp = Console.ReadLine();
                if(exp == null || exp == "") break;
                System.Console.WriteLine(exp + " = " + interpreter.ExecuteInfix(exp));
            }
           

			_ = Console.ReadKey();
		}	
	}	
}
