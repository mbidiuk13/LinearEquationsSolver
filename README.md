# Linear Equations Solver

## Overview
This repository contains a C# application designed to solve Systems of Linear Algebraic Equations (SLAE) using advanced numerical methods. 

As a Software Engineering student transitioning into Data Analytics, I developed this project to strengthen my algorithmic thinking and apply linear algebra concepts in software development. Understanding these underlying mathematical models is crucial for data processing and machine learning algorithms.

## Implemented Mathematical Methods
The application supports the following numerical methods for solving linear systems:

* **Gaussian Elimination (Метод Гауса):** A classic algorithmic process for solving SLAE by transforming the system's matrix into an upper triangular form through elementary row operations.
* **Gauss-Cholesky / Square Root Method (Метод Гауса-Холецького):** An efficient algorithm optimized for symmetric, positive-definite matrices, decomposing the matrix into the product of a lower triangular matrix and its transpose.
* **Givens Rotation Method (Метод обертання):** A highly numerically stable orthogonal transformation method used to introduce zeros into matrices, particularly useful for QR decomposition and avoiding division by zero errors.
