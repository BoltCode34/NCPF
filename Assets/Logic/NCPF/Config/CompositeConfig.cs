using NCPF.Domain;
using NCPF.Shared.Presentation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Config
{
    /// <summary>
    /// Composite agent footprint: a list of domain shapes, each picked by type in the inspector and
    /// placed by its offset and rotation inside the composite, plus the anchor point the body is
    /// dragged by. Assembled into a <see cref="CompositeAgent"/> on every request.
    /// </summary>
    [CreateAssetMenu(fileName = "CompositeConfig", menuName = "NCPF/CompositeConfig")]
    public class CompositeConfig : AgentConfig
    {
        [SerializeField] private List<Part> Parts = new List<Part>();

        [SerializeField]
        [Tooltip("Anchor point in fractions of the bounding rectangle of all parts: (0,0) bottom-left, (1,1) top-right, (0.5,0.5) centre, (0.5,1) front edge. Must match the point the body is dragged by in game. Rebake the map after changing it.")]
        private Vector2 Anchor = new Vector2(0.5f, 0.5f);

        public override Vector2 FootprintSize => Build(1f).Size;
        public override Vector2 FootprintAnchor => Anchor;

        public override Agent GetAgent(float cellSize) => Build(cellSize);

        private CompositeAgent Build(float cellSize)
        {
            List<CompositeAgent.Part> parts = new List<CompositeAgent.Part>(Parts.Count);
            foreach (Part part in Parts)
            {
                if (part != null && part.Shape != null)
                {
                    parts.Add(new CompositeAgent.Part(part.Shape, part.Offset, part.Rotation));
                }
            }
            return new CompositeAgent(parts, cellSize, Anchor);
        }

        [Serializable]
        private class Part
        {
            [Tooltip("Centre of the shape inside the composite, metres: x across the heading, y along it.")]
            public Vector2 Offset;

            [Tooltip("Rotation of the shape inside the composite, degrees counter-clockwise.")]
            public float Rotation;

            [SerializeReference, SubclassSelector]
            public Agent Shape;
        }
    }
}
