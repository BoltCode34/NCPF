using NCPF.Domain;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Decorator for <see cref="IControlSetBuilder{T}"/> that opens a log
    /// generation around the wrapped bake.
    /// </summary>
    public class ControlSetBuilderLogDecorator<T> : IControlSetBuilder<T> where T : IPath
    {
        private readonly IControlSetBuilder<T> _decoratee;
        private readonly IControlSetBuildLogger _logger;

        public ControlSetBuilderLogDecorator(IControlSetBuilder<T> decoratee, IControlSetBuildLogger logger)
        {
            _decoratee = decoratee;
            _logger = logger;
        }

        public ControlSet<T> Create(IGridGeometry3D gridSpace, float maxRadius)
        {
            _logger.StartLogGeneration();
            ControlSet<T> controlSet = _decoratee.Create(gridSpace, maxRadius);
            return controlSet;
        }
    }
}
