using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    class Hero : Character
    {
        //make this a singleton?

        public Hero()
            : base()
        {
        }

        public Hero(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
        }

        public void converse(Character c)
        {
        }

        public void pickpocket(Character c) //need content manager
        {
            //make bar and needle pop up on screen
        }
    }
}
