using NCPF.Pipeline.Application;
using System;
using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// A RING achievement: any cell of the [min..max] metre annulus around the goal counts. For
    /// "approach and keep distance" semantics (orbiting prey) the goal is a REGION, and the search
    /// picks the best entry point — rope, walls and heuristic included. A band, not a thin ring: a
    /// thin ring may hit no lattice node; keep the width at a couple of cells or more. Distances are
    /// in METRES. The heuristic stays "to the center"; it overestimates the remainder by at most
    /// maxDistance — acceptable greed under the ε-ladder.
    /// </summary>
    [Serializable]
    public class RingAchivementFeature : AchivementFeature
    {
        [SerializeField] private float _minDistance = 2f;
        [SerializeField] private float _maxDistance = 3f;

        public override IAchivementBuilder CreateBuilder(PathAgentContext ctx)
            => new RingAchivementBuilder(ctx.Grid, _minDistance, _maxDistance);
    }
}
