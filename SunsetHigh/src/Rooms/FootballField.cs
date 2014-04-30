using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunsetHigh
{
    public class FootballField : Room
    {
        private Character fbplayer1, fbplayer2, fbplayer3, fbplayer4, fbplayer5,
            fbplayer6, fbplayer7, fbplayer8, fbplayer9;
        private Character fbplayer10, fbplayer11, fbplayer12, fbplayer13, fbplayer14,
            fbplayer15, fbplayer16, fbplayer17, fbplayer18;
        private FreeMovingSprite football;
        private Character coach;

        private List<Sprite> roomExclusives;
        private bool addedCharacters = false;

        private bool steppingRunPlay = false;
        private bool steppingPassPlay = false;

        public FootballField()
            : base()
        {
            roomExclusives = new List<Sprite>();
            fbplayer1 = new Character(11 * TILE_SIZE, 6 * TILE_SIZE);
            fbplayer2 = new Character(11 * TILE_SIZE, 8 * TILE_SIZE);
            fbplayer3 = new Character(11 * TILE_SIZE, 10 * TILE_SIZE);
            fbplayer4 = new Character(11 * TILE_SIZE, 12 * TILE_SIZE);
            fbplayer5 = new Character(11 * TILE_SIZE, 14 * TILE_SIZE);  //offensive line
            fbplayer6 = new Character(9 * TILE_SIZE, 9 * TILE_SIZE);    //qb
            fbplayer7 = new Character(9 * TILE_SIZE, 11 * TILE_SIZE);   //rb
            fbplayer8 = new Character(10 * TILE_SIZE, 4 * TILE_SIZE);   //wr
            fbplayer9 = new Character(10 * TILE_SIZE, 16 * TILE_SIZE);   //wr

            fbplayer10 = new Character(13 * TILE_SIZE, 6 * TILE_SIZE);  
            fbplayer11 = new Character(13 * TILE_SIZE, 8 * TILE_SIZE);
            fbplayer12 = new Character(13 * TILE_SIZE, 10 * TILE_SIZE);
            fbplayer13 = new Character(13 * TILE_SIZE, 12 * TILE_SIZE);
            fbplayer14 = new Character(13 * TILE_SIZE, 14 * TILE_SIZE); //d line
            fbplayer15 = new Character(15 * TILE_SIZE, 9 * TILE_SIZE); 
            fbplayer16 = new Character(15 * TILE_SIZE, 11 * TILE_SIZE); //lbs
            fbplayer17 = new Character(16 * TILE_SIZE, 4 * TILE_SIZE);
            fbplayer18 = new Character(16 * TILE_SIZE, 16 * TILE_SIZE); //lbs

            football = new FreeMovingSprite(9 * TILE_SIZE, 9 * TILE_SIZE);
            football.setVisible(false);
            coach = new Character(20 * TILE_SIZE, 16 * TILE_SIZE);  //coach

            roomExclusives.Add(fbplayer1);
            roomExclusives.Add(fbplayer2);
            roomExclusives.Add(fbplayer3);
            roomExclusives.Add(fbplayer4);
            roomExclusives.Add(fbplayer5);
            roomExclusives.Add(fbplayer6);
            roomExclusives.Add(fbplayer7);
            roomExclusives.Add(fbplayer8);
            roomExclusives.Add(fbplayer9);
            roomExclusives.Add(fbplayer10);
            roomExclusives.Add(fbplayer11);
            roomExclusives.Add(fbplayer12);
            roomExclusives.Add(fbplayer13);
            roomExclusives.Add(fbplayer14);
            roomExclusives.Add(fbplayer15);
            roomExclusives.Add(fbplayer16);
            roomExclusives.Add(fbplayer17);
            roomExclusives.Add(fbplayer18);
            roomExclusives.Add(football);
            roomExclusives.Add(coach);
        }

        public override void loadContent(Microsoft.Xna.Framework.Content.ContentManager content, string filename)
        {
            base.loadContent(content, filename);
            fbplayer1.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
            fbplayer2.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
            fbplayer3.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
            fbplayer4.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
            fbplayer5.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
            fbplayer6.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
            fbplayer7.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
            fbplayer8.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");
            fbplayer9.loadImage(content, Directories.CHARACTERS_TEMP + "jock2");

            fbplayer10.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");
            fbplayer11.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");
            fbplayer12.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");
            fbplayer13.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");
            fbplayer14.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");
            fbplayer15.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");
            fbplayer16.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");
            fbplayer17.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");
            fbplayer18.loadImage(content, Directories.CHARACTERS_TEMP + "jock3");

            football.loadImage(content, Directories.SPRITES + "bad_football");
            coach.loadImage(content, Directories.CHARACTERS_TEMP + "bully2");   //:p
            coach.setScript(Directories.INTERACTIONS + "Coach.txt");
            
        }

        public override void updateState()
        {
            base.updateState();
            if (!addedCharacters /*and right time*/)
            {
                foreach (Sprite c in roomExclusives)
                {
                    this.addObject(c);
                }
            }
            //else remove
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
            if (steppingRunPlay)
            {
                fbplayer7.moveToDestination(fbplayer7.getX() + TILE_SIZE * 3, fbplayer7.getY(), null);
                steppingRunPlay = false;
            }
            if (steppingPassPlay)
            {
                fbplayer9.moveToDestination(fbplayer9.getX() + TILE_SIZE * 5, fbplayer9.getY(), 3.0f, null);
                football.moveToDestination(fbplayer9.getX() + TILE_SIZE * 5, fbplayer9.getY(), 3.0f, delegate()
                    {
                        football.setVisible(false);
                        football.setPosition(fbplayer6.getX(), fbplayer6.getY());
                    });
                football.setVisible(true);
                steppingPassPlay = false;
            }
        }

        public void startRunPlay() { steppingRunPlay = true; }
        public void startPassPlay() { steppingPassPlay = true; }
    }
}
