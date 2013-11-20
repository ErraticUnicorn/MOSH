MOSH
====

MOnogame Sunset High project. Work here guys.

Organization
---
Some pointers on file organization in this repo:

1) Source files go in the top directory.

2) All content (.png, .wav), compressed audio files (.mp3, .wma, .m4a) and game data files (.xml, .tmx) go into the "Content" folder.

4) All outside libraries and utilities (e.g. NAudio.dll) go into the "Resources" folder.

5) All save game states (coming soon!) go into "SaveData" folder.

Running and Debugging Instructions
---
Until we can figure out if there are viable and easy-to-use Git extensions for Visual Studio, we'll have to manually copy code into our local project folders and run it from there. To do that:

1) If you haven't already done so, create a new project in Visual Studio (C#) and select the Monogame WindowsGL template (it should be visible after downloading Monogame at http://monogame.codeplex.com/)

2) Download the source and assets from Github by either downloading as a ZIP or cloning to Desktop. Both options are on the right side of the Github website. Cloning is preferred; it should be possible after downloading Git for Windows at http://windows.github.com/ . Once you clone the repo, you can find it under Documents/Github/

3) Copy the downloaded content into your newly created project in Visual Studio. Some files may be replaced.

4) The assets should be in the "Content" folder. Once they are copied, inspect their properties and change the "Copy to output directory" attribute to "Copy if newer."

5) Build and run the project. Use "Game1.cs" as your sandbox for new features. When you need to commit code, copy your files back into the repo folder and use the GitHub GUI to commit and sync the changes.

Importing NAudio (for background music)
---
In Visual Studio, go to Project->Add reference and find "naudio.dll" (which is in this repo under "Resources"). NOTE: Do NOT get the NAudio release DLL from online, as that is an earlier version. Either try the one in this repo or build it from the source code. Email me if there are questions.

BGMusic.cs should compile with the NAudio reference. Use its static methods to manipulate background music. When passing in a filename as a string, do include the file extension.

Adding Warp to a .tmx File
---
In the Tiled map, you need to add an object layer called "Teleport". In this layer, you put in all the collision boxes for the warp spots. Each collision box currently has three attributes (which can be set in the Object Properties dialogue box): warpMap, warpX, and warpY.
1) warpMap is a string which represents the name of the map that the player should warp to
2) warpX is an integer which represents the x coordinate that the player should warp to (measured in tiles, not pixels)
3) warpY is the same thing as warpX except it represents the y coordinate
4) I plan on adding another attribute for the direction that the player should be facing after teleporting (probably called warpDirection)
