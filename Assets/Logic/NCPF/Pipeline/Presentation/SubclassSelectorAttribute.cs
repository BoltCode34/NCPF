using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Marker for <see cref="SubclassSelectorDrawer"/>: on a [SerializeReference] field, draws a
    /// picker of a concrete type among the heirs of the declared type. A single field — an object
    /// selector (click → search window like Add Component, instance fields below); a list — the
    /// same per element plus an "Add Override" button. The attribute itself does nothing — it is
    /// a signal to the drawer.
    /// </summary>
    public class SubclassSelectorAttribute : PropertyAttribute { }
}
