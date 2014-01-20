﻿using System;
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
        private Character npcHiding;
        private Character npcDruggie;
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
            npcHiding = new Character(23 * 32, 7 * 32);
            npcHiding.inventory.addItem(Item.PokeBall, 5);
            npcHiding.setScript(Directories.INTERACTIONS + "Lucas.cafeteriaInformationInteraction.txt");
            this.addObject(npcHiding);

            npcDruggie = new Character(25 * 32, 1 * 32);
            npcDruggie.inventory.addItem(Item.Meds, 1);
            npcDruggie.setScript(Directories.INTERACTIONS + "Stoner.druggieInteraction.txt");
            npcDruggie.setDirection(Direction.South);
            this.addObject(npcDruggie);
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);
            apple = content.Load<Texture2D>(Directories.SPRITES + "apple");
            cow = content.Load<Texture2D>(Directories.SPRITES + "cattle");
            hamburger = content.Load<Texture2D>(Directories.SPRITES + "burger");
            cheese = content.Load<Texture2D>(Directories.SPRITES + "cheese");

            npcHiding.loadImage(content, Directories.CHARACTERS + "sprite_sheet_herbert", 4, 3, 0.25f);
            npcDruggie.loadImage(content, Directories.CHARACTERS_TEMP + "slacker2");

        }
        public override void updateState()
        {
            base.updateState();
            if (Quest.isQuestAccepted(QuestID.FoodFight) && 
                Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress1 | QuestState.Progress2))
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
            updateState();
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
            else
            {
                //Interactables.Clear();

                // too lazy to clean up projectiles..
                // it can be done easily though
            }
        }

        public void foodCollideEvent()
        {
            WorldManager.setRoom("map_HallwayEast");
            Hero.instance.setX(19 * 32);
            Hero.instance.setY(3 * 32);
            Hero.instance.setDirection(Direction.East);
        }
    }
}
