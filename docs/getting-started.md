# Getting Started with NCPF

This guide will help you get started with integrating Non-Circular Path Finding (NCPF) into your Unity project.

## Prerequisites

Before installing NCPF, ensure you have:
- Unity 2020.3 or later
- Basic understanding of Unity's navigation system
- Familiarity with pathfinding concepts

## Installation

### Option 1: Package Manager (Recommended)
1. In Unity, open the Package Manager (Window > Package Manager)
2. Click the "+" button and select "Add package from git URL"
3. Enter the NCPF repository URL
4. Click "Add"

### Option 2: Manual Installation
1. Download the latest release from the GitHub releases page
2. Extract the package contents to your Unity project's Assets folder
3. Wait for Unity to compile the scripts

## Basic Setup

### 1. Create a Navigation Grid
NCPF requires a navigation grid to understand the environment:

```csharp
// Create a new grid component
var gridGO = new GameObject("NavigationGrid");
var grid = gridGO.AddComponent<Unity2DGrid>();

// Configure grid properties
grid.CellSize = 0.5f; // Size of each grid cell in meters
grid.Width = 100;    // Number of cells in width
grid.Height = 100;   // Number of cells in height
```

### 2. Configure the Agent
Define the physical properties of your navigating object:

```csharp
// Create agent configuration
var agent = new DynamicAgent();
agent.Dimensions = new Vector2(2.0f, 1.0f); // Width x Height in meters
agent.MaxVelocity = 5.0f;                   // Maximum speed in m/s
agent.MaxAcceleration = 2.0f;              // Maximum acceleration in m/s²
agent.MaxAngularVelocity = 1.5f;           // Maximum angular velocity in rad/s
```

### 3. Generate Configuration Space
Build the configuration space map that accounts for the agent's shape:

```csharp
// Bake the configuration space
var baker = new NCPFMapBuilder();
baker.BakeConfigurationSpace(grid, agent);
```

### 4. Create Path Planner
Set up the path planner component:

```csharp
// Create planner GameObject
var plannerGO = new GameObject("PathPlanner");
var planner = plannerGO.AddComponent<PathPlanner>();

// Configure planner
planner.Agent = agent;
planner.Map = bakedMap;
```

## Simple Pathfinding Example

Here's a basic example of requesting a path between two points:

```csharp
public class SimplePathfinder : MonoBehaviour
{
    public PathPlanner planner;
    public Transform startPosition;
    public Transform endPosition;

    void Start()
    {
        // Request a path
        var target = new PointTarget(endPosition.position);
        planner.Target = target;
        
        // Start pathfinding
        planner.StartFollowing(startPosition.position, startPosition.forward);
    }
}
```

## Understanding the Core Concepts

### Configuration Space (C-Space)
Instead of treating obstacles as simple 2D shapes, NCPF expands them based on the agent's dimensions, creating a configuration space where collisions are represented as expanded obstacles.

### Motion Primitives
NCPF uses pre-computed motion primitives (basic movement patterns) that are dynamically feasible for the agent. These primitives are combined to form complex paths.

### Dynamic Constraints
All paths respect the agent's physical limitations:
- Maximum velocity
- Acceleration limits
- Turning radius constraints
- Gear restrictions (forward/reverse)

## Next Steps

After completing the basic setup, explore these topics:

1. [Architecture Overview](architecture.md) - Learn about NCPF's system design
2. [Advanced Configuration](advanced-config.md) - Fine-tune performance and behavior
3. [Custom Primitives](custom-primitives.md) - Create specialized motion patterns
4. [Integration Examples](examples/) - See NCPF in action with sample projects

## Troubleshooting

### Common Issues

**Issue: Paths seem to avoid the destination**
Solution: Check that your agent dimensions match your actual object and that the destination is reachable.

**Issue: Path generation is slow**
Solution: Reduce the resolution of your navigation grid or limit the search area.

**Issue: Agent collides with obstacles**
Solution: Verify that your configuration space baking accounts for the agent's full dimensions.

### Need Help?
If you're experiencing issues not covered here, check the [FAQ](faq.md) or visit our [Support](support.md) page for additional resources.