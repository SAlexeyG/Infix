using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Program
{
	public struct Operator
	{
		public string symbol;
		public int priority;
        public Func<double, double, double> function;
	}

	public class Interpreter
	{
		private List<Operator> vocabulary = new List<Operator>();

        private const char numberSpecialSymbol = '?';
        private const char operatorSpecialSymbol = '@';
        private const string numberPattern = @"(-?(0|[1-9]\d*)(\.\d+)?(?<=\d)(e-?(0|[1-9]\d*))?)";

		public List<Operator> Vocabulary
		{
		    get { return vocabulary; }
		}

        public void AddOperator(Operator op)
        {
            if(op.priority < 1) return;

            if(new Regex(
                        Regex.Escape($"[)( {numberSpecialSymbol}{operatorSpecialSymbol}]")
                        ).Count(op.symbol) > 0
              ) return;

            for(int i = 0; i < vocabulary.Count; i++)
            {
                if(new Regex(Regex.Escape(op.symbol)).Count(vocabulary[i].symbol) > 0) return;
            }

            vocabulary.Add(op);
        }

		public string InfixToPostfix(string infixExp)
        {
            Queue<string> numbers = new Queue<string>(GetNumbers(infixExp));
            infixExp = ReplaceNumbers(infixExp);

            Queue<Operator> operators = new Queue<Operator>(GetOperators(infixExp));
            infixExp = ReplaceOperators(infixExp);

            infixExp = infixExp.Replace(" ", "");

            if(!IsExpValid(infixExp)) return "";

            StringBuilder postfixExp = new StringBuilder(infixExp.Length);
            Stack<Operator> stack = new Stack<Operator>();

            Operator parenthesis = new Operator()
            {
                symbol = "(",
                priority = 0,
                function = (a, b) => 0,
            };

            for (int i = 0; i < infixExp.Length; i++)
            {
                switch (infixExp[i])
                {
                    case numberSpecialSymbol:
                        postfixExp.Append(numbers.Dequeue() + " ");
                        break;

                    case '(':
                        stack.Push(parenthesis);
                        break;

                    case ')':
                        while (stack.Count > 0 && stack.Peek().symbol != "(")
                            postfixExp.Append(stack.Pop().symbol + " ");
                        stack.Pop();
                        break;

                    case operatorSpecialSymbol:
                        var operation = operators.Dequeue();
                        while ((stack.Count > 0) && (operation.priority <= stack.Peek().priority))
                            postfixExp.Append(stack.Pop().symbol + " ");
                        stack.Push(operation);
                        break;
                }
            }

            foreach (var o in stack)
                postfixExp.Append(o.symbol + " ");

            return postfixExp.ToString();
        }

        public double ExecutePostfix(string postfixExp)
        {
            Queue<string> numbers = new Queue<string>(GetNumbers(postfixExp));
            postfixExp = new Regex(numberPattern)
                .Replace(postfixExp, numberSpecialSymbol.ToString());

            Queue<Operator> operators = new Queue<Operator>(GetOperators(postfixExp));
            postfixExp = ReplaceOperators(postfixExp);

            postfixExp = postfixExp.Replace(" ", "");

            Stack<double> stack = new Stack<double>(numbers.Count);
			var doubleFormat = new NumberFormatInfo { NumberDecimalSeparator = "." };

            for(int i = 0; i < postfixExp.Length; i++)
            {
                switch (postfixExp[i])
                {
                    case numberSpecialSymbol:
						stack.Push(double.Parse(numbers.Dequeue(), doubleFormat));
                        break;

                    case operatorSpecialSymbol:
                        var b = stack.Pop();
                        var a = stack.Pop();
                        stack.Push(operators.Dequeue().function(a, b));
                        break;
                }
            }

            return stack.Pop();
        }

        private bool IsExpValid(string infixExp)
        {
            bool isValid = true;

            isValid = (
                    new Regex(Regex.Escape(")")).Count(infixExp) == 
                    new Regex(Regex.Escape("(")).Count(infixExp)) ? isValid : false;

            foreach (var symbol in infixExp)
            {
                isValid = $"(){numberSpecialSymbol}{operatorSpecialSymbol}".Contains(symbol) ? isValid : false;
            }

            var regex = new Regex(Regex.Escape(operatorSpecialSymbol.ToString()));
			foreach(Match match in regex.Matches(infixExp))
			{
				if((match.Index > 0) && (match.Index < (infixExp.Length - 1)))
				{
                    isValid = $"){numberSpecialSymbol}".Contains(infixExp[match.Index-1]) ? isValid : false;
                    isValid = $"({numberSpecialSymbol}".Contains(infixExp[match.Index+1]) ? isValid : false;
				}
				else
				{
					isValid = false;
				}
			}

            return isValid;
        }

        public double ExecuteInfix(string infixExp)
        {
            var postfixExp = InfixToPostfix(infixExp);
            return postfixExp != "" ? ExecutePostfix(postfixExp) : double.NaN;
        }

        private string ReplaceOperators(string infixExp)
        {
            return new Regex(GetOperatorsPattern())
                .Replace(infixExp, operatorSpecialSymbol.ToString());
        }

        private IEnumerable<Operator> GetOperators(string infixExp)
        {
            return new Regex(GetOperatorsPattern())
                .Matches(infixExp)
                .Select(match => vocabulary
                        .First(op => op.symbol == match.Value));
        }

        private string GetOperatorsPattern()
        {
            var ops = vocabulary.Select(op => "[" + Regex.Escape(op.symbol) + "]");
            return string.Join("|", ops);
        }

        private string ReplaceNumbers(string infixExp)
        {
            var n = Regex.Escape(numberSpecialSymbol.ToString());
            var unaryMinusPattern = $"{n}{n}|{n} +{n}";

            infixExp = new Regex(numberPattern)
                .Replace(infixExp, numberSpecialSymbol.ToString());
            
            infixExp = new Regex(unaryMinusPattern)
                .Replace(infixExp, numberSpecialSymbol + "+" + numberSpecialSymbol);

            return infixExp;
        }

        private IEnumerable<string> GetNumbers(string infixExp)
        {
            return new Regex(numberPattern)
                .Matches(infixExp)
                .Select(match => match.Value);
        }
	} 
}
