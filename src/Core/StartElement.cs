using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class StartElement
    {
        private Texture2D StartTexture;
        private Rectangle StartRect;
        private string assetName;

        /// <summary>
        /// Getter/setter
        /// </summary>
        public string AssetName
        {
            get { return assetName; }
            set { assetName = value; }
        }

        public delegate void ElementClicked(string Element);
        public event ElementClicked clickEvent;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="assetName"></param>
        public StartElement(string assetName)
        {
            this.assetName = assetName;
        }

        public void LoadContent(ContentManager content)
        {
            StartTexture = content.Load<Texture2D>(assetName);
            StartRect = new Rectangle(0, 0, StartTexture.Width, StartTexture.Height);
        }

        public void Update()
        {
            if (StartRect.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                clickEvent(assetName);
            }
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            SpriteBatch.Draw(StartTexture, StartRect, Color.White);
        }

        public void CenterElement(int height, int width)
        {
            StartRect = new Rectangle((width / 2) - (this.StartTexture.Width / 2), (height / 2) - (this.StartTexture.Height / 2), this.StartTexture.Width, this.StartTexture.Height);
        }

        public void MoveElement(int x, int y)
        {
            StartRect = new Rectangle(StartRect.X += x, StartRect.Y += y, StartRect.Width, StartRect.Height);
        }
    }
}
