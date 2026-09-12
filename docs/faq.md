# Frequently Asked Questions (FAQ)

This document addresses common questions and issues that users may encounter when working with Non-Circular Path Finding (NCPF).

## General Questions

### What makes NCPF different from Unity's built-in NavMesh system?

Unity's NavMesh system treats agents as point entities with circular boundaries, which works well for many applications but has limitations when dealing with objects that have significant dimensions or specific orientation requirements. NCPF:

- Accounts for the actual geometry of agents, not just circular approximations
- Considers orientation (heading) at each point along the path
- Generates paths that are dynamically feasible for the agent's physical capabilities
- Provides smoother, more natural-looking paths using clothoid curves
- Handles precise positioning requirements for industrial or robotic applications

### When should I use NCPF instead of simpler pathfinding solutions?

Consider NCPF when you need:

- Precise path planning for vehicles or robots with specific dimensions
- Smooth, natural-looking movement that respects physical constraints
- Orientation control throughout the path (not just at the destination)
- Dynamic feasibility guarantees for executed paths
- Complex environments where circular approximations would be inadequate

### What are the performance implications of using NCPF?

NCPF has different performance characteristics compared to simpler solutions:

**Advantages:**
- Pre-computation shifts expensive calculations to development time
- Optimized data structures for runtime pathfinding
- Configurable resolution to balance accuracy and performance

**Considerations:**
- Initial baking process can be computationally intensive
- Memory usage scales with environment complexity
- Runtime performance depends on search space size and resolution

For most applications, the performance impact is acceptable, especially given the improved path quality and feasibility guarantees.

## Setup and Configuration

### How do I determine the appropriate grid resolution?

Grid resolution is a trade-off between accuracy and performance:

- **Higher resolution** (smaller cells):
  - More accurate collision detection
  - Smoother paths
  - Higher memory usage
  - Slower pathfinding

- **Lower resolution** (larger cells):
  - Faster pathfinding
  - Lower memory usage
  - Less precise collision detection
  - Potentially less optimal paths

Start with a resolution slightly smaller than your smallest agent dimension, then adjust based on performance requirements.

### How do I configure agent dimensions properly?

Agent dimensions should match your actual moving objects:

```csharp
var agent = new DynamicAgent();
agent.Dimensions = new Vector2(width, height); // Actual dimensions in meters
agent.MaxVelocity = maxSpeed;                 // Maximum speed capability
agent.MaxAcceleration = maxAccel;             // Physical acceleration limits
```

Be conservative with dimensions—slightly larger is better than too small, as underestimating can lead to collisions.

### Why are my paths avoiding the intended destination?

Common causes include:

1. **Incorrect agent dimensions**: If your agent is configured as larger than it actually is, paths will avoid areas that seem blocked.

2. **Configuration space inflation**: Obstacles are expanded by the agent's dimensions, potentially making narrow passages impassable.

3. **Resolution mismatches**: Grid resolution that's too coarse may make valid destinations appear unreachable.

4. **Orientation constraints**: The destination may be reachable but not in a valid orientation.

Check your agent configuration and environment representation first.

## Path Generation Issues

### Why is path generation taking too long?

Slow path generation typically indicates one of these issues:

1. **Large search space**: Reduce the searchable area using bounding boxes or hierarchical pathfinding.

2. **High grid resolution**: Consider if you need such fine granularity for your application.

3. **Complex environment**: Environments with many obstacles or narrow passages increase search complexity.

4. **Suboptimal heuristics**: The default heuristic may not be well-suited to your specific environment.

Try reducing the search area first, then consider adjusting resolution or heuristic parameters.

### How can I improve path quality?

Several factors affect path quality:

1. **Increase primitive library size**: More motion primitives allow for better path optimization.

2. **Adjust pricing parameters**: Fine-tune curvature and time penalties to match your preferences.

3. **Use higher resolution maps**: Finer environmental representation enables more precise paths.

4. **Custom primitives**: Implement domain-specific motion patterns for your application.

### Why are paths not smooth?

Paths should naturally be smooth due to the clothoid-based primitives, but issues can arise from:

1. **Low-resolution configuration space**: Insufficient detail in the environment representation.

2. **Limited primitive library**: Too few primitives restrict path options.

3. **Aggressive optimization settings**: Over-prioritization of speed over smoothness.

Ensure your primitive library is adequately populated and consider adjusting the curvature penalty parameters.

## Technical Questions

### What coordinate system does NCPF use?

NCPF uses a right-handed coordinate system:

- **X-axis**: Horizontal (positive to the right)
- **Y-axis**: Vertical (positive upward)
- **Z-axis**: Forward (positive forward, used for 3D applications)
- **Angle measurement**: Counterclockwise from the positive Y-axis (0° = up)

This convention matches many robotics and navigation systems.

### How does NCPF handle dynamic obstacles?

Currently, NCPF focuses on static path planning. For dynamic obstacles:

1. **Replanning**: Periodically regenerate paths as the environment changes.

2. **Local avoidance**: Combine NCPF with local obstacle avoidance systems.

3. **Predictive planning**: Anticipate moving obstacle positions in the planning phase.

Future versions may include native support for dynamic obstacle handling.

### Can NCPF handle 3D environments?

NCPF primarily focuses on 2D navigation with orientation (3D configuration space: x, y, θ). For fully 3D environments:

1. **Layered approach**: Use separate 2D layers for different elevations.

2. **Hierarchical planning**: Plan at a high level between layers, then use NCPF within layers.

3. **Extension**: Modify the system to include additional dimensions (though this significantly increases complexity).

Most applications can be effectively handled with the 2.5D approach NCPF provides.

## Troubleshooting

### Common Error Messages

**"Path not found"**
- Verify that a path actually exists between start and destination
- Check agent dimensions against available space
- Ensure the destination is marked as traversable

**"Invalid configuration space"**
- Regenerate the configuration space map
- Verify agent dimensions match the baked assets
- Check for corrupted map files

**"Primitive library empty"**
- Ensure the baking process completed successfully
- Verify primitive library assets are in the correct location
- Check that the agent configuration matches the baked primitives

### Performance Profiling

To identify performance bottlenecks:

1. **Use Unity's Profiler**: Monitor CPU and memory usage during pathfinding.

2. **Enable debug visualization**: Visualize the search process to identify inefficient expansions.

3. **Measure generation times**: Track path generation duration for different scenarios.

4. **Monitor memory allocation**: Large allocations may indicate inefficient data structures.

## Best Practices

### Environment Design

- Provide adequate clearance around obstacles for your largest agents
- Minimize narrow passages that could become bottlenecks
- Use consistent grid alignment throughout your environment
- Mark clearly defined traversable and non-traversable areas

### Agent Configuration

- Match agent dimensions closely to actual object sizes
- Set realistic physical constraints based on actual hardware/software capabilities
- Test with extreme cases (largest/smallest agents, longest paths)
- Document configurations for consistency across team members

### Development Workflow

- Bake assets regularly during development as environments change
- Version control configuration space maps and primitive libraries
- Test pathfinding performance with representative scenarios
- Profile and optimize iteratively rather than attempting perfect optimization upfront

## Getting Help

If your question isn't addressed here:

1. **Check the documentation**: Ensure you've reviewed all relevant documentation sections.

2. **Search existing issues**: Look for similar problems in the GitHub issue tracker.

3. **Create a minimal reproduction**: Isolate the issue in a simple example project.

4. **Contact support**: Reach out through appropriate channels with detailed information.

Remember to include:
- NCPF version
- Unity version
- Relevant configuration details
- Steps to reproduce the issue
- Expected vs. actual behavior

This information will help maintainers provide more effective assistance.