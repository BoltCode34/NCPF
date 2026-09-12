# NCPF Architecture

Understanding the architecture of Non-Circular Path Finding (NCPF) is essential for effectively using and extending the system. This document provides a comprehensive overview of the system's structure and components.

## System Overview

NCPF follows a layered architecture that separates concerns and promotes modularity:

```
┌─────────────────────────────────────────────────────────────┐
│                        Presentation                         │
├─────────────────────────────────────────────────────────────┤
│                          Pipeline                           │
├─────────────────────────────────────────────────────────────┤
│                           Domain                            │
├─────────────────────────────────────────────────────────────┤
│                            Bake                             │
└─────────────────────────────────────────────────────────────┘
```

## Layer Details

### Bake Layer

The Bake layer handles all pre-computation and asset generation tasks that occur during development time rather than runtime.

#### Key Components:
- **ControlSetBuilder**: Generates libraries of motion primitives for different orientations
- **ClothoidPathBuilder**: Creates smooth transition curves using Euler spirals
- **NCPFMapBuilder**: Constructs configuration space maps from 2D occupancy grids
- **ControlSetBaker**: Coordinates the baking process for control sets

#### Responsibilities:
- Pre-compute motion primitives for various steering commands
- Generate configuration space representations of the environment
- Create optimized data structures for runtime performance
- Serialize baked assets for distribution

### Domain Layer

The Domain layer defines the core abstractions and fundamental concepts used throughout the system.

#### Key Interfaces:
- **IPath**: Represents a pure trajectory as pose (position and direction) as a function of arc length
- **IGridMap3D**: 3D configuration space representation (x, y, θ coordinates)
- **IPolynomialPath**: Paths with cubic polynomial curvature functions
- **ControlSet**: Library of geometric primitives organized by start direction

#### Key Implementations:
- **PathAgentModel**: The agent model containing path parts, cursor, and current state
- **DiscretizedPath3D**: Discrete representation of 3D paths
- **Map3D**: 3D voxel-based map implementation
- **ClothoidPath**: Euler spiral-based path implementation

#### Responsibilities:
- Define core contracts and data structures
- Provide geometric and mathematical foundations
- Implement path representations and transformations
- Handle agent state management

### Pipeline Layer

The Pipeline layer implements the search algorithms and real-time pathfinding components.

#### Search Components:
- **CurvateMap3D**: 3D configuration space graph for path search
- **Projector3D**: Projects primitive libraries onto specific configurations
- **CurvateEdgeGenerator**: Prices transitions based on time and curvature metrics
- **ControlSetValidator3D**: Validates collision-free paths

#### Agent Components:
- **PathPlanner**: Main planning component that orchestrates the search process
- **PathFollowAgent**: Executes planned paths in the Unity environment
- **BehaviorBus**: Manages behavioral state during path execution
- **PathFindingAgent**: Specialized agent for pathfinding tasks

#### Responsibilities:
- Execute pathfinding algorithms in real-time
- Manage the search graph and transition pricing
- Coordinate between domain abstractions and presentation components
- Handle dynamic replanning and obstacle avoidance

### Shared Layer

The Shared layer contains cross-cutting concerns and utility components.

#### Key Components:
- **AgentConfig**: Configuration data for agents
- **ControlSetAsset**: Serialized control set assets
- **MapAsset**: Serialized map assets
- **SpacedMap3D**: Memory-optimized map representation

## Data Flow

The typical data flow through the NCPF system follows these steps:

1. **Baking Phase**:
   ```
   2D Occupancy Grid → NCPFMapBuilder → Configuration Space Map
   Control Parameters → ControlSetBuilder → Motion Primitive Library
   ```

2. **Planning Phase**:
   ```
   Target Position → PathPlanner → Search Graph → Optimal Path
   Path Constraints → CurvateEdgeGenerator → Transition Weights
   ```

3. **Execution Phase**:
   ```
   Planned Path → PathFollowAgent → Agent Movement Commands
   Runtime State → PathAgentModel → Current Position/Velocity
   ```

## Key Design Principles

### Separation of Concerns
Each layer has distinct responsibilities with minimal overlap, allowing for easier maintenance and extension.

### Immutable Core Abstractions
Core interfaces like `IPath` and `IGridMap3D` define immutable contracts that ensure predictable behavior.

### Composition Over Inheritance
Components are designed to be composed rather than extended, promoting flexibility and reducing coupling.

### Performance Through Pre-computation
Expensive calculations are performed during the baking phase, leaving runtime components to focus on pathfinding.

## Extensibility Points

NCPF is designed to be extensible at several key points:

1. **Custom Motion Primitives**: Implement `IPath` for new path types
2. **Alternative Search Algorithms**: Extend the pipeline with custom finders
3. **Specialized Validators**: Create new `ControlSetValidator3D` implementations
4. **Custom Pricing Models**: Replace `CurvateEdgeGenerator` with domain-specific logic

## Integration with Unity

NCPF integrates with Unity through MonoBehaviour components that expose the underlying system through familiar interfaces:

- **PathPlanner**: The main component for configuring and initiating pathfinding
- **PathFollowAgent**: Handles the execution of planned paths
- **Unity2DGrid**: Bridge between Unity's grid system and NCPF's internal representations

## Performance Considerations

The architecture is designed with performance in mind:

- **Memory Efficiency**: Shared assets and optimized data structures minimize memory footprint
- **Cache Locality**: Spatially coherent data organization improves cache performance
- **Parallel Processing**: Certain operations can be parallelized for improved performance
- **Lazy Evaluation**: Computation is deferred until results are actually needed

## Future Extensions

The modular architecture supports several potential extensions:

- **Multi-Agent Coordination**: Adding layers for managing multiple agents
- **Dynamic Obstacle Avoidance**: Real-time obstacle detection and avoidance
- **Learning-Based Optimization**: Integrating machine learning for improved heuristics
- **Hardware Acceleration**: GPU-accelerated pathfinding for large-scale environments

Understanding this architecture will help you effectively utilize NCPF's capabilities and extend the system to meet your specific requirements.