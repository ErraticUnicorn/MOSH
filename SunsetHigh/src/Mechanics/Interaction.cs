using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SunsetHigh
{
    /// <summary>
    /// Enumeration of the valid event types for a line. Can be combined as flags.
    /// </summary>
    [Flags]
    public enum Events
    {
        None        = 0x0,  //should never be used
        NextLine    = 0x1,  //specifies next line in dialogue (response node only)
        Quest       = 0x2,  //quest state changes
        Reputation  = 0x4,  //reputation state changes
        Inventory   = 0x8,  //inventory state changes
        Fight       = 0x10, //not implemented
        Special     = 0x20, //not implemented
        End         = 0x40  //end of conversation
    }

    /// <summary>
    /// The basic interaction node, used for hero responses. Contains event type and arguments for each event.
    /// </summary>
    public class InteractionTreeNode
    {
        public Events eventType;
        public string line;

        //for use when eventType is Events.Quest
        public QuestID questID;
        public QuestState questState;
        //for use when eventType is Events.Reputation
        public Clique repClique;
        public int repChange;
        //for use when eventType is Events.Inventory
        public Item item;
        public int itemChange; 

        //for use when eventType is Events.NextLine (indicating the NPC will talk more)
        public int nextLine;        
    }

    /// <summary>
    /// An extension of the interaction node with a list of responses. Used for NPC lines (which are linked to hero responses).
    /// </summary>
    public class InteractionLinkedTreeNode : InteractionTreeNode
    {
        public List<InteractionTreeNode> responses;
        public InteractionLinkedTreeNode()
        {
            this.responses = new List<InteractionTreeNode>();
        }

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
        //the old version
        //private const string matcherString2 = @"(?<line>^""[^""]+"") -> \[(?<response>""[^""]+"" -> (\d+|End|Fight|Quest .+? .+?|Reputation .+? (\+|-)\d+)(, )*)*(?<end>End)??,??(?<quest>Quest .+? .+?)??,??(?<fight>Fight)??\]";
            
        //the new version
        private const string eventTypesString = @"\(?(?<eventargs>(\d+|End|Fight|Quest .+? .+?|Reputation .+? (\+|-)\d+|Inventory .+? (\+|-)\d+|Event .+?)(, )*)+\)?";
        private const string matcherString = @"(?<line>^""[^""]+"") -> \[((?<response>""[^""]+"" -> ("+eventTypesString+@")(, )*)+|(?<noresponse>"+eventTypesString+@"))\]";
        private static Regex lineMatcher = new Regex(matcherString, RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
        public List<InteractionLinkedTreeNode> dialogue;
        public string name;
        private int defaultLine;
        private List<InteractionConditional> conditionalList;

        public Interaction(string interactionFile)
        {
            if (interactionFile.StartsWith(Directories.INTERACTIONS))
                interactionFile = interactionFile.Substring(Directories.INTERACTIONS.Length);

            string[] lines = System.IO.File.ReadAllLines(Directories.INTERACTIONS + interactionFile);
            this.dialogue = new List<InteractionLinkedTreeNode>();
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
                    var interactionNode = new InteractionLinkedTreeNode();
                    var matches = lineMatcher.Match(line);
                    if (!matches.Success)
                        throw new LineException(string.Format("Bad Line: {0}", line));
                    var groups = matches.Groups;
                    interactionNode.line = groups["line"].Value;
                    //debug
                    //if (groups["eventargs"].Success)
                    //{
                    //    System.Diagnostics.Debug.WriteLine("Event args:");
                    //    for (int i = 0; i < groups["eventargs"].Captures.Count; ++i)
                    //    {
                    //        System.Diagnostics.Debug.WriteLine(groups["eventargs"].Captures[i].Value);
                    //    }
                    //    System.Diagnostics.Debug.WriteLine("");
                    //}
                    //end debug
                    if (groups["noresponse"].Success)
                    {
                        var eventargline = groups["noresponse"].Captures[0].Value.Trim();
                        if (eventargline.EndsWith(","))
                            eventargline = eventargline.Substring(0, eventargline.Length - 1);
                        System.Diagnostics.Debug.WriteLine(eventargline);

                        var eventargparts = eventargline.Split(',');
                        for (int j = 0; j < eventargparts.Length; ++j)
                        {
                            eventargparts[j] = eventargparts[j].Trim();
                            if (eventargparts[j].StartsWith("("))
                                eventargparts[j] = eventargparts[j].Substring(1);
                            if (eventargparts[j].EndsWith(")"))
                                eventargparts[j] = eventargparts[j].Substring(0, eventargparts[j].Length - 1);
                            string[] eventarguments = eventargparts[j].Split(' ');
                            System.Diagnostics.Debug.WriteLine(eventargparts[j]);
                            switch (eventarguments[0])
                            {
                                case "End":
                                    interactionNode.eventType |= Events.End;
                                    break;
                                case "Fight":
                                    interactionNode.eventType |= Events.Fight;
                                    break;
                                case "Quest":
                                    interactionNode.eventType |= Events.Quest;
                                    interactionNode.questID = SunsetUtils.parseEnum<QuestID>(eventarguments[1]);
                                    interactionNode.questState = SunsetUtils.parseEnum<QuestState>(eventarguments[2]);
                                    break;
                                case "Reputation":
                                    interactionNode.eventType |= Events.Reputation;
                                    interactionNode.repClique = SunsetUtils.parseEnum<Clique>(eventarguments[1]);
                                    interactionNode.repChange = Int32.Parse(eventarguments[2]);
                                    break;
                                case "Inventory":
                                    interactionNode.eventType |= Events.Inventory;
                                    interactionNode.item = SunsetUtils.parseEnum<Item>(eventarguments[1]);
                                    interactionNode.itemChange = Int32.Parse(eventarguments[2]);
                                    break;
                                case "Event":
                                    interactionNode.eventType |= Events.Special;
                                    break;
                                default:
                                    interactionNode.eventType |= Events.NextLine;
                                    interactionNode.nextLine = parseNext(eventarguments[0]);
                                    break;
                            }
                        }
                    }
                    else if (groups["response"].Success)
                    {
                        interactionNode.eventType = Events.None;
                        for (int i = 0; i < groups["response"].Captures.Count; ++i)
                        {
                            var responseline = groups["response"].Captures[i].Value.Trim();
                            if (responseline.EndsWith(","))
                                responseline = responseline.Substring(0, responseline.Length - 1);
                            System.Diagnostics.Debug.WriteLine(responseline);
                            var responseparts = responseline.Split(new string[] { " -> " }, StringSplitOptions.None);

                            InteractionTreeNode responseNode = new InteractionTreeNode();
                            responseNode.line = responseparts[0];
                            string[] responsearguments = responseparts[1].Split(',');
                            for (int j = 0; j < responsearguments.Length; ++j)
                            {
                                responsearguments[j] = responsearguments[j].Trim();
                                if (responsearguments[j].StartsWith("("))
                                    responsearguments[j] = responsearguments[j].Substring(1);
                                if (responsearguments[j].EndsWith(")"))
                                    responsearguments[j] = responsearguments[j].Substring(0, responsearguments[j].Length - 1);
                                string[] eventarguments = responsearguments[j].Split(' ');
                                System.Diagnostics.Debug.WriteLine(responsearguments[j]);
                                switch (eventarguments[0])
                                {
                                    case "End":
                                        responseNode.eventType |= Events.End;
                                        responseNode.nextLine = 0;
                                        break;
                                    case "Fight":
                                        responseNode.eventType |= Events.Fight;
                                        responseNode.nextLine = 0;
                                        break;
                                    case "Quest":
                                        responseNode.eventType |= Events.Quest;
                                        responseNode.questID = SunsetUtils.parseEnum<QuestID>(eventarguments[1]);
                                        responseNode.questState = SunsetUtils.parseEnum<QuestState>(eventarguments[2]);
                                        break;
                                    case "Reputation":
                                        responseNode.eventType |= Events.Reputation;
                                        responseNode.repClique = SunsetUtils.parseEnum<Clique>(eventarguments[1]);
                                        responseNode.repChange = Int32.Parse(eventarguments[2]);
                                        break;
                                    case "Inventory":
                                        responseNode.eventType |= Events.Inventory;
                                        responseNode.item = SunsetUtils.parseEnum<Item>(eventarguments[1]);
                                        responseNode.itemChange = Int32.Parse(eventarguments[2]);
                                        break;
                                    case "Event":
                                        responseNode.eventType |= Events.Special;
                                        break;
                                    default:
                                        responseNode.eventType |= Events.NextLine;
                                        responseNode.nextLine = parseNext(eventarguments[0]);
                                        break;
                                }
                            }
                            interactionNode.responses.Add(responseNode);
                        }
                    }
                    else
                        throw new LineException(string.Format("No valid continuation: {0}", line));

                    this.dialogue.Add(interactionNode);
                    lineCounter++;
                }
            }
        }

        public InteractionLinkedTreeNode getStartingLine()
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
