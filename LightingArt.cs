using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    public static class LightingArt
    {
        public static Texture2D LightningSegment, HalfCircle, Pixel;

        public static void Load(ContentManager content)
        {
            LightningSegment = content.Load<Texture2D>("lightcenter");
            HalfCircle = content.Load<Texture2D>("lightend");
            Pixel = content.Load<Texture2D>("lightcenter");
        }
    }
}
