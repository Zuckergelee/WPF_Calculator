using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace CalculatorWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string symbol = b.Content.ToString();

            switch (symbol)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "0":
                    Operations.AddFigure(Int32.Parse(symbol));
                    break;
                case "C":
                    Operations.Clear();
                    break;
                case "⟵":
                    Operations.DelLast();
                    break;
                case "±":
                    Operations.ChangeSign();
                    break;
                case "√":
                    Operations.SquareRoot();
                    break;
                case "x²":
                    Operations.DoubleDegree();
                    break;
                case "=":
                    Operations.Equals();
                    break;
                case ".":
                    Operations.Dot();
                    break;
                case "n!":
                    Operations.Factorial();
                    break;
                case "+":
                    Operations.ExecuteOperation(Operations.EnumOfOperations.ToAdd);
                    break;
                case "-":
                    Operations.ExecuteOperation(Operations.EnumOfOperations.ToSubtract);
                    break;
                case "*":
                    Operations.ExecuteOperation(Operations.EnumOfOperations.ToMultiply);
                    break;
                case "/":
                    Operations.ExecuteOperation(Operations.EnumOfOperations.ToDivide);
                    break;
                case "sin":
                    Operations.Sin();
                    break;
                case "cos":
                    Operations.Cos();
                    break;
                case "tan":
                    Operations.Tan();
                    break;
            }
        }
    }
}
