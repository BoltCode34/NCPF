using NCPF.Pipeline.Application;
using System;
using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>Dijkstra-field heuristic — exact, but expensive. The parameter is the turn cost.</summary>
    [Serializable]
    public class DijkstraHeuristicFeature : HeuristicFeature
    {
        [SerializeField] private float _turnPenaltyCells = 0.7f;

        public override IHeuristicBuilder CreateBuilder(PathAgentContext ctx)
            => new DijkstraHeuristicBuilder(ctx.Grid, _turnPenaltyCells, ctx.DynamicAgent.MaxVelocity);
    }
}
