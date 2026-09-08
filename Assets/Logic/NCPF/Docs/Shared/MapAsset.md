# MapAsset

The abstract ScriptableObject port a baked C-space map is stored through. Fields typed by this base accept any concrete map asset Unity binds to them.

## Details

- `Read` reassembles the `Map3D` (occupancy cube plus agent rig); `Write` flattens it into serialisable layers with the agent signature attached.
- The rig travels with the map: an asset whose footprint anchor changed is detected as stale, not silently accepted.

## Used by

`MapContainer` implements it; `NCPFMapBuilder` writes maps through it, the search side reads them through it.
