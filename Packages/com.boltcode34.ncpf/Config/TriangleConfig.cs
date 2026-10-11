using NCPF.Domain;
using NCPF.Shared.Presentation;
using UnityEngine;

namespace NCPF.Config
{
    /// <summary>
    /// Isosceles triangle agent footprint: base width and height along the heading in world units,
    /// apex forward, and the anchor point the body is dragged by.
    /// </summary>
    [CreateAssetMenu(fileName = "TriangleConfig", menuName = "NCPF/TriangleConfig")]
    public class TriangleConfig : AgentConfig
    {
        [SerializeField, Min(0f)] private float Width = 0.5f;
        [SerializeField, Min(0f)] private float Height = 1f;

        [SerializeField]
        [Tooltip("Anchor point in fractions of the bounding rectangle: (0,0) bottom-left, (1,1) top-right, (0.5,0.5) centre, (0.5,1) apex. Must match the point the body is dragged by in game. Rebake the map after changing it.")]
        private Vector2 Anchor = new Vector2(0.5f, 0.5f);

        public override Vector2 FootprintSize => new Vector2(Width, Height);
        public override Vector2 FootprintAnchor => Anchor;

        public override Agent GetAgent(float cellSize) => new Triangle(Width, Height, cellSize, Anchor);
    }
}
