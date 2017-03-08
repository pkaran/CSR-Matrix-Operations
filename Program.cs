using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSR_Operations
{
    class Program
    {
        static String dataFileRelativePath = @"Data\CSRFormat\M1.txt";
        
        static void Main(string[] args)
        {
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDir = System.IO.Path.GetDirectoryName(exeLocation);
            string dataFilePath = System.IO.Path.Combine(exeDir, dataFileRelativePath);

            Console.WriteLine("Following is the given matrix : \n");

            Matrix_CSR_Format m;

            try
            {
                m = new Matrix_CSR_Format(dataFilePath, FileType.CSRFromat);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError : Something might have wrong while reading data file");
                Console.WriteLine(e.Message);

                Console.Write("\n\nType anything and press enter to exit : ");
                Console.ReadLine();
                return;
            }

            m.printMatrix();
            m.printMatrixInCSR();

            Console.WriteLine("\n\n*** Multiplying above matrix with a column vector ***\n");
            int[] columnVector = new int[] { 1, 2, 3, 4 };
            Matrix_CSR_Format product = Matrix_CSR_Format_Operations.MatrixTimesVector(m, columnVector);

            if (product != null)
            {
                product.printMatrix();
                product.printMatrixInCSR();
            }
            else
            {
                Console.WriteLine("Matrix times a vector is not possible, check the size of both the matrix as well as the vector");
            }

            Console.WriteLine("\n\n*** Transpose of matrix ***\n");
            Matrix_CSR_Format transposeMatrix = Matrix_CSR_Format_Operations.Transpose(m);
            transposeMatrix.printMatrix();
            transposeMatrix.printMatrixInCSR();

            Console.WriteLine("\n\n*** Product of 2 matrices ***:\n");
            
            try
            {
                Matrix_CSR_Format productMatrix = Matrix_CSR_Format_Operations.Multiply(m, transposeMatrix);

                if(productMatrix != null)
                {
                    productMatrix.printMatrix();
                    productMatrix.printMatrixInCSR();
                }
                else
                {
                    Console.WriteLine("Matrix multiplication is not possible, check the sizes of both the matrices");
                }
                
            }
            catch(FormatException e)
            {
                Console.WriteLine("\nERROR : The inputs passed on to Matrix_CSR_Format_Operations.innerProduct() were not in proper format");
                Console.WriteLine("Multiplication of 2 matrices failed !");
            }

            Console.Write("\n\nType anything and press enter to exit : ");
            Console.ReadLine();
        }
    }
}
