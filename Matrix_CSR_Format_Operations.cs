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
        public static Matrix_CSR_Format MatrixTimesVector(Matrix_CSR_Format m, int[] columnVector)
        {
            if (m.NumOfColumns != columnVector.Length) return null;

            int[] nonZeroEntries = m.NonZeroEntries;
            int[] newNonZeroEntries = new int[nonZeroEntries.Length];
            int[] columnInfo = m.ColumnInfo;

            for(int i = 0; i < nonZeroEntries.Length; i++)
            {
                newNonZeroEntries[i] = nonZeroEntries[i] * columnVector[columnInfo[i]];
            }

            return new Matrix_CSR_Format(newNonZeroEntries.ToList(), m.RowInfo.ToList(), m.ColumnInfo.ToList(), m.NumOfColumns);
        }

        public static Matrix_CSR_Format Transpose(Matrix_CSR_Format m)
        {
            Matrix_CSR_Format mTranspose = new Matrix_CSR_Format(null, null, null, m.NumOfRows);

            for(int i = 0; i < m.NumOfColumns; i++)
            {
                mTranspose.addRow(m.getColumn(i));
            }

            return mTranspose;
        }

        //return null if matrix multiplication is not possible
        public static Matrix_CSR_Format Multiply(Matrix_CSR_Format m1, Matrix_CSR_Format m2)
        {
            if (m1.NumOfColumns != m2.NumOfRows) return null;

            Matrix_CSR_Format productMatrix = new Matrix_CSR_Format(null, null, null, m2.NumOfColumns);
            int[] newRow = new int[m2.NumOfColumns];
            int temp = 0;

            for(int i = 0; i < m1.NumOfRows; i++)
            {
                for(int j = 0; j < m2.NumOfColumns; j++)
                {
                    newRow[temp++] = innerProduct(m1.getRow(i), m2.getColumn(j));
                }

                productMatrix.addRow(newRow);
                temp = 0;
                Array.Clear(newRow, 0, newRow.Length);
            }

            return productMatrix;
        }

        // throws FormatException if the length of arr1 and arr2 are not equal
        private static int innerProduct(int[] arr1, int[] arr2)
        {
            if(arr1.Length != arr2.Length)
            {
                throw new FormatException();
            }

            int sum = 0;

            for(int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] == 0 || arr2[i] == 0) continue;
                sum += (arr1[i] * arr2[i]);
            }

            return sum;
        }
    }
}
