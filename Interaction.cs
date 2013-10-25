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
        // This is a stupid design pattern, but I'll fix it later
        public List<Tuple<string, Events, int>> responses;

        public InteractionTreeNode()
        {
            this.responses = new List<Tuple<string,Events,int>>();
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
        private const string matcherString = @"(?<line>^""[^""]+"") -> \[(?<response>""[^""]+"" -> (\d+|End|Fight)(, )*)*(?<end>End)??,??(?<quest>Quest (#|%)\d+)??,??(?<fight>Fight)??\]";
        private static Regex lineMatcher = new Regex(matcherString, RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
        public List<InteractionTreeNode> dialogue;   
        public Interaction(string interactionFile)
        {
            string[] lines = System.IO.File.ReadAllLines(interactionFile);
            this.dialogue = new List<InteractionTreeNode>();
            foreach (var line in lines)
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
                    int questNum = 1;
                    int numIndex = 0;
                    string questSay = string.Empty;
                    // Kids, this is terrible, terrible practice. *Never* do it. I'm disappointed that C# lets you. I'm fixing it after Friday.
                    if((numIndex = questLine.IndexOf('#')+1) == 0) 
                    {
                        questNum = int.Parse(questLine.Substring(questLine.IndexOf('%') + 1));
                        questSay = "End quest";
                    }
                    else 
                    {
                        questNum = int.Parse(questLine.Substring(numIndex));
                        questSay = "Begin quest";
                    }

                    temp.responses.Add(new Tuple<string, Events, int>(questSay, Events.Quest, questNum));
                }
                else if (groups["fight"].Success)
                    temp.eventType = Events.Fight;
                else if (groups["response"].Success)
                {
                    temp.eventType = Events.None;
                    for (int i = 0; i < groups["response"].Captures.Count; ++i)
                    {
                        var responseline = groups["response"].Captures[i].Value;
                        var responseparts = responseline.Split( new string[] {" -> "}, StringSplitOptions.None);
                        string response = responseparts[0];
                        Events eventType;
                        int next;
                        switch (responseparts[1])
                        {
                            case "End":
                                eventType = Events.End;
                                next = 0;
                                break;
                            case "Fight":
                                eventType = Events.Fight;
                                next = 0;
                                break;
                            default:
                                eventType = Events.None;
                                next = parseNext(responseparts[1]);
                                break;
                        }
                        temp.responses.Add(new Tuple<string, Events, int>(response, eventType, next));
                    }
                }
                else 
                    throw new LineException(string.Format("No valid continuation: {0}", line));
                this.dialogue.Add(temp);
            }
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
