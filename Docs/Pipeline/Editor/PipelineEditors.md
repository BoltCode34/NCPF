# Pipeline editors

Inspector tools for the pipeline. They live in an Editor-only assembly and are not included in builds.

`NCPF.Pipeline.Editor`

## SubclassSelectorDrawer

`SubclassSelectorDrawer.cs`

Draws fields marked with `[SerializeReference, SubclassSelector]` as a type picker. It is what lets you choose a feature on [PathPlanner](../Presentation/PathPlanner.md) from a dropdown.

- **A single field** is drawn as a section with the slot's name, a searchable dropdown of types — like **Add Component** — and the chosen object's fields below it.
- **A list** is drawn with Unity's default list, and each element gets its own picker.

The dropdown lists every concrete class that derives from the field's type, across all assemblies, as long as it has a parameterless constructor and is not marked obsolete. Your own features appear automatically.

The `SubclassSelectorAttribute` itself does nothing; it only tells this drawer to handle the field.

## Related

- [Planner features](../Presentation/PlannerFeatures.md) — the fields it is used for.
- [Bake editors](../../Bake/Editor/BakeEditors.md) — the matching tools for the bake.
