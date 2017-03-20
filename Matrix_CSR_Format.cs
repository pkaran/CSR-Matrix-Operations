using System;
using System.Collections.Generic;
using System.Linq;

namespace CSR_Operations
{
    // each enum constant is used to represent data format of a data file containing data about a matrix
    enum FileType
    {
        CSRFromat,
        NormalFormat,
        MTH343Format
    }
    
    class Matrix_CSR_Format
    {

        List<double> nonZeroEntries = new List<double>();
        List<int> columnInfo = new List<int>(), rowInfo = new List<int>();

        int numOfRows, numOfColumns;
        int[,] rowBounds;

        // defining properties for the class
        public int[,] RowBounds
        {
            get
            {
                return rowBounds;
            }
        }

        public double[] NonZeroEntries
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
                
                this.nonZeroEntries = csrMatrixInfo[0].Split(' ').Select(double.Parse).ToList<double>();
                this.rowInfo = csrMatrixInfo[1].Split(' ').Select(int.Parse).ToList<int>();
                this.columnInfo = csrMatrixInfo[2].Split(' ').Select(int.Parse).ToList<int>();

                numOfColumns = int.Parse(csrMatrixInfo[3]);
                numOfRows = this.rowInfo.Count - 1;
            }

            if (fileType == FileType.MTH343Format)
            {
                // Read each line of the file into a string array. Each element of the array is one line of the file.
                string[] csrMatrixInfo = System.IO.File.ReadAllLines(filePath);
                double[] lineInfo;
                bool firstLine = true;
                rowInfo.Add(0);
                Csr_matrix_info[] newMatrixRowInfo = null;

                foreach (String line in csrMatrixInfo)
                {
                    lineInfo = line.Split(' ').Select(double.Parse).ToArray();

                    if (firstLine == true)
                    {
                        numOfRows = (int)lineInfo[0];
                        numOfColumns = (int)lineInfo[1];
                        firstLine = false;

                        newMatrixRowInfo = new Csr_matrix_info[NumOfRows];

                        for (int i = 0; i < NumOfRows; i++)
                        {
                            newMatrixRowInfo[i].nonZeroEntries = new List<double>();
                            newMatrixRowInfo[i].columnInfo = new List<int>();
                        }

                        continue;
                    }

                    newMatrixRowInfo[(int)lineInfo[0] - 1].nonZeroEntries.Add(lineInfo[2]);
                    newMatrixRowInfo[(int)lineInfo[0] - 1].columnInfo.Add((int)lineInfo[1] - 1);
                }

                for (int i = 0; i < newMatrixRowInfo.Length; i++)
                {
                    nonZeroEntries.AddRange(newMatrixRowInfo[i].nonZeroEntries);
                    columnInfo.AddRange(newMatrixRowInfo[i].columnInfo);

                    rowInfo.Add(rowInfo.Last() + newMatrixRowInfo[i].nonZeroEntries.Count);
                }
            }

            generateRowBounds();
        }

        public Matrix_CSR_Format(List<double> nonZeroEntries, List<int> rowInfo, List<int> columnInfo, int numColumns)
        {
            if(nonZeroEntries == null)
            {
                nonZeroEntries = new List<double>();
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
            double[] matrixRow;    // temp int array
            double num;            // temp int
            int numOfNonZeroNumInRow = 0;

            // traverse through all rows of matrix
            for (int i = 0; i < rows.Length; i++)
            {
                matrixRow = rows[i].Split(' ').Select(double.Parse).ToArray();

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
        
        // print matrix to console in CSR format
        public void printMatrixInCSR()
        {
            Console.WriteLine("\nNon zero entries : ");
            foreach (double i in nonZeroEntries)
            {
                Console.Write("{0:0.000} ", i);
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

        // print matrix in human readable format 
        public void printMatrix()
        {
            string rowString = "";
            double[] row;

            // traverse through all rows
            for(int i = 0; i < NumOfRows; i++)
            {
                row = getRow(i);
                foreach(double j in row) rowString += String.Format("{0:0.000} ", j);
                Console.WriteLine(rowString);
                rowString = "";
            }
        }

        // get row rowNum from the matrix, the length of the returned double array is equal to the number of columns in matrix
        public double[] getRow(int rowNum)
        {
            double[] row = new double[NumOfColumns];

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

        // adds a new row to the matrix, newRow must be of length equal to the number of columns in matrix
        // newRow should contain all entries (including 0's) in the row to be added to the matrix
        public void addRow(double[] newRow)
        {
            double num;
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

        //returns row number of entry in matrix stored at index indexInNonZeroEntries of nonZeroEntries list
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

        //generates (upper and lower) row bounds for each row in matrix 
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

        private struct Csr_matrix_info
        {
            public List<int> columnInfo;
            public List<double> nonZeroEntries;
        }
    }
}
