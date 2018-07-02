# Dezinated's Rust Map Editor

Make sure you're using a Unity 2018 version. 2018.2 is confirmed working, not sure about 2018.1.

Set your Unity .NET version to 4.0.
Edit > Project Settings > Player > Other Settings > Configuration > Scripting Runtime Version

- Extended SDK to allow loading prefabs from bundle files
	- Warning: Unity gets really laggy since there's alot of prefabs and the monuments are pretty big
	- The prefab loading is really basic, it works but its not the best
	- Prefabs don't have any materials and appear black
	- Unity will use around 4GB RAM if you load all of the prefabs a map has. It's currently set to spawn every prefab the map has so you might want to change that if you don't have enough RAM.