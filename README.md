# Dezinated's Rust Map Editor

Have fun

- Importing maps should fully work
- Exporting kind of works. Custom maps can't be downloaded by other players yet.
- Extended SDK to allow loading prefabs from bundle files
	- Warning: unity gets really laggy since there's alot of prefabs and the monuments are pretty big
	- The prefab loading is really basic, it works but its not the best
	- Prefabs don't have any materials and appear black
	- Unity will use around 4GB RAM if you load all of the prefabs a map has. It's currently set to spawn every prefab the map has so you might want to change that if you don't have enough RAM.