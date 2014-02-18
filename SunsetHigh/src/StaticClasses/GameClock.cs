using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public struct ClockSave
    {
        public int hours;
        public int minutes;
        public int seconds;
    }

    public static class GameClock
    {
        public static DateTime gameStartTime = DateTime.Now;
        private static TimeSpan previousTime = TimeSpan.Zero;

        public static TimeSpan getTotalTimeSpan()
        {
            return previousTime + (DateTime.Now - gameStartTime);
        }

        public static string formatTimeSpan(TimeSpan total)
        {
            return String.Format("{0:00}:{1:00}:{2:00}", (total.Days * 24 + total.Hours), total.Minutes, total.Seconds);
        }
        public static string formatTimeSpan(ClockSave total)
        {
            return String.Format("{0:00}:{1:00}:{2:00}", (total.hours), total.minutes, total.seconds);
        }

        public static void renewClock()
        {
            gameStartTime = DateTime.Now;
            previousTime = TimeSpan.Zero;
        }

        public static ClockSave getSaveStructure()
        {
            TimeSpan totalTime = getTotalTimeSpan();
            ClockSave data;
            data.hours = totalTime.Days * 24 + totalTime.Hours;
            data.minutes = totalTime.Minutes;
            data.seconds = totalTime.Seconds;
            return data;
        }

        public static void loadSaveStructure(ClockSave data)
        {
            previousTime = new TimeSpan(data.hours, data.minutes, data.seconds);
            gameStartTime = DateTime.Now;
        }
    }
}
