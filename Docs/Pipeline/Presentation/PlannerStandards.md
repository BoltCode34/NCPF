# PlannerStandards

The standard features a planner uses when a feature slot is empty.

`NCPF.Pipeline.Presentation` · `PlannerStandards.cs`

## Description

Every [PathPlanner](PathPlanner.md) needs an achievement, a heuristic and a main planner to work. PlannerStandards supplies one of each, so a planner with empty feature slots still behaves sensibly.

| Property | Feature | Behaviour |
|---|---|---|
| `Achivement` | `PointCellAchivementFeature` | Reach the target's cell. |
| `Heuristic` | `DirectDistHeuristicFeature` | Straight-line distance divided by top speed. |
| `Main` | [TravelPlannerFeature](TravelPlannerFeature.md) | Drive as fast as the curves allow, facing along the path. |

It is an instance rather than a static class, so a planner can be given a different set of standards.

## Related

- [Planner features](PlannerFeatures.md) — all features and how slots work.
- [PathFeatureContainer](PathFeatureContainer.md) — substitutes a standard for an empty slot.
