using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class Expression
    {
        public string StringExpression { get; set; }
        private string operationsChars = "*/+-";

        public Expression(string expression)
        {
            Expressions = new List<Expression>();
            Operations = new List<char>();
            StringExpression = expression;
            int acikParantez = 0;
            int parantezStartIndex = -1;
            int kapaliParantez = 0;
            int lastProcessEndIndex = -1;

            for (int i = 0; i < StringExpression.Length; i++)
            {
                if (StringExpression[i] == '(')
                {
                    if (parantezStartIndex == -1)
                        parantezStartIndex = i;
                    acikParantez++;
                }
                else if (StringExpression[i] == ')')
                {
                    kapaliParantez++;
                    if (acikParantez == kapaliParantez)
                    {
                        lastProcessEndIndex = i;
                        Expressions.Add(new Expression(StringExpression.Substring(parantezStartIndex + 1, i - parantezStartIndex - 1)));
                        if (i + 1 < StringExpression.Length && operationsChars.Contains(StringExpression[i + 1]))
                        {
                            i++;
                            Operations.Add(StringExpression[i]);
                            lastProcessEndIndex = i;
                        }
                        continue;
                    }
                }
                else if (acikParantez == kapaliParantez && (operationsChars.Contains(StringExpression[i]) || i == StringExpression.Length - 1))
                {
                    bool lastIndex = i == StringExpression.Length - 1;
                    string _expression = StringExpression.Substring(lastProcessEndIndex + 1, i - lastProcessEndIndex - 1 + (lastIndex ? 1 : 0));
                    if (_expression == StringExpression)
                        return;
                    //if (StringExpression.Substring(lastProcessEndIndex + 1, i - lastProcessEndIndex - 1 + (i == StringExpression.Length - 1 ? 1 : 0)) == StringExpression)
                    //    return;
                    Expressions.Add(new Expression(_expression));
                    lastProcessEndIndex = i - 1;
                    if (i + 1 < StringExpression.Length && operationsChars.Contains(StringExpression[i]))
                    {
                        Operations.Add(StringExpression[i]);
                        lastProcessEndIndex++;
                    }
                }
            }
        }

        public override string ToString()
        {
            if (Operations.Count == 0)
                return StringExpression;
            string result = string.Empty;
            for (int i = 0; i < Expressions.Count; i++)
                result += Expressions[i].ToString() + (Operations.Count > i ? " " + Operations[i].ToString() : "") + " ";
            return result.Substring(0, result.Length - 1);
        }

        public List<Expression> Expressions { get; set; }
        public List<char> Operations { get; set; }

        public decimal Value
        {
            get
            {
                return GetProcess(Expressions, Operations).Value;
            }
        }

        private static Process GetProcess(List<Expression> expressions, List<char> operations)
        {
            Process process = null;
            if (operations.Count == 0)
            {
                if (expressions[0].Operations.Count > 0)
                    return new Process(expressions[0].Value);
                return new Process(expressions[0].StringExpression);
            }

            if (operations.Contains('+') || operations.Contains('-'))
                for (int i = operations.Count - 1; i >= 0; i--)
                {
                    if (operations[i] == '+' || operations[i] == '-')
                    {
                        return new Process(
                            GetProcess(
                                expressions.GetRange(0, i + 1),
                                operations.GetRange(0, i)),
                            operations[i],
                            GetProcess(
                                expressions.GetRange(i + 1, expressions.Count - (i + 1)),
                                operations.GetRange(i + 1, operations.Count - (i + 1))));
                    }
                }

            if (operations.Contains('*') || operations.Contains('/'))
                for (int i = operations.Count - 1; i >= 0; i--)
                {
                    if (operations[i] == '*' || operations[i] == '/')
                    {
                        return new Process(
                            GetProcess(
                                expressions.GetRange(0, i + 1),
                                operations.GetRange(0, i)),
                            operations[i],
                            GetProcess(
                                expressions.GetRange(i + 1, expressions.Count - (i + 1)),
                                operations.GetRange(i + 1, operations.Count - (i + 1))));
                    }
                }
            return process;
        }
    }

    public class Process
    {
        private decimal value;
        public Process Left { get; set; }
        public Process Right { get; set; }
        public char Operation { get; set; }

        public Process()
        {

        }

        public Process(Process p1, char operation, Process p2)
        {
            Left = p1;
            Operation = operation;
            Right = p2;
        }

        public Process(decimal v1, char operation, decimal v2)
        {
            Left = new Process(v1);
            Right = new Process(v2);
            Operation = operation;
        }

        public Process(decimal value)
        {
            this.value = value;
            Operation = '=';
        }

        public Process(string value)
        {
            this.value = decimal.Parse(value);
            Operation = '=';
        }

        public decimal Value
        {
            get
            {
                switch (Operation)
                {
                    case '+':
                        return Left.Value + Right.Value;
                    case '-':
                        return Left.Value - Right.Value;
                    case '*':
                        return Left.Value * Right.Value;
                    case '/':
                        return Left.Value / Right.Value;
                    default:
                        return value;
                }
            }
        }
    }
}
