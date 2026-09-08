using System.Linq;
using UnityEngine;
namespace NCPF.Domain
{
    public interface IGraphSpace
    {
        public Vector2 IDToWorld(int id);
        public int WorldToID(Vector2 pos);
    }
}

namespace NCPF.Domain
{
    public interface ISpacedGraph : IDirectGraph, IGraphSpace
    {

    }
}

namespace NCPF.Domain
{
    public interface IGraph
    {
        public ITransition[] GetNeightbors(int id);
        public bool PointFree(int id);
    }
}

namespace NCPF.Domain
{
    public interface IGraph<T> : IGraph
        where T : ITransition
    {
        public new T[] GetNeightbors(int id);
        ITransition[] IGraph.GetNeightbors(int id) => GetNeightbors(id).Select(t=>(ITransition)t).ToArray();
    }
}

namespace NCPF.Domain
{
    public interface IDirectGraph : IGraph<DirectTransition>
    {

    }
}

namespace NCPF.Domain
{
    public interface ITransition
    {

        public int Neightbor { get; }
        public float Weight { get; }
    }
}

namespace NCPF.Domain
{
    public struct DirectTransition : ITransition
    {
        public int Neightbor { get; set; }
        public float Weight { get; set; }

        public DirectTransition(int neightbor, float weight)
        {
            Neightbor = neightbor;
            Weight = weight;
        }
    }
}
