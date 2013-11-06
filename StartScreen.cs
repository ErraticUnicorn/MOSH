using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SunsetHigh
{
    public static class StartScreen
    {
        private static List<StartElement> main = new List<StartElement>();

        public static void init()
        {
            main.Add(new StartElement("background"));
            main.Add(new StartElement("startButton"));
        }

        public static void loadContent(ContentManager content)
        {
            //loading each element
            foreach (StartElement element in main)
            {
                element.LoadContent(content);
                element.CenterElement(600, 800);
                //element.clickEvent += OnClick;
            }
        }

        public static void update(float elapsed)
        {
            foreach (StartElement element in main)
            {
                element.Update();
            }
        }

        public static void draw(SpriteBatch sb)
        {
            foreach (StartElement element in main)
            {
                element.Draw(sb);
            }
        }

        /*
        /// <summary>
        /// When the start button is clicked. Not sure if this is this best place to put...
        /// </summary>
        /// <param name="element"></param>
        public void OnClick(string element)
        {
            if (element == "startButton")
            {
                //play game
                gameState = GameState.inGame;
            }

        }
         */
    }
}
