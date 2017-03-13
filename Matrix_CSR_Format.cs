using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSR_Operations
{
    enum FileType
    {
        CSRFromat,
        NormalFormat
    }

    class Matrix_CSR_Format
    {

        List<decimal> nonZeroEntries = new List<decimal>();
        List<int> columnInfo = new List<int>(), rowInfo = new List<int>();

        int[,] rowBounds;
        public int[,] RowBounds
        {
            get
            {
                return rowBounds;
            }
        }

        public decimal[] NonZeroEntries
        {
            get
            {
                return nonZeroEntries.ToArray();
            }
        }

        public int[] ColumnInfo
        {
            get
            {
                return columnInfo.ToArray();
            }
        }

        public int[] RowInfo
        {
            get
            {
                return rowInfo.ToArray();
            }
        }

        int numOfRows, numOfColumns;

        public int NumOfRows
        {
            get
            {
                return numOfRows;
            }
        }
        public int NumOfColumns
        {
            get
            {
                return numOfColumns;
            }
        }

        //filePath is path to a text file containing data for a matrix and specify file format using fileType
        public Matrix_CSR_Format(String filePath, FileType fileType)
        {
            if(fileType == FileType.NormalFormat)
            {
                // Read each line of the file into a string array. Each element of the array is one line of the file.
                string[] rowsOfMatrix = System.IO.File.ReadAllLines(filePath);
               
                numOfRows = rowsOfMatrix.Length;
                numOfColumns = convertToCSRFormat(rowsOfMatrix);
            }

            if(fileType == FileType.CSRFromat)
            {
                // Read each line of the file into a string array. Each element of the array is one line of the file.
                string[] csrMatrixInfo = System.IO.File.ReadAllLines(filePath);
                
                this.nonZeroEntries = csrMatrixInfo[0].Split(' ').Select(decimal.Parse).ToList<decimal>();
                this.rowInfo = csrMatrixInfo[1].Split(' ').Select(int.Parse).ToList<int>();
                this.columnInfo = csrMatrixInfo[2].Split(' ').Select(int.Parse).ToList<int>();

                numOfColumns = int.Parse(csrMatrixInfo[3]);
                numOfRows = this.rowInfo.Count - 1;
            }

            generateRowBounds();
        }

        public Matrix_CSR_Format(List<decimal> nonZeroEntries, List<int> rowInfo, List<int> columnInfo, int numColumns)
        {
            if(nonZeroEntries == null)
            {
                nonZeroEntries = new List<decimal>();
            }
            else
            {
                this.nonZeroEntries = nonZeroEntries;
            }

            if (rowInfo == null)
            {
                rowInfo = new List<int>();
                numOfRows = 0;
            }
            else
            {
                this.rowInfo = rowInfo;
                numOfRows = this.rowInfo.Count - 1;
            }

            if (columnInfo == null)
            {
                columnInfo = new List<int>();
            }
            else
            {
                this.columnInfo = columnInfo;
            }

            numOfColumns = numColumns;
            generateRowBounds();
        }

        //converts an array of strings (where each string represents a row) representing a matrix into CSR format 
        //returns number of columns in matrix
        private int convertToCSRFormat(string[] rows)
        {
            int numColumns = -1;
            decimal[] matrixRow;    // temp int array
            decimal num;            // temp int
            int numOfNonZeroNumInRow = 0;

            // traverse through all rows of matrix
            for (int i = 0; i < rows.Length; i++)
            {
                matrixRow = rows[i].Split(' ').Select(decimal.Parse).ToArray();

                if (i == 0)
                {
                    numColumns = matrixRow.Length;
                    rowInfo.Add(0);
                }

                // traverse through each number in each row
                for (int j = 0; j < numColumns; j++)
                {
                    num = matrixRow[j];

                    //if number is not zero
                    if (num != 0)
                    {
                        nonZeroEntries.Add(num);
                        columnInfo.Add(j);
                        numOfNonZeroNumInRow++;
                    }
                }

                rowInfo.Add(rowInfo.Last() + numOfNonZeroNumInRow);
                numOfNonZeroNumInRow = 0;
            }

            return numColumns;
        }
        
        public void printMatrixInCSR()
        {
            Console.WriteLine("\nNon zero entries : ");
            foreach (decimal i in nonZeroEntries)
            {
                Console.Write(i + " ");
            }

            Console.WriteLine("\n\nRow Info : ");
            foreach (int i in rowInfo)
            {
                Console.Write(i + " ");
            }

            Console.WriteLine("\n\nColumn Info : ");
            foreach (int i in columnInfo)
            {
                Console.Write(i + " ");
            }
        }

        public void printMatrix()
        {
            string rowString = "";
            decimal[] row;

            // traverse through all rows
            for(int i = 0; i < NumOfRows; i++)
            {
                row = getRow(i);
                foreach(decimal j in row) rowString += String.Format("{0} ", j);
                Console.WriteLine(rowString);
                rowString = "";
            }
        }

        public decimal[] getRow(int rowNum)
        {
            decimal[] row = new decimal[NumOfColumns];

            int lowerBound = rowInfo[rowNum], upperBound = rowInfo[rowNum + 1] - 1;

            // if row has at least one non-zero int in it
            if(lowerBound <= upperBound)
            {
                for(int i = lowerBound; i <= upperBound; i++)
                {
                    row[columnInfo[i]] = nonZeroEntries[i];
                }
            }

            return row;
        }

        public decimal[] getColumn(int columnNum)
        {
            decimal[] column = new decimal[NumOfRows];

            for(int rowNum = 0; rowNum < NumOfRows; rowNum++)
            {
                int lowerBound = rowInfo[rowNum], upperBound = rowInfo[rowNum + 1] - 1;

                // if row has at least one non-zero int in it
                if (lowerBound <= upperBound)
                {
                    for (int i = lowerBound; i <= upperBound; i++)
                    {
                        if(columnInfo[i] == columnNum)
                        {
                            column[rowNum] = nonZeroEntries[i];
                        }
                    }
                }
            }

            return column;
        }

        public void addRow(decimal[] newRow)
        {
            decimal num;
            int numOfNonZeroNumInRow = 0;        // temp int variable

            if (numOfRows == 0)
            {
                rowInfo.Add(0);
            }

            // traverse through each number in each row
            for (int j = 0; j < newRow.Length; j++)
            {
                num = newRow[j];

                //if number is not zero
                if (num != 0)
                {
                    nonZeroEntries.Add(num);
                    columnInfo.Add(j);
                    numOfNonZeroNumInRow++;
                }
            }

            rowInfo.Add(rowInfo.Last() + numOfNonZeroNumInRow);

            numOfRows++;

            generateRowBounds();
        }

        public int getRowNumber(int indexInNonZeroEntries)
        {
            for(int i = 0; i < numOfRows; i++)
            {
                if(rowBounds[i,0] <= indexInNonZeroEntries && indexInNonZeroEntries <= rowBounds[i,1])
                {
                    return i;
                }
            }

            return -1;
        }

        private void generateRowBounds()
        {
            rowBounds = new int[numOfRows, 2];
            int lowerBound, upperBound;

            for (int i = 0; i < numOfRows; i++)
            {
                lowerBound = RowInfo[i];
                upperBound = RowInfo[i + 1] - 1;

                // if row has at least one non-zero int in it
                if (lowerBound <= upperBound)
                {
                    rowBounds[i, 0] = lowerBound;
                    rowBounds[i, 1] = upperBound;
                }
                else
                {
                    rowBounds[i, 0] = -1;
                    rowBounds[i, 1] = -1;
                }
            }
        }
    }
}
