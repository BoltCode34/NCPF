using UnityEngine;

namespace NCPF.Shared.Presentation
{
    /// <summary>
    /// Marker for the subclass selector drawer: on a [SerializeReference] field, draws a picker of a
    /// concrete type among the heirs of the declared type, with the instance fields below. On a list
    /// the same applies per element. The attribute itself does nothing — it is a signal to the drawer.
    /// </summary>
    public class SubclassSelectorAttribute : PropertyAttribute { }
}
