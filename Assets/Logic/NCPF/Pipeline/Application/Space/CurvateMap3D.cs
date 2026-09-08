using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The geometric search graph. Describing the graph — INCLUDING the weights of outgoing
    /// transitions — is its duty under <see cref="ICurvateGraph"/>; graph decorators rely on exactly
    /// those weights. In THIS graph the weight-handling responsibility is DELEGATED to
    /// <see cref="ITransitionGenerator"/>: enumerating neighbors and pricing them is entirely its
    /// business; the graph knows neither the projectors nor where weights come from.
    /// </summary>
    public class CurvateMap3D : ICurvateGraph
    {
        private readonly IGridMap3D _grid;
        private readonly ITransitionGenerator _generator;

        public CurvateMap3D(IGridMap3D grid, ITransitionGenerator generator)
        {
            _grid = grid;
            _generator = generator;
        }

        public CurvateTransition[] GetNeightbors(int id) => _generator.GetPrimitives(_grid.IdToCell3D(id), _grid);

        public bool PointFree(int id) => _grid.Cell3DFree(_grid.IdToCell3D(id));
    }
}
