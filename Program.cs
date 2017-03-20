using System;

namespace CSR_Operations
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataFilePath = @"C:\Users\pkara\OneDrive\College\MTH 343\Final Project\CSR_Operations\CSR_Operations\M1.txt";
            //demoCSROperations(dataFilePath);

            dataFilePath = @"C:\Users\pkara\OneDrive\College\MTH 343\Final Project\CSR_Operations\CSR_Operations\M2.txt";
            GMRES_demo(dataFilePath);

            Console.Write("\n\nType anything and press enter to exit : ");
            Console.ReadLine();
        }

        /* Demonstrates the following implemented library functions :
           1. Matrix (stored in CSR format) times a vector
           2. Transpose of a Matrix (stored in CSR format), resulting matrix is stored in CSR format
           3. Matrix (stored in CSR format) times a matrix (stored in CSR format), resulting matrix is stored in CSR format

           dataFilePath is path to file containing matrix stored in either CSRFromat, NormalFormat or MTH343Format
         */

        static void demoCSROperations(String dataFilePath)
        {
            Console.WriteLine("Following is the given matrix : \n");

            Matrix_CSR_Format m;

            try
            {
                m = new Matrix_CSR_Format(dataFilePath, FileType.NormalFormat);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError : Something might have wrong while reading data file");
                Console.WriteLine(e.Message);

                Console.Write("\n\nType anything and press enter to exit : ");
                Console.ReadLine();
                return;
            }

            //m.printMatrix();
            m.printMatrixInCSR();

            Console.WriteLine("\n\n*** Multiplying above matrix with a column vector ***\n");
            double[] columnVector = generateVector(m.NumOfColumns, 2);
            double[] product = Matrix_CSR_Format_Operations.MatrixTimesComlumVector(m, columnVector);

            if (product != null)
            {
                foreach (double i in product) Console.WriteLine("{0:0.000}", i);
            }
            else
            {
                Console.WriteLine("Matrix times a vector is not possible, check the size of both the matrix as well as the vector");
            }

            Console.WriteLine("\n\n*** Transpose of matrix ***\n");
            Matrix_CSR_Format transposeMatrix = Matrix_CSR_Format_Operations.Transpose(m);
            //transposeMatrix.printMatrix();
            transposeMatrix.printMatrixInCSR();

            Console.WriteLine("\n\n*** Product of 2 matrices ***:\n");

            try
            {
                Matrix_CSR_Format productMatrix = Matrix_CSR_Format_Operations.Multiply(m, transposeMatrix);

                if (productMatrix != null)
                {
                    //productMatrix.printMatrix();
                    productMatrix.printMatrixInCSR();
                }
                else
                {
                    Console.WriteLine("Matrix multiplication is not possible, check the sizes of both the matrices");
                }

            }
            catch (FormatException e)
            {
                Console.WriteLine("\nERROR : The inputs passed on to Matrix_CSR_Format_Operations.innerProduct() were not in proper format");
                Console.WriteLine("Multiplication of 2 matrices failed !");
            }
        }

        /*
            The function below demonstrates a sample use of the GMRES algorithm on an unsymmetric sparse matrix.
            The GMRES class [more specifically, GMRES.solveUsingGMRES()] can be used to solve Ax = b where A is
            an unsymmetric sparse matrix. 

            dataFilePath is path to file containing matrix stored in either CSRFromat, NormalFormat or MTH343Format
         */
        static void GMRES_demo(String dataFilePath)
        {
            Matrix_CSR_Format A = new Matrix_CSR_Format(dataFilePath, FileType.MTH343Format);
            double[] ranX = generateVector(A.NumOfColumns, 3);
            double[] b = Matrix_CSR_Format_Operations.MatrixTimesComlumVector(A, ranX);

            double[] initialX = generateVector(A.NumOfColumns, 75);
            double tolerance = 10E-6;
            int maxIterations = A.NumOfColumns;
            int restartAfterEveryNIterations = A.NumOfColumns;

            DateTime startTime = DateTime.Now;

            GMRES.solveUsingGMRES(tolerance, maxIterations, restartAfterEveryNIterations, A, b, initialX);

            DateTime endTime = DateTime.Now;

            Console.WriteLine("\nTotal Time taken = {0} seconds", endTime.Subtract(startTime).TotalSeconds);
        }

        // generates a vector with size length and initializes each entry in the vector with initializeWith
        static double[] generateVector(int length, int initializeWith)
        {
            double[] vector = new double[length];

            for(int i = 0; i < length; i++)
            {
                vector[i] = initializeWith;
            }

            return vector;
        }
    }
}