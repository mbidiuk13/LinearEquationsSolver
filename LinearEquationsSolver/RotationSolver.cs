using System;
using System.IO;
using LinearEquationsSolver;

public class RotationSolver : ISolver
{
    public string Name => "Метод обертання";

    public double[] Solve(double[,] initialMatrix, TextWriter writer)
    {
        int n = initialMatrix.GetLength(0);
        double[,] matrix = new double[n, n + 1];
        MatrixHelper.CopyMatrix(initialMatrix, matrix);

        writer.WriteLine("Початкова розширена матриця:");
        MatrixHelper.PrintMatrix(matrix, writer);

        int iterationCount = 0; // Лічильник ітерацій внутрішнього циклу

        // Прямий хід — обнулення нижче головної діагоналі
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                double aii = matrix[i, i];
                double aji = matrix[j, i];
                double norm = Math.Sqrt(aii * aii + aji * aji);

                if (norm < 1e-10)
                    continue;

                double cos = aii / norm;
                double sin = aji / norm;

                writer.WriteLine($"\n Обертання між рядками {i + 1} і {j + 1}:");
                writer.WriteLine($"   a[{i + 1},{i + 1}] = {aii:F4}, a[{j + 1},{i + 1}] = {aji:F4}");
                writer.WriteLine($"   Обчислено: norm = √({aii:F4}² + {aji:F4}²) = {norm:F4}");
                writer.WriteLine($"   cos = {cos:F4}, sin = {sin:F4}");

                for (int k = i; k <= n; k++)
                {
                    double aik = matrix[i, k];
                    double ajk = matrix[j, k];

                    double newI = cos * aik + sin * ajk;
                    double newJ = -sin * aik + cos * ajk;

                    writer.WriteLine($"     Стовпець {k + 1}:");
                    writer.WriteLine($"     a[{i + 1},{k + 1}] = {aik:F4} → {newI:F4}");
                    writer.WriteLine($"     a[{j + 1},{k + 1}] = {ajk:F4} → {newJ:F4}");

                    matrix[i, k] = newI;
                    matrix[j, k] = newJ;

                    iterationCount++; // Збільшуємо лічильник для кожної ітерації внутрішнього циклу
                }

                writer.WriteLine("     Матриця після цього обертання:");
                MatrixHelper.PrintMatrix(matrix, writer);
            }
        }

        // Зворотній хід — обчислення розв'язку
        writer.WriteLine("\n Зворотній хід (обчислення розв’язку):");
        double[] result = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            if (Math.Abs(matrix[i, i]) < 1e-10)
            {
                if (Math.Abs(matrix[i, n]) < 1e-10)
                    throw new InvalidOperationException("Система має нескінченну кількість розв'язків");
                else
                    throw new InvalidOperationException("Система не має розв'язків");
            }

            // Обчислюємо суму вже відомих значень
            double sum = 0;
            for (int j = i + 1; j < n; j++)
            {
                sum += matrix[i, j] * result[j];
                iterationCount++; // Збільшуємо лічильник для зворотного ходу
            }

            double numerator = matrix[i, n] - sum;
            result[i] = numerator / matrix[i, i];

            writer.WriteLine($"x[{i + 1}] = ({matrix[i, n]:F4} - {sum:F4}) / {matrix[i, i]:F4} = {result[i]:F6}".Replace('.', ','));
        }

        writer.WriteLine("\n Остаточний розв’язок:");
        for (int i = 0; i < n; i++)
            writer.WriteLine($"    x[{i + 1}] = {result[i]:F6}");

        writer.WriteLine($"\n Кількість ітерацій внутрішнього циклу: {iterationCount}");

        return result;
    }
}