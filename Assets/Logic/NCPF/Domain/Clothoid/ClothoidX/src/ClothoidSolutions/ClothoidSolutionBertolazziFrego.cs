using System;
using System.Collections.Generic;
using System.Numerics;

namespace ClothoidX
{
    public static class ClothoidSolutionBertolazziFrego
    {
        /// <summary>
        /// Threshold for A to solve the fresnel equation with different solutions
        /// </summary>
        public static readonly double A_THRESHOLD = 1E-2;
        /// <summary>
        /// When A is small the momenta integrals are evaluated with an infinite series.
        /// Turns out this series converges very fast so we only need the first few terms.
        /// This is that number of terms.
        /// </summary>
        public static readonly int A_SMALL_SERIES_SIZE = 3;
        /// <summary>
        /// Root finding tolerance
        /// </summary>
        private const double ROOT_TOLERANCE = 1E-4;
        /// <summary>
        /// These are coefficients used in the initial guess of A in the root finding algorithm
        /// </summary>
        private static readonly double[] CF = new double[6] { 2.989696028701907, 0.716228953608281, -0.458969738821509, -0.502821153340377, 0.261062141752652, -0.045854475238709 };

        /// <summary>
        /// Build a G1 continuous clothoid spline with posture data. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ClothoidCurve G1Spline(Posture[] data)
        {
            ClothoidCurve c = new ClothoidCurve();
            for (int i = 0; i + 1 < data.Length; i++)
            {
                c += G1Segment(data[i].X, data[i].Z, data[i].Angle, data[i + 1].X, data[i + 1].Z, data[i + 1].Angle);
            }
            return c;
        }

        /// <summary>
        /// Build a G1 continuous clothoid spline with a list of VectorDouble points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static ClothoidCurve G1Spline(List<Mathc.VectorDouble> points, bool loop = false)
        {
            return G1Spline(Posture.CalculatePostures(points, loop).ToArray());
        }

        /// <summary>
        /// Build a G1 clothoid spline given a list of Vector3 points. 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static ClothoidCurve G1Spline(List<Vector3> points, bool loop = false)
        {
            return G1Spline(Posture.CalculatePostures(points, loop).ToArray());
        }
        
        /// <summary>
        /// Build a G2 segment from the given parameters. Note this isn't finished yet which is why it is private.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="z0"></param>
        /// <param name="v0"></param>
        /// <param name="k0"></param>
        /// <param name="x1"></param>
        /// <param name="z1"></param>
        /// <param name="v1"></param>
        /// <param name="k1"></param>
        /// <returns></returns>
        public static ClothoidCurve G2Segment(double x0, double z0, double v0, double k0, double x1, double z1, double v1, double k1)
        {
            throw new NotImplementedException();
            return ClothoidG2Solver3Arc.Build(x0, z0, v0, k0, x1, z1, v1, k1);
        }

