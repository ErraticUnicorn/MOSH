MOSH
====

MOnogame Sunset High project. Work here guys.

Organization
---
Some pointers on file organization in this repo:

1) Source files go in the SunsetHigh/src directory, in the appropriate subfolder. Make sure the namespace for these files is always "SunsetHigh"! When adding source with VS in these folders, additional names may be appended to the namespace.

2) All content (music, art, text, maps) go into the SunsetHigh/Content folder, in the appropriate subfolder. Note that tile sheets go into Content/Maps/Tilesets/, while Content/Maps/ is for Tiled maps.

4) All outside libraries and utilities (e.g. NAudio.dll) go into the "Resources" folder.

5) All save game states go into "SaveData" folder.

Running and Debugging Instructions
---
1) Download Visual Studio 2012 or 2013 (if you have an earlier version you can try that, but I'm not sure how it'll work) at http://www.visualstudio.com/en-us/downloads/. VS Express is free; alternatively, you can go to DreamSpark (Microsoft's student thing) and get VS Professional for free.

2) Download Monogame at http://monogame.codeplex.com/. You should have some new project templates for C# afterward.

3) Download Git; I used Git for Windows at http://windows.github.com/. Now clone the repo to desktop. You can find this option on the right side of the repo webpage. Once you clone the repo, you can find it under Documents/Github/

4) If you want, you can download some Git extensions for VS. I am using Visual Studio Tools for Git http://visualstudiogallery.msdn.microsoft.com/abafc7d6-dcaa-40f4-8a5e-d6724bdb980c, which works on VS 2012, but apparently is already included on 2013. Alternatively, you can just control Git using the command line or the GUI.

5) Now start up Visual Studio and open the solution file found in the top level of your cloned MOSH repo. If you have an earlier version of VS, you probably cannot open it, so try opening the .csproj file in the SunsetHigh directory.

6) Hopefully, the project will load fine. Try building and running it. If there are issues then email me.

7) Congrats, now it works. You can start coding and committing. Please only commit when the game is in a stable state; otherwise create a new branch or something.

8) The assets should be in the Content folder. Please note that whenever you copy new assets into that folder, you MUST change the file's "Copy to output directory" attribute to "Copy if newer." Otherwise, you won't find the file when you're debugging the game.

Adding references to other libraries
---
In Visual Studio, go to Project->Add reference. From there, browse to find the libraries in the Resources folder. Currently, there are two such resources, NAudio.dll and Tiled.exe.

Adding Warp to a .tmx File
---
In the Tiled map, you need to add an object layer called "Teleport". In this layer, you put in all the collision boxes for the warp spots. Each collision box currently has three attributes (which can be set in the Object Properties dialogue box): warpMap, warpX, and warpY.
1) warpMap is a string which represents the name of the map that the player should warp to
2) warpX is an integer which represents the x coordinate that the player should warp to (measured in tiles, not pixels)
3) warpY is the same thing as warpX except it represents the y coordinate
4) I plan on adding another attribute for the direction that the player should be facing after teleporting (probably called warpDirection)
