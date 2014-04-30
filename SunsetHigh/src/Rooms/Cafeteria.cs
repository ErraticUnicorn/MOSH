using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledPipelineExtensions;

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
        private int foodFightDifficulty;
        private float FC1timer;
        private float FC2timer;
        private float FC3timer;
        private float FC4timer;
        private int FC1acounter;
        private int FC2counter;
        private int FC2acounter;
        private int FC3counter;
        private int FC3acounter;
        private int FC4counter;
        private int FC4acounter;

        public Cafeteria() : base()
        {
            isFoodFight = false;
            FC1timer = 0;
            FC2timer = 0;
            FC3timer = 0;
            FC4timer = 0;
            FC1acounter = 0;
            FC2counter = 0;
            FC2acounter = 0;
            FC3counter = 0;
            FC3acounter = 0;
            FC4counter = 0;
            FC4acounter = 0;
            foodType = 0;
            foodFightDifficulty = 0;
        }

        public override void loadContent(ContentManager content, string filename)
        {
            base.loadContent(content, filename);
            apple = content.Load<Texture2D>(Directories.SPRITES + "apple");
            cow = content.Load<Texture2D>(Directories.SPRITES + "cattle");
            hamburger = content.Load<Texture2D>(Directories.SPRITES + "burger");
            cheese = content.Load<Texture2D>(Directories.SPRITES + "cheese");
        }

        public override void onEnter()
        {
            base.onEnter();
            if (isFoodFight)
                BGMusic.transitionToSong("chipjank loop 2");
        }

        public override void onExit()
        {
            base.onExit();
            BGMusic.transitionToSong("sunset high ambient");
        }

        public override void updateState()
        {
            base.updateState();
            if (Quest.isQuestAccepted(QuestID.FoodFight) && 
                Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress9) &&
                Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress8))
            {
                isFoodFight = true;
                foodFightDifficulty = 0;
                if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress2))
                    foodFightDifficulty++;
                if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress4))
                    foodFightDifficulty++; 
                if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress6))
                    foodFightDifficulty++;
            }
            else
            {
                isFoodFight = false;
            }

            if (!Hero.instance.hasFollower())
            {
                if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress1)
                    && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress2))
                    Hero.instance.setFollower(PersonID.Phil);
                else if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress3)
                    && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress4))
                    Hero.instance.setFollower(PersonID.Artie);
                else if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress5)
                    && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress6))
                    Hero.instance.setFollower(PersonID.Bill);
                else if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress7)
                    && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress8))
                    Hero.instance.setFollower(PersonID.Claude);
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

                if (FC1timer > 2.5)
                {
                    foodType++;
                    FC1timer = 0;
                    Projectile food = new Projectile(5 * 32, 17 * 32, 180f, Direction.North);
                    food.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));

                    if (foodType % 3 == 0)
                    {
                        if (foodFightDifficulty == 0)
                            food.setImage(hamburger);
                        else
                        {
                            food.setImage(cow);
                            food.setSpeed(250f);
                            food.setXCenter(5 * 32);
                        }
                    }
                    if (foodType % 3 != 0)
                        food.setImage(apple);
                    if (foodFightDifficulty == 3)
                        food.setSpeed(350f);
                    this.addObject(food);

                    if (foodFightDifficulty > 0)
                    {
                        FC1acounter++;
                        if (FC1acounter % 2 == 0)
                        {
                            Projectile food2 = new Projectile(4 * 32, 17 * 32, 275f, Direction.North);
                            food2.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));
                            food2.setImage(hamburger);
                            this.addObject(food2);
                        }
                    }
                }

                if (FC2timer > .5)
                {
                    FC2counter++;
                    FC2timer = 0;
                    if (FC2counter >= 0 && FC2counter <= 5)
                    {
                        Projectile food = new Projectile(10 * 32, 4 * 32, 90f, Direction.South);
                        food.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));

                        if (foodType % 3 == 0)
                            food.setImage(hamburger);
                        if (foodType % 3 != 0)
                            food.setImage(apple);
                        this.addObject(food);
                    }
                    if (FC2counter > 7)
                        FC2counter = 0;

                    if (foodFightDifficulty > 0)
                    {
                        FC2acounter++;
                        if (FC2acounter >= 0 && FC2acounter <= 13)
                        {
                            Projectile food2 = new Projectile(11 * 32, 4 * 32, 120f, Direction.South);
                            food2.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));
                            if (foodType % 2 == 0)
                                food2.setImage(hamburger);
                            else
                                food2.setImage(apple);
                            this.addObject(food2);
                        }
                        if (FC2acounter > 19)
                            FC2acounter = 0;
                    }
                }



                if (FC3timer > .35)
                {
                    FC3timer = 0;
                    FC3counter++;
                    if (FC3counter <= 11 + foodFightDifficulty && FC3counter >= 0)
                    {
                        Projectile food = new Projectile(15 * 32, 17 * 32, 120f, Direction.North);
                        food.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));

                        if (foodType % 3 == 0)
                            food.setImage(hamburger);
                        if (foodType % 3 != 0)
                            food.setImage(apple);
                        this.addObject(food);
                    }
                    if (FC3counter > 18)
                        FC3counter = 0;

                    if (foodFightDifficulty > 1)
                    {
                        FC3acounter++;
                        if (FC3acounter >= 0 && FC3acounter <= 19)
                        {
                            Projectile food2 = new Projectile(14 * 32, 17 * 32, 150f, Direction.North);
                            food2.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));
                            if (foodType % 2 == 0)
                                food2.setImage(hamburger);
                            else
                                food2.setImage(apple);
                            this.addObject(food2);
                        }
                        if (FC3acounter > 25)
                            FC3acounter = 0;
                    }

                }
                if (FC4timer > .65)
                {
                    FC4counter++;
                    FC4timer = 0;
                    if (FC4counter >= 0 && FC4counter <= 13)
                    {
                        Projectile food = new Projectile(20 * 32, 4 * 32, 130f - (5f * foodFightDifficulty), Direction.South);
                        food.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));

                        if (FC4counter == 13)
                        {
                            food.setImage(cow);
                            food.setXCenter(20 * 32);
                        }
                        else if (foodType % 3 == 0)
                            food.setImage(hamburger);
                        else if (foodType % 3 != 0)
                            food.setImage(apple);
                        this.addObject(food);
                    }
                    if (FC4counter >= 16)
                        FC4counter = 0;

                    if (foodFightDifficulty > 2)
                    {
                        FC4acounter++;
                        if (FC4acounter >= 0 && FC4acounter <= 13 && foodFightDifficulty > 0)
                        {
                            Projectile food2 = new Projectile(21 * 32, 4 * 32, 100f, Direction.South);
                            food2.setCollideEvent(new ProjectileCollideEvent(foodCollideEvent));
                            if (foodType % 2 == 0)
                                food2.setImage(hamburger);
                            else
                                food2.setImage(apple);
                            this.addObject(food2);
                        }
                        if (FC4acounter > 12)
                            FC4acounter = 0;
                    }
                }
            }
        }

        public void foodCollideEvent(IInteractable collider)
        {
            if (collider is Hero
                || (Hero.instance.hasFollower() && CharacterManager.getCharacter(Hero.instance.getFollowerID()) == collider))
            {
                if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress1)
                    && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress2))
                {
                    ScreenTransition.requestTransition(delegate()
                    {
                        WorldManager.setRoomNoTransition(PlaceID.Cafeteria, 24 * TILE_SIZE, 10 * TILE_SIZE, Direction.West);
                        CharacterManager.getCharacter(PersonID.Phil).setPosition(25 * TILE_SIZE, 10 * TILE_SIZE);
                        CharacterManager.getCharacter(PersonID.Phil).reset();
                    });
                }
                else if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress3)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress4))
                {
                    ScreenTransition.requestTransition(delegate()
                    {
                        WorldManager.setRoomNoTransition(PlaceID.Cafeteria, 12 * TILE_SIZE, 9 * TILE_SIZE, Direction.West);
                        CharacterManager.getCharacter(PersonID.Artie).setPosition(12 * TILE_SIZE, 8 * TILE_SIZE);
                        CharacterManager.getCharacter(PersonID.Artie).reset();
                    });
                }
                else if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress5)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress6))
                {
                    ScreenTransition.requestTransition(delegate()
                    {
                        WorldManager.setRoomNoTransition(PlaceID.Cafeteria, 17 * TILE_SIZE, 14 * TILE_SIZE, Direction.West);
                        CharacterManager.getCharacter(PersonID.Bill).setPosition(16 * TILE_SIZE, 14 * TILE_SIZE);
                        CharacterManager.getCharacter(PersonID.Bill).reset();
                    });
                }
                else if (Quest.isQuestStateActive(QuestID.FoodFight, QuestState.Progress7)
                && Quest.isQuestStateInactive(QuestID.FoodFight, QuestState.Progress8))
                {
                    ScreenTransition.requestTransition(delegate()
                    {
                        WorldManager.setRoomNoTransition(PlaceID.Cafeteria, 24 * TILE_SIZE, 18 * TILE_SIZE, Direction.West);
                        CharacterManager.getCharacter(PersonID.Claude).setPosition(25 * TILE_SIZE, 18 * TILE_SIZE);
                        CharacterManager.getCharacter(PersonID.Claude).reset();
                    });
                }
                else
                    WorldManager.setRoom(PlaceID.Cafeteria, 1 * TILE_SIZE, 8 * TILE_SIZE, Direction.East);
            }
        }
    }
}
