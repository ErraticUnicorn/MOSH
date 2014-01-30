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
    /// each type. Types currently supported: [Quest]
    /// 
    ///     Quest: The second word must be a valid QuestID; the third to nth words specify a state that
    ///     quest should have (i.e. some combination of QuestStates). States can be combined through the
    ///     use of the words "and", "or", and "not" (as well as their code equivalents, "&&", "||", and "!").
    ///     These word operators have no order of precedence; the expression is simply read left to right.
    ///     E.g.    Quest FoodFight Progress1 or Progress2 and not Complete
    ///     E.g.    Quest TeacherChase Available && !Accepted
    /// 
    /// </summary>
    public static class ConditionalParser
    {
        public static bool evaluateStatement(string statement)
        {
            string[] args = statement.Split(' ');
            switch (args[0])
            {
                case "Quest":
                    QuestID questID = SunsetUtils.parseEnum<QuestID>(args[1]);
                    bool and_on = false, or_on = false, not_on = false;
                    bool statementTrue = false;
                    for (int i = 2; i < args.Length; i++)
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
                    //todo: implement lookup of hero's reputation
                    break;
                case "Inventory":
                    //todo: implement lookup of hero's inventory
                    break;
                case "Location":
                    //maybe?
                    break;
            }
            System.Diagnostics.Debug.WriteLine("Conditional statement \""+ statement + "\" cannot be evaluated correctly.");
            return false;
        }
    }
}
