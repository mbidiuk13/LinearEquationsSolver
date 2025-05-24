using System;
using System.IO;

public static class MatrixHelper
{
    public static void CopyMatrix(double[,] source, double[,] destination)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                destination[i, j] = source[i, j];
            }
        }
    }

    public static void PrintMatrix(double[,] matrix, TextWriter writer, bool showVariables = false)
    {
        int n = matrix.GetLength(0);
    
        // Визначаємо максимальну довжину числа для вирівнювання
        int maxLength = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= n; j++)
            {
                int currentLength = $"{matrix[i, j]:F2}".Length;
                if (currentLength > maxLength) maxLength = currentLength;
            }
        }
    
        for (int i = 0; i < n; i++)
        {
            // Виведення коефіцієнтів
            for (int j = 0; j < n; j++)
            {
                if (showVariables)
                {
                    string coeff = $"{matrix[i, j],8:F2}";
                    string sign = j < n - 1 ? "+" : "";
                    writer.Write($"{coeff}x{j + 1} {sign} ");
                }
                else
                {
                    writer.Write($"{matrix[i, j],10:F2}");
                }
            }
        
            // Виведення вільного члена
            if (showVariables)
            {
                writer.Write($"= {matrix[i, n],8:F2}");
            }
            else
            {
                writer.Write($" | {matrix[i, n],10:F2}");
            }
            writer.WriteLine();
        }
        writer.WriteLine();
    }
}