        /// <summary>
        /// Build a G1 clothoid segment given two points and two angles.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="z0"></param>
        /// <param name="t0"></param>
        /// <param name="x1"></param>
        /// <param name="z1"></param>
        /// <param name="t1"></param>
        /// <param name="tol"></param>
        /// <param name="maxIter"></param>
        /// <returns></returns>
        public static ClothoidSegment G1Segment(double x0, double z0, double t0, double x1, double z1, double t1, double tol = ROOT_TOLERANCE, int maxIter = 45)
        {
            double dx = x1 - x0;
            double dz = z1 - z0;
            double phi = Math.Atan2(dz, dx);
            double r = Math.Sqrt((dx * dx) + (dz * dz));
            double phi0 = NormalizeAngle(t0 - phi);
            double phi1 = NormalizeAngle(t1 - phi);

            // Straight-line degenerate cases: tangents aligned with the chord (both
            // phi ≈ 0) or anti-aligned/collinear (phi0 ≈ ∓π and phi1 ≈ ±π). The bounds
            // MUST be wrapped in Math.Abs — a bare `phi - Math.PI < tol` is trivially
            // true for any phi ≤ π, which made the solver return a straight segment
            // along t0 for almost any turning target (phi normalising near −π).
            if ((Math.Abs(phi0) < tol && Math.Abs(phi1) < tol)
                || (Math.Abs(phi0 + Math.PI) < tol && Math.Abs(phi1 - Math.PI) < tol)
                || (Math.Abs(phi0 - Math.PI) < tol && Math.Abs(phi1 + Math.PI) < tol))
            {
                return new ClothoidSegment(new Mathc.VectorDouble(x0, 0, z0), t0, 0, 0, r, SolutionType.BERTOLAZZIFREGO);
            }

            double d = phi1 - phi0;

            //Calculate the bounds of the solution A_max and T_max
            double absd = Math.Abs(d);
            double Tmax = Math.Max(0, (Math.PI / 2) + (Math.Sign(phi1) * phi0));
            double Amax = Tmax == 0 ? absd : absd + (2 * Tmax * (1 + Math.Sqrt(1 + (absd / Tmax))));

            double g;
            double dg;
            List<double[]> IntCS;
            int u = 0;
            double A = InitialGuessA(phi0, phi1);

            //print amax
            //Console.WriteLine($"{-Amax} <= {A} <= {Amax}");
            if (A > Amax || A < -Amax)
            {
                //print amax
                //Console.WriteLine($"{-Amax} <= {A} <= {Amax}\n");
                A = 0;
            }

            double change;
            double prevChange = double.MaxValue;

            do
            {
                IntCS = GeneralizedFresnelCS(3, 2 * A, d - A, phi0);
                g = IntCS[1][0];
                dg = IntCS[0][2] - IntCS[0][1];
                change = g / dg;

                //When the change is larger than the previous change, we are probably oscillating around the root, so cut the change in half
                //I noticed this happening when the initial guess was very close to the actual root. 
                if (Math.Abs(change) >= Math.Abs(prevChange))
                {
                    change = Math.Abs(prevChange) * Math.Sign(change) / 2;
                }
                prevChange = change;

                if (Math.Abs(g) > tol) A -= change;
                else break;
                //Console.WriteLine($"g: {g}, dg: {dg}, A: {A}, u: {u}");
            } while (++u < maxIter && Math.Abs(g) > tol);

            //print all parameters and final values
            //Console.WriteLine($"A: {A}, g: {g}, dg: {dg}, u: {u}, phi0: {phi0}, phi1: {phi1}, d: {d}, r: {r}");


            //double[] intCS;

            //intCS = GeneralizedFresnelCS(2 * A, d - A, phi0);
            double s = r / IntCS[0][0];//intCS[0];

            double startCurvature = (d - A) / s;
            double sharpness = 2 * A / (s * s);

            /*ClothoidCurve c = ClothoidCurve.FromSegments(new ClothoidSegment(startCurvature, sharpness, s));
            c.Offset = new Mathc.VectorDouble(x0, 0, z0);
            c.AngleOffset = t0;*/
            ClothoidSegment s2 = new ClothoidSegment(new Mathc.VectorDouble(x0, 0, z0), t0, startCurvature, sharpness, s, SolutionType.BERTOLAZZIFREGO);
            return s2;
        }

        /// <summary>
        /// Normalize an angle in radians to be between -pi and pi.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static double NormalizeAngle(double angle)
        {
            while (angle > Math.PI) angle -= 2 * Math.PI;
            while (angle < -Math.PI) angle += 2 * Math.PI;
            return angle;
        }

        /// <summary>
        /// Normalize an angle in radians to be between 0 and 2pi.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static double NormalizeAngle2(double angle)
        {
            while (angle > 2 * Math.PI) angle -= 2 * Math.PI;
            while (angle < 0) angle += 2 * Math.PI;
            return angle;
        }

        /// <summary>
        /// Guess the initial value of A given the two values of phi. Phi is the angle difference between the tangent and the vector from start to end. 
        /// </summary>
        /// <param name="phi0"></param>
        /// <param name="phi1"></param>
        /// <returns></returns>
        private static double InitialGuessA(double phi0, double phi1)
        {
            double X = phi0 / Math.PI;
            double Y = phi1 / Math.PI;
            double xy = X * Y;
            X *= X;
            Y *= Y;
            return (phi0 + phi1) * (CF[0] + xy * (CF[1] + xy * CF[2]) + (CF[3] + xy * CF[4]) * (X + Y) + CF[5] * (X * X + Y * Y));
        }

