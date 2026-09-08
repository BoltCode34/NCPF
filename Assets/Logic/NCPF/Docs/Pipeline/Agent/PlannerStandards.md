# PlannerStandards

The three standing features the planner falls back to when an inspector slot is empty: a point-cell achievement, a direct-distance heuristic and a travel main planner. An instance, not static — a container holds it for null→standard substitution.

`NCPF.Pipeline.Presentation · PlannerStandards.cs`

## What it is

Replaces the old static `PlannerUtility`. The instance holds:

```csharp
public AchivementFeature Achivement { get; } = new PointCellAchivementFeature();
public HeuristicFeature Heuristic { get; } = new DirectDistHeuristicFeature();
public MainPlannerFeature Main { get; } = new TravelPlannerFeature();
```

Created once per [[PathFeatureContainer]]; the container uses it for null fallback on the three main feature slots.

## Related

- [[PathFeatureContainer]] — holds the instance and does null→standard (pres)
- [[PointCellAchivementFeature]] / [[DirectDistHeuristicFeature]] / [[TravelPlannerFeature]] — the three standards
- [[PlannerOverrideFeature]] — overrides are NOT defaulted; they are additive only