using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorWPF
{
    internal class Operations
    {
        private static int _inputRestriction;

        private static string _ioField;

        private static bool isFirstNumValueSet; // Is this the first number in the operation?
        private static bool isResOrPrevNumber; // Does the input contain the result or the previous number?
        private static bool isConstantValue; // Does the input contain a constant value?
        private static bool isBlocked; // Is there an option to enter data in the IOfield?
        private static bool isUnaryOperation; // A single operand operation?

        private static double firstNumber; // Variable for the first number
        private static double secondNumber; // Variable for the second number (if operation is binary)

        public static event EventHandler IOFieldChanged;
        public static event EventHandler ComputationEnded;

        private static EnumOfOperations nextOperationType;
        private static EnumOfOperations prevOperationType;

        private const double PI = Math.PI; // For sin, cos and tan
        private static readonly char decimalSeparator; // For Dot()

        public enum EnumOfOperations
        {
            ToAdd,
            ToSubtract,
            ToMultiply,
            ToDivide,
            None
        };
        static Operations()
        {
            IOField = "0";
            InputRestriction = -1;

            isFirstNumValueSet = false;
            isBlocked = false;
            isResOrPrevNumber = false;
            isConstantValue = false;
            isUnaryOperation = false;

            firstNumber = 0;
            secondNumber = 0;

            nextOperationType = EnumOfOperations.None;
            prevOperationType = EnumOfOperations.None;

            decimalSeparator = Convert.ToChar(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }

        // Properties
        public static string IOField
        {
            get { return _ioField; }
            private set
            {
                if (isResOrPrevNumber || isConstantValue)
                {
                    if (_ioField != string.Empty)
                        value = value.Remove(0, _ioField.Length);

                    if (value != string.Empty && value[0] == decimalSeparator)
                        value = value.Insert(0, "0");

                    isResOrPrevNumber = false;
                    isConstantValue = false;
                }

                if (value.Length > 1 && value[0] == '0')
                {
                    if (value[1] != decimalSeparator)
                        value = value.Substring(1);
                }
                _ioField = value;
                OnIOFieldChanged(EventArgs.Empty);
            }
        }

        public static int InputRestriction
        {
            get { return _inputRestriction; }
            set
            {
                if (value < -1) value = Math.Abs(value);
                _inputRestriction = value;
            }
        }

        // Operations:
        public static void AddFigure(int number) // To add a number from 0 to 9
        {
            if (isInputAllowed())
            {
                if (isUnaryOperation)
                {
                    IOField = number.ToString();
                }
                else IOField += number.ToString();
            }
        }

        public static void Clear() // To clear TextBlock "text"
        {
            isResOrPrevNumber = false;
            ClearIOField();
            isBlocked = false;
            IOField = "0";
        }

        public static void DelLast() // To delete the last entered number
        {
            if (!isResOrPrevNumber && !isBlocked && !isConstantValue && !isUnaryOperation)
            {
                IOField = IOField.Remove(IOField.Length - 1);
                if (IOField.Length == 0 || (IOField.Length == 1 && IOField[0] == '-'))
                    IOField = "0";
            }
        }

        public static void ChangeSign() // To change the sign before a number
        {

            if (!isBlocked)
            {
                double number;
                if (Double.TryParse(IOField, out number))
                {
                    IOField = (number * (-1)).ToString();
                }
            }
        }

        public static void SquareRoot() // To calculate the square root
        {
            if (!isBlocked)
            {
                if (Double.TryParse(IOField, out double number))
                {
                    if (number >= 0)
                    {
                        IOField = Math.Sqrt(number).ToString();
                    }
                    else
                    {
                        InvokeError();
                        return;
                    }
                }
            }
        }

        public static void DoubleDegree() // To double the number
        {
            if (!isBlocked)
            {
                double number;
                if (Double.TryParse(IOField, out number))
                {
                    IOField = Math.Pow(number, 2).ToString();
                }
            }
        }

        public static void Equals() // To show the result of operations
        {
            if (!isBlocked)
            {
                if (isResOrPrevNumber || (nextOperationType == EnumOfOperations.None && isUnaryOperation))
                {
                    isResOrPrevNumber = true;
                    if (isUnaryOperation || prevOperationType != EnumOfOperations.None)
                    {
                        ComputationEnded?.Invoke(null, new EventArgs());
                    }
                }
                else if ((prevOperationType != EnumOfOperations.None || (prevOperationType == EnumOfOperations.None && nextOperationType != EnumOfOperations.None)))
                {
                    if (Double.TryParse(IOField, out secondNumber))
                    {
                        string secondNum = IOField;
                        ExecuteOperationImpl(nextOperationType);
                        ComputationEnded?.Invoke(null, new EventArgs());
                    }
                }
            }
        }

        public static void Dot() // To set the dot after the number
        {
            if (isInputAllowed() && (!IOField.Contains(decimalSeparator.ToString()) || isResOrPrevNumber || isConstantValue))
            {
                if (isUnaryOperation)
                {
                    IOField = "0" + decimalSeparator.ToString();
                }
                else IOField += decimalSeparator;
            }
        }

        public static void Factorial() // To calculate the factorial
        {
            if (!isBlocked && !IOField.Contains(decimalSeparator.ToString()))
            {
                if (Int32.TryParse(IOField, out int number))
                {
                    int result = number;
                    if (number > 0)
                    {
                        for (int i = number - 1; i > 0; i--)
                        {
                            result *= i;
                        }
                    }
                    else if (number == 0)
                        result = 1;
                    else if (number < 0)
                    {
                        InvokeError();
                        return;
                    }
                    IOField = result.ToString();
                }
            }
        }

        public static void Sin() // Calculation in degrees
        {
            if (!isBlocked)
            {
                double number;
                if (Double.TryParse(IOField, out number))
                {
                    number = PI * (number / 180);

                    if (isConstantValue)
                        isConstantValue = false;
                    IOField = Math.Round(Math.Sin(number), 5).ToString(); // Rounding the final result to the 5th symbol
                }
            }
        }

        public static void Cos() // Calculation in degrees
        {
            if (!isBlocked)
            {
                double number;
                if (Double.TryParse(IOField, out number))
                {
                    number = PI * (number / 180);

                    if (isConstantValue)
                        isConstantValue = false;
                    IOField = Math.Round(Math.Cos(number), 5).ToString(); // Rounding the final result to the 5th symbol
                }
            }
        }

        public static void Tan() // Calculation in degrees
        {
            if (!isBlocked)
            {
                double number;
                if (Double.TryParse(IOField, out number))
                {
                    number = PI * (number / 180);

                    number = Math.Tan(number);
                    if (number > 1)
                    {
                        InvokeError();
                        return;
                    }
                    if (isConstantValue)
                        isConstantValue = false;
                    IOField = Math.Round(number, 5).ToString(); // Rounding the final result to the 5th symbol
                }
            }
        }

        private static void AddNextOperation(in EnumOfOperations operation)
        {
            if (Double.TryParse(IOField, out double number))
            {
                if (!isFirstNumValueSet)
                {
                    firstNumber = number;
                    isFirstNumValueSet = true;
                }
                else secondNumber = number;

                if (nextOperationType != EnumOfOperations.None && (!isResOrPrevNumber || isConstantValue))
                {
                    ExecuteOperationImpl(nextOperationType);
                    if (isBlocked)
                        return;
                }
                else if (!isResOrPrevNumber || isConstantValue)
                {
                    if (IOField[IOField.Length - 1] == decimalSeparator || (IOField.Length > 1 && IOField[IOField.Length - 2] == decimalSeparator && IOField[IOField.Length - 1] == '0'))
                        IOField = firstNumber.ToString();
                }
                nextOperationType = operation;
                isResOrPrevNumber = true;
                isConstantValue = false;
            }
        }

        private static void ExecuteOperationImpl(EnumOfOperations operation)
        {
            switch (operation)
            {
                case EnumOfOperations.ToAdd:
                    firstNumber += secondNumber;
                    break;
                case EnumOfOperations.ToSubtract:
                    firstNumber -= secondNumber;
                    break;
                case EnumOfOperations.ToDivide:
                    if (secondNumber == 0)
                    {
                        InvokeError();
                        return;
                    }
                    else
                        firstNumber /= secondNumber;
                    break;
                case EnumOfOperations.ToMultiply:
                    firstNumber *= secondNumber;
                    break;
            }
            prevOperationType = nextOperationType;
            nextOperationType = EnumOfOperations.None;
            isConstantValue = false;
            IOField = firstNumber.ToString();
            isResOrPrevNumber = true;

        }
        public static void ExecuteOperation(EnumOfOperations operation)
        {
            if (operation != EnumOfOperations.None && !isBlocked)
            {
                switch (operation)
                {
                    case EnumOfOperations.ToAdd:
                        AddNextOperation(EnumOfOperations.ToAdd);
                        break;
                    case EnumOfOperations.ToSubtract:
                        AddNextOperation(EnumOfOperations.ToSubtract);
                        break;
                    case EnumOfOperations.ToDivide:
                        AddNextOperation(EnumOfOperations.ToDivide);
                        break;
                    case EnumOfOperations.ToMultiply:
                        AddNextOperation(EnumOfOperations.ToMultiply);
                        break;
                }
            }
        }

        private static bool isInputAllowed()
        {
            return (!isBlocked && (isResOrPrevNumber || (InputRestriction == -1 || IOField.Length + 1 <= InputRestriction) || isUnaryOperation));
        }

        private static void ClearIOField()
        {
            isFirstNumValueSet = false;
            isConstantValue = false;
            isUnaryOperation = false;
            nextOperationType = EnumOfOperations.None;
            prevOperationType = EnumOfOperations.None;
        }

        private static void InvokeError()
        {
            prevOperationType = EnumOfOperations.None;
            nextOperationType = EnumOfOperations.None;
            isResOrPrevNumber = false;
            isFirstNumValueSet = false;
            isBlocked = true;
            IOField = "error";
        }

        protected static void OnIOFieldChanged(EventArgs e) => IOFieldChanged?.Invoke(null, e);
    }
}