        /// <summary>
        /// Get the initial guess for A and the bounds for the smallest A root.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="A"></param>
        /// <param name="Amax"></param>
        /// <returns></returns>
        public static void PInitialGuessA(Mathc.VectorDouble start, Mathc.VectorDouble end, double startAngle, double endAngle, out double A, out double Amax, out double d, out double phi0, out double phi1, out double r)
        {
            double dx = end.X - start.X;
            double dz = end.Z - start.Z;
            double phi = Math.Atan2(dz, dx);
            r = Math.Sqrt((dx * dx) + (dz * dz));
            phi0 = NormalizeAngle(startAngle - phi);
            phi1 = NormalizeAngle(endAngle - phi);
            A = InitialGuessA(phi0, phi1);

            d = phi1 - phi0;

            double absd = Math.Abs(phi1 - phi0);
            double Tmax = Math.Max(0, (Math.PI / 2) + (Math.Sign(phi1) * phi0));
            Amax = Tmax == 0 ? absd : absd + (2 * Tmax * (1 + Math.Sqrt(1 + (absd / Tmax))));
        }

        private static double RLommel(double mu, double v, double b)
        {

            double t = 1 / ((mu + v + 1) * (mu - v + 1));
            double r = t;
            double n = 1;
            double e = .00000000001;

            while (Math.Abs(t) > e * Math.Abs(r))
            {
                t *= ((-b) / ((2 * n) + mu - v + 1)) * (b / ((2 * n) + mu + v + 1));
                r += t;
                n++;
            }

            return r;
        }

        /// <summary>
        /// Evaluate XY when a is 0
        /// </summary>
        /// <param name="b"></param>
        /// <param name="k"></param>
        /// <param name="e">The allowable error</param>
        /// <returns></returns>
        private static List<double[]> EvalXYaZero(double b, int k)
        {
            double b2 = b * b;
            double sb = Math.Sin(b);
            double cb = Math.Cos(b);

            double[] X = new double[45];
            double[] Y = new double[45];

            if (Math.Abs(b) < 1E-3)
            {
                X[0] = 1 - (b2 / 6) * (1 - (b2 / 20) * (1 - (b2 / 42)));
                Y[0] = (b2 / 2) * (1 - (b2 / 12) * (1 - (b2 / 30)));
            }
            else
            {
                X[0] = sb / b;
                Y[0] = (1 - cb) / b;
            }

            //int m = (int)Math.Min(Math.Max(1, Math.Floor(2 * b)), k);
            int m = (int)Math.Floor(2 * b);

            if (m >= k) m = k - 1;
            if (m < 1) m = 1;

            for (int i = 1; i < m; ++i)
            {
                X[i] = (sb - (i * Y[i - 1])) / b;
                Y[i] = ((i * X[i - 1]) - cb) / b;
            }

            if (m < k)
            {
                double A = b * sb;
                double D = sb - (b * cb);
                double B = b * D;
                double C = -b2 * sb;
                double rLa = RLommel(m + 0.5, 1.5, b);
                double rLd = RLommel(m + 0.5, 0.5, b);
                double rLb;
                double rLc;

                for (int i = m; i < k; ++i)
                {
                    rLb = RLommel(i + 1.5, 0.5, b);
                    rLc = RLommel(i + 1.5, 1.5, b);
                    X[i] = ((i * A * rLa) + (B * rLb) + cb) / (1 + i);
                    Y[i] = (((C * rLc) + sb) / (2 + i)) + (D * rLd);
                    rLa = rLc;
                    rLd = rLb;
                }

            }

            return new List<double[]>() { X, Y };
        }

        private static List<double[]> EvalXYaSmall(int k, double a, double b, int p)
        {

            int nkk = k + (4 * p) + 2;
            List<double[]> points = EvalXYaZero(b, nkk);
            double[] X0 = points[0];
            double[] Y0 = points[1];
            double[] X = new double[3];
            double[] Y = new double[3];

            for (int j = 0; j < k; j++)
            {
                X[j] = X0[j] - (a / 2) * Y0[j + 2];
                Y[j] = Y0[j] + (a / 2) * X0[j + 2];
            }

            double t = 1;
            double aa = -a * a / 4;
            double bf;
            int jj;
            for (int n = 1; n <= p; n++)
            {
                t *= aa / (2 * n * ((2 * n) - 1));
                bf = a / ((4 * n) + 2);

                for (int j = 0; j < k; j++)
                {
                    jj = (4 * n) + j;
                    X[j] += t * (X0[jj] - (bf * Y0[jj + 2]));
                    Y[j] += t * (Y0[jj] + (bf * X0[jj + 2]));
                }
            }

            return new List<double[]>() { X, Y };
        }

