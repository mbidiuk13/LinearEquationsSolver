using System;
using System.IO;
using LinearEquationsSolver;

public class GaussianSolver : ISolver
{
    public string Name => "Метод Гауса з одиничною діагоналлю";

    public double[] Solve(double[,] initialMatrix, TextWriter writer)
    {
        int n = initialMatrix.GetLength(0);
        double[,] matrix = new double[n, n + 1];
        MatrixHelper.CopyMatrix(initialMatrix, matrix);

        writer.WriteLine("Початкова розширена матриця:");
        MatrixHelper.PrintMatrix(matrix, writer);

        int iterationCount = 0; // Лічильник ітерацій внутрішнього циклу

        for (int i = 0; i < n; i++)
        {
            writer.WriteLine($"\n--- Крок {i + 1} ---");

            // Пошук опорного елемента
            if (Math.Abs(matrix[i, i]) < 1e-10)
            {
                bool found = false;
                for (int k = i + 1; k < n; k++)
                {
                    iterationCount++; // Збільшуємо лічильник для кожної ітерації пошуку
                    if (Math.Abs(matrix[k, i]) > 1e-10)
                    {
                        SwapRows(matrix, i, k);
                        writer.WriteLine($"Перестановка рядків {i + 1} та {k + 1}");
                        MatrixHelper.PrintMatrix(matrix, writer);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new InvalidOperationException("Система не має розв’язків або має безліч розв’язків.");
                }
            }

            // Нормалізація опорного елемента
            double pivot = matrix[i, i];
            for (int j = 0; j <= n; j++)
            {
                matrix[i, j] /= pivot;
                iterationCount++; // Збільшуємо лічильник для кожної ітерації нормалізації
            }
            writer.WriteLine($"Нормалізація рядка {i + 1}, ділення на {pivot:F4}");
            MatrixHelper.PrintMatrix(matrix, writer);

            // Обнулення у всіх інших рядках
            for (int k = 0; k < n; k++)
            {
                if (k == i) continue;

                double factor = matrix[k, i];
                for (int j = 0; j <= n; j++)
                {
                    matrix[k, j] -= factor * matrix[i, j];
                    iterationCount++; // Збільшуємо лічильник для кожної ітерації обнулення
                }
                writer.WriteLine($"Обнулення стовпця {i + 1} в рядку {k + 1} (множення на {factor:F4})");
                MatrixHelper.PrintMatrix(matrix, writer);
            }
        }

        // Зчитування результату
        double[] result = new double[n];
        for (int i = 0; i < n; i++)
        {
            result[i] = matrix[i, n];
        }

        writer.WriteLine("\nРозв'язок системи:");
        for (int i = 0; i < n; i++)
        {
            writer.WriteLine($"x[{i + 1}] = {result[i]:F4}");
        }

        writer.WriteLine($"\nКількість ітерацій внутрішнього циклу: {iterationCount}");

        return result;
    }

    // Допоміжні методи
    static void SwapRows(double[,] matrix, int row1, int row2)
    {
        int cols = matrix.GetLength(1);
        for (int i = 0; i < cols; i++)
        {
            double temp = matrix[row1, i];
            matrix[row1, i] = matrix[row2, i];
            matrix[row2, i] = temp;
        }
    }

    static void PrintMatrix(double[,] matrix, TextWriter writer)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                writer.Write($"{matrix[i, j],10:F4} "); // Форматування виводу
            }
            writer.WriteLine();
        }
    }
}