Text Data grammar
===
There are text files in the Content/TextData folder used for text information in journal entries, inventory descriptions, and monologue lines (but not dialogue; that is in the Interactions folder). Most of these files have the same grammar, but there are some subtle differences, so I'll explain each one...

ItemInfo.txt
---
Start each item description entry with "#entry" followed by the name of the Item in the Item enum (found in Inventory.cs). The line specifying the actual title of the item seen the game (you can put spaces here) goes in the line starting with "#title". Next, write any text for the description. End the entry with the line "#end". An example of one entry:

#entry PokeBall
#title Poke Ball
A small red-and-white sphere popular among the nerds. Has no practical use (except for chucking at people).
#end

MonologueLines.txt
---
All the hero's lines when he talks to himself will go here. Lines can be grouped into conditional blocks; to start a block, write a line starting with "#if" and follow it with the appropriate conditional (for which the grammar is on another text file). Then, any text on one line will be considered a monologue line within that conditional. End a conditional block with "#end". An additional tag, "#once", can be used before an important comment that will only appear once (but will appear immediately). If multiple conditionals are fulfilled when requesting a line in-game, a block will be selected randomly (with the blocks appearing later being weighted more) and a line will be selected randomly from that block (the weights for each line in a block are equal). A sample of monologue lines:

#if Location Cafeteria and Quest FoodFight not Accepted
(What's for lunch?)
(Hmm.. nothing too appetizing here..)
#end

#if Location not Cafeteria and Quest FoodFight Accepted and not Progress1
#once
(I need to find Lucas in the cafeteria.)
(Wonder if Lucas is safe?)
#end

PeopleJournalInfo.txt
---
For entries containing bios about NPCs. Start with the line "#entry" followed by a PersonID (all IDs are listed in an enum in the Character.cs class). The line "#title" contains the in-game title. The following lines can contain text about the character. Text that should only appear after certain events (i.e. quests) can go inside conditional blocks (start with "#if" and end with "#end"). Be sure to end the entry with "#end". Sample below:

#entry Phil
#title Phil(ippa?)
A somewhat frenetic female nerd always clinging to her fanfic.

#if Quest FoodFight Complete
She concerns herself with the well-being of Lucas, although it can't really be said that they are friends.
#end

#end

PlacesJournalInfo.txt
---
For entries containing info about places in the game. Grammar is identical to "PeopleJournalInfo.txt", except the "#entry" line contains a PlaceID (all IDs are listed in an enum in the Room.cs class). Sample below:

#entry Cafeteria
#title Cafeteria
A gathering place for all students between the hours of 12:00 and 2:00 for lunch. Famous for its hot dogs.

#if Quest FoodFight Complete
Occasionally, the bullies rally the students to start a food fight, which can present very real physical danger. Avoid the area when this happens.
#end

#end

QuestJournalInfo.txt
---
For entries containing info about quests accepted. Grammar is identical to "PeopleJournalInfo.txt", except the "#entry" line contains a QuestID (all IDs are listed in an enum in the Quest.cs class). Also, conditional blocks do not need to start with "Quest <ID>", as that will be inserted automatically. Sample below:

#entry FoodFight
#title Food Fight!

Accepted Phil's plea to rescue her friend Lucas from the dangerous cafeteria. Head over ASAP!

#if Progress1	
Braved flying apples and burgers to reach Lucas, who told you that the head nerds might have some interesting information about dat girl's disappearance...
#end

#if Progress2
Braved flying apples and burgers to reach Lucas, who did not reveal any interesting information. At least he is safe now.
#end

#if Complete
Reported back to Phil, who showered you with praise. Few are the people who would navigate a food fight to rescue a nerd.
#end

#end