        private static List<double[]> EvalXYaLarge(int n, double a, double b)
        {
            if (n > 3 || n < 0) throw new ArgumentOutOfRangeException();

            double s = a > 0 ? 1 : -1;
            double absa = Math.Abs(a);
            double z = Math.Sqrt(absa) / Math.Sqrt(Math.PI);
            double ell = s * b / Math.Sqrt(Math.PI * absa);
            double g = -s * b * b / (2 * absa);
            double cg = Math.Cos(g) / z;
            double sg = Math.Sin(g) / z;
            List<double[]> CS1 = FresnelCS(n, ell);
            List<double[]> CSz = FresnelCS(n, ell + z);
            double[] C1 = CS1[0];
            double[] S1 = CS1[1];
            double[] Cz = CSz[0];
            double[] Sz = CSz[1];

            double[] dC = new double[n];
            double[] dS = new double[n];

            for (int i = 0; i < n; i++)
            {
                dC[i] = Cz[i] - C1[i];
                dS[i] = Sz[i] - S1[i];
            }
            double[] X = new double[3];
            double[] Y = new double[3];

            //UnityEngine.Debug.LogWarning($"Number of elements in CS1: ({C1.Length}, {S1.Length}) | in CSZ: ({Cz.Length}, {Sz.Length})");

            X[0] = (cg * dC[0]) - (s * sg * dS[0]);
            Y[0] = (sg * dC[0]) + (s * cg * dS[0]);
            if (n > 1)
            {
                cg /= z;
                sg /= z;
                double DC = dC[1] - (ell * dC[0]);
                double DS = dS[1] - (ell * dS[0]);

                X[1] = (cg * DC) - (s * sg * DS);
                Y[1] = (sg * DC) + (s * cg * DS);

                if (n > 2)
                {
                    cg /= z;
                    sg /= z;
                    DC = dC[2] + (ell * ((ell * dC[0]) - (2 * dC[1])));
                    DS = dS[2] + (ell * ((ell * dS[0]) - (2 * dS[1])));
                    X[2] = (cg * DC) - (s * sg * DS);
                    Y[2] = (sg * DC) + (s * cg * DS);
                }
            }

            return new List<double[]>() { X, Y };
        }

        /// <summary>
        /// Evaluate the fresnel integral and its momentae at arc length t
        /// </summary>
        /// <param name="n"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static List<double[]> FresnelCS(int n, double t)
        {
            double[] C = new double[n];
            double[] S = new double[n];
            Mathc.FresnelCS(t, out double c, out double s);
            C[0] = c;
            S[0] = s;

            if (n > 1)
            {
                C[1] = C1(t);
                S[1] = S1(t);
                /*
                double tt = Math.PI * t * t / 2;
                double stt = Math.Sin(tt);
                double ctt = Math.Cos(tt);
                C[1] = stt / Math.PI;
                S[1] = (1 - ctt) / Math.PI;*/
                if (n > 2)
                {
                    C[2] = C2(t); //((t * stt) - S[0]) / Math.PI;
                    S[2] = S2(t); //(C[0] - (t * ctt)) / Math.PI;
                }
            }

            return new List<double[]>() { C, S };
        }

        /// <summary>
        /// Compute the Generalized Fresnel integral as described by Bertolazzi and Frego.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static List<double[]> GeneralizedFresnelCS(int n, double a, double b, double c)
        {
            if (n < 1 || n > 3) throw new ArgumentOutOfRangeException($"Value of index: {n} | Expected value between 1 and 3 inclusive.");
            double cc = Math.Cos(c);
            double sc = Math.Sin(c);
            List<double[]> CS;

            if (Math.Abs(a) < A_THRESHOLD)
            {
                CS = EvalXYaSmall(n, a, b, A_SMALL_SERIES_SIZE);
            }
            else
            {
                CS = EvalXYaLarge(n, a, b);
            }

            double ci;
            double si;

            for (int i = 0; i < n; i++)
            {
                ci = CS[0][i];
                si = CS[1][i];
                CS[0][i] = (ci * cc) - (si * sc);
                CS[1][i] = (ci * sc) + (si * cc);
            }

            return CS;
        }

