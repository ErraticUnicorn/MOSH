Conditional Parser Grammar
===
The grammar for the conditional parser should be fairly straightforward. The first word specifies the type of information to look up. Subsequent words are specific to each type. Types currently supported: [Quest, Reputation, Inventory]

Quest
---
The first word is "Quest"; the second word must be a valid QuestID; the third to nth words specify a state that quest should have (i.e. some combination of QuestStates). States can be combined through the use of the words "and", "or", and "not". These word operators have no hierarchy or precedence; the expression is simply read left to right.
    E.g.    Quest FoodFight Progress1 or Progress2 and not Complete
    
Reputation
---
The first word is "Reputation"; the second word must specify the clique (Nerd, Jock, Prep, Bully, Slacker). The next token is a C relational operator (<, >, <=, >=, ==). The last token can either be an integer specifying the amount of reputation points to compare, OR it can be the name of another clique (to compare against the reputation points of that clique).
    E.g.    Reputation Nerd > 20
    E.g.    Reputation Prep <= Jock
    
Inventory
---
The first word is "Inventory"; the second word must specify the item type in question (found in the Item enum in Inventory.cs). The next token is a C relational operator (<, >, <=, >=, ==). The last token must be an integer specifying the number of that item to compare. NOTE: If the last two arguments are ignored, "> 0" will automatically be appended to the statement.
    E.g.    Inventory Meds > 3

Location
---    
The first word is "Location"; The last word must specify the room (select from an ID in the PlaceID enum in Room.cs). The word modifier "not" can be used to negate the statement.
    E.g.    Location Cafeteria
    E.g.    Location not Library
    
On a final note, different types of information can be combined into one statement by taking each individual statement and inserting "and" or "or" between them as necessary. The word operators have no order of precedence; the expression is read left to right.
    E.g.    Quest FoodFight Complete and Location Cafeteria