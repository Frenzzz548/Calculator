using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CalculatorApp
{
    /// <summary>
    /// Service class that handles all calculator logic.
    /// Separates business logic from UI concerns.
    /// </summary>
    public class CalculatorService
    {
        // State variables
        private double firstNumber = 0;
        private double secondNumber = 0;
        private double result = 0;
        private string operation = "";
        private bool isResultShown = false;
        private string expression = "";

        // Configuration
        private const int MaxDigits = 16;
        private const int MaxHistoryEntries = 50;

        // History tracking
        private List<string> calculationHistory = new List<string>();

        // Events
        public event EventHandler<DisplayChangedEventArgs> DisplayChanged;
        public event EventHandler<ExpressionChangedEventArgs> ExpressionChanged;
        public event EventHandler<HistoryChangedEventArgs> HistoryChanged;
        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        // Properties
        public string Display { get; private set; } = "0";
        public string Expression => expression;
        public IReadOnlyList<string> CalculationHistory => calculationHistory.AsReadOnly();
        public bool IsResultShown => isResultShown;

        public CalculatorService()
        {
            ResetState();
        }

        /// <summary>
        /// Handle number button click
        /// </summary>
        public void OnNumberInput(string digit)
        {
            if (isResultShown)
                ResetDisplay();

            if (Display == "0" || isResultShown)
            {
                Display = digit;
                isResultShown = false;
            }
            else if (Display.Length < MaxDigits)
            {
                Display += digit;
            }

            OnDisplayChanged();
        }

        /// <summary>
        /// Handle operator button click
        /// </summary>
        public void OnOperatorInput(string op)
        {
            // Operator pressed twice in a row - just replace it
            if (Display == "")
            {
                if (operation != "")
                {
                    operation = op;
                    SetExpression(expression.Substring(0, expression.Length - 2) + op + " ");
                }
                return;
            }

            // Build expression history
            string typed = Display;
            bool hasPending = operation != "";
            string newExpression = (hasPending ? expression : "") + typed + " " + op + " ";

            // Chaining: 10 + 20 + ... -> calculate 10 + 20 first
            if (hasPending)
            {
                if (!Evaluate())
                    return;
            }

            if (!TryReadDisplay(out firstNumber))
                return;

            operation = op;
            isResultShown = false;
            Display = "";
            SetExpression(newExpression);
        }

        /// <summary>
        /// Handle decimal point input
        /// </summary>
        public void OnDecimalInput()
        {
            if (isResultShown || Display == "")
            {
                if (isResultShown)
                    ResetDisplay();

                Display = "0.";
                isResultShown = false;
            }
            else if (!Display.Contains(".") && Display.Length < MaxDigits)
            {
                Display += ".";
            }

            OnDisplayChanged();
        }

        /// <summary>
        /// Handle equals button
        /// </summary>
        public bool OnEquals()
        {
            return Evaluate();
        }

        /// <summary>
        /// Clear all state
        /// </summary>
        public void OnClear()
        {
            ResetState();
            OnDisplayChanged();
        }

        /// <summary>
        /// Backspace - remove last character from display
        /// </summary>
        public void OnBackspace()
        {
            if (isResultShown || Display == "" || Display == "0")
                return;

            if (Display.Length == 1)
            {
                Display = "0";
            }
            else
            {
                Display = Display.Substring(0, Display.Length - 1);
            }

            OnDisplayChanged();
        }

        /// <summary>
        /// Toggle sign of current display value
        /// </summary>
        public void OnToggleSign()
        {
            if (Display == "" || Display == "0")
                return;

            if (TryReadDisplay(out double value))
            {
                Display = (-value).ToString("G15", CultureInfo.InvariantCulture);
                OnDisplayChanged();
            }
        }

        /// <summary>
        /// Calculate percentage
        /// </summary>
        public void OnPercentage()
        {
            if (!TryReadDisplay(out double value))
                return;

            if (operation == "")
            {
                // Simple percentage: 150% = 1.5
                Display = (value / 100).ToString("G15", CultureInfo.InvariantCulture);
            }
            else
            {
                // Percentage relative to first number: 200 + 50% = 200 + 100
                double percentage = (firstNumber * value) / 100;
                Display = percentage.ToString("G15", CultureInfo.InvariantCulture);
            }

            OnDisplayChanged();
        }

        /// <summary>
        /// Calculate square root
        /// </summary>
        public void OnSquareRoot()
        {
            if (!TryReadDisplay(out double value))
                return;

            if (value < 0)
            {
                OnError("Cannot calculate square root of negative number");
                return;
            }

            Display = Math.Sqrt(value).ToString("G15", CultureInfo.InvariantCulture);
            isResultShown = true;
            OnDisplayChanged();
        }

        /// <summary>
        /// Calculate square (x²)
        /// </summary>
        public void OnSquare()
        {
            if (!TryReadDisplay(out double value))
                return;

            Display = (value * value).ToString("G15", CultureInfo.InvariantCulture);
            isResultShown = true;
            OnDisplayChanged();
        }

        /// <summary>
        /// Calculate factorial (x!)
        /// </summary>
        public void OnFactorial()
        {
            if (!TryReadDisplay(out double value))
                return;

            if (value < 0 || value != Math.Floor(value))
            {
                OnError("Factorial requires non-negative integer");
                return;
            }

            try
            {
                double factorial = 1;
                for (int i = 2; i <= (int)value; i++)
                    factorial *= i;

                Display = factorial.ToString("G15", CultureInfo.InvariantCulture);
                isResultShown = true;
                OnDisplayChanged();
            }
            catch
            {
                OnError("Factorial calculation failed");
            }
        }

        /// <summary>
        /// Calculate sine (in degrees)
        /// </summary>
        public void OnSine()
        {
            if (!TryReadDisplay(out double value))
                return;

            double radians = value * Math.PI / 180; // Convert degrees to radians
            Display = Math.Sin(radians).ToString("G15", CultureInfo.InvariantCulture);
            isResultShown = true;
            OnDisplayChanged();
        }

        /// <summary>
        /// Calculate cosine (in degrees)
        /// </summary>
        public void OnCosine()
        {
            if (!TryReadDisplay(out double value))
                return;

            double radians = value * Math.PI / 180; // Convert degrees to radians
            Display = Math.Cos(radians).ToString("G15", CultureInfo.InvariantCulture);
            isResultShown = true;
            OnDisplayChanged();
        }

        /// <summary>
        /// Calculate tangent (in degrees)
        /// </summary>
        public void OnTangent()
        {
            if (!TryReadDisplay(out double value))
                return;

            double radians = value * Math.PI / 180; // Convert degrees to radians
            Display = Math.Tan(radians).ToString("G15", CultureInfo.InvariantCulture);
            isResultShown = true;
            OnDisplayChanged();
        }

        /// <summary>
        /// Evaluate the current operation
        /// </summary>
        private bool Evaluate()
        {
            if (operation == "" || Display == "")
                return false;

            string typed = Display;

            try
            {
                secondNumber = double.Parse(Display, NumberStyles.Float, CultureInfo.InvariantCulture);

                switch (operation)
                {
                    case "+": result = firstNumber + secondNumber; break;
                    case "−": result = firstNumber - secondNumber; break;
                    case "×": result = firstNumber * secondNumber; break;
                    case "÷":
                        if (secondNumber == 0)
                            throw new DivideByZeroException("Cannot divide by zero");
                        result = firstNumber / secondNumber;
                        break;
                    default:
                        return false;
                }

                if (double.IsInfinity(result) || double.IsNaN(result))
                    throw new OverflowException("The result is too large to display.");

                Display = result.ToString("G15", CultureInfo.InvariantCulture);
                string historyEntry = expression + typed + " = " + Display;
                AddToHistory(historyEntry);

                operation = "";
                isResultShown = true;
                SetExpression(expression + typed + " =");
                OnDisplayChanged();
                return true;
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                ResetState();
                return false;
            }
        }

        /// <summary>
        /// Add entry to calculation history
        /// </summary>
        private void AddToHistory(string entry)
        {
            calculationHistory.Insert(0, entry);
            if (calculationHistory.Count > MaxHistoryEntries)
                calculationHistory.RemoveAt(calculationHistory.Count - 1);

            OnHistoryChanged();
        }

        /// <summary>
        /// Clear history
        /// </summary>
        public void ClearHistory()
        {
            calculationHistory.Clear();
            OnHistoryChanged();
        }

        /// <summary>
        /// Reset internal state
        /// </summary>
        private void ResetState()
        {
            firstNumber = 0;
            secondNumber = 0;
            result = 0;
            operation = "";
            isResultShown = false;
            Display = "0";
            SetExpression("");
        }

        /// <summary>
        /// Reset display only
        /// </summary>
        private void ResetDisplay()
        {
            SetExpression("");
            Display = "0";
            isResultShown = false;
        }

        /// <summary>
        /// Set expression history
        /// </summary>
        private void SetExpression(string value)
        {
            expression = value;
            OnExpressionChanged();
        }

        /// <summary>
        /// Try to read display as double
        /// </summary>
        private bool TryReadDisplay(out double value)
        {
            return double.TryParse(Display, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        // Event raising methods
        protected virtual void OnDisplayChanged()
        {
            DisplayChanged?.Invoke(this, new DisplayChangedEventArgs { Display = Display });
        }

        protected virtual void OnExpressionChanged()
        {
            ExpressionChanged?.Invoke(this, new ExpressionChangedEventArgs { Expression = Expression });
        }

        protected virtual void OnHistoryChanged()
        {
            HistoryChanged?.Invoke(this, new HistoryChangedEventArgs { History = CalculationHistory.ToList() });
        }

        protected virtual void OnError(string message)
        {
            ErrorOccurred?.Invoke(this, new ErrorEventArgs { Message = message });
        }
    }

    // Event argument classes
    public class DisplayChangedEventArgs : EventArgs
    {
        public string Display { get; set; }
    }

    public class ExpressionChangedEventArgs : EventArgs
    {
        public string Expression { get; set; }
    }

    public class HistoryChangedEventArgs : EventArgs
    {
        public List<string> History { get; set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