        /// <summary>
        /// Investigate the GFC solution with different A values. This can be utilized for more artistic functions.
        /// How it works is given a value A between -Amax and Amax given by <seealso cref="PInitialGuessA(Mathc.VectorDouble, Mathc.VectorDouble, double, double, out double, out double, out double, out double, out double, out double)"/>,
        /// This function will generate a segment at the start position, with the start and end angles, but the the end position will lie on a line
        /// perpendicular to the vector from start to end. The end position of the final segment will only match the input end parameter if A is the 
        /// true solution to the Fresnel integral.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="A"></param>
        /// <param name="d"></param>
        /// <param name="phi0"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startAngle"></param>
        /// <returns></returns>
        public static ClothoidSegment PGeneralizedFresnelCS(double A, double startAngle, double endAngle, Mathc.VectorDouble start, Mathc.VectorDouble end)
        {
            double dx = end.X - start.X;
            double dz = end.Z - start.Z;
            double phi = Math.Atan2(dz, dx);
            double r = Math.Sqrt((dx * dx) + (dz * dz));
            double phi0 = NormalizeAngle(startAngle - phi);
            double phi1 = NormalizeAngle(endAngle - phi);
            double d = phi1 - phi0;
            var XY = GeneralizedFresnelCS(1, 2 * A, d - A, phi0);
            double s = r / XY[0][0];
            double startCurvature = (d - A) / s;
            double sharpness = 2 * A / (s * s);
            return new ClothoidSegment(start, startAngle, startCurvature, sharpness, s, SolutionType.BERTOLAZZIFREGO);
        }

        private static double[] GeneralizedFresnelCS(double a, double b, double c)
        {
            List<double[]> CS = GeneralizedFresnelCS(1, a, b, c);
            return new double[] { CS[0][0], CS[1][0] };
        }


        private static double C0(double t)
        {
            return Mathc.C(t);
        }

        private static double S0(double t)
        {
            return Mathc.S(t);
        }


        private static double C1(double t)
        {
            return Math.Sin(Math.PI * t * t / 2) / Math.PI;
        }


        private static double S1(double t)
        {
            return (1 - Math.Cos(Math.PI * t * t / 2)) / Math.PI;
        }


        private static double C2(double t)
        {
            return ((t * Math.Sin(Math.PI * t * t / 2)) - S0(t)) / Math.PI;
        }


        private static double S2(double t)
        {
            return (C0(t) - (t * Math.Cos(Math.PI * t * t / 2))) / Math.PI;
        }

        /// <summary>
        /// Evaluate the clothoid curve along its arc length using the internal GeneralizedFresnelCS function instead of the built in sampling methods in the ClothoidCurve class. 
        /// </summary>
        /// <param name="arcLength"></param>
        /// <param name="curvature"></param>
        /// <param name="sharpness"></param>
        /// <param name="startAngle"></param>
        /// <param name="start"></param>
        /// <param name="numSamples"></param>
        /// <returns></returns>
        public static List<Mathc.VectorDouble> Eval(double arcLength, double curvature, double sharpness, double startAngle, Mathc.VectorDouble start, int numSamples = 100)
        {
            double x0 = start.X;
            double y0 = start.Z;
            var points = new List<Mathc.VectorDouble>();
            double increment = arcLength / (numSamples - 1);
            for (double s = 0; s < arcLength; s += increment)
            {
                points.Add(EvalArcLength(s, curvature, sharpness, startAngle, start));
            }
            //add final point
            //points.Add(EvalArcLength(arcLength, curvature, sharpness, startAngle, start));
            return points;
        }  

        public static Mathc.VectorDouble EvalArcLength(double arcLength, double curvature, double sharpness, double startAngle, Mathc.VectorDouble start)
        {
            double[] XY = GeneralizedFresnelCS(sharpness * arcLength * arcLength, curvature * arcLength, startAngle);
            return new Mathc.VectorDouble(start.X + (XY[0] * arcLength), 0, start.Z + (XY[1] * arcLength));
        }

        //Below are functions for G2 interpolation. I keep them seperated because they are from different papers.

