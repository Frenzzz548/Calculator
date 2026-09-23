using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CalculatorApp
{
    public partial class Form1 : Form
    {
        private CalculatorService calculator;
        private Dictionary<char, Button> keyMap;

        public Form1()
        {
            InitializeComponent();
            InitializeCalculator();
        }

        private void InitializeCalculator()
        {
            calculator = new CalculatorService();

            // Subscribe to calculator events
            calculator.DisplayChanged += Calculator_DisplayChanged;
            calculator.ExpressionChanged += Calculator_ExpressionChanged;
            calculator.HistoryChanged += Calculator_HistoryChanged;
            calculator.ErrorOccurred += Calculator_ErrorOccurred;

            // Initialize keyboard map
            keyMap = new Dictionary<char, Button>
            {
                ['0'] = btn0,
                ['1'] = btn1,
                ['2'] = btn2,
                ['3'] = btn3,
                ['4'] = btn4,
                ['5'] = btn5,
                ['6'] = btn6,
                ['7'] = btn7,
                ['8'] = btn8,
                ['9'] = btn9,
                ['+'] = btnPlus,
                ['-'] = btnMinus,
                ['*'] = btnMultiply,
                ['/'] = btnDivide,
                ['.'] = btnDecimal,
                [','] = btnDecimal,
                ['='] = btnEquals
            };

            // Update display
            txtDisplay.Text = calculator.Display;
        }

        // Event Handlers
        private void Calculator_DisplayChanged(object sender, DisplayChangedEventArgs e)
        {
            txtDisplay.Text = e.Display;
        }

        private void Calculator_ExpressionChanged(object sender, ExpressionChangedEventArgs e)
        {
            // Show expression, truncate if too long
            string expr = e.Expression;
            lblExpression.Text = expr.Length > 34 ? "…" + expr.Substring(expr.Length - 33) : expr;
        }

        private void Calculator_HistoryChanged(object sender, HistoryChangedEventArgs e)
        {
            lstHistory.DataSource = null;
            lstHistory.DataSource = new List<string>(e.History);
        }

        private void Calculator_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Number button handler
        private void NumberButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            calculator.OnNumberInput(button.Text);
        }

        // Operator button handler
        private void OperatorButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            calculator.OnOperatorInput(button.Text);
        }

        // Decimal button handler
        private void btnDecimal_Click(object sender, EventArgs e)
        {
            calculator.OnDecimalInput();
        }

        // Clear button handler
        private void btnClear_Click(object sender, EventArgs e)
        {
            calculator.OnClear();
        }

        // Equals button handler
        private void btnEquals_Click(object sender, EventArgs e)
        {
            calculator.OnEquals();
        }

        // Backspace button handler
        private void btnBackspace_Click(object sender, EventArgs e)
        {
            calculator.OnBackspace();
        }

        // Toggle sign button handler
        private void btnToggleSign_Click(object sender, EventArgs e)
        {
            calculator.OnToggleSign();
        }

        // Percentage button handler
        private void btnPercent_Click(object sender, EventArgs e)
        {
            calculator.OnPercentage();
        }

        // Square root button handler
        private void btnSqrt_Click(object sender, EventArgs e)
        {
            calculator.OnSquareRoot();
        }

        // Square button handler
        private void btnSquare_Click(object sender, EventArgs e)
        {
            calculator.OnSquare();
        }

        // Factorial button handler
        private void btnFactorial_Click(object sender, EventArgs e)
        {
            calculator.OnFactorial();
        }

        // Sine button handler
        private void btnSin_Click(object sender, EventArgs e)
        {
            calculator.OnSine();
        }

        // Cosine button handler
        private void btnCos_Click(object sender, EventArgs e)
        {
            calculator.OnCosine();
        }

        // Tangent button handler
        private void btnTan_Click(object sender, EventArgs e)
        {
            calculator.OnTangent();
        }

        // Keyboard support
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (keyMap.TryGetValue(e.KeyChar, out Button button))
            {
                button.PerformClick();
                e.Handled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                btnEquals.PerformClick();
                return true;
            }
            if (keyData == Keys.Escape)
            {
                btnClear.PerformClick();
                return true;
            }
            if (keyData == Keys.Back)
            {
                btnBackspace.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}