# Non-Circular Path Finding (NCPF)

Non-Circular Path Finding (NCPF) is a sophisticated pathfinding system designed for Unity that specializes in finding dynamically feasible paths for objects with non-circular (rectangular or irregular) shapes. Unlike traditional pathfinding algorithms that treat agents as simple circles or points, NCPF accounts for the actual geometry of objects, making it ideal for realistic vehicle navigation, robotics, and precise object placement.

## Overview

NCPF addresses the limitations of circular approximation in pathfinding by implementing a comprehensive system that considers the actual shape, orientation, and dynamics of moving objects. The system generates paths that are not only geometrically feasible but also dynamically achievable, ensuring that the planned trajectories can be executed by the physical systems they represent.

## Key Features

### Shape-Aware Pathfinding
- **Non-circular collision detection**: Accounts for rectangular and irregular object geometries
- **Orientation-aware planning**: Considers object heading at each point along the path
- **3D configuration space**: Uses (x, y, θ) coordinates for precise positioning

### Dynamic Feasibility
- **Curvature-constrained paths**: Generates paths with realistic turning radii
- **Speed profile integration**: Plans velocity profiles that respect acceleration limits
- **Gear-aware planning**: Considers forward and reverse movement capabilities

### Advanced Algorithms
- **Clothoid paths**: Uses Euler spiral curves for smooth, natural-looking transitions
- **G1 Hermite interpolation**: Solves for paths that match both position and tangent direction at endpoints
- **Multi-layered projection**: Projects primitive libraries onto specific configurations

## Architecture

The NCPF system is organized into several key modules:

### 1. Bake Layer
Responsible for pre-computing path primitives and configuration spaces:
- **ControlSetBuilder**: Generates libraries of motion primitives
- **ClothoidPathBuilder**: Creates smooth transition curves using Euler spirals
- **NCPFMapBuilder**: Builds configuration space maps from 2D occupancy grids

### 2. Domain Layer
Defines core interfaces and implementations:
- **IPath**: Core path interface representing pose as a function of arc length
- **PathAgentModel**: Agent model containing path parts and cursor state
- **ControlSet**: Library of geometric primitives organized by start direction

### 3. Pipeline Layer
Implements the search and planning algorithms:
- **CurvateMap3D**: 3D configuration space graph for path search
- **Projector3D**: Projects primitive libraries onto specific configurations
- **CurvateEdgeGenerator**: Prices transitions based on time and curvature metrics
- **PathPlanner**: Main planning component that orchestrates the search process

### 4. Agent Layer
High-level components for integrating with Unity:
- **PathPlanner**: MonoBehaviour that wires together search components
- **PathFollowAgent**: Executes planned paths in the Unity environment
- **BehaviorBus**: Manages behavioral state during path execution

## How It Works

1. **Configuration Space Generation**: The system first builds a 3D configuration space (x, y, θ) from the 2D environment map, accounting for the agent's shape and orientation.

2. **Primitive Library Creation**: Motion primitives (basic path segments) are pre-computed using clothoid curves, organized by starting direction.

3. **Graph Construction**: A search graph is constructed where nodes represent configurations and edges represent feasible transitions.

4. **Path Search**: A modified A* algorithm searches for the optimal path through the configuration space, considering both geometric feasibility and dynamic costs.

5. **Path Optimization**: The resulting path is refined and converted into executable commands for the agent.

## Use Cases

- **Autonomous Vehicles**: Planning realistic driving paths that account for vehicle dimensions and turning capabilities
- **Robotics**: Navigation for robots with non-circular footprints in constrained environments
- **Industrial Automation**: Precise object placement and movement in manufacturing settings
- **Game AI**: Realistic NPC movement that considers character dimensions and movement constraints

## Advantages Over Traditional Pathfinding

- **Realistic Geometry**: No more circle approximations that lead to collisions or unreachable targets
- **Dynamic Feasibility**: Paths are guaranteed to be executable by the agent's physical capabilities
- **Smooth Transitions**: Clothoid-based paths provide natural-looking movement
- **Orientation Control**: Explicit handling of object heading throughout the path
- **Performance**: Pre-computation and caching optimize runtime performance

## Getting Started

(TODO: Add setup and usage instructions here)

## Documentation

Detailed documentation for each component can be found in the Docs folder:
- [Bake Documentation](Assets/Logic/NCPF/Docs/Bake)
- [Domain Documentation](Assets/Logic/NCPF/Docs/Domain)
- [Pipeline Documentation](Assets/Logic/NCPF/Docs/Pipeline)
- [Shared Documentation](Assets/Logic/NCPF/Docs/Shared)

## Contributing

(TODO: Add contribution guidelines here)