        /// <summary>
        /// The G2 solver includes quite a lot of parameters and functions with a bunch of shared interdependencies, so I've encapsulated it in its own class to keep things tidy.
        /// </summary>
        private static class ClothoidG2Solver3Arc
        {
            //unscaled knowns, these values will not be transformed into the new reference frame
            /*
            private static double x0;
            private static double x1;
            private static double z0;
            private static double z1;
            private static double v0;
            private static double v1;
            private static double k0;
            private static double k1;

            private static double dx;
            private static double dz;
            private static double d;
            private static double lambda;
            private static double phi;


            //unscaled unknowns
            private static double km;
            private static double sm;
            //private static double s0;
            //private static double s1;
            private static double kp0; //sharpness
            private static double kp1;
            private static double kpm;

            //Scaled unknowns
            private static double Km => km * sm;
            private static double K0 => k0 * s0;
            private static double K1 => k1 * s1;
            private static double Kpm => kpm * sm * sm;
            private static double Kp0 => kp0 * s0 * s0;
            private static double Kp1 => kp1 * s1 * s1;

            //t coefficients
            private static double t0 => K0 + v0;
            private static double t1 => K1 - v1;
            private static double t2 => K0 + v0 + v0;
            private static double t3 => K1 - v1 - v1;

            //c coefficients
            private static double c0 => s0 * s1;
            private static double c1 => s0 + s0;
            private static double c2 => ((((t3 - (6 * t0)) * s0) - (3 * K0 * s1))) / 4;
            private static double c3 => -c0 * t0;
            private static double c4 => s1 + s1;
            private static double c5 => (((((6 * t1) - t2) * s1) + (3 * K1 * s0))) / 4;
            private static double c6 => c0 * t1;
            private static double c7 => -(s0 + s1) / 2;
            private static double c8 => (t2 - t3) / 2;
            private static double c9 => ((t2 * s1) - (t3 * s0)) / 4;
            private static double c10 => (s1 - s0) / 2;
            private static double c11 => -(t2 + t3) / 4;
            private static double c12 => ((t3 * s0) + (t2 * s1)) / 4;
            private static double c13 => c0 / 2;
            private static double c14 => -(3 * c7) / 2;
            */

