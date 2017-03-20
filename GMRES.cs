using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace CSR_Operations
{
    class GMRES
    {
        //given by the user
        Matrix_CSR_Format A;
        Vector<double> b;
        Vector<double> initialX;
        double tolerance;
        int maxIterations;
        int restartAfterEveryNIterations;

        //used by the class
        Matrix<double> matrixP, matrixB;
        int initialXLength;
        static int currentGlobalIterationNumber;
        static bool convergenceReached;

        public GMRES(double tolerance, int maxIterations, int restartAfterEveryNIterations, Matrix_CSR_Format A, double[] b, double[] initialX)
        {
            this.tolerance = tolerance;
            this.maxIterations = maxIterations;
            this.A = A;
            this.b = Vector<double>.Build.Dense(b);
            this.initialX = Vector<double>.Build.Dense(initialX);
            this.restartAfterEveryNIterations = restartAfterEveryNIterations;
            initialXLength = initialX.Length;

            matrixB = Matrix<double>.Build.Dense(initialX.Length, maxIterations);
            matrixP = Matrix<double>.Build.Dense(initialX.Length, maxIterations);
        }

        public static void solveUsingGMRES(double tolerance, int maxIterations, int restartAfterEveryNIterations, Matrix_CSR_Format A, double[] b, double[] initialX)
        {
            currentGlobalIterationNumber = 0;
            convergenceReached = false;

            GMRES gmres = new GMRES(tolerance, maxIterations, restartAfterEveryNIterations, A, b, initialX);
            double[] xMax = gmres.solveEachRestart(ref convergenceReached);

            while (xMax != null)
            {
                Console.WriteLine("\nRestarting\n");

                gmres = new GMRES(tolerance, maxIterations, restartAfterEveryNIterations, A, b, xMax);
                xMax = gmres.solveEachRestart(ref convergenceReached);

                if (convergenceReached == true)
                {
                    break;
                }

                if (currentGlobalIterationNumber >= maxIterations)
                {
                    Console.WriteLine("\nMaxmium iterations have been completed with no convergence found");
                }
            }
        }

        private double[] solveEachRestart(ref bool convergenceReached)
        {
            double normRn;

            Vector<double> r = computeInitialResedual(); 
            normRn = r.Norm(2);

            if(currentGlobalIterationNumber == 0)
            {
                Console.WriteLine("Initial R = {0}\n", normRn);
            }
            else
            {
                Console.WriteLine("R{0} = {1}", currentGlobalIterationNumber, normRn);
            }
            

            // break out of loop if residual is within tolerance
            if (normRn <= tolerance)
            {
                Console.WriteLine("\nR{0} is within tolerance. Convergence reached ! It took {1} iterations to reach convergence.", currentGlobalIterationNumber, currentGlobalIterationNumber + 1);
                return null;
            }

            currentGlobalIterationNumber++;

            storeColumnInMatrixP(r.Multiply((1 / r.Norm(2))), 0);   //p1
            storeColumnInMatrixB(Vector<double>.Build.Dense(Matrix_CSR_Format_Operations.MatrixTimesComlumVector(A, matrixP.Column(0).ToArray())), 0);       //b1
            double t = (matrixB.Column(0).PointwiseMultiply(r)).ToArray().Sum() / (matrixB.Column(0).PointwiseMultiply(matrixB.Column(0))).ToArray().Sum(); // t
            Vector<double> x = initialX + matrixP.Column(0).Multiply(t);    //x1
            r = r - matrixB.Column(0).Multiply(t);   // r1
            normRn = r.Norm(2);

            Console.WriteLine("R{0} = {1}", currentGlobalIterationNumber, normRn);
            // break out of loop if residual is within tolerance
            if (normRn <= tolerance)
            {
                Console.WriteLine("\nR{0} is within tolerance. Convergence reached ! It took {1} iterations to reach convergence.", currentGlobalIterationNumber, currentGlobalIterationNumber + 1);
                return null;
            }

            currentGlobalIterationNumber++;

            Vector<double> pmTilda = null, pm = null, vecT = null;

            for (int currentIterationNum = 2; currentGlobalIterationNumber <= maxIterations; currentIterationNum++, currentGlobalIterationNumber++)
            {

                pmTilda = r;    // pm tilda

                double beta;
                Vector<double> v;

                for (int i = 0; i < currentIterationNum - 1; i++)
                {
                    beta = calculateBeta(i, r);
                    v = matrixP.Column(i).Multiply(beta);
                    pmTilda -= v;
                }

                pm = pmTilda.Multiply((1 / pmTilda.Norm(2)));       //pm
                storeColumnInMatrixP(pm, currentIterationNum - 1);

                storeColumnInMatrixB(Vector<double>.Build.Dense(Matrix_CSR_Format_Operations.MatrixTimesComlumVector(A, matrixP.Column(currentIterationNum - 1).ToArray())), currentIterationNum - 1); //bm

                Matrix<double> subMatrixB = matrixB.SubMatrix(0, initialXLength, 0, currentIterationNum);
                var qr = subMatrixB.GramSchmidt();

                vecT = qr.R.Solve(qr.Q.TransposeThisAndMultiply(r));

                x = x + matrixP.SubMatrix(0, initialXLength, 0, currentIterationNum).Multiply(vecT);
                r = r - matrixB.SubMatrix(0, initialXLength, 0, currentIterationNum).Multiply(vecT);

                normRn = r.Norm(2);
                Console.WriteLine("R{0} = {1}", currentGlobalIterationNumber, normRn);

                // break out of loop if residual is within tolerance
                if (normRn <= tolerance)
                {
                    Console.WriteLine("\nR{0} is within tolerance. Convergence reached ! It took {1} iterations to reach convergence.", currentGlobalIterationNumber, currentGlobalIterationNumber + 1);
                    convergenceReached = true;
                    break;
                }

                //return x if it the alogrithm needs to be restarted
                if (currentIterationNum == restartAfterEveryNIterations - 1)
                {
                    currentGlobalIterationNumber++;
                    return x.ToArray();
                }
            }

            // return null if residual is within tolerance 
            // or if maximum number of iterations are over and convergence has not been achieved
            return null;
        }

        Vector<double> computeInitialResedual()
        {
            double[] product = Matrix_CSR_Format_Operations.MatrixTimesComlumVector(this.A, initialX.ToArray());
            Vector<double> initialR = b - Vector<double>.Build.Dense(product);

            return initialR;
        }

        void storeColumnInMatrixB(Vector<double> columnVector, int columnNumber)
        {
            double[] colB = columnVector.ToArray();
            int i = 0;

            foreach (double d in colB) matrixB[i++, columnNumber] = d;
        }

        void storeColumnInMatrixP(Vector<double> columnVector, int columnNumber)
        {
            double[] colB = columnVector.ToArray();
            int i = 0;

            foreach (double d in colB) matrixP[i++, columnNumber] = d;
        }

        double calculateBeta(int i, Vector<double> r)
        {
            return matrixP.Column(i).PointwiseMultiply(r).ToArray().Sum();
        }
    }
}
