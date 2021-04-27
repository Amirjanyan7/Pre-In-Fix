using System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InfixExpressionCalculator
{
 
    public static class InfixExpressionCalculator
    {
    
        private static readonly Dictionary<char, int> Operators = new Dictionary<char, int>
        {
            {'-', 1},
            {'+', 2},
            {'/', 3},
            {'*', 4}
        };

        
        public static decimal EvaluateInfix(string infix)
        {
            return EvaluatePostfix(InfixToPostfix(infix));
        }

        # region InfixToPostfix

     
        public static string InfixToPostfix(string infix)
        {
            if (string.IsNullOrWhiteSpace(infix)) throw new ArgumentException("Expression is empty.");

            var operatorStack = new Stack<char>();
            var output = new StringBuilder();

            foreach (char token in Regex.Replace(infix, @"\s+", ""))
            {
                if (Operators.ContainsKey(token))
                {
                    HandleOperatorCase(token, operatorStack, output);
                }
                else
                    switch (token)
                    {
                        case '(':
                            operatorStack.Push(token);
                            break;
                        case ')':
                            HandleRightParenthesisCase(operatorStack, output);
                            break;
                        default:
                         
                            output.Append(token);
                            break;
                    }
            }

            EmptyOperatorStack(operatorStack, output);

            return output.ToString();
        }

      
        private static void HandleOperatorCase(char operatorToken, Stack<char> operatorStack, StringBuilder output)
        {
           
            if (output.Length > 0 && output[output.Length - 1] == ' ')
            {
                throw new Exception(String.Format(
                    "Operators {0} and {1} are adjacent.", operatorStack.Peek(), operatorToken));
            }

           
            while (operatorStack.Count > 0 && Operators.ContainsKey(operatorStack.Peek()) &&
                   Operators[operatorStack.Peek()] >= Operators[operatorToken])
            {
                output.Append(" ").Append(operatorStack.Pop());
            }

            output.Append(" ");
            operatorStack.Push(operatorToken);
        }

        private static void HandleRightParenthesisCase(Stack<char> operatorStack, StringBuilder output)
        {
            while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
            {
                output.Append(" ").Append(operatorStack.Pop());
            }
            if (operatorStack.Count == 0)
            {
                throw new Exception("Missing ( parenthesis.");
            }
            operatorStack.Pop();
        }

       
        private static void EmptyOperatorStack(Stack<char> operatorStack, StringBuilder output)
        {
            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == '(')
                {
                    throw new Exception("Missing ) parenthesis.");
                }
                output.Append(" ").Append(operatorStack.Pop());
            }
        }

        #endregion

        #region EvaluatePostfix

       
        public static decimal EvaluatePostfix(string postfix)
        {
            if (string.IsNullOrWhiteSpace(postfix)) throw new ArgumentException("Expression is empty.");

            var stack = new Stack<decimal>();

            foreach (string token in Regex.Split(postfix.Trim(), @"\s+"))
            {
                if (token.Length == 1 && Operators.ContainsKey(token[0]))
                {
                    if (stack.Count < 2) throw new Exception("Too many operators.");
                    decimal operand2 = stack.Pop(), operand1 = stack.Pop();
                    ApplyOperatorToOperands(token[0], operand1, operand2, stack);
                }
                else
                {
                    try
                    {
                      
                        stack.Push(Decimal.Parse(token));
                    }
                    catch (FormatException)
                    {
                        throw new Exception(String.Format("{0} is not a valid number.", token));
                    }
                }
            }

            return stack.Pop();
        }
        
        private static void ApplyOperatorToOperands(char operatorToken, decimal operand1, decimal operand2,
            Stack<decimal> stack)
        {
            switch (operatorToken)
            {
                case '-':
                    stack.Push(operand1 - operand2);
                    break;
                case '+':
                    stack.Push(operand1 + operand2);
                    break;
                case '/':
                    if (operand2 == 0) throw new DivideByZeroException();
                    stack.Push(operand1/operand2);
                    break;
                case '*':
                    stack.Push(operand1*operand2);
                    break;
                default:
                    throw new Exception(
                        String.Format("{0} is an unsupported operator.", operatorToken));
            }
        }

        #endregion
    }
}
namespace InfixExpressionCalculator.CLI
{
    internal static class Program
    {
        internal static void Main()
        {
            Console.WriteLine("When done, enter 'exit' to quit the calculator.");
            while (true)
            {
                Console.Write("Enter an infix expression: ");
                string input = Console.ReadLine(), output;
                if (input.ToLower().Equals("exit")) break;
                try
                {
                    output = InfixExpressionCalculator.EvaluateInfix(input).ToString();
                }
                catch (Exception e)
                {
                    output = "Invalid expression: " + e.Message;
                }
                Console.WriteLine("=> {0}\n", output);
            }
        }
    }
}