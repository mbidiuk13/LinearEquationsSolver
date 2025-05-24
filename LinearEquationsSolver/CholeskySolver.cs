using System;
using System.IO;
using LinearEquationsSolver;

public class CholeskySolver : ISolver
{
    public string Name => "Метод Гауса-Холецького (квадратного кореня)";

    public double[] Solve(double[,] initialMatrix, TextWriter writer)
    {
        int n = initialMatrix.GetLength(0);
        double[,] matrix = new double[n, n + 1];
        MatrixHelper.CopyMatrix(initialMatrix, matrix);

        writer.WriteLine("Початкова матриця:");
        MatrixHelper.PrintMatrix(matrix, writer);

        int iterationCount = 0; // Лічильник ітерацій внутрішнього циклу

        // Перевірка на симетричність
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < i; j++)
            {
                iterationCount++; // Збільшуємо лічильник для кожної ітерації перевірки
                if (Math.Abs(matrix[i, j] - matrix[j, i]) > 1e-10)
                {
                    throw new InvalidOperationException("Матриця не симетрична для методу Холецького");
                }
            }
        }

        double[,] L = new double[n, n];

        writer.WriteLine("\nРозклад A = L * L^T:");

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = 0;
                if (j == i)
                {
                    for (int k = 0; k < j; k++)
                    {
                        sum += L[j, k] * L[j, k];
                        iterationCount++; // Збільшуємо лічильник для кожної ітерації суми
                        writer.WriteLine($"  Додаємо L[{j + 1},{k + 1}]² = {L[j, k]:F4}² = {L[j, k] * L[j, k]:F4}");
                    }

                    double value = matrix[j, j] - sum;
                    writer.WriteLine($"  L[{j + 1},{j + 1}] = √({matrix[j, j]:F4} - {sum:F4}) = √({value:F4})");

                    if (value <= 0)
                    {
                        throw new InvalidOperationException("Матриця не є додатно визначеною");
                    }

                    L[j, j] = Math.Sqrt(value);
                }
                else
                {
                    for (int k = 0; k < j; k++)
                    {
                        sum += L[i, k] * L[j, k];
                        iterationCount++; // Збільшуємо лічильник для кожної ітерації суми
                        writer.WriteLine($"  Додаємо L[{i + 1},{k + 1}] * L[{j + 1},{k + 1}] = {L[i, k]:F4} * {L[j, k]:F4} = {L[i, k] * L[j, k]:F4}");
                    }

                    double numerator = matrix[i, j] - sum;
                    writer.WriteLine($"  L[{i + 1},{j + 1}] = ({matrix[i, j]:F4} - {sum:F4}) / {L[j, j]:F4} = {numerator:F4} / {L[j, j]:F4}");

                    L[i, j] = numerator / L[j, j];
                }
            }

            writer.WriteLine($"\nМатриця L після обчислення рядка {i + 1}:");
            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    writer.Write($"{L[row, col],10:F4}");
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        // Розв’язання Ly = b
        writer.WriteLine("Розв’язання L·y = b:");
        double[] yVec = new double[n];
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < i; j++)
            {
                sum += L[i, j] * yVec[j];
                iterationCount++; // Збільшуємо лічильник для кожної ітерації суми
                writer.WriteLine($"  Додаємо L[{i + 1},{j + 1}] * y[{j + 1}] = {L[i, j]:F4} * {yVec[j]:F4} = {L[i, j] * yVec[j]:F4}");
            }

            double numerator = matrix[i, n] - sum;
            yVec[i] = numerator / L[i, i];
            writer.WriteLine($"  y[{i + 1}] = ({matrix[i, n]:F4} - {sum:F4}) / {L[i, i]:F4} = {yVec[i]:F4}");
        }

        writer.WriteLine("\nВектор y:");
        for (int i = 0; i < n; i++)
        {
            writer.WriteLine($"  y[{i + 1}] = {yVec[i]:F4}");
        }
        writer.WriteLine();

        // Розв’язання L^T x = y
        writer.WriteLine("Розв’язання Lᵗ·x = y:");
        double[] xVec = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            double sum = 0;
            for (int j = i + 1; j < n; j++)
            {
                sum += L[j, i] * xVec[j];
                iterationCount++; // Збільшуємо лічильник для кожної ітерації суми
                writer.WriteLine($"  Додаємо Lᵗ[{i + 1},{j + 1}] * x[{j + 1}] = {L[j, i]:F4} * {xVec[j]:F4} = {L[j, i] * xVec[j]:F4}");
            }

            double numerator = yVec[i] - sum;
            xVec[i] = numerator / L[i, i];
            writer.WriteLine($"  x[{i + 1}] = ({yVec[i]:F4} - {sum:F4}) / {L[i, i]:F4} = {xVec[i]:F4}");
        }

        writer.WriteLine("\nОстаточний розв’язок:");
        for (int i = 0; i < n; i++)
        {
            writer.WriteLine($"  x[{i + 1}] = {xVec[i]:F6}");
        }

        writer.WriteLine($"\nКількість ітерацій внутрішнього циклу: {iterationCount}");

        return xVec;
    }
}