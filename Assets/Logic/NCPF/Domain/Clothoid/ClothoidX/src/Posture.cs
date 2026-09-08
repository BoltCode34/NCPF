using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClothoidX
{

    /// <summary>
    /// A Posture is a method to generate target tangents and curvatures for a set of input nodes.
    /// For every subsequent three points a, b, c the posture position P = point b (except for the first
    /// and last posture, which use P = a and P = c respectively). The curvature is the radius of the 
    /// circle C that passes through all three points (it handles collinear points as well), the sign of which
    /// denotes the direction of curvature. The angle is defined by the tangent of C at P. 
    /// </summary>
    public class Posture
    {
        public bool isMirroredX = false;
        public Vector3 Position => new Vector3((float)X, 0, (float)Z);
        public Mathc.VectorDouble PositionD => new Mathc.VectorDouble(X, 0, Z);
        public Mathc.VectorDouble Tangent { get; private set; }

        /// <summary>
        /// The point on the circumference of the circle represented by this posture.
        /// </summary>
        public double X { get; private set; }
        public double Z { get; private set; }

        private double _angle = double.NaN;

        /// <summary>
        /// The the tangent angle in radians of the circle at point (x, z)
        /// </summary>
        public double Angle
        {
            get
            {
                if (double.IsNaN(_angle))
                {
                    _angle = Math.Atan2(Tangent.Z, Tangent.X);
                    //while (_angle < 0) _angle += 360;
                }
                return _angle;
            }
        }
        public double Curvature { get; private set; }
        public double Radius { get { return 1 / Curvature; } }
        public Mathc.VectorDouble CircleCenter { get; private set; }
        /*
                public Posture(float x, float z, float angle, float curvature, Vector3 circleCenter) {
                    this.X = x;
                    this.Z = z;
                    this.Angle = angle;
                    this.Curvature = curvature;
                    this.CircleCenter = circleCenter;
                }
        */
        public Posture(double x, double z, double curvature, Mathc.VectorDouble tangent, Mathc.VectorDouble circleCenter)
        {
            this.X = x;
            this.Z = z;
            this.Curvature = curvature;
            this.Tangent = tangent;
            this.CircleCenter = circleCenter;
        }

        public override string ToString()
        {
            return $"position: ({X}, {Z}) | angle: {Angle} | curvature: {Curvature}";
        }

        /// <summary>
        /// Get a list of samples along the circle represented by this posture.
        /// We do not need double precision here.
        /// </summary>
        /// <param name="numSamples"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        public List<Mathc.VectorDouble> GetSamplesD(int numSamples, double startAngle = 0, double endAngle = 2 * Math.PI)
        {
            //Debug.Log(posture.ToString());
            if (startAngle > endAngle)
            {
                //swap values without introducing temp variable
                (endAngle, startAngle) = (startAngle, endAngle);
            }
            if (Curvature != 0)
            {
                Mathc.VectorDouble sampleCircle = new Mathc.VectorDouble(Math.Abs(Radius), 0, 0);
                List<Mathc.VectorDouble> samples = new List<Mathc.VectorDouble>();
                double sampleAngleDeg = (endAngle - startAngle) / numSamples;
                for (double angleRad = -startAngle; angleRad > -endAngle; angleRad -= sampleAngleDeg)
                {
                    samples.Add((Mathc.VectorDouble.RotateAboutAxis(sampleCircle, Mathc.VectorDouble.UnitY, angleRad) + CircleCenter));
                }
                //add last point manually
                //samples.Add((Mathc.VectorDouble.RotateAboutAxis(sampleCircle, Mathc.VectorDouble.UnitY, -endAngle) + CircleCenter));
                return samples;
            }
            else
            {
                //CircleCenter is the endpoint if the curvature is 0
                var samples = new List<Mathc.VectorDouble>();
                double increment = 1.0 / (numSamples - 1);
                for (double i = 0; i < 1; i += increment)
                {
                    samples.Add(Mathc.VectorDouble.Lerp(PositionD, CircleCenter, i));
                }
                return samples; 
            }
        }

        public List<Vector3> GetSamples(int numSamples, double startAngle = 0, double endAngle = 2 * Math.PI)
        {
            return GetSamplesD(numSamples, startAngle, endAngle).ConvertAll<Vector3>(v => v);
        }

        /// <summary>
        /// Get the Arc Length given an angle in radians.
        /// </summary>
        /// <param name="angleDiff"></param>
        /// <returns></returns>
        public double GetArcLength(double angleDiff)
        {
            //S = R * Theta
            return Math.Abs(Radius * angleDiff);
        }

        /// <summary>
        /// Helper method to calculate the postures for a polyline using System.Numerics.Vector3.
        /// This is less precise than using VectorDouble.
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        public static List<Posture> CalculatePostures(List<Mathc.VectorDouble> polyline, bool loop = false)
        {
            List<Posture> postures = new List<Posture>();
            if (polyline.Count < 3) return postures;

            for (int i = 0; i < polyline.Count; i++)
            {
                if (i == 0)
                {
                    if (loop) postures.Add(CalculatePosture(polyline[^1], polyline[0], polyline[1]));
                    else postures.Add(CalculatePosture(polyline[0], polyline[1], polyline[2], 1));
                    continue;
                }
                if (i == polyline.Count - 1)
                {
                    if (loop) postures.Add(CalculatePosture(polyline[^2], polyline[^1], polyline[0]));
                    else postures.Add(CalculatePosture(polyline[^3], polyline[^2], polyline[^1], 3));
                    continue;
                }
                postures.Add(CalculatePosture(polyline[i - 1], polyline[i], polyline[i + 1]));
            }
            //copy the first posture to the end of the list if we are looping
            if (loop)
            {
                postures.Add(postures[0]);
            }
            return postures;
        }

        public static List<Posture> CalculatePostures(List<Vector3> polyline, bool loop = false)
        {
            return CalculatePostures(polyline.ConvertAll(p => p.ToVD()), loop);
        }

        /// <summary>
        /// Calulates a posture on point{I}, where point1 is the first point and point3 is the last point of the subsequence in the input polyline.
        /// The Posture is simply a list of useful properties of each point on the polyline, position, heading, curvature, etc. 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="I">The index that should be considered the position of the posture, for tangent calculations.</param>
        /// <returns></returns>
        private static Posture CalculatePosture(Mathc.VectorDouble point1, Mathc.VectorDouble point2, Mathc.VectorDouble point3, int I = 2)
        {
            double x;
            double z;
            if (I == 1)
            {
                x = point1.X;
                z = point1.Z;
            }
            else if (I == 2)
            {
                x = point2.X;
                z = point2.Z;
            }
            else
            {
                x = point3.X;
                z = point3.Z;
            }

            double curvature;
            Mathc.VectorDouble tangent;
            Mathc.VectorDouble circleCenter;
            bool areCollinear = Mathc.AreColinearPoints(point1, point2, point3);
            bool foundCenter = false;
            if (areCollinear)
            {
                curvature = 0;
                //tangent = point3 - point1;
                //use the adjacent point to calculate tangent and final position
                if (I == 1)
                {
                    circleCenter = point2;
                    tangent = point2 - point1;
                }
                else if (I == 2)
                {
                    circleCenter = point3;
                    tangent = point3 - point2;
                }
                else
                {
                    circleCenter = point2;
                    tangent = point3 - point2;
                }
            }
            else
            {
                curvature = Mathc.MoretonSequinCurvature(point1, point2, point3);
                if (Mathc.CenterOfCircleOfThreePoints(out Mathc.VectorDouble center, point1, point2, point3))
                {
                    circleCenter = center;
                    //tangent slope is negative reciprocal of normal slope since they are orthogonal.
                    tangent = new Mathc.VectorDouble(z - circleCenter.Z, 0, -(x - circleCenter.X));
                    //Debug.Log($"New Tangent: {tangent}");
                    if (curvature > 0) tangent *= -1;

                }
                else
                {
                    Console.WriteLine($"Warning, something is wrong with the center of the circle calculations! Points: ({point1.X}, {point1.Z}) | ({point2.X}, {point2.Z}) | ({point3.X}, {point3.Z})  || Colinear: {areCollinear}");
                    circleCenter = point1;
                    tangent = Mathc.VectorDouble.Zero;
                }
            }

            //angle = Mathf.Abs(angle);

            if (foundCenter)
            {
                //Debug.Log($"We found the center! point1: {point1} | point2: {point2} | point3: {point3} | areCollinear: {areCollinear} | curvature: {curvature} | circleCenter: {circleCenter} | angle: {angle}");
            }

            return new Posture(x, z, curvature, tangent, circleCenter);
        }


        /// <summary>
        /// Given two postures, return two of the same postures but in standard form. 
        /// The first Posture will be centered on the origin with its tangent aligned with the +X-Axis.
        /// The second posture will be relative to that. If the second posture ends up in the fourth 
        /// quadrant, it will be mirrored along the X-Axis and it will be flagged isMirroredX.
        /// 
        /// TODO: This isn't finished.
        /// </summary>
        /// <param name="postures"></param>
        /// <returns></returns>
        public static (Posture, Posture) InStandardForm(Posture posture1, Posture posture2)
        {
            Posture newPosture1;
            Posture newPosture2;

            //Debug.Log($"Rotate by {posture1.Angle} degrees");
            //Debug.Log($"Rotate 2 by {posture2.Angle} degrees");
            //rotate relative positions by the start tangent angle
            Mathc.VectorDouble endPosition = Mathc.VectorDouble.RotateAboutAxis((posture2.PositionD - posture1.PositionD), Mathc.VectorDouble.UnitY, posture1.Angle);
            Mathc.VectorDouble circle1 = Mathc.VectorDouble.RotateAboutAxis((posture1.CircleCenter - posture1.PositionD), Mathc.VectorDouble.UnitY, posture1.Angle);
            Mathc.VectorDouble circle2 = Mathc.VectorDouble.RotateAboutAxis((posture2.CircleCenter - posture1.PositionD), Mathc.VectorDouble.UnitY, posture1.Angle);
            Mathc.VectorDouble endTangent = Mathc.VectorDouble.RotateAboutAxis(posture2.Tangent, Mathc.VectorDouble.UnitY, posture1.Angle);

            newPosture1 = new Posture(0, 0, Math.Abs(posture1.Curvature), Mathc.VectorDouble.UnitX * posture1.Tangent.Length, circle1);
            newPosture2 = new Posture(endPosition.X, Math.Abs(endPosition.Z), Math.Abs(posture2.Curvature), endTangent, circle2);

            if (endPosition.Z < 0)
            {
                //Debug.Log($"IS MIRRORED: {endPosition}");
                newPosture2.isMirroredX = true;
                newPosture2.Tangent = new Mathc.VectorDouble(newPosture2.Tangent.X, newPosture2.Tangent.Y, -newPosture2.Tangent.Z);

                newPosture1.CircleCenter = new Mathc.VectorDouble(newPosture1.CircleCenter.X, newPosture1.CircleCenter.Y, -newPosture1.CircleCenter.Z);
                newPosture2.CircleCenter = new Mathc.VectorDouble(newPosture2.CircleCenter.X, newPosture2.CircleCenter.Y, -newPosture2.CircleCenter.Z);
            }

            return (newPosture1, newPosture2);
        }

        public static IEnumerator<(Posture, Posture)> GetStandardPostures(List<Posture> postures)
        {
            for (int i = 0; i + 1 < postures.Count; i++)
            {
                yield return InStandardForm(postures[i], postures[i + 1]);
            }
        }

        /// <summary>
        /// Given a list of postures, return a polyline of samples along the small arcs defined by the postures.
        /// </summary>
        /// <param name="postures"></param>
        /// <param name="numSamples"></param>
        /// <returns></returns>
        public static List<Mathc.VectorDouble> GetSmallArcSamples(List<Posture> postures, int numSamples = 100)
        {
            List<Mathc.VectorDouble> biarc = new List<Mathc.VectorDouble>();
            if (postures.Count < 2)
            {
                //throw new ArgumentException("At least two postures are required to create a biarc.");
                return biarc;
            }
            double angle1, angle2;
            var arcLength = new List<double>();
            double temp;

            for (int i = 1; i < postures.Count; i++)
            {
                angle1 = Math.Atan2(postures[i - 1].Z - postures[i].CircleCenter.Z, postures[i - 1].X - postures[i].CircleCenter.X);
                angle2 = Math.Atan2(postures[i].Z - postures[i].CircleCenter.Z, postures[i].X - postures[i].CircleCenter.X);
                arcLength.Add(postures[i].GetArcLength(angle2 - angle1));
            }            

            double totalArcLength = arcLength.Sum();
            int totalSamples;

            for (int i = 1; i < postures.Count; i++)
            {
                angle1 = Math.Atan2(postures[i - 1].Z - postures[i].CircleCenter.Z, postures[i - 1].X - postures[i].CircleCenter.X);
                angle2 = Math.Atan2(postures[i].Z - postures[i].CircleCenter.Z, postures[i].X - postures[i].CircleCenter.X);
                temp = arcLength[i - 1];
                totalSamples = (int)Math.Floor(temp * numSamples / totalArcLength);
                biarc.AddRange(postures[i].GetSamplesD(totalSamples, angle1, angle2));
            }
            return biarc;
        }
    }
}