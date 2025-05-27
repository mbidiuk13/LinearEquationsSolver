using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LinearEquationSolver
{
    public partial class MainForm : Form
    {
        private int size = 2;
        private EquationSystem equationSystem;
        private TextBox[,] coefficientBoxes;
        private TextBox[] constantTerms;

        public MainForm()
        {
            InitializeComponent();
            equationSystem = new EquationSystem(2);
            InitializeMatrixInputs(2);
        }

        private void InitializeMatrixInputs(int n)
        {
            panelMatrix.Controls.Clear();
            coefficientBoxes = new TextBox[n, n];
            constantTerms = new TextBox[n];

            int startX = 10;
            int startY = 10;
            int boxWidth = 50;
            int boxHeight = 20;
            int spacing = 5;
            int varLabelWidth = 25;

            for (int i = 0; i < n; i++)
            {
                Label eqLabel = new Label
                {
                    Text = $"Рівняння {i + 1}:",
                    Location = new Point(startX, startY + i * (boxHeight + spacing)),
                    AutoSize = true
                };
                panelMatrix.Controls.Add(eqLabel);

                for (int j = 0; j < n; j++)
                {
                    coefficientBoxes[i, j] = new TextBox
                    {
                        Location = new Point(startX + 70 + j * (boxWidth + varLabelWidth + spacing * 3), 
                        startY + i * (boxHeight + spacing)),
                        Size = new Size(boxWidth, boxHeight),
                        Text = "0",
                        TextAlign = HorizontalAlignment.Right
                    };
                    coefficientBoxes[i, j].TextChanged += TextBox_TextChanged;
                    panelMatrix.Controls.Add(coefficientBoxes[i, j]);

                    Label varLabel = new Label
                    {
                        Text = $"x{j + 1}",
                        Location = new Point(coefficientBoxes[i, j].Right + spacing, 
                        startY + i * (boxHeight + spacing)),
                        AutoSize = true
                    };
                    panelMatrix.Controls.Add(varLabel);

                    if (j < n - 1)
                    {
                        Label plusLabel = new Label
                        {
                            Text = "+",
                            Location = new Point(varLabel.Right + spacing, 
                            startY + i * (boxHeight + spacing)),
                            AutoSize = true
                        };
                        panelMatrix.Controls.Add(plusLabel);
                    }
                }

                Label eqSignLabel = new Label
                {
                    Text = "=",
                    Location = new Point(coefficientBoxes[i, n - 1].Right + varLabelWidth + spacing * 2, 
                    startY + i * (boxHeight + spacing)),
                    AutoSize = true
                };
                panelMatrix.Controls.Add(eqSignLabel);

                constantTerms[i] = new TextBox
                {
                    Location = new Point(eqSignLabel.Right + spacing, 
                    startY + i * (boxHeight + spacing)),
                    Size = new Size(boxWidth, boxHeight),
                    Text = "0",
                    TextAlign = HorizontalAlignment.Right
                };
                constantTerms[i].TextChanged += TextBox_TextChanged;
                panelMatrix.Controls.Add(constantTerms[i]);
            }

            // Додати підказку про формат введення
            Label hintLabel = new Label
            {
                Text = "Введіть цілі числа (від -1000 до 1000) або дробові з крапкою або комою (максимум 4 знаки після коми)",
                Location = new Point(startX, startY + n * (boxHeight + spacing) + 20),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font(this.Font, FontStyle.Italic)
            };
            panelMatrix.Controls.Add(hintLabel);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            
            // Перевірка на коректність введення
            if (string.IsNullOrEmpty(text)) return;

            // Заміна коми на крапку
            if (text.Contains(","))
            {
                textBox.Text = text.Replace(",", ".");
                textBox.SelectionStart = textBox.Text.Length;
                return;
            }

            // Перевірка на кількість знаків після коми
            if (text.Contains("."))
            {
                string[] parts = text.Split('.');
                if (parts.Length > 1 && parts[1].Length > 4)
                {
                    textBox.Text = parts[0] + "." + parts[1].Substring(0, 4);
                    textBox.SelectionStart = textBox.Text.Length;
                    MessageBox.Show("Максимально допустима кількість знаків після коми - 4", 
                                    "Попередження", 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Warning);
                }
            }

            // Перевірка на коректність числа
            if (!Regex.IsMatch(text, @"^-?\d*\.?\d*$"))
            {
                int cursorPos = textBox.SelectionStart - 1;
                textBox.Text = text.Remove(cursorPos, 1);
                textBox.SelectionStart = cursorPos;
                MessageBox.Show("Дозволено вводити тільки цифри, знак мінус і крапку", 
                                "Попередження", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
            }
        }

        private bool ValidateNumberInput(string input, out double value)
        {
            value = 0;
            
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Заміна коми на крапку для уніфікації
            input = input.Replace(",", ".");

            // Перевірка формату числа
            if (!double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return false;

            // Перевірка кількості знаків після коми
            int decimalPlaces = 0;
            if (input.Contains("."))
            {
                decimalPlaces = input.Split('.')[1].Length;
                if (decimalPlaces > 4)
                {
                    MessageBox.Show("Максимально допустима кількість знаків після коми - 4", 
                                    "Попередження", 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void btnSetSize_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtSize.Text, out int newSize) && newSize >= 2 && newSize <= 10)
            {
                size = newSize;
                equationSystem = new EquationSystem(size);
                InitializeMatrixInputs(size);
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть ціле число від 2 до 10", "Помилка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (!ValidateNumberInput(coefficientBoxes[i, j].Text, out double coefficientValue))
                            throw new FormatException($"Невірний формат числа у коефіцієнті [{i + 1},{j + 1}]");

                        if (coefficientValue < -1000 || coefficientValue > 1000)
                            throw new ArgumentOutOfRangeException($"Коефіцієнт [{i + 1},{j + 1}] повинен бути в діапазоні від -1000 до 1000");

                        equationSystem.SetCoefficient(i, j, coefficientValue);
                    }

                    if (!ValidateNumberInput(constantTerms[i].Text, out double constantValue))
                        throw new FormatException($"Невірний формат вільного члена у рівнянні {i + 1}");

                    if (constantValue < -1000 || constantValue > 1000)
                        throw new ArgumentOutOfRangeException($"Вільний член у рівнянні {i + 1} повинен бути в діапазоні від -1000 до 1000");

                    equationSystem.SetConstant(i, constantValue);
                }

                using (StringWriter writer = new StringWriter())
                {
                    string selectedMethod = rbGaussian.Checked ? "Метод Гауса з одиничною діагоналлю" :
                                           rbRotation.Checked ? "Метод обертання" :
                                           "Метод Гауса-Холецького";
                    var solver = SolverFactory.CreateSolver(selectedMethod);

                    double[] solution = equationSystem.Solve(solver, writer);

                    txtSolutionSteps.Text = $"Метод: {solver.Name}\r\n";
                    txtSolutionSteps.Text += writer.ToString();

                    txtSolution.Text = "Розв'язок:\r\n";
                    if (solution != null)
                    {
                        for (int i = 0; i < solution.Length; i++)
                        {
                            txtSolution.Text += $"x{i + 1} = {solution[i]:F4}\r\n"; // Виводимо з точністю до 4 знаків
                        }
                    }
                    else
                    {
                        txtSolution.Text += "Розв'язок не знайдено або система невизначена.\r\n";
                    }

                    if (size == 2 && solution != null)
                    {
                        DrawGraph(solution);
                    }
                    else
                    {
                        chart1.Series.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSolution.Clear();
                txtSolutionSteps.Clear();
                chart1.Series.Clear();
            }
        }

        private void DrawGraph(double[] solution)
        {
            chart1.Series.Clear(); // Очищаємо всі існуючі серії (лінії та точки)
            chart1.ChartAreas[0].AxisX.Title = "x1"; // Назва осі X
            chart1.ChartAreas[0].AxisY.Title = "x2"; // Назва осі Y

            // Налаштування сітки та осей для кращої видимості
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray; // Колір основної сітки
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot; // Пунктирний стиль сітки
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;

            chart1.ChartAreas[0].AxisX.LineColor = Color.Black; // Колір ліній осей
            chart1.ChartAreas[0].AxisY.LineColor = Color.Black;
            chart1.ChartAreas[0].AxisX.LineWidth = 2; // Товщина ліній осей
            chart1.ChartAreas[0].AxisY.LineWidth = 2;

            chart1.ChartAreas[0].AxisX.Crossing = 0; // Перетин осі X на Y=0
            chart1.ChartAreas[0].AxisY.Crossing = 0; // Перетин осі Y на X=0
            chart1.ChartAreas[0].AxisX.IsStartedFromZero = false; // Дозволити осі починатися не з нуля
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;

            // Визначення діапазону для промальовки ліній та налаштування осей
            double xSolution = solution[0];
            double ySolution = solution[1];

            double plotRange = 5.0; // Початковий діапазон для промальовки
            double axisRange = 2.0; // Початковий діапазон для осей (для "зуму")

            // Якщо розв'язок дуже близький до (0,0), встановлюємо фіксований, чіткий діапазон
            if (Math.Abs(xSolution) < 1e-6 && Math.Abs(ySolution) < 1e-6) // Використовуємо поріг для "близько до нуля"
            {
                xSolution = 0.0; // Гарантуємо, що розв'язок буде сприйматися як точний нуль
                ySolution = 0.0;
                plotRange = 3.0; // Менший діапазон для промальовки
                axisRange = 2.0; // Фіксовані межі для осей від -2 до 2
                chart1.ChartAreas[0].AxisX.Interval = 0.5; // Зменшити інтервал для кращої деталізації
                chart1.ChartAreas[0].AxisY.Interval = 0.5;
            }
            else
            {
                // Динамічно розширюємо діапазон промальовки, якщо розв'язок великий
                plotRange = Math.Max(Math.Abs(xSolution), Math.Abs(ySolution)) * 1.5 + 2.0;
                axisRange = plotRange / 2.0; // Межі осей приблизно половина діапазону промальовки
                chart1.ChartAreas[0].AxisX.Interval = 1; // Задати інтервал за замовчуванням
                chart1.ChartAreas[0].AxisY.Interval = 1;
            }

            double minPlotX = xSolution - plotRange;
            double maxPlotX = xSolution + plotRange;
            double minPlotY = ySolution - plotRange;
            double maxPlotY = ySolution + plotRange;

            // Налаштування меж осей чарту
            chart1.ChartAreas[0].AxisX.Minimum = Math.Floor(xSolution - axisRange);
            chart1.ChartAreas[0].AxisX.Maximum = Math.Ceiling(xSolution + axisRange);
            chart1.ChartAreas[0].AxisY.Minimum = Math.Floor(ySolution - axisRange);
            chart1.ChartAreas[0].AxisY.Maximum = Math.Ceiling(ySolution + axisRange);

            // Забезпечення мінімального діапазону осей, якщо розв'язок не нуль, але діапазон занадто малий
            if (chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum < 3)
            {
                chart1.ChartAreas[0].AxisX.Minimum = xSolution - 1.5;
                chart1.ChartAreas[0].AxisX.Maximum = xSolution + 1.5;
            }
            if (chart1.ChartAreas[0].AxisY.Maximum - chart1.ChartAreas[0].AxisY.Minimum < 3)
            {
                chart1.ChartAreas[0].AxisY.Minimum = ySolution - 1.5;
                chart1.ChartAreas[0].AxisY.Maximum = ySolution + 1.5;
            }

            // Автоматична установка інтервалів сітки, якщо діапазон великий
            chart1.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chart1.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;


            // Промальовка ліній рівнянь
            for (int i = 0; i < size; i++)
            {
                var series = chart1.Series.Add($"Рівняння {i + 1}");
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 3; // Збільшена товщина лінії
                series.MarkerStyle = MarkerStyle.None; // Не використовуємо маркери на самих лініях, щоб не захаращувати

                // Вибір кольорів для ліній (можна використовувати палітру)
                if (i == 0) series.Color = Color.Blue;
                else if (i == 1) series.Color = Color.Orange;
                else series.Color = Color.DarkGreen; // Додатковий колір для більших систем

                double a1 = equationSystem.GetCoefficient(i, 0);
                double a2 = equationSystem.GetCoefficient(i, 1);
                double b = equationSystem.GetConstant(i);

                // Кількість точок для промальовки
                int numPoints = 200; // Збільшена кількість точок для більш гладких ліній
                double stepX = (maxPlotX - minPlotX) / numPoints;

                if (Math.Abs(a2) > 1e-9) // Якщо x2 не нуль, можемо виразити y = (b - a1*x) / a2
                {
                    for (int j = 0; j <= numPoints; j++)
                    {
                        double x = minPlotX + j * stepX;
                        double y = (b - a1 * x) / a2;
                        series.Points.AddXY(x, y);
                    }
                }
                else if (Math.Abs(a1) > 1e-9) // Якщо x1 не нуль, це вертикальна лінія x = b / a1
                {
                    double xConst = b / a1;
                    // Додаємо дві точки для вертикальної лінії в межах видимості
                    series.Points.AddXY(xConst, minPlotY);
                    series.Points.AddXY(xConst, maxPlotY);
                    series.BorderDashStyle = ChartDashStyle.Dash; // Можна зробити пунктирною для вертикальної лінії
                }
            }

            // Додаємо серію для точки розв'язку
            var solutionPoint = chart1.Series.Add("Розв'язок");
            solutionPoint.ChartType = SeriesChartType.Point;
            solutionPoint.MarkerSize = 12; // Збільшений розмір маркера
            solutionPoint.MarkerStyle = MarkerStyle.Circle; // Круглий маркер
            solutionPoint.Color = Color.Red; // Червоний колір
            solutionPoint.Points.AddXY(xSolution, ySolution); // Додаємо точку розв'язку

            solutionPoint.Label = $"({xSolution:F2}; {ySolution:F2})"; // Підпис точки
            solutionPoint.IsValueShownAsLabel = false; // Показувати підпис
            solutionPoint.LabelBackColor = Color.LightYellow; // Жовтий фон для підпису для кращої читабельності
            solutionPoint.LabelBorderColor = Color.Gray; // Сіра рамка підпису
            solutionPoint.LabelBorderWidth = 1; // Товщина рамки підпису
            solutionPoint.Font = new Font("Arial", 9, FontStyle.Bold); // Шрифт підпису
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "Зберегти результати";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            writer.WriteLine("Розв'язок системи рівнянь");
                            writer.WriteLine($"Дата та час збереження: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                            writer.WriteLine(txtSolution.Text);
                            writer.WriteLine();
                            writer.WriteLine("Покроковий розв'язок:");
                            writer.WriteLine(txtSolutionSteps.Text);
                        }

                        MessageBox.Show("Результати успішно збережено", "Успіх", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при збереженні файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}
