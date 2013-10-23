using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace SunsetHigh
{
    public class Cafeteria : Room
    {
        private Texture2D apple;
        private Texture2D cow;
        private Texture2D hamburger;
        private Texture2D cheese;
        private int foodType;
        private bool isFoodFight;
        private float FC1timer;
        private float FC2timer;
        private float FC3timer;
        private float FC4timer;
        private float FC2counter;
        private float FC4counter;

        public Cafeteria() : base()
        {
            isFoodFight = false;
            FC1timer = 0;
            FC2timer = 0;
            FC3timer = 0;
            FC4timer = 0;
            FC2counter = 0;
            FC4counter = 0;
            foodType = 0;
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);
            apple = content.Load<Texture2D>("apple");
            cow = content.Load<Texture2D>("cattle");
            hamburger = content.Load<Texture2D>("burger");
            cheese = content.Load<Texture2D>("cheese");

        }
        public override void updateState()
        {
            base.updateState();
            if (Quest.isTriggered(QuestID.FoodFight1))
            {
                isFoodFight = true;
            }
            else
            {
                isFoodFight = false;
            }
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);
            if (isFoodFight)
            {
                FC1timer += elapsed;
                FC2timer += elapsed;
                FC3timer += elapsed;
                FC4timer += elapsed;
                
                if (FC1timer > 1)
                {
                    foodType++;
                    FC1timer = 0;
                    Projectile food = new Projectile(5 * 32, 21 * 32, 5, Direction.North);
                    
                    if (foodType % 3 == 0)
                    {
                        food.setImage(hamburger);
                    }
                    if (foodType % 3 != 0)
                    {
                        food.setImage(apple);
                    }
                    Interactables.Add(food);

                }
                if (FC2timer > .5)
                {
                    FC2counter++;
                    FC2timer = 0;
                    if (FC2counter >= 0 && FC2counter <= 5)
                    {
                        Projectile food = new Projectile(10 * 32, 0, 3, Direction.South);
                        if (foodType % 3 == 0)
                        {
                            food.setImage(hamburger);
                        }
                        if (foodType % 3 != 0)
                        {
                            food.setImage(apple);
                        }
                        Interactables.Add(food);
                    }
                    if (FC2counter > 7)
                    {
                        FC2counter = 0;
                    }
                }

                

                if (FC3timer > .75)
                {
                    FC3timer = 0;
                    Projectile food = new Projectile(15 * 32, 21 * 32, 10, Direction.North);
                    if (foodType % 3 == 0)
                    {
                        food.setImage(hamburger);
                    }
                    if (foodType % 3 != 0)
                    {
                        food.setImage(apple);
                    }
                    Interactables.Add(food);

                }
                if (FC4timer > .25)
                {
                    FC4counter++;
                    FC4timer = 0;
                    if (FC4counter >= 0 && FC4counter <= 15)
                    {
                        Projectile food = new Projectile(20 * 32, 0, 4, Direction.South);
                        if (FC4counter == 13)
                        {
                            food.setImage(cow);
                            food.setXCenter(20*32);
                        }
                        else if (foodType % 3 == 0)
                        {
                            food.setImage(hamburger);
                        }
                        else if (foodType % 3 != 0)
                        {
                            food.setImage(apple);
                        }
                        Interactables.Add(food);
                    }
                    if (FC4counter >= 17)
                    {
                        FC4counter = 0;
                    }
                }
            }
        }

    }
}
