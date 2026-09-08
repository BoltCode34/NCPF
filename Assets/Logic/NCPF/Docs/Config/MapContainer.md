# MapContainer

The concrete `MapAsset`: stores the baked C-space map as serialisable occupancy layers plus the agent signature, and reassembles a `Map3D` on read.

## Details

- Each heading layer is a flattened occupancy plane; the agent signature carries the discrete footprint and its anchor, so an asset baked for a different reference point reads as stale instead of silently valid.
- An empty store reads as an empty map (0×0×0) rather than failing.

## Used by

Held through the `MapAsset` port; written by `NCPFMapBuilder`, read by the search side. No assembly references the concrete type — the inspector preview (`NCPFMapEditor`) works over the port through reflection.
