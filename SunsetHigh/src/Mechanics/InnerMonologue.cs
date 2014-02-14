using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class InnerMonologue
    {
        private class MonologueLine
        {
            public string line;
            public int oneTimeIndex;
            public MonologueLine() { line = ""; oneTimeIndex = -1; }
            public MonologueLine(string line, int oneTimeIndex = -1)
            { this.line = line; this.oneTimeIndex = oneTimeIndex; }
        }

        private class ConditionalGroup
        {
            public string statement;
            public List<MonologueLine> lines = new List<MonologueLine>();
        }

        private const int NUM_ONE_TIME_LINES = 200;
        private List<ConditionalGroup> conditionalLines;
        private List<MonologueLine> defaultLines;
        private Random rng;
        private bool[] seenLine;

        public InnerMonologue()
        {
            conditionalLines = new List<ConditionalGroup>();
            defaultLines = new List<MonologueLine>();
            rng = new Random();
            seenLine = new bool[NUM_ONE_TIME_LINES];
        }

        public string chooseLine()
        {
            List<MonologueLine> candidates = new List<MonologueLine>(defaultLines);
            List<int> groupIndices = new List<int>();
            groupIndices.Add(0);

            foreach (ConditionalGroup condLines in conditionalLines)
            {
                if (ConditionalParser.evaluateStatement(condLines.statement))
                {
                    bool hasLine = false;
                    foreach (MonologueLine mono in condLines.lines)
                    {
                        if (mono.oneTimeIndex == -1 || (mono.oneTimeIndex != -1 && !seenLine[mono.oneTimeIndex]))
                        {
                            if (!hasLine) { hasLine = true; groupIndices.Add(candidates.Count); }
                            candidates.Add(mono);
                            if (mono.oneTimeIndex != -1 && !seenLine[mono.oneTimeIndex])
                            {
                                seenLine[mono.oneTimeIndex] = true;
                                return mono.line;
                            }
                        }
                    }
                }
            }
            int chooseIndexStart = (int)Math.Log(rng.NextDouble() * (Math.Pow(2, groupIndices.Count - 1)) + 1, 2);
            int chooseStart = groupIndices[chooseIndexStart];
            int chooseEnd;
            if (chooseIndexStart == groupIndices.Count - 1)
                chooseEnd = candidates.Count;
            else
                chooseEnd = groupIndices[chooseIndexStart + 1];

            return candidates[rng.Next(chooseStart, chooseEnd)].line;
        }

        public bool[] getSaveStructure()
        {
            return this.seenLine;
        }

        public void loadSaveStructure(bool[] saveStructure)
        {
            if (saveStructure.Length == seenLine.Length)
                saveStructure.CopyTo(seenLine, 0);
            else
                System.Diagnostics.Debug.WriteLine("Monologue: Invalid array input; size must be same");
        }

        public void loadEntriesFromFile(string filename)
        {
            string[] allFileLines = System.IO.File.ReadAllLines(filename);
            int oneTimeIndex = 0;
            for (int i = 0; i < allFileLines.Length; i++)
            {
                string fileLine = allFileLines[i].Trim();
                if (fileLine.Length == 0) continue;

                if (fileLine.StartsWith("#if "))
                {
                    ConditionalGroup condLines = new ConditionalGroup();
                    condLines.statement = fileLine.Substring("#if ".Length).Trim();
                    bool once_on = false;
                    for (int j = i + 1; j < allFileLines.Length; j++)
                    {
                        fileLine = allFileLines[j].Trim();
                        if (fileLine.Length == 0) continue;
                        if (fileLine.StartsWith("#once"))
                        {
                            once_on = true;
                            continue;
                        }
                        if (fileLine.StartsWith("#end"))
                        {
                            conditionalLines.Add(condLines);
                            i = j;
                            break;
                        }
                        MonologueLine mLine = new MonologueLine(fileLine);
                        if (once_on)
                        {
                            mLine.oneTimeIndex = oneTimeIndex++;
                            once_on = false;
                        }
                        condLines.lines.Add(mLine);
                    }
                }
                if (fileLine.StartsWith("#default"))
                {
                    for (int j = i + 1; j < allFileLines.Length; j++)
                    {
                        fileLine = allFileLines[j].Trim();
                        if (fileLine.Length == 0) continue;
                        if (fileLine.StartsWith("#end"))
                        {
                            i = j;
                            break;
                        }
                        defaultLines.Add(new MonologueLine(fileLine));
                    }
                }
            }
        }
    }
}
