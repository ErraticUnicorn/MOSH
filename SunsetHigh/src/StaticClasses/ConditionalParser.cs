using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    /// <summary>
    /// A parser for evaluating conditional statements in text files (interactions, journal entries, etc.).
    /// 
    /// Grammar explanation:
    /// The first word specifies the type of information to look up. The following words are specific to 
    /// each type. Types currently supported: [Quest, Reputation, Inventory]
    /// 
    ///     Quest: The second word must be a valid QuestID; the third to nth words specify a state that
    ///     quest should have (i.e. some combination of QuestStates). States can be combined through the
    ///     use of the words "and", "or", and "not". These word operators have no hierarchy or precedence; 
    ///     the expression is simply read left to right.
    ///     E.g.    Quest FoodFight Progress1 or Progress2 and not Complete
    ///     
    ///     Reputation: The second word must specify the clique (Nerd, Jock, Prep, Bully, Slacker). The next
    ///     token is a C relational operator (<, >, <=, >=, ==). The last token can either be an integer 
    ///     specifying the amount of reputation points to compare, OR it can be the name of another clique
    ///     (to compare against the reputation points of that clique).
    ///     E.g.    Reputation Nerd > 20
    ///     E.g.    Reputation Prep >= Jock
    ///     
    ///     Inventory: The second word must specify the item type in question (found in the Item enum in 
    ///     Inventory.cs). The next token is a C relational operator (<, >, <=, >=, ==). The last token must
    ///     be an integer specifying the number of that item to compare.
    ///     NOTE: If the last two arguments are ignored, "> 0" will automatically be appended to the statement.
    ///     E.g.    Inventory Meds > 3
    ///     
    ///     Location: The last word must specify the room (select from an ID in the PlaceID enum in Room.cs).
    ///     The word modifier "not" can be used to negate the statement.
    ///     E.g.    Location Cafeteria
    ///     E.g.    Location not Library
    ///     
    /// On a final note, different types of information can be combined into one statement by taking each 
    /// individual statement and inserting "and" or "or" between them as necessary. The word operators have no
    /// order of precedence; the expression is read left to right.
    ///     E.g.    Quest FoodFight Complete and Location Cafeteria
    /// 
    /// </summary>
    public static class ConditionalParser
    {
        private static string[] statementKeywords = { "Quest", "Reputation", "Inventory", "Location" };
        public static bool evaluateStatement(string statement)
        {
            List<string> args = new List<string>(statement.Trim().Split(' '));
            List<int> indices = new List<int>();
            args.RemoveAll(stringIsEmpty);

            //Break the statement into "sub-statements"
            for (int i = 0; i < args.Count; i++)
            {
                if (statementKeywords.Contains(args[i]))
                {
                    indices.Add(i);
                }
            }

            //Evaluate the full statement as sub-statements separated by "and's" and "or's"
            bool truth = false;
            bool and_on = false, or_on = false;
            for (int i = 0; i < indices.Count - 1; i++)
            {
                int end = indices[i + 1] - 1;
                bool subStateTruth = evaluateStatementHelper(args.GetRange(indices[i], end - indices[i]));
                
                if (and_on) { truth = truth && subStateTruth; and_on = false; }
                else if (or_on) { truth = truth || subStateTruth; or_on = false; }
                else truth = subStateTruth;

                if (args[end].Equals("and") || args[end].Equals("&&")) and_on = true;
                else if (args[end].Equals("or") || args[end].Equals("||")) or_on = true;
                else System.Diagnostics.Debug.WriteLine("No \"and\" or \"or\" found between two different statements!");
            }
            bool finalStateTruth = evaluateStatementHelper(args.GetRange(indices[indices.Count - 1], args.Count - indices[indices.Count - 1]));
            if (and_on) { truth = truth && finalStateTruth; and_on = false; }
            else if (or_on) { truth = truth || finalStateTruth; or_on = false; }
            else truth = finalStateTruth;

            return truth;
        }

        private static bool stringIsEmpty(string s)
        {
            return s.Length == 0;
        }

        private static string buildStatement(List<string> args)
        {
            string retVal = "";
            foreach (string str in args)
                retVal += str + " ";
            return retVal;
        }

        private static bool evaluateStatementHelper(List<string> args)
        {
            switch (args[0])
            {
                case "Quest":
                    if (args.Count < 3)
                    {
                        System.Diagnostics.Debug.WriteLine("Conditional statement \"" + buildStatement(args) + "\" must have at least 3 arguments.");
                        return false;
                    }
                    QuestID questID = SunsetUtils.parseEnum<QuestID>(args[1]);
                    bool and_on = false, or_on = false, not_on = false;
                    bool statementTrue = false;
                    for (int i = 2; i < args.Count; i++)
                    {
                        if (args[i].Equals("and") || args[i].Equals("&&"))
                        { and_on = true; continue; }
                        if (args[i].Equals("or") || args[i].Equals("||"))
                        { or_on = true; continue; }
                        if (args[i].Equals("not"))
                        { not_on = true; continue; }
                        if (args[i].Equals("is")) continue;

                        if (args[i].StartsWith("!"))
                        {
                            not_on = true;
                            args[i] = args[i].Substring(1);
                        }
                        QuestState questState = SunsetUtils.parseEnum<QuestState>(args[i]);
                        bool otherTrue;
                        if (not_on)
                        { otherTrue = Quest.isQuestStateInactive(questID, questState); not_on = false; }
                        else
                            otherTrue = Quest.isQuestStateActive(questID, questState);
                        if (and_on)
                        {
                            statementTrue = statementTrue && otherTrue;
                            and_on = false;
                        }
                        else if (or_on)
                        {
                            statementTrue = statementTrue || otherTrue;
                            or_on = false;
                        }
                        else
                        {
                            statementTrue = otherTrue;
                        }
                    }
                    return statementTrue;

                case "Reputation":
                    if (args.Count != 4)
                    {
                        System.Diagnostics.Debug.WriteLine("Conditional statement \"" + buildStatement(args) + "\" must have exactly 4 arguments.");
                        return false;
                    }
                    Clique first = SunsetUtils.parseEnum<Clique>(args[1]);
                    int points1 = Hero.instance.getReputation(first);
                    int points2;
                    if (!Int32.TryParse(args[3], out points2))
                    {
                        Clique second = SunsetUtils.parseEnum<Clique>(args[3]);
                        points2 = Hero.instance.getReputation(second);
                    }
                    if (args[2].Equals("==") || args[2].Equals("=")) return points1 == points2;
                    if (args[2].Equals("<")) return points1 < points2;
                    if (args[2].Equals("<=")) return points1 <= points2;
                    if (args[2].Equals(">")) return points1 > points2;
                    if (args[2].Equals(">=")) return points1 >= points2;

                    System.Diagnostics.Debug.WriteLine("Conditional statement \"" + buildStatement(args) + "\" does not have a valid relational operator.");
                    return false;

                case "Inventory":
                    if (args.Count < 2)
                    {
                        System.Diagnostics.Debug.WriteLine("Conditional statement \"" + buildStatement(args) + "\" must have at least 2 arguments.");
                        return false;
                    }
                    Item itemType = SunsetUtils.parseEnum<Item>(args[1]);
                    int itemQuantity = Hero.instance.inventory.numItem(itemType);
                    if (args.Count == 2) return itemQuantity > 0; //special case
                    int compareQuantity;
                    if (!Int32.TryParse(args[3], out compareQuantity))
                    {
                        System.Diagnostics.Debug.WriteLine("Conditional statement \"" + buildStatement(args) + "\" needs a valid integer in the fourth argument.");
                        return false;
                    }
                    if (args[2].Equals("==") || args[2].Equals("=")) return itemQuantity == compareQuantity;
                    if (args[2].Equals("<")) return itemQuantity < compareQuantity;
                    if (args[2].Equals("<=")) return itemQuantity <= compareQuantity;
                    if (args[2].Equals(">")) return itemQuantity > compareQuantity;
                    if (args[2].Equals(">=")) return itemQuantity >= compareQuantity;

                    System.Diagnostics.Debug.WriteLine("Conditional statement \"" + buildStatement(args) + "\" does not have a valid relational operator.");
                    return false;

                case "Location":
                    if (args.Count < 2)
                    {
                        System.Diagnostics.Debug.WriteLine("Conditional statement \"" + buildStatement(args) + "\" must at least 2 arguments.");
                        return false;
                    }

                    bool not2_on = false;
                    for (int i = 1; i < args.Count; i++)
                    {
                        if (args[i].Equals("not"))
                        { not2_on = true; continue; }
                        if (args[i].Equals("is")) continue;
                        if (args[i].StartsWith("!"))
                        {
                            not2_on = true;
                            args[i] = args[i].Substring(1);
                        }
                        PlaceID placeID = SunsetUtils.parseEnum<PlaceID>(args[i]);
                        if (not2_on)
                            return WorldManager.m_currentRoomID != placeID;
                        else
                            return WorldManager.m_currentRoomID == placeID;
                    }
                    break;

            }
            System.Diagnostics.Debug.WriteLine("Conditional statement \"" + buildStatement(args) + "\" does not have an appropriate starting token.");
            return false;
        }
    }
}
