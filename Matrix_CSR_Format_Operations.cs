using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSR_Operations
{
    static class Matrix_CSR_Format_Operations
    {
        // return null if multiplication is not possible
        public static Matrix_CSR_Format MatrixTimesComlumVector(Matrix_CSR_Format m, decimal[] columnVector)
        {
            if (m.NumOfColumns != columnVector.Length) return null;

            decimal[] nonZeroEntries = m.NonZeroEntries;
            int[] rowInfo = m.RowInfo;
            int[] columnInfo = m.ColumnInfo;

            List<int> productColumnInfo = new List<int>(), productRowInfo = new List<int>();
            List<decimal> productNonZeroEntries = new List<decimal>();
            productRowInfo.Add(0);

            decimal sum = 0;
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

                    productNonZeroEntries.Add(sum);
                    productColumnInfo.Add(0);
                    productRowInfo.Add(productRowInfo.Last() + 1);
                }
                else
                {
                    productRowInfo.Add(productRowInfo.Last());
                }

                sum = 0;
            }

            return new Matrix_CSR_Format(productNonZeroEntries, productRowInfo, productColumnInfo, 1);
        }

        public static Matrix_CSR_Format Transpose(Matrix_CSR_Format m)
        {
            int numOfRowsInTranspose = m.NumOfColumns;

            Csr_matrix_info[] newMatrixRowInfo = new Csr_matrix_info[numOfRowsInTranspose];

            for (int i = 0; i < numOfRowsInTranspose; i++)
            {
                newMatrixRowInfo[i].nonZeroEntries = new List<decimal>();
                newMatrixRowInfo[i].columnInfo = new List<int>();
            }

            for(int i = 0; i < m.NonZeroEntries.Length; i++)
            {
                newMatrixRowInfo[m.ColumnInfo[i]].nonZeroEntries.Add(m.NonZeroEntries[i]);
                newMatrixRowInfo[m.ColumnInfo[i]].columnInfo.Add(m.getRowNumber(i));
            }

            List<decimal> nonZeroEntries = new List<decimal>();
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

            Csr_matrix_info[] newMatrixRowInfo = new Csr_matrix_info[numOfRowsInProductMatrix];

            for (int i = 0; i < numOfRowsInProductMatrix; i++)
            {
                newMatrixRowInfo[i].nonZeroEntries = new List<decimal>();
                newMatrixRowInfo[i].columnInfo = new List<int>();
            }

            int[,] m1RowBounds = m1.RowBounds, m2RowBounds = m2.RowBounds;
            decimal sum = 0;
            int operationsCount = 0;

            // for each new row of product matrix
            for (int i = 0; i < numOfRowsInProductMatrix; i++)
            {
                // if row of m1 has no non-zero elements, continue
                if (m1RowBounds[i, 0] == -1 || m1RowBounds[i, 1] == -1) continue;
                
                // for each column in m2
                for (int j = 0; j < m2.NumOfColumns; j++)
                {
                    for(int k = m1RowBounds[i, 0]; k <= m1RowBounds[i, 1]; k++)
                    {
                        if (m2RowBounds[m1.ColumnInfo[k], 0] == -1 || m2RowBounds[m1.ColumnInfo[k], 1] == -1) continue;

                        for(int z = m2RowBounds[m1.ColumnInfo[k], 0]; z <= m2RowBounds[m1.ColumnInfo[k], 1]; z++)
                        {
                            if(m2.ColumnInfo[z] == j)
                            {
                                operationsCount += 1;
                                sum += (m1.NonZeroEntries[k] * m2.NonZeroEntries[z]);
                            }
                        }
                    }

                    if(sum != 0)
                    {
                        newMatrixRowInfo[i].nonZeroEntries.Add(sum);
                        newMatrixRowInfo[i].columnInfo.Add(j);
                        sum = 0;
                    }  
                }
            }

            List<decimal> nonZeroEntries = new List<decimal>();
            List<int> columnInfo = new List<int>();
            int[] rowInfo = new int[numOfRowsInProductMatrix + 1];

            for (int i = 0; i < newMatrixRowInfo.Length; i++)
            {
                nonZeroEntries.AddRange(newMatrixRowInfo[i].nonZeroEntries);
                columnInfo.AddRange(newMatrixRowInfo[i].columnInfo);

                rowInfo[i + 1] = rowInfo[i] + newMatrixRowInfo[i].nonZeroEntries.Count;
            }

            return new Matrix_CSR_Format(nonZeroEntries, rowInfo.ToList<int>(), columnInfo, m2.NumOfColumns);
        }

        private struct Csr_matrix_info
        {
            public List<int> columnInfo ;
            public List<decimal> nonZeroEntries;
        }
    }
}