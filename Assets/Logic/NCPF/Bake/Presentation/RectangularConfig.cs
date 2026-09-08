using NCPF.Domain;
using NCPF.Shared;
using UnityEngine;

namespace NCPF.Bake.Presentation
{
    /// <summary>
    /// Rectangular agent footprint: width and length in world units, orientation,
    /// and the anchor point the body is dragged by.
    /// </summary>
    [CreateAssetMenu(fileName = "RectangularConfig", menuName = "NCPF/RectangularConfig")]
    public class RectangularConfig : AgentConfig
    {
        [SerializeField] private Vector2 Size;
        [SerializeField] private float Orientation;

        [SerializeField]
        [Tooltip("Anchor point in rectangle fractions: (0,0) bottom-left, (1,1) top-right, (0.5,0.5) centre. Must match the point the body is dragged by in game. Rebake the map after changing it.")]
        private Vector2 Anchor = new Vector2(0.5f, 0.5f);

        public override Vector2 FootprintSize => Size;
        public float FootprintOrientation => Orientation;
        public override Vector2 FootprintAnchor => Anchor;

        public override Agent GetAgent(float cellSize)
        {
            Rectangle agent = new Rectangle(Size.x, Size.y, cellSize, Anchor);
            agent.Angle = Orientation;
            return agent;
        }
    }
}
