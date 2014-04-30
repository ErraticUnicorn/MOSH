using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public static class DialogueEventParser
    {
        public static void parseEvent(string eventTitle)
        {
            FootballField fbField = (FootballField)WorldManager.getRoom(PlaceID.FootballField);
            switch (eventTitle)
            {
                case "FootballRun":
                    fbField.startRunPlay();
                    break;
                case "FootballPass":
                    fbField.startPassPlay();
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Unknown dialogue event command: " + eventTitle);
                    break;
            }
        }
    }
}
