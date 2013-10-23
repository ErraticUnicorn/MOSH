using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    class Line
    {
        public Vector2 A;
        public Vector2 B;
        public float Thickness;

        public Line() { }
        public Line(Vector2 a, Vector2 b, float thickness = 1)
        {
            A = a;
            B = b;
            Thickness = thickness;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            Vector2 tangent = B - A;
            float rotation = (float)Math.Atan2(tangent.Y, tangent.X);
 
            const float ImageThickness = 8;
            float thicknessScale = Thickness / ImageThickness;

            Vector2 capOrigin = new Vector2(LightingArt.HalfCircle.Width, LightingArt.HalfCircle.Height / 2f);
            Vector2 middleOrigin = new Vector2(0, LightingArt.LightningSegment.Height / 2f);
            Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);
 
            spriteBatch.Draw(LightingArt.LightningSegment, A, null, color, rotation, middleOrigin, middleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(LightingArt.HalfCircle, A, null, color, rotation, capOrigin, thicknessScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(LightingArt.HalfCircle, B, null, color, rotation + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
        }

    }
}
