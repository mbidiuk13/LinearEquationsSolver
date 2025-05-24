using System.IO;

namespace LinearEquationsSolver
{
    public interface ISolver
    {
        double[] Solve(double[,] matrix, TextWriter writer);
        string Name { get; }
    }
}