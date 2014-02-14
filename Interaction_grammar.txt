Interaction Grammar
===
Dialogue has a very powerful parser to allow for the state of multiple game mechanics to change through the lines you choose. Moving through the dialogue tree is fairly simple, once you have a starting line.

Each character in the game has one interaction file. Each line has the syntax:
"<NPC Line>" -> [<Set of Events OR Set of Hero Responses>]
where Hero Response = "<Hero Line>" -> Set of Events
and Events have specific syntax.

Sets can have one or more items; when using multiple items, separate them with commas (,).

There are multiple types of events that can happen after an NPC line or a Hero's line. They are:
	# (just a number, no words) - points to next line in the dialogue (numbering starts at 1)
	End - ends the dialogue
	Inventory <Item> (+ or -)# - adds or removes the given item to the Hero's inventory
	Quest <QuestID> <QuestState> - adds the given quest state to the given quest ID.
	Reputation <Clique> (+ or -)# - adds or subtracts points for the hero's reputation with a given clique.
	(Fight and Event are other unimplemented keywords)

Lastly, the line that the NPC starts with can be specified by having a conditional statement (starting with "if") before the line. Conditional grammar is in another file. A default starting line (if no conditionals are fulfilled) can be specified with the line "default" before the starting line.

Sample:
if Quest FoodFight Progress1 and not Complete
"Wow, you found Lucas! You're so cool! I'm not worthy!" -> [Quest FoodFight Complete, Reputation Nerd +10, Inventory PokeBall +2]

if Quest FoodFight Accepted and not Complete
"Have you found Lucas yet?!?" -> [End]

if Quest FoodFight Available and not Complete
"HELP!!!" -> ["What's wrong?" -> 4, "You're annoying." -> End, "You're cute." -> 7]
"My friend Lucas is stuck in the cafeteria and there's a massive food fight going on! Will you go rescue him?" -> ["Sure, I can try!" -> 5, "I'm busy right now." -> 6]
"Rescue or don't rescue, there is no try! Good luck!" -> [Quest FoodFight Accepted, Reputation Nerd +5]
"May you die a thousand frozen deaths. I will find the chosen one yet!" -> [End, Reputation Nerd -5]
"(blushing and giggling ensues...)" -> [End]

default
"O hai thar." -> [End]
