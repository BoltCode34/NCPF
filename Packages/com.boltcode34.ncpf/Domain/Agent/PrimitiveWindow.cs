using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// The speed window: the chain of primitives lookahead metres ahead of the
    /// cursor, ROUNDED UP to a primitive boundary — the horizon decides whether
    /// to start the next primitive, but never cuts the one it started. Built
    /// once per replan and read by the map and the heuristic; geometry is not
    /// re-asked from the model, only length, gear and curvature matter here.
    /// </summary>
    public class PrimitiveWindow
    {
        /// <summary>A degenerate part remainder: shorter than this is junction garbage, not a part.</summary>
        public const float MinPartLength = 1e-4f;

        private const float KappaEps = 1e-6f;

        public readonly WindowPart[] Parts;
        public readonly int Count;

        /// <summary>Cumulative metres from the cursor to each part's ENTRY; [Count] is the whole window arc.</summary>
        public readonly float[] Arc;

        public float TotalArc => Arc[Count];

        public PrimitiveWindow(PathAgentModel model, float lookahead, float vTop,
                               float aMax, float wMaxDegPerSec)
        {
            List<WindowPart> parts = new List<WindowPart>();
            float a = Mathf.Max(0.001f, aMax);
            float wMaxRad = Mathf.Max(1f, wMaxDegPerSec) * Mathf.Deg2Rad;

            if (model?.CurrentPart != null)
            {
                float budget = Mathf.Max(0.2f, lookahead);
                for (int i = model.CurrentPartIndex; i < model.Parts.Count && budget > MinPartLength; i++)
                {
                    IPath part = model.Parts[i];
                    float startArc = i == model.CurrentPartIndex ? model.CurrentPartArc : 0f;
                    float rest = part.Length - startArc;
                    if (rest <= MinPartLength)
                    {
                        continue;
                    }
                    parts.Add(BuildPart(part, rest, startArc, vTop, a, wMaxRad));
                    budget -= rest;
                }
            }

            Parts = parts.ToArray();
            Count = Parts.Length;
            Arc = new float[Count + 1];
            for (int i = 0; i < Count; i++)
            {
                Arc[i + 1] = Arc[i] + Parts[i].Length;
            }
        }

        /// <summary>Metres from part's entry to the window's end (for the heuristic; clamped).</summary>
        public float RemainingArc(int part)
        {
            if (part < 0)
            {
                return TotalArc;
            }
            if (part > Count)
            {
                return 0f;
            }
            return TotalArc - Arc[part];
        }

        /// <summary>The local-curvature probe step, m.</summary>
        public const float ProbeStep = 0.05f;

        /// <summary>
        /// The LOCAL curvature at arc: the tangent's turn over a <see cref="ProbeStep"/>
        /// probe FORWARD along the arc (BACKWARD near the part's end), rad/m. Not the
        /// part-wide peak of <see cref="WindowPart.KappaPeak"/> — the curvature right
        /// here; the runtime speed clamps price their limits from it at every instant.
        /// A degenerately short part (under a millimetre both ways) yields 0.
        /// </summary>
        public static float KappaAt(IPath path, float arc)
        {
            const float eps = 1e-4f;
            float h = ProbeStep;
            float rest = path.Length - arc;

            float a0, a1;
            if (rest >= h)
            {
                a0 = path.Evaluate(arc).Angle;
                a1 = path.Evaluate(arc + h).Angle;
            }
            else if (arc >= h)
            {
                a0 = path.Evaluate(arc - h).Angle;
                a1 = path.Evaluate(arc).Angle;
            }
            else
            {
                h = Mathf.Max(arc, rest);
                if (h < eps)
                {
                    return 0f;
                }
                if (rest >= arc)
                {
                    a0 = path.Evaluate(arc).Angle;
                    a1 = path.Evaluate(path.Length).Angle;
                }
                else
                {
                    a0 = path.Evaluate(0f).Angle;
                    a1 = path.Evaluate(arc).Angle;
                }
            }
            return Mathf.Abs(Mathf.DeltaAngle(a0, a1)) * Mathf.Deg2Rad / h;
        }

        private static WindowPart BuildPart(IPath path, float length, float startArc,
                                            float vTop, float aMax, float wMaxRad)
        {
            int samples = Mathf.Clamp(Mathf.CeilToInt(length / 0.05f), 4, 32);
            float ds = length / samples;
            float prev = path.Evaluate(startArc).Angle;
            float peak = 0f;
            for (int i = 1; i <= samples; i++)
            {
                float angle = path.Evaluate(startArc + i * ds).Angle;
                float kappa = Mathf.Abs(Mathf.DeltaAngle(prev, angle)) * Mathf.Deg2Rad / ds;
                if (kappa > peak)
                {
                    peak = kappa;
                }
                prev = angle;
            }

            float vCap = vTop;
            if (peak > KappaEps)
            {
                vCap = Mathf.Min(vTop, Mathf.Min(Mathf.Sqrt(aMax / peak), wMaxRad / peak));
            }
            return new WindowPart(length, PathGearUtility.GearOf(path), peak, vCap);
        }
    }
}
