using System.IO;
using LinearEquationsSolver;

public class EquationSystem
{
    private double[,] matrix;
    private int size;

    public EquationSystem(int size)
    {
        this.size = size;
        this.matrix = new double[size, size + 1];
    }

    public void SetCoefficient(int row, int col, double value)
    {
        if (row >= 0 && row < size && col >= 0 && col < size)
            matrix[row, col] = value;
    }
    
    public double GetCoefficient(int row, int col)
    {
        if (row >= 0 && row < size && col >= 0 && col < size)
            return matrix[row, col];
        return 0;
    }

    public void SetConstant(int row, double value)
    {
        if (row >= 0 && row < size)
            matrix[row, size] = value;
    }
    
    public double GetConstant(int row)
    {
        if (row >= 0 && row < size)
            return matrix[row, size];
        return 0;
    }

    public double[] Solve(ISolver solver, TextWriter writer)
    {
        return solver.Solve(matrix, writer);
    }

    public void Print(TextWriter writer)
    {
        MatrixHelper.PrintMatrix(matrix, writer);
    }
}