using NCPF.Pipeline.Application;
using System.Collections.Generic;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Owns the feature-to-pass wiring and the dirty-work around inspector features: dedupes a
    /// changed feature against a buffer, substitutes a standing standard for an empty slot,
    /// materializes the pass/builder on the live <see cref="BehaviorBus"/>, and adds/removes
    /// override passes as the override list changes. Created fresh on each (re)construct so the
    /// buffers and the feature→pass map start clean.
    /// </summary>
    public class PathFeatureContainer
    {
        private readonly PlannerStandards _standards;
        private MainPlannerFeature _mainBuf;
        private AchivementFeature _achivementBuf;
        private HeuristicFeature _heuristicBuf;
        private readonly List<PlannerOverrideFeature> _overridesBuf = new List<PlannerOverrideFeature>();
        private readonly Dictionary<PlannerOverrideFeature, SAPass> _passes = new Dictionary<PlannerOverrideFeature, SAPass>();

        public PathFeatureContainer(PlannerStandards standards)
        {
            _standards = standards;
        }

        /// <summary>Reconciles the live features with the bus: a changed slot is re-materialized,
        /// a null slot falls back to its standard, and override passes are added/removed to
        /// match the list. No-ops when nothing changed.</summary>
        public void Bind(BehaviorBus bus, PathAgentContext context,
            MainPlannerFeature main, AchivementFeature achivement, HeuristicFeature heuristic,
            List<PlannerOverrideFeature> overrides)
        {
            if (main != _mainBuf)
            {
                _mainBuf = main;
                bus.SetMain((main ?? _standards.Main).CreatePass(context));
            }
            if (achivement != _achivementBuf)
            {
                _achivementBuf = achivement;
                bus.SetAchivement((achivement ?? _standards.Achivement).CreateBuilder(context));
            }
            if (heuristic != _heuristicBuf)
            {
                _heuristicBuf = heuristic;
                bus.SetHeuristic((heuristic ?? _standards.Heuristic).CreateBuilder(context));
            }

            for (int i = 0; i < overrides.Count; i++)
            {
                if (!_overridesBuf.Contains(overrides[i]))
                {
                    _overridesBuf.Add(overrides[i]);
                    SAPass pass = overrides[i].CreatePass(context);
                    _passes[overrides[i]] = pass;
                    bus.AddOverride(pass);
                }
            }
            for (int i = _overridesBuf.Count - 1; i >= 0; i--)
            {
                if (!overrides.Contains(_overridesBuf[i]))
                {
                    bus.RemoveOverride(_passes[_overridesBuf[i]]);
                    _passes.Remove(_overridesBuf[i]);
                    _overridesBuf.RemoveAt(i);
                }
            }
        }
    }
}
