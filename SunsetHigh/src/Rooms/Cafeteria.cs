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
        private float FC3Counter;

        public Cafeteria() : base()
        {
            isFoodFight = false;
            FC1timer = 0;
            FC2timer = 0;
            FC3timer = 0;
            FC4timer = 0;
            FC2counter = 0;
            FC4counter = 0;
            FC3Counter = 0;
            foodType = 0;
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);
            apple = content.Load<Texture2D>(Directories.SPRITES + "apple");
            cow = content.Load<Texture2D>(Directories.SPRITES + "cattle");
            hamburger = content.Load<Texture2D>(Directories.SPRITES + "burger");
            cheese = content.Load<Texture2D>(Directories.SPRITES + "cheese");
        }

        public override void updateState()
        {
            base.updateState();
            if (Quest.isQuestAccepted(QuestID.FoodFight) && 
                !Quest.isQuestComplete(QuestID.FoodFight))
            {
                isFoodFight = true;
            }
            else
            {
                isFoodFight = false;
            }

            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress1)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress2)
                && !Hero.instance.hasFollower())
                Hero.instance.setFollower(PersonID.Phil);
        }

        public override void update(float elapsed)
        {
            updateState();
            base.update(elapsed);
            if (isFoodFight)
            {
                FC1timer += elapsed;
                FC2timer += elapsed;
                FC3timer += elapsed;
                FC4timer += elapsed;

                if (FC1timer > 2.5)
                {
                    foodType++;
                    FC1timer = 0;
                    Projectile food = new Projectile(5 * 32, 21 * 32, 150f, Direction.North);
                    food.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));

                    if (foodType % 3 == 0)
                    {
                        food.setImage(hamburger);
                    }
                    if (foodType % 3 != 0)
                    {
                        food.setImage(apple);
                    }
                    this.addObject(food);

                }
                if (FC2timer > .5)
                {
                    FC2counter++;
                    FC2timer = 0;
                    if (FC2counter >= 0 && FC2counter <= 5)
                    {
                        Projectile food = new Projectile(10 * 32, 0, 90f, Direction.South);
                        food.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));

                        if (foodType % 3 == 0)
                        {
                            food.setImage(hamburger);
                        }
                        if (foodType % 3 != 0)
                        {
                            food.setImage(apple);
                        }
                        this.addObject(food);
                    }
                    if (FC2counter > 7)
                    {
                        FC2counter = 0;
                    }
                }



                if (FC3timer > .35)
                {
                    FC3timer = 0;
                    FC3Counter++;
                    if (FC3Counter <= 14 && FC3Counter >= 0)
                    {
                        Projectile food = new Projectile(15 * 32, 21 * 32, 120f, Direction.North);
                        food.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));

                        if (foodType % 3 == 0)
                        {
                            food.setImage(hamburger);
                        }
                        if (foodType % 3 != 0)
                        {
                            food.setImage(apple);
                        }
                        this.addObject(food);
                    }
                    if (FC3Counter > 17)
                    {
                        FC3Counter = 0;
                    }

                }
                if (FC4timer > .65)
                {
                    FC4counter++;
                    FC4timer = 0;
                    if (FC4counter >= 0 && FC4counter <= 15)
                    {
                        Projectile food = new Projectile(20 * 32, 0, 120f, Direction.South);
                        food.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));

                        if (FC4counter == 13)
                        {
                            food.setImage(cow);
                            food.setXCenter(20 * 32);
                        }
                        else if (foodType % 3 == 0)
                        {
                            food.setImage(hamburger);
                        }
                        else if (foodType % 3 != 0)
                        {
                            food.setImage(apple);
                        }
                        this.addObject(food);
                    }
                    if (FC4counter >= 17)
                    {
                        FC4counter = 0;
                    }
                }
            }

            //cleanup of projectiles offscreen
            foreach (IInteractable i in new List<IInteractable>(this.Interactables))
            {
                if (i is Projectile)
                {
                    Projectile p = (Projectile)i;
                    if (p.getY() > 21 * 32 + 100 || p.getY() < 0 - 100)
                    {
                        this.removeObject(p);
                    }
                }
            }
        }

        public void foodCollideEvent()
        {
            if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress1)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress2))
            {
                ScreenTransition.requestTransition(delegate()
                {
                    WorldManager.setRoomNoTransition(PlaceID.Cafeteria, 24 * TILE_SIZE, 10 * TILE_SIZE, Direction.West);
                    Hero.instance.setFollower(PersonID.Phil);
                    CharacterManager.getCharacter(PersonID.Phil).setPosition(25 * TILE_SIZE, 10 * TILE_SIZE);
                });
            }
            else if (Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress1))
                WorldManager.setRoom(PlaceID.HallwayEast, 19 * TILE_SIZE, 3 * TILE_SIZE, Direction.East);
        }
    }
}
