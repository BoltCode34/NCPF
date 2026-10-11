using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClothoidX
{

    /// <summary>
    /// A clothoid curve that uses the ClothoidSegment22 class, and generates the curve using the ClothoidSegment22.Position property, to attempt to guarantee G0 continuity.
    /// This is different from ClothoidCurve, which doesn't can only have a single offset, the rest of the curve is solely determined by the segment properties such as curvature, sharpness and arc length.
    /// </summary>
    public class ClothoidCurve
    {

        /// <summary>
        /// Overall Offset vector for the curve. If the curve passes through all the polyline nodes, this should be the position of the first node.
        /// </summary>
        public Mathc.VectorDouble Offset = Mathc.VectorDouble.Zero;
        /// <summary>
        /// Overall angle offset of the curve in radians. This should be close to if not equal to the first Posture's tangent angle. 
        /// </summary>
        public double AngleOffset = 0;

        /// <summary>
        /// Number of segments in this curve. 
        /// </summary>
        public int Count { get { return segments.Count; } }

        public ClothoidSegment this[int index]
        {
            get => this.segments[index];
        }

        public Mathc.VectorDouble Endpoint => this[^1].Endpoint.ToVD();

        public int PolylineCount { get { return this.inputPolyline.Count; } }

        public double StartCurvature
        {
            get
            {
                if (segments.Count == 0) return 0;
                return segments[0].StartCurvature;
            }
        }

        public double EndCurvature
        {
            get
            {
                if (segments.Count == 0) return 0;
                return segments[^1].EndCurvature;
            }
        }

        public double TotalArcLength
        {
            get
            {
                if (segments.Count == 0) return 0;
                return segments[^1].ArcLengthEnd - segments[0].ArcLengthStart;
            }
        }
        protected List<ClothoidSegment> segments = new List<ClothoidSegment>();
        protected List<Vector3> inputPolyline = new List<Vector3>();

        /// <summary>
        /// If the curve position and rotation are approximated, this will be the center of mass of the generated curve in local space.
        /// </summary>
        protected Mathc.VectorDouble curveCM = Mathc.VectorDouble.Zero;

        /// <summary>
        /// If the curve position and rotation are approximated, this will be the center of mass of the input polyline nodes.
        /// </summary>
        protected Mathc.VectorDouble polylineCM = Mathc.VectorDouble.Zero;

        /// <summary>
        /// If the curve position and rotation are approximated, this will be a 3 column row vector that the curve sample point (3 row column vector)
        /// gets multiplied by  after offsetting the point by the overall curve center of mass.
        /// </summary>
        public double[][] rotationMatrix = Mathc.SVDJacobiProgram.MatMake(1, 3);// new double[3][] { new double[] { 0, 0, 0 } };

        /// <summary>
        /// Create a clothoid curve with just a curve factor.
        /// This curve factor is used to scale down the curve before sampling the fresnel integrals. 
        /// Then the point is scaled back up by the same factor.
        /// As far as I know it is only used in the SinghMcCrae solution.
        /// </summary>
        /// <param name="CURVE_FACTOR"></param>
        public ClothoidCurve()
        {
            this.segments = new List<ClothoidSegment>();
            this.inputPolyline = new List<Vector3>();
        }

        public ClothoidCurve(List<Vector3> inputPolyline)
        {
            this.segments = new List<ClothoidSegment>();
            this.inputPolyline = inputPolyline;
        }

        public ClothoidCurve(List<ClothoidSegment> orderedSegments, List<Vector3> inputPolyline)
        {
            this.segments = orderedSegments;
            this.inputPolyline = inputPolyline;
        }

        /// <summary>
        /// Create a copy of a ClothoidCurve object. 
        /// </summary>
        /// <param name="curve"></param>
        public ClothoidCurve(ClothoidCurve curve) : this(CopySegments(curve), curve.inputPolyline)
        {
            Offset = curve.Offset;
            AngleOffset = curve.AngleOffset;
        }

        /// <summary>
        /// Get the tangent angle from the positive X axis at a specific arc length (in radians).
        /// </summary>
        /// <param name="arcLength"></param>
        /// <returns></returns>
        public double Tangent(double arcLength)
        {
            if (arcLength < 0 || arcLength > TotalArcLength) throw new ArgumentOutOfRangeException("arc length needs to be positive and less than the total arc length of the curve");
            //TODO: this doesn't provide a good value after the first segment, fix ur shit bro
            double angle = AngleOffset;
            for (int i = 0; i < Count; i++)
            {
                if (arcLength >= this[i].ArcLengthStart && arcLength <= this[i].ArcLengthEnd)
                {
                    double relativeAL = arcLength - this[i].ArcLengthStart; //arc length relative to this segment
                    //this works because segments are only represented in the standard frame, with their initial tangent oriented along the +x axis
                    angle += this[i].Tangent(relativeAL);
                    break;
                }
                else
                {
                    angle += this[i].AngleEnd;
                }
            }
            return angle;
        }

        /// <summary>
        /// Best fit translation and rotation are used when the curve is an approximation of your input polyline. 
        /// This as of now is only used in the SinghMcCrae solution. But if you can't calculate an offset for your
        /// clothoid curve (typically with the offset and tangent of the fist polyline node), you can solve for a best fit
        /// using the SinghMcCrae solution and then running this method. 
        /// 
        /// TODO: See if this can be replaced with the Offset and AngleOffset properties. 
        /// </summary>
        /// <param name="curveCM"></param>
        /// <param name="polylineCM"></param>
        /// <param name="rotationMatrix"></param>
        public void AddBestFitTranslationRotation(Mathc.VectorDouble curveCM, Mathc.VectorDouble polylineCM, double[][] rotationMatrix)
        {
            Console.WriteLine($"New Rotation matrix: ");
            for (int i = 0; i < rotationMatrix.Length; i++)
            {
                Console.WriteLine();
                Console.Write("[");
                for (int j = 0; j < rotationMatrix[i].Length; j++)
                {
                    Console.Write($"{rotationMatrix[i][j]}");
                    if (j < rotationMatrix[i].Length - 1) Console.Write(",");
                }
                Console.Write("]");
            }
            Console.WriteLine();
            //Mathc.SVDJacobiProgram.MatShow(this.rotationMatrix);
            Console.WriteLine($"Curve Center Mass: {curveCM.ToString()}");
            Console.WriteLine($"Polyline Center Mass: {polylineCM.ToString()}");
            foreach (ClothoidSegment segment in segments)
            {
                segment.AddBestFitTranslationRotation(curveCM, polylineCM, rotationMatrix);
            }
        }

        public ClothoidCurve AddSegment(ClothoidSegment newSegment)
        {
            if (segments.Count > 0) newSegment.ShiftStartArcLength(segments[^1].ArcLengthEnd);
            else newSegment.ShiftStartArcLength(0);
            this.segments.Add(newSegment);
            return this;
        }

        public ClothoidCurve AddSegments(params ClothoidSegment[] segments)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                AddSegment(segments[i]);
            }
            return this;
        }

        public void Reset()
        {
            this.segments.Clear();
            this.inputPolyline.Clear();
        }

        /// <summary>
        /// Sample positions from this curve, without applying the input polyline transformations.
        /// In local space of the curve.
        /// </summary>
        /// <param name="arcLength"></param>
        /// <returns></returns>
        public Vector3 SampleCurveFromArcLength(double arcLength)
        {
            //if (arcLength == 0) return Vector3.Zero;
            Vector3 value = Vector3.Zero;
            for (int i = 0; i < segments.Count; i++)
            {
                ClothoidSegment segment = segments[i];
                if (arcLength >= segment.ArcLengthStart && arcLength <= segment.ArcLengthEnd)
                {
                    //Debug.Log($"Now sampling curve type: {segment.LineType}");
                    return segment.GetSample(arcLength - segment.ArcLengthStart);
                }
            }
            return value;
        }

        /// <summary>
        /// Get samples along the curve, if it was generated from a polyline this will return points that approximate the input polyline as well. If not the points will start from the origin.
        /// </summary>
        /// <param name="numSamples"></param>
        /// <returns></returns>
        public List<Vector3> GetSamples(int numSamples)
        {
            List<Vector3> points = new List<Vector3>();
            double increment = TotalArcLength / (numSamples - 1);
            //Debug.Log($"Increment size: {increment}, totalArcLength: {TotalArcLength}, numSamples: {numSamples}");
            for (double arcLength = 0; arcLength < TotalArcLength; arcLength += increment)
            {
                //if (point == Vector3.Zero && arcLength != 0) continue;
                //Console.WriteLine($"Sampling arc length (curve samples): {arcLength} / {TotalArcLength}");
                points.Add(SampleCurveFromArcLength(arcLength)); //untranslated, unrotated
            }
            points.Add(SampleCurveFromArcLength(TotalArcLength));  //untranslated, unrotated

            return points;
        }

        /// <summary>
        /// Get the position of a polyline node by its index.
        /// </summary>
        /// <param name="polylineIndex"></param>
        /// <returns></returns>
        public Vector3 GetPositionFromPolyline(int polylineIndex)
        {
            if (polylineIndex >= this.inputPolyline.Count) polylineIndex = this.inputPolyline.Count - 1;
            return this.inputPolyline[polylineIndex];
        }

        /// <summary>
        /// Get the arc length of a polyline node based on its index.
        /// </summary>
        /// <param name="polylineIndex"></param>
        /// <returns></returns>
        public float GetStartingArcLengthFromPolylineIndex(int polylineIndex)
        {
            return EstimateArcLength(polylineIndex);
        }

        public void ChangeSolutionType(SolutionType newDrawingMethod)
        {
            foreach (ClothoidSegment segment in segments)
            {
                segment.ChangeSolutionType(newDrawingMethod);
            }
        }

        protected float EstimateArcLength(Vector3 node)
        {
            if (this.inputPolyline.Contains(node) == false) throw new ArgumentException("Node not contained in the input polyline.");
            return EstimateArcLength(this.inputPolyline.IndexOf(node));
        }

        protected float EstimateArcLength(int polylineIndex)
        {
            if (polylineIndex > this.inputPolyline.Count - 2) polylineIndex = this.inputPolyline.Count - 2;
            if (this.inputPolyline == null) throw new ArgumentNullException();
            if (this.inputPolyline.Count == 0) throw new ArgumentOutOfRangeException();
            float sum = 0;
            for (int i = 0; i + 1 <= polylineIndex; i++)
            {
                sum += Vector3.Distance(this.inputPolyline[i], this.inputPolyline[i + 1]);
            }
            return sum;
        }

        /// <summary>
        /// Estimate the arc length at a given node along the polyline index
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static float EstimateArcLength(List<Vector3> polyline, int index)
        {
            if (index < 0 || index >= polyline.Count) throw new ArgumentOutOfRangeException();
            float sum = 0;
            Vector3 prev = polyline[0];
            for (int i = 1; i <= index; i++)
            {
                sum += Vector3.Distance(prev, polyline[i]);
                prev = polyline[i];
            }
            return sum;
        }


        /// <summary>
        /// Add two clothoid curves together. This operation is not commutative, the order matters. The end curve will be added on to the start curve.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static ClothoidCurve operator +(ClothoidCurve start, ClothoidCurve end)
        {
            ClothoidCurve result = new ClothoidCurve();
            //set the offsets the same as the start offset
            result.Offset = start.Offset;
            result.AngleOffset = start.AngleOffset;
            // Add the segments one at a time
            result.AddSegments(start.segments.ToArray())
            .AddSegments(end.segments.ToArray());/*
            for (int i = 0; i < start.segments.Count; i++) {
                result.AddSegment(start.segments[i]);
            }
            for (int i = 0; i < end.segments.Count; i++) {
                result.AddSegment(end.segments[i]);
            }*/

            return result;
        }

        public static ClothoidCurve operator +(ClothoidCurve start, ClothoidSegment newSegment)
        {
            ClothoidCurve result = new ClothoidCurve
            {
                Offset = start.Offset,
                AngleOffset = start.AngleOffset
            };
            result.AddSegments(start.segments.ToArray()).AddSegment(newSegment);
            return result;
        }

        /// <summary>
        /// Copy the segments of a clothoid curve into a list
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static List<ClothoidSegment> CopySegments(ClothoidCurve curve)
        {
            List<ClothoidSegment> segments = new List<ClothoidSegment>();
            for (int i = 0; i < curve.Count; i++)
            {
                segments.Add(new ClothoidSegment(curve.Endpoint, curve[i].AngleEnd, curve[i].StartCurvature, curve[i].Sharpness, curve[i].TotalArcLength, curve[i].SolutionType));
            }
            return segments;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < segments.Count; i++)
            {
                s += segments[i].Description() + ", ";
            }
            return s;
        }
    }
}