            /// <summary>
            /// This solves the 3 arc solution. As of this comment, it is not quite functional or ready to use.
            /// TODO: Debug this method, starting with the final output segments, it could be as simple as not 
            /// properly transforming the parameters back.
            /// </summary>
            /// <param name="x0"></param>
            /// <param name="z0"></param>
            /// <param name="v0"></param>
            /// <param name="k0"></param>
            /// <param name="x1"></param>
            /// <param name="z1"></param>
            /// <param name="v1"></param>
            /// <param name="k1"></param>
            /// <returns></returns>
            internal static ClothoidCurve Build(double x0, double z0, double v0, double k0, double x1, double z1, double v1, double k1)
            {
                double _x0 = x0;
                double _x1 = x1;
                double _z0 = z0;
                double _z1 = z1;
                double dx = x1 - x0;
                double dz = z1 - z0;
                double _k0 = k0;
                double _k1 = k1;
                double _v0 = v0;
                double _v1 = v1;

                double phi = Math.Atan2(dz, dx); 
                //inverse of lambda from paper
                double inv_lambda = 2 / Math.Sqrt((dx * dx) + (dz * dz));

                double _t0 = NormalizeAngle(_v0 - phi);
                double _t1 = NormalizeAngle(_v1 - phi);

                //transformed curvature
                double _K0 = _k0 / inv_lambda;
                double _K1 = _k1 / inv_lambda;

                double Dmax = Math.PI;
                double dmax = Math.PI / 8;

                ClothoidSegment s = G1Segment(-1, 0, _t0, 1, 0, _t1);

                double ka = s.StartCurvature;
                double kb = s.EndCurvature;
                double dk = s.Sharpness;
                double s_3 = s.TotalArcLength / 3;

                double s0 = s_3;
                double s1 = s_3;

                //temp variable
                double t = Math.Abs(_k0 - ka) / (2 * dmax);
                if (t * s0 < 1) s0 = 1 / t;
                t = (Math.Abs(_k0 + ka) + (s0 * dk)) / (2 * Dmax);
                if (t * s0 < 1) s0 = 1 / t;
                t = Math.Abs(_k1 - kb) / (2 * dmax);
                if (t * s1 < 1) s1 = 1 / t;
                t = (Math.Abs(_k1 + kb) + (s1 * dk)) / (2 * Dmax);
                if (t * s1 < 1) s1 = 1 / t;

                double frac = Math.Pow(Math.Abs(_t0 - _t1), 4) / (32 * Math.PI * Math.PI * Math.PI);
                t = Math.Pow(Math.Cos(frac), 3);

                s0 *= t;
                s1 *= t;

                double smN = (s.TotalArcLength - s0 - s1) / 2;
                double vmN = s.Tangent(s0 + smN);

                _t0 = s.AngleStart;
                _t1 = s.AngleEnd;

                _K0 *= s0;
                _K1 *= s1;

                double t0 = _K0 + _t0;
                double t1 = _K1 - _t1;
                double t2 = _K0 + (2 * _t0);
                double t3 = _K1 - (2 * _t1);

                double c0 = s0 * s1;
                double c1 = s0 + s0;
                double c2 = ((((t3 - (6 * t0)) * s0) - (3 * _K0 * s1))) / 4;
                double c3 = -c0 * t0;
                double c4 = s1 + s1;
                double c5 = (((((6 * t1) - t2) * s1) + (3 * _K1 * s0))) / 4;
                double c6 = c0 * t1;
                double c7 = -(s0 + s1) / 2;
                double c8 = (t2 - t3) / 2;
                double c9 = ((t2 * s1) - (t3 * s0)) / 4;
                double c10 = (s1 - s0) / 2;
                double c11 = -(t2 + t3) / 4;
                double c12 = ((t3 * s0) + (t2 * s1)) / 4;
                double c13 = c0 / 2;
                double c14 = -(3 * c7) / 2;

                //starting guesses are smN for the arc length and vmN for the start angle of the middle arc
                //now we solve

                int u = 0;
                int maxIter = 100;
                bool converged = false;

                double[] guess = new double[] { smN, vmN };

                double[][] jacobian = new double[][] { new double[2], new double[2] };
                double[] derivative = new double[2];
                double[] F = new double[2];

                double dsm;
                double dsm2;
                double dK0;
                double dK1;
                double dKm;
                double Km;

                List<double[]> cs0;
                List<double[]> cs1;
                List<double[]> cs2;
                List<double[]> cs3;

                double e0;
                double e1;

                double g0;
                double g1;
                double g2;

                double dK0_smN;
                double dK1_smN;
                double dKm_smN;
                double Km_smN;


                double dK0_vmN;
                double dK1_vmN;
                double dKm_vmN;
                double Km_vmN;

                double f0;
                double f1;
                double f2;
                double f3;
                double f4;
                double f5;
                double f6;
                double f7;

                double tol = 1E-6;
                double evaluation;

                do
                {
                    dsm = 1 / (c13 + (c14 * smN) + (smN * smN));
                    dK0 = dsm * ((c0 * vmN) + (c1 * smN * vmN) - (k0 * s0 * smN * smN) + (c2 * smN) + c3);
                    dK1 = dsm * ((c0 * vmN) + (c4 * smN * vmN) + (k1 * s1 * smN * smN) + (c5 * smN) + c6);
                    dKm = dsm * smN * ((c7 * vmN) - (2 * vmN * smN) + (c8 * smN) + (c2 * smN) + c9);
                    Km = dsm * smN * ((c10 * vmN) + (c11 * smN) + c12);

                    cs0 = GeneralizedFresnelCS(3, dK0, _K0, _t0);
                    cs1 = GeneralizedFresnelCS(3, dK1, -_K1, _t1);
                    cs2 = GeneralizedFresnelCS(3, dKm, Km, vmN);
                    cs3 = GeneralizedFresnelCS(3, dKm, -Km, vmN);

                    e0 = cs2[0][0] + cs3[0][0];
                    e1 = cs2[1][0] + cs3[1][0];

                    F[0] = (s0 * cs0[0][0]) + (s1 * cs1[0][0]) + (smN * e0) - 2; //- 2 (dx)
                    F[1] = (s0 * cs0[1][0]) + (s1 * cs1[1][0]) + (smN * e1); //- 0 (dz)

                    dsm2 = dsm * dsm;

                    g0 = -((2 * smN) + c14) * dsm2;
                    g1 = (c13 - (smN * smN)) * dsm2;
                    g2 = smN * ((smN * c14) + (2 * c13)) * dsm2;

                    dK0_smN = (((c0 * vmN) + c3) * g0) + (((c1 * vmN) + c2) * g1) - (_K0 * g2);
                    dK1_smN = (((c0 * vmN) + c6) * g0) + (((c4 * vmN) + c5) * g1) + (_K1 * g2);
                    dKm_smN = (((c7 * vmN) + c9) * g1) + ((c8 - (2 * vmN)) * g2);
                    Km_smN =  (((c10 * vmN) + c12) * g1) + c11 * g2;

                    dK0_vmN = (c0 + (c1 * smN)) * dsm;
                    dK1_vmN = (c0 + (c4 * smN)) * dsm;
                    dKm_vmN = (c7 - (2 * smN)) * dsm * smN;
                    Km_vmN = c10 * dsm * smN;

                    f0 = -s0 * cs0[1][2] / 2;
                    f1 = -s1 * cs1[1][2] / 2;
                    f2 = -smN * (cs3[1][2] + cs2[1][2]) / 2;
                    f3 = smN * (cs3[1][1] - cs2[1][1]);
                    f4 = s0 * cs0[0][2] / 2;
                    f5 = s1 * cs1[0][2] / 2;
                    f6 = smN * (cs3[0][2] + cs2[0][2]) / 2;
                    f7 = smN * (cs3[0][1] - cs2[0][1]);

                    jacobian[0][0] = (f0 * dK0_smN) + (f1 * dK1_smN) + (f2 * dKm_smN) + (f3 * Km_smN) + e0;
                    jacobian[0][1] = (f0 * dK0_vmN) + (f1 * dK1_vmN) + (f2 * dKm_vmN) + (f3 * Km_vmN) - (smN * e1);
                    jacobian[1][0] = (f4 * dK0_smN) + (f5 * dK1_smN) + (f6 * dKm_smN) + (f7 * Km_smN) + e1;
                    jacobian[1][1] = (f4 * dK0_vmN) + (f5 * dK1_vmN) + (f6 * dKm_vmN) + (f7 * Km_vmN) + (smN * e0);

                    evaluation = Math.Sqrt((F[0] * F[0]) + (F[1] * F[1]));
                    converged = evaluation < tol;
                    if (converged) break;

                    if (Mathc.Solver2x2.TrySolve(jacobian, F, out double[] x))
                    {
                        smN -= x[0];
                        vmN -= x[1];
                    }

                    //LU factorization
                    //https://math.libretexts.org/Bookshelves/Linear_Algebra/A_First_Course_in_Linear_Algebra_(Kuttler)/02%3A_Matrices/2.10%3A_LU_Factorization


                } while (++u < maxIter);

                if (converged)
                {
                    converged = !double.IsNaN(smN) && !double.IsInfinity(smN) && !double.IsNaN(vmN) && !double.IsInfinity(vmN);
                }

                if (converged)
                {
                    double K0 = _K0 * s0;
                    double K1 = _K1 * s1;

                    double kp0 = dK0 / (s0 * s0);
                    double kp1 = dK1 / (s1 * s1);
                    double kpm = dKm / (smN * smN);
                    double km = Km / smN;


                    //TODO: Debug this method starting with these generated arcs. Compare with G1 segment. 
                    //build the solution parameters
                    //first arc
                    ClothoidSegment firstArc = new ClothoidSegment(new Vector3(-1, 0, 0), _t0, _K0, kp0, s0);

                    Vector3 endpoint1 = firstArc.Endpoint;
                    ClothoidSegment secondArc = new ClothoidSegment(endpoint1, _t0 + K0 + (dK0 / 2), km - (smN * kpm), kpm, 2 * smN);

                    Vector3 endpoint2 = secondArc.Endpoint;
                    ClothoidSegment thirdArc = new ClothoidSegment(endpoint2, _t1 - K1 - (dK1 / 2), _K1 - (s1 * kp1), kp1, s1);

                    return new ClothoidCurve().AddSegments(firstArc, secondArc, thirdArc);
                }

                return new ClothoidCurve();
            }
        }
    }
}