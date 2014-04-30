using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class Library : Room
    {
        public Library() : base()
        {
        }

        public override void onEnter()
        {
            base.onEnter();
            BGMusic.transitionToSong("rough thing");
        }

        public override void onExit()
        {
            base.onExit();
            BGMusic.transitionToSong("sunset high ambient");
        }

        public override void loadContent(Microsoft.Xna.Framework.Content.ContentManager content, string filename)
        {
            base.loadContent(content, filename);
        }
    }
}
