using System;
using System.Collections.Generic;
using System.Linq;

namespace CSR_Operations
{
    static class Matrix_CSR_Format_Operations
    {
        // return null if multiplication is not possible
        public static double[] MatrixTimesComlumVector(Matrix_CSR_Format m, double[] columnVector)
        {
            if (m.NumOfColumns != columnVector.Length) return null;

            double[] nonZeroEntries = m.NonZeroEntries;
            int[] rowInfo = m.RowInfo;
            int[] columnInfo = m.ColumnInfo;

            double[] productVector = new double[m.NumOfRows];
            double sum = 0;
            int lowerBound = 0, upperBound = 0;

            // traverse through all rows
            for(int rowNum = 0; rowNum < m.NumOfRows; rowNum++)
            {
                lowerBound = rowInfo[rowNum];
                upperBound = rowInfo[rowNum + 1] - 1;

                // if row has at least one non-zero int in it
                if (lowerBound <= upperBound)
                {
                    for (int i = lowerBound; i <= upperBound; i++)
                    {
                        sum += (nonZeroEntries[i] * columnVector[columnInfo[i]]);
                    }

                    productVector[rowNum] = sum;
                }

                sum = 0;
            }

            return productVector;
        }

        public static Matrix_CSR_Format Transpose(Matrix_CSR_Format m)
        {
            int numOfRowsInTranspose = m.NumOfColumns;

            Csr_matrix_info[] newMatrixRowInfo = new Csr_matrix_info[numOfRowsInTranspose];

            for (int i = 0; i < numOfRowsInTranspose; i++)
            {
                newMatrixRowInfo[i].nonZeroEntries = new List<double>();
                newMatrixRowInfo[i].columnInfo = new List<int>();
            }

            for(int i = 0; i < m.NonZeroEntries.Length; i++)
            {
                newMatrixRowInfo[m.ColumnInfo[i]].nonZeroEntries.Add(m.NonZeroEntries[i]);
                newMatrixRowInfo[m.ColumnInfo[i]].columnInfo.Add(m.getRowNumber(i));
            }

            List<double> nonZeroEntries = new List<double>();
            List<int> columnInfo = new List<int>();
            int[] rowInfo = new int[numOfRowsInTranspose + 1];

            for (int i = 0; i < newMatrixRowInfo.Length; i++)
            {
                nonZeroEntries.AddRange(newMatrixRowInfo[i].nonZeroEntries);
                columnInfo.AddRange(newMatrixRowInfo[i].columnInfo);

                rowInfo[i + 1] = rowInfo[i] + newMatrixRowInfo[i].nonZeroEntries.Count;
            }

            return new Matrix_CSR_Format(nonZeroEntries, rowInfo.ToList<int>(), columnInfo, m.NumOfRows);
        }

        //return null if matrix multiplication is not possible
        public static Matrix_CSR_Format Multiply(Matrix_CSR_Format m1, Matrix_CSR_Format m2)
        {
            if (m1.NumOfColumns != m2.NumOfRows) return null;

            int numOfRowsInProductMatrix = m1.NumOfRows;

            //create dynimically expanding lists which will contain non-zero entries and their corresponding column location
            //for each row of product matrix
            Csr_matrix_info[] newMatrixRowInfo = new Csr_matrix_info[numOfRowsInProductMatrix];

            // initialize the lists
            for (int i = 0; i < numOfRowsInProductMatrix; i++)
            {
                newMatrixRowInfo[i].nonZeroEntries = new List<double>();
                newMatrixRowInfo[i].columnInfo = new List<int>();
            }

            int[,] m1RowBounds = m1.RowBounds, m2RowBounds = m2.RowBounds;
            double sum = 0;
            int operationsCount = 0;    // will help keep track of the number of times the most significant operation is executed

            // for each row of in m1
            for (int i = 0; i < numOfRowsInProductMatrix; i++)
            {
                // if row of m1 has no non-zero elements, skip this iteration
                if (m1RowBounds[i, 0] == -1 || m1RowBounds[i, 1] == -1) continue;
                
                // for each column in m2
                for (int j = 0; j < m2.NumOfColumns; j++)
                {
                    // get lower and upper bounds for row i in first matrix m1
                    for(int k = m1RowBounds[i, 0]; k <= m1RowBounds[i, 1]; k++)
                    {
                        // if row m1.ColumnInfo[k] in m2 has no non-zero entries, skip this iteration
                        if (m2RowBounds[m1.ColumnInfo[k], 0] == -1 || m2RowBounds[m1.ColumnInfo[k], 1] == -1) continue;

                        // for each non-zero entry in row m1.ColumnInfo[k] of m2
                        for(int z = m2RowBounds[m1.ColumnInfo[k], 0]; z <= m2RowBounds[m1.ColumnInfo[k], 1]; z++)
                        {
                            // If Aij == Ajk exists for desired i and j, caulculate their product and add them to sum
                            if(m2.ColumnInfo[z] == j)
                            {
                                operationsCount += 1;
                                sum += (m1.NonZeroEntries[k] * m2.NonZeroEntries[z]);
                            }
                        }
                    }

                    //if sum is not zero, add it as a new entry along with its column location to the appropriate list
                    //corresponding to its new row in the product matrix
                    if(sum != 0)
                    {
                        newMatrixRowInfo[i].nonZeroEntries.Add(sum);
                        newMatrixRowInfo[i].columnInfo.Add(j);
                        sum = 0;
                    }  
                }
            }
            
            // 2 lists and array below will be used to store information about the product in CSR format
            List<double> nonZeroEntries = new List<double>();
            List<int> columnInfo = new List<int>();
            int[] rowInfo = new int[numOfRowsInProductMatrix + 1];

            //merge all lists created at the begining of the method
            for (int i = 0; i < newMatrixRowInfo.Length; i++)
            {
                nonZeroEntries.AddRange(newMatrixRowInfo[i].nonZeroEntries);
                columnInfo.AddRange(newMatrixRowInfo[i].columnInfo);

                rowInfo[i + 1] = rowInfo[i] + newMatrixRowInfo[i].nonZeroEntries.Count;
            }

            return new Matrix_CSR_Format(nonZeroEntries, rowInfo.ToList<int>(), columnInfo, m2.NumOfColumns);
        }

        // this struct helps store columnInfo and nonZeroEntries for a row in a matrix
        private struct Csr_matrix_info
        {
            public List<int> columnInfo ;
            public List<double> nonZeroEntries;
        }
    }
}