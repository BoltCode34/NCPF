# PathAgentModel

The agent MODEL: what the agent is, not what it decides. It holds the path parts, the cursor along them, and the current state — nothing else.

## Details

- The cursor is ARC LENGTH, not time: the path is pure geometry, so "where am I" can only be a distance. Speed exists solely as a planned value in `CurrentDynamicState` (signed — the sign is the gear of the current part).
- `ContinueWithStateSaving` is the hot plan swap: the stitch begins with the same bridge primitive, so the saved cursor stays valid without a projection sweep; `StartFollowing` is the cold start — the seed is a state (nose survives, speed survives only in a matching gear).
- `AdvanceByArc` moves the cursor and returns the raw geometric pose; `PeekAheadArc` samples ahead without moving it. Every decision (how fast, which way to look) belongs to the passes; `Apply` is how the pipeline writes the planned state back.

## Used by

The pipeline brain and the follower own one; the runtime body is led from the model, never from a parallel copy.
