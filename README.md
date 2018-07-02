# Dezinated's Rust Map Editor + Extended SDK

Make sure you're using a Unity 2018 version. 2018.2 is confirmed working, not sure about 2018.1.

Set your Unity .NET version to 4.0.
Edit > Project Settings > Player > Other Settings > Configuration > Scripting Runtime Version

Check out my patreon and help support developement for this Map Editor and new future Rust content creation tools and programs!
https://www.patreon.com/dezinated

## Features
- Extended SDK that allows Prefab loading direct from game content files
- Importing and exporting .map files
- Ability to edit
	- Terrain Map
	- Ground Textures
	- Biomes
	- Topology
	- Alpha Map
- Simple painting tools to easily edit map features
- Spawn in prefabs such as monuments and decor


## How to use the Editor

### 1) Opening the Project
Make sure you're on Unity 2018.2 and then launch Unity. When the Unity projects window opens up click on Open. Do not create a new project. Select the folder that contains the files downloaded from GitHub. Load `Dezinated's Rust Map Editor` project and it should take you into the editor view. Then at the bottom of the editor view there should be a file explorer; navigate to the Scenes folder and open `SampleScene`.

### 2) Loading a Map
On the left side of the editor screen, in the Hierarchy you will see all of the object the scene contains. Click on the `MapIO` object and you will notice map option on the right side of the editor in the Inspector. Import/Export buttons are to load and save .map files.

### 3) Editing Map Features
Again with `MapIO` selected you can switch between editing different map features. The dropdown list will contain Ground, Biome, Alpha, and Topology. Simply select a feature you would like to edit and then click on `Land` in the Hierarchy. From there you will see the Terrain component on the right side in the Inspector. Changing the terrain height will change the heightmap no matter which feature you are editing. To change features go to the texture painting tool on the terrain options. Depending on which feature you are editing there will be different textures available to use.

<b>What are the different features you can edit?</b>
- <b>Ground</b>: The ground textures you see when walking around in game. (Sand, dirt, rock, grass, etc.)
- <b>Biome</b>: Affects how the ground textures are coloured and what type of foilage spawns. (Arid, Arctic, Tundra, Temperate)
- <b>Alpha</b>: Makes parts of the map invisible. This is used for cave entrances and underground tunnels. Basically anything you need to punch a hole in the map for.
- <b>Topology</b>: One of the most fun features to mess with. This controls quite a few things and I haven't messed around with all of them yet. With this you're able to flag certain areas and control what happens there. For instance areas marked beach will spawn boats and count as spawn points for naked. Setting areas to `RoadSide` will make them spawn road loot.
	 - <b>List of Topologies</b>: `Field`, `Cliff`, `Summit`, `Beachside`, `Beach`, `Forest`, `Forestside`, `Ocean`, `Oceanside`, `Decor`, `Monument`, `Road`, `Roadside`, `Swamp`, `River`, `Riverside`, `Lake`, `Lakeside`, `Offshore`, `Powerline`, `Runway`, `Building`, `Cliffside`, `Mountain`, `Clutter`, `Alt`, `Tier0`, `Tier1`, `Tier2`, `Mainland`, `Hilltop`


### 4) Using Prefabs
Prefabs still need to be worked on a bit. How it's currently being done is every Prefab that spawns on the map is represented by a white cube in the editor. When the map is saved and loaded in to Rust, the client spawns the actual prefab there. In the editor if you load a map you will notice white cubes everywhere. Clicking on a white cube will show you information about that Prefab and moving it around will update it's position values. If you use your file explorer to naviate to Resources/Prefabs/Monuments you will notice I made a Prefab called `Dome`. To make Prefabs like this simply select a Prefab that you have in your scene and drag it in to the file explorer to save it. You can then rename it to anything you like and drag it back in to the Scene view as many times as you'd like.

Prefabs also have an option to load them from the game files on play. If you press the play button the editor will load the actual Prefab from the game files.

- Extended SDK to allow loading prefabs from bundle files
	- Warning: Unity will use alot RAM if you load prefabs from the game files. So don't press play if you have unsaved changes.
	- The prefab loading is really basic, it works but its not the best
	- Prefabs don't have any materials and appear black