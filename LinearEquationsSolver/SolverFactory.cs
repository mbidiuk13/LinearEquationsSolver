using System;
using LinearEquationsSolver;

namespace LinearEquationSolver
{
    public static class SolverFactory
    {
        public static ISolver CreateSolver(string methodName)
        {
            switch (methodName)
            {
                case "Метод Гауса з одиничною діагоналлю":
                    return new GaussianSolver();
                case "Метод обертання":
                    return new RotationSolver();
                case "Метод Гауса-Холецького":
                    return new CholeskySolver();
                default:
                    throw new ArgumentException("Невідомий метод розв'язання");
            }
        }
    }
}