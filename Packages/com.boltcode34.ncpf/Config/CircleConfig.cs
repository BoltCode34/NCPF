using NCPF.Domain;
using NCPF.Shared.Presentation;
using UnityEngine;

namespace NCPF.Config
{
    /// <summary>Circular agent footprint: radius in world units and the anchor point the body is dragged by.</summary>
    [CreateAssetMenu(fileName = "CircleConfig", menuName = "NCPF/CircleConfig")]
    public class CircleConfig : AgentConfig
    {
        [SerializeField, Min(0f)] private float Radius = 0.5f;

        [SerializeField]
        [Tooltip("Anchor point in fractions of the bounding square: (0,0) bottom-left, (1,1) top-right, (0.5,0.5) centre, (0.5,1) front point. Must match the point the body is dragged by in game. Rebake the map after changing it.")]
        private Vector2 Anchor = new Vector2(0.5f, 0.5f);

        public override Vector2 FootprintSize => new Vector2(Radius * 2f, Radius * 2f);
        public override Vector2 FootprintAnchor => Anchor;

        public override Agent GetAgent(float cellSize) => new Circle(Radius, cellSize, Anchor);
    }
}
