# Shared editors

Inspector tools used by both the bake and the pipeline. They live in an Editor-only assembly and are not included in builds.

`NCPF.Shared.Editor`

## SubclassSelectorDrawer

`SubclassSelectorDrawer.cs`

Draws fields marked with `[SerializeReference, SubclassSelector]` as a type picker. It is what lets you choose a feature on [PathPlanner](../../Pipeline/Presentation/PathPlanner.md) and a part's shape in a [CompositeConfig](../../Config/CompositeConfig.md) from a dropdown.

- **A single field** is drawn as a section headed with the field's name, a searchable dropdown of types — like **Add Component** — and the chosen object's fields below it.
- **A list** is drawn with Unity's default list, and each element gets its own picker, headed with the list's name and index, for example **Overrides 0**.

The dropdown lists every concrete class that derives from the field's type, across all assemblies, as long as it has a parameterless constructor and is not marked obsolete. Your own types appear automatically.

`SubclassSelectorAttribute` lives in `NCPF.Shared.Presentation`, so any presentation or config assembly can use it. The attribute itself does nothing; it only tells this drawer to handle the field.

## Related

- [Planner features](../../Pipeline/Presentation/PlannerFeatures.md) — the planner slots it draws.
- [CompositeConfig](../../Config/CompositeConfig.md) — the shape slots it draws.
- [Bake editors](../../Bake/Editor/BakeEditors.md) — the bake's Inspector tools.
