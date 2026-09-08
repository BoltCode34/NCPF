# TargetRefHolder

A holder of a reference to an [[ITarget]]. It also implements `ITarget` and simply proxies the pose through. The point is that the **reference can be swapped**, and everyone holding the holder continues to see the current target.

`NCPF.Pipeline.Application · Agent/Targets.cs`

## Constructor

```csharp
TargetRefHolder(ITarget target)
```

`target` — the initial target. Changed later via the `Target` property.

## Key API

```csharp
public ITarget Target { get; set; }
```
The current target. **Swapped at runtime** — this is the whole purpose. Position/rotation/scale proxy through to it.

## Why

Without a holder, anything that once got an `ITarget` would have captured a **concrete** target. When a state swapped it, the old reference would still be alive and the object would keep looking at the wrong thing.

Therefore [[PathPlanner]] puts a holder into the [[PathAgentContext]] for passes; passes hold the holder, and swapping (`Planner.Target = ...` from [[ChainHead]]) is invisible to them and breaks nothing.

## Related

- [[ITarget]] — the contract
- [[PathPlanner]] — owns the holder, passes it in [[PathAgentContext]]
- [[ChainHead]] — swaps the target on Follow