# Non-Circular Path Finding (NCPF)

Welcome to the documentation for Non-Circular Path Finding (NCPF), an advanced pathfinding system designed for Unity that specializes in finding dynamically feasible paths for objects with non-circular shapes.

## What is NCPF?

Traditional pathfinding algorithms often treat moving agents as simple circles or points, which can lead to unrealistic or even impossible paths when dealing with objects that have actual dimensions and shapes. NCPF solves this problem by implementing a comprehensive system that considers the actual geometry, orientation, and dynamics of moving objects.

Unlike basic pathfinding solutions, NCPF generates paths that are not only geometrically feasible but also dynamically achievable, ensuring that the planned trajectories can be executed by the physical systems they represent.

## Key Features

### Shape-Aware Pathfinding
NCPF accounts for rectangular and irregular object geometries rather than relying on circular approximations. The system considers object heading at each point along the path using a 3D configuration space with (x, y, θ) coordinates for precise positioning.

### Dynamic Feasibility
Paths are generated with realistic turning radii and integrated with speed profiles that respect acceleration limits. The system also includes gear-aware planning that considers both forward and reverse movement capabilities.

### Advanced Algorithms
NCPF uses sophisticated mathematical concepts:
- **Clothoid paths**: Euler spiral curves for smooth, natural-looking transitions
- **G1 Hermite interpolation**: Solves for paths that match both position and tangent direction at endpoints
- **Multi-layered projection**: Projects primitive libraries onto specific configurations

## Architecture Overview

The NCPF system is organized into several key modules:

### 1. Bake Layer
Responsible for pre-computing path primitives and configuration spaces:
- Generating libraries of motion primitives
- Creating smooth transition curves using Euler spirals
- Building configuration space maps from 2D occupancy grids

### 2. Domain Layer
Defines core interfaces and implementations:
- Core path interface representing pose as a function of arc length
- Agent model containing path parts and cursor state
- Library of geometric primitives organized by start direction

### 3. Pipeline Layer
Implements the search and planning algorithms:
- 3D configuration space graph for path search
- Projection of primitive libraries onto specific configurations
- Pricing of transitions based on time and curvature metrics
- Main planning component that orchestrates the search process

### 4. Agent Layer
High-level components for integrating with Unity:
- MonoBehaviour that wires together search components
- Execution of planned paths in the Unity environment
- Management of behavioral state during path execution

## Use Cases

NCPF is particularly well-suited for applications requiring precise and realistic movement:

- **Autonomous Vehicles**: Planning realistic driving paths that account for vehicle dimensions and turning capabilities
- **Robotics**: Navigation for robots with non-circular footprints in constrained environments
- **Industrial Automation**: Precise object placement and movement in manufacturing settings
- **Game AI**: Realistic NPC movement that considers character dimensions and movement constraints

## Why Choose NCPF?

### Realistic Geometry Handling
Unlike traditional approaches that use circle approximations (which can lead to collisions or unreachable targets), NCPF provides accurate collision-free path planning.

### Guaranteed Dynamic Feasibility
All generated paths are guaranteed to be executable by the agent's physical capabilities, eliminating unrealistic or impossible trajectories.

### Natural Movement Patterns
Clothoid-based paths provide smooth, natural-looking movement that mimics real-world physics.

### Comprehensive Orientation Control
Explicit handling of object heading throughout the path ensures proper orientation at the destination.

### Optimized Performance
Pre-computation and caching techniques optimize runtime performance for responsive applications.

## Getting Started

To begin using NCPF in your project, please refer to our [Getting Started Guide](getting-started.md) which provides detailed installation instructions and basic usage examples.

## Documentation

Explore our comprehensive documentation to understand how NCPF works and how to integrate it into your projects:

- [Architecture](architecture.md) - Detailed overview of the system architecture
- [API Reference](api/) - Complete API documentation for all classes and methods
- [Tutorials](tutorials/) - Step-by-step guides for common scenarios
- [Examples](examples/) - Sample projects demonstrating various features
- [FAQ](faq.md) - Frequently asked questions and troubleshooting

## Contributing

We welcome contributions from the community! Please read our [Contributing Guidelines](contributing.md) to learn how you can help improve NCPF.

## Support

If you encounter any issues or have questions about NCPF, please check our [Support](support.md) page for contact information and community resources.