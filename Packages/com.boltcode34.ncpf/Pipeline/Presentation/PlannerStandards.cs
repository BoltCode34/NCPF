namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// The three standing features the planner falls back to when an inspector slot is empty: a
    /// point-cell achivement, a direct-distance heuristic and a travel main planner. An instance,
    /// not static — a container holds it for null→standard substitution.
    /// </summary>
    public class PlannerStandards
    {
        public AchivementFeature Achivement { get; } = new PointCellAchivementFeature();
        public HeuristicFeature Heuristic { get; } = new DirectDistHeuristicFeature();
        public MainPlannerFeature Main { get; } = new TravelPlannerFeature();
    }
}
