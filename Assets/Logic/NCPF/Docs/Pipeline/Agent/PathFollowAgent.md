# PathFollowAgent

The agent FOLLOWER and the loop itself: the model holds the state, the MAIN pass decides the control from it, optional passes amend that decision, and only then does the follower move the cursor and write the result back. Moving is NOT a pass's job — the move happens once, after everyone has spoken, so the last word holds.

`NCPF.Pipeline.Application · Agent/PathFollowAgent.cs`

## What it is

Renamed from the old `PathAgentPipeline`. The planner hands in a GEOMETRIC path and never touches the follower otherwise. Works purely on the [[PathAgentModel]].

## Constructor

```csharp
PathFollowAgent(PathAgentModel model, MainSAPass mainPlanner, params SAPass[] passes)
```

`model` — the [[PathAgentModel]]: holds the path parts, cursor, and state. The single truth.

`mainPlanner` — the [[MainSAPass]] (from the bus's `Main` ref): decides facing and speed together from the state.

`passes` — optional override passes ([[SAPass]]): amend the control input sequentially. The bus's `AddOverride`/`RemoveOverride` mutates this list.

## Key API

```csharp
DynamicState GetNext(float dt)
```
One control tick: reads the model's current state, asks the main pass for a control input, runs override passes in order, advances the cursor by `|v|·dt`, writes the result back to the model, and returns the new state.

```csharp
void StartFollowing(CurvateTransition[] geometry, DynamicState state)
```
Cold start: takes the path as pure GEOMETRY (the model stores nothing else). The seed STATE carries position, nose and speed: on a cold start the planner passes the body's live state, and the model projects it onto the corridor.

```csharp
void ContinueWithStateSaving(CurvateTransition[] geometry)
```
Hot-swap the plan mid-motion: state (speed/nose) survives the path change. The stitch delivers a result that begins with the current bridge primitive — the same object — so the saved cursor stays valid untouched (no position projection needed).

```csharp
bool HasPath / bool Completed / DynamicState CurrentDynamicState / PathAgentModel Model / int CurrentPartIndex
```
Read-only views into the model.

## Details

- The cursor is ARC LENGTH, not time: the path is pure geometry and carries no timing, so "where am I" can only be a distance.
- Speed is signed: its sign is the GEAR of the current part (+ nose-first, − rear-first). A gear change passes through a full stop.
- Override passes run AFTER the main pass and can only nudge the control input; they cannot move the cursor themselves.

## Related

- [[PathAgentModel]] — the model (domain)
- [[PathAgentPipeline]] — the brain that owns this (app)
- [[BehaviorBus]] — owns the pass list and ref slots (app)
- [[MainSAPass]] / [[SAPass]] — the pass contracts (app)
- [[PathGearUtility]] — gear detection (domain)