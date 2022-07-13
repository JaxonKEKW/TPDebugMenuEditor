# Twilight Princess Map Select Editor
An editor for The Legend of Zelda: Twilight Princess Debug Map Select file (Menu1.dat) written in C# .NET 6.0. This tool makes it trivial to modify entries on the map select to your liking, including the display names, maps, rooms, spawnpoints, and actor setups. 

This can easily be used to localize the entries since they are all in Japanese (and it's the original reason I wrote this tool) or can be used to load unused actor and scene setups.

Features
-
- Lists all map select entries, including Display Names, Map Name, Room ID, Spawn ID, and the Actor Setup ID of the entry
- Allows all entries to be changed or replaced (strings up to 64 bytes)
- Writes the new menu file (with your changes) to the directory you run the program from
- Light/Dark mode (light mode disabled atm)
- stupid code

Usage
-
- [Get the stock Menu1.dat file if you don't have it](https://github.com/JaxonKEKW/TPDebugMenuEditor/raw/main/Menu1.dat)
- Compile using Visual Studio 2022 or use binary in [Releases](https://github.com/JaxonKEKW/TPDebugMenuEditor/releases)
- Done???

Screenshots
-
- Fully translated Map Select
>
![Fully translated Map Select](/assets/ss2.png)

- Image of the GUI
>
![Image of the GUI](/assets/ss1.png)
