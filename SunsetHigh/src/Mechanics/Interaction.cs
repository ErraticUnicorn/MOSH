using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SunsetHigh
{
    /// <summary>
    /// Enumeration of the valid event types for a line.
    /// </summary>
    public enum Events
    {
        None, End, Quest, Fight
    }

    /// <summary>
    /// The basic interaction node type. Consists of a NPC line, an event type, and a list of conditional targets.
    /// </summary>
    public class InteractionTreeNode
    {
        public Events eventType;
        public string line;
        public QuestID questID;     //for use when eventType is Events.Quest
        public QuestState questState;   //ditto above

        // This is a stupid design pattern, but I'll fix it later
        public List<InteractionResponseNode> responses;

        public InteractionTreeNode()
        {
            this.responses = new List<InteractionResponseNode>();
        }

    }

    /// <summary>
    /// The node type for the hero's response. Consists of the hero's line, an event type and the appropriate arguments (if applicable)
    /// </summary>
    public class InteractionResponseNode
    {
        public Events eventType;
        public string line;
        public QuestID questID;     //for use when eventType is Events.Quest
        public QuestState questState;   //ditto above
        public int nextLine;        //for use when eventType is Events.None (indicating the NPC will talk more)
    }

    public class InteractionConditional
    {
        public string statement;
        public int lineNumber;
        public InteractionConditional() { }
        public InteractionConditional(string statement, int lineNumber)
        {
            this.statement = statement;
            this.lineNumber = lineNumber;
        }
    }

    /// <summary>
    /// Wrapper class for Exception designed for when a line in a dialogue file isn't valid.
    /// </summary>
    sealed class LineException : Exception, ISerializable
    {
        public LineException() : base()
        {
        }

        public LineException(string message) : base(message)
        {
        }

        public LineException(string message, Exception inner) : base(message, inner)
        {
        }

        public LineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Main interaction class. Responsible for loading dialogue from an interaction file, presenting list of lines.
    /// </summary>
    public class Interaction
    {
        private const string matcherString = @"(?<line>^""[^""]+"") -> \[(?<response>""[^""]+"" -> (\d+|End|Fight|Quest .+? .+?)(, )*)*(?<end>End)??,??(?<quest>Quest .+? .+?)??,??(?<fight>Fight)??\]";
        private static Regex lineMatcher = new Regex(matcherString, RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
        public List<InteractionTreeNode> dialogue;
        public string name;
        private int defaultLine;
        private List<InteractionConditional> conditionalList;

        public Interaction(string interactionFile)
        {
            if (interactionFile.StartsWith(Directories.INTERACTIONS))
                interactionFile = interactionFile.Substring(Directories.INTERACTIONS.Length);

            string[] lines = System.IO.File.ReadAllLines(Directories.INTERACTIONS + interactionFile);
            this.dialogue = new List<InteractionTreeNode>();
            this.conditionalList = new List<InteractionConditional>();
            this.name = interactionFile.Split(new char[] { '.' })[0];
            this.defaultLine = 0;
            int lineCounter = 0;

            foreach (var line in lines)
            {
                // ignore empty lines
                if (line.Length == 0)
                    continue;
                
                // conditional statements go here
                // If the statement is true, then the line of dialogue directly below
                //  will be the first line of the conversation
                if (line.StartsWith("if "))
                {
                    string conditional = line.Substring("if ".Length);
                    this.conditionalList.Add(new InteractionConditional(conditional, lineCounter));
                }
                else if (line.StartsWith("default"))
                {
                    this.defaultLine = lineCounter;
                }
                else // otherwise we assume it's a line of dialogue
                {
                    var temp = new InteractionTreeNode();
                    var matches = lineMatcher.Match(line);
                    if (!matches.Success)
                        throw new LineException(string.Format("Bad Line: {0}", line));
                    var groups = matches.Groups;
                    temp.line = groups["line"].Value;
                    if (groups["end"].Success)
                        temp.eventType = Events.End;
                    else if (groups["quest"].Success)
                    {
                        temp.eventType = Events.Quest;
                        var questLine = groups["quest"].Value;

                        string[] questLinePieces = questLine.Split(' ');
                        if (questLinePieces.Length != 3)
                            throw new LineException(string.Format("Improper quest data formatting: {0}", line));
                        temp.questID = SunsetUtils.parseEnum<QuestID>(questLinePieces[1]);
                        temp.questState = SunsetUtils.parseEnum<QuestState>(questLinePieces[2]);
                    }
                    else if (groups["fight"].Success)
                        temp.eventType = Events.Fight;
                    else if (groups["response"].Success)
                    {
                        temp.eventType = Events.None;
                        for (int i = 0; i < groups["response"].Captures.Count; ++i)
                        {
                            var responseline = groups["response"].Captures[i].Value.Trim();
                            if (responseline.EndsWith(","))
                                responseline = responseline.Substring(0, responseline.Length - 1);
                            var responseparts = responseline.Split(new string[] { " -> " }, StringSplitOptions.None);
                            var responsepartsarguments = responseparts[1].Split(' ');

                            InteractionResponseNode responseNode = new InteractionResponseNode();
                            responseNode.line = responseparts[0];
                            switch (responsepartsarguments[0])
                            {
                                case "End":
                                    responseNode.eventType = Events.End;
                                    responseNode.nextLine = 0;
                                    break;
                                case "Fight":
                                    responseNode.eventType = Events.Fight;
                                    responseNode.nextLine = 0;
                                    break;
                                case "Quest":
                                    responseNode.eventType = Events.Quest;
                                    responseNode.questID = SunsetUtils.parseEnum<QuestID>(responsepartsarguments[1]);
                                    responseNode.questState = SunsetUtils.parseEnum<QuestState>(responsepartsarguments[2]);
                                    break;
                                default:
                                    responseNode.eventType = Events.None;
                                    responseNode.nextLine = parseNext(responsepartsarguments[0]);
                                    break;
                            }
                            temp.responses.Add(responseNode);
                        }
                    }
                    else
                        throw new LineException(string.Format("No valid continuation: {0}", line));

                    this.dialogue.Add(temp);
                    lineCounter++;
                }
            }
        }

        public InteractionTreeNode getStartingLine()
        {
            foreach (InteractionConditional cond in this.conditionalList)
            {
                if (ConditionalParser.evaluateStatement(cond.statement))
                    return this.dialogue[cond.lineNumber];
            }
            return this.dialogue[this.defaultLine];
        }

        private int parseNext(string next)
        {
            int val = 0, temp = 0;
            foreach(var c in next)
            {
                if (int.TryParse(c.ToString(), out temp))
                    val = 10*val + temp;
            }

            return val;
        }
    }
}
