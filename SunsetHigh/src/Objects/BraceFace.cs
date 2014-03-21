using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    /// <summary>
    /// A character that encapsulates all the movement and attack logic for the 
    /// mini-boss
    /// </summary>
    public class BraceFace : Character
    {
        //battle AI vars
        private bool isAggressive = false;
        private Random rand;
        private int attackPattern = 0;
        private float attackPatternSwitchTimer = 0.0f;
        private float attackTimer1 = 0.0f;
        private float attackTimer2 = 0.0f;
        private float attackAngleCounter2 = 0.0f;
        private float attackTimer3 = 0.0f;
        private float attackTimer4 = 0.0f;
        private volatile bool initializeFire = false;
        private volatile bool initializeFire2 = false;
        private volatile bool readyToFire = false;
        private Vector2 nextDestFiring;
        private Direction fireDirection;
        private const int ROOM_MARGIN = 100;
        private bool isMovingRandomly;
        private float moveRandomTimer = 0.0f;
        private bool isDeflecting;

        //health vars
        private const int DEFAULT_MAX_HEALTH = 200;
        private int health;
        private bool healthBarVisible;
        private HealthBarTexture healthBar;

        public BraceFace()
            : base()
        {
            rand = new Random();
            this.setSpeed(250f);
            healthBar = new HealthBarTexture();
            this.setHealth(DEFAULT_MAX_HEALTH);
        }

        public void setAggressive(bool aggressive) { isAggressive = aggressive; }
        public void setHealthBarVisible(bool visible) { healthBarVisible = visible; }
        public void setHealth(int health) { 
            this.health = health;
            healthBar.setFullRatio((float)health / DEFAULT_MAX_HEALTH); 
        }
        public int getHealth() { return this.health; }
        public void shiftHealth(int dhp) { this.setHealth(this.getHealth() + dhp); }

        public override void loadContent(ContentManager content)
        {
            base.loadContent(content);
            healthBar.loadContent(content);
            Sprite.loadCommonImage(content, Directories.SPRITES + "projectile");
        }

        public override void update(float elapsed)
        {
            base.update(elapsed);

            if (isAggressive)
            {
                switch (attackPattern)
                {
                    case 0: //star burst
                        attackTimer1 += elapsed;
                        if (attackTimer1 > 0.5)
                        {
                            for (float angle = 0; angle < 2 * Math.PI; angle += (float)Math.PI / 4)
                            {
                                Projectile proj = new Projectile(this.getXCenter() + (int)(this.getWidth() / 2 * Math.Cos(angle)), 
                                    this.getYCenter() - (int)(this.getHeight() / 2 * Math.Sin(angle)), 300f, angle);
                                proj.setImage(Sprite.getCommonImage(Directories.SPRITES + "projectile"));
                                proj.setCollideEvent(new ProjectileCollideEvent(braceBulletCollideEvent));
                                WorldManager.enqueueObjectToCurrentRoom(proj);
                            }
                            attackTimer1 = 0.0f;
                        }
                        break;
                    case 1: //spiral
                        attackTimer2 += elapsed;
                        if (attackTimer2 > 0.15)
                        {
                            attackAngleCounter2 += 0.35f;
                            Projectile proj = new Projectile(this.getXCenter() + (int)(this.getWidth() / 2 * Math.Cos(attackAngleCounter2)),
                                this.getYCenter() - (int)(this.getHeight() / 2 * Math.Sin(attackAngleCounter2)), 300f, attackAngleCounter2);
                            proj.setImage(Sprite.getCommonImage(Directories.SPRITES + "projectile"));
                            proj.setCollideEvent(new ProjectileCollideEvent(braceBulletCollideEvent));
                            WorldManager.enqueueObjectToCurrentRoom(proj);
                            attackTimer2 = 0.0f;
                        }
                        break;
                    case 2: //snipe
                        attackTimer3 += elapsed;
                        if (attackTimer3 > 0.40)
                        {
                            int dx = Hero.instance.getXCenter() - this.getXCenter();
                            int dy = -(Hero.instance.getYCenter() - this.getYCenter());
                            float angle = (float)Math.Atan2(dy, dx);
                            Projectile proj = new Projectile(this.getXCenter() + (int)(this.getWidth() / 2 * Math.Cos(angle)),
                                this.getYCenter() - (int)(this.getHeight() / 2 * Math.Sin(angle)), 300f, angle);
                            proj.setImage(Sprite.getCommonImage(Directories.SPRITES + "projectile"));
                            proj.setCollideEvent(new ProjectileCollideEvent(braceBulletCollideEvent));
                            WorldManager.enqueueObjectToCurrentRoom(proj);
                            attackTimer3 = 0.0f;
                        }
                        break;
                    case 3: //firing squad
                        if (!initializeFire)
                        {
                            Vector2[] chooseLoc = new Vector2[4];   //list for four corners
                            chooseLoc[0] = new Vector2(ROOM_MARGIN/2, ROOM_MARGIN/2);
                            chooseLoc[1] = new Vector2(WorldManager.m_currentRoom.background.Width * Room.TILE_SIZE - ROOM_MARGIN, ROOM_MARGIN/2);
                            chooseLoc[3] = new Vector2(ROOM_MARGIN/2, WorldManager.m_currentRoom.background.Height * Room.TILE_SIZE - ROOM_MARGIN);
                            chooseLoc[2] = new Vector2(WorldManager.m_currentRoom.background.Width * Room.TILE_SIZE - ROOM_MARGIN,
                                WorldManager.m_currentRoom.background.Height * Room.TILE_SIZE - ROOM_MARGIN);
                            int choose = rand.Next(4);
                            nextDestFiring = chooseLoc[(choose + 1) % 4];
                            if (choose == 0) fireDirection = Direction.South;
                            else if (choose == 1) fireDirection = Direction.West;
                            else if (choose == 2) fireDirection = Direction.North;
                            else fireDirection = Direction.East;
                            this.moveToDestination((int)chooseLoc[choose].X, (int)chooseLoc[choose].Y, delegate()
                            {
                                readyToFire = true;
                            });
                            initializeFire = true;
                        }
                        if (readyToFire)
                        {
                            if (!initializeFire2)
                            {
                                this.moveToDestination((int)nextDestFiring.X, (int)nextDestFiring.Y, delegate()
                                {
                                    initializeFire = false;
                                    initializeFire2 = false;
                                    readyToFire = false;
                                    attackPattern = 0;
                                });
                                initializeFire2 = true;
                            }
                            attackTimer4 += elapsed;
                            if (attackTimer4 > 0.2f)
                            {
                                float angle = SunsetUtils.convertDirectionToAngle(fireDirection);
                                Projectile proj = new Projectile(this.getXCenter() + (int)(this.getWidth() / 2 * Math.Cos(angle)),
                                    this.getYCenter() - (int)(this.getHeight() / 2 * Math.Sin(angle)), 300f, angle);
                                proj.setImage(Sprite.getCommonImage(Directories.SPRITES + "projectile"));
                                proj.setCollideEvent(new ProjectileCollideEvent(braceBulletCollideEvent));
                                WorldManager.enqueueObjectToCurrentRoom(proj);
                                attackTimer4 = 0.0f;
                            }
                        }
                        break;
                }

                if (isMovingRandomly && attackPattern != 3)
                {
                    moveRandomTimer += elapsed;
                    if (moveRandomTimer > 0.30)
                    {
                        int xbound = WorldManager.m_currentRoom.background.Width * Room.TILE_SIZE;
                        int ybound = WorldManager.m_currentRoom.background.Height * Room.TILE_SIZE;
                        this.moveToDestination(rand.Next(xbound), rand.Next(ybound), null);
                        moveRandomTimer = 0.0f;
                    }
                }

                attackPatternSwitchTimer += elapsed;
                if (attackPatternSwitchTimer > 5.0f && attackPattern != 3)
                {
                    attackPatternSwitchTimer = 0.0f;
                    attackPattern = (attackPattern + 1) % 4;
                    isMovingRandomly = rand.Next(2) == 1;
                    //attackPattern = 3;
                    this.cancelMoveToDestination();
                }
            }
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
            if (healthBarVisible)
                healthBar.draw(sb);
        }

        private void braceBulletCollideEvent(IInteractable collider)
        {
            if (collider is Hero)
            {
                Hero h1 = (Hero)collider;
                h1.shiftHealth(-1);
                //dec health
            }
        }

        public override void onCollide(IInteractable other)
        {
            base.onCollide(other);
            if (other is Hero)
            {
                Hero h1 = (Hero)other;
                h1.shiftHealth(-1);
                //dec health?
            }
        }

        public override void reset()
        {
            base.reset();
            isAggressive = true;
            attackPattern = 0;
            attackPatternSwitchTimer = 0.0f;
            attackTimer1 = 0.0f;
            attackTimer2 = 0.0f;
            attackAngleCounter2 = 0.0f;
            attackTimer3 = 0.0f;
            attackTimer4 = 0.0f;
            initializeFire = false;
            initializeFire2 = false;
            readyToFire = false;
            isMovingRandomly = false;
            moveRandomTimer = 0.0f;
            isDeflecting = false;
            this.setHealth(DEFAULT_MAX_HEALTH);
        }

        private class HealthBarTexture  //A container for the health bar texture
        {
            private const int X_OFFSET = 800 - 200;
            private const int Y_OFFSET = 50;
            private const int OUTLINE_THICKNESS = 5;
            private const int BAR_WIDTH = 100;
            private const int BAR_HEIGHT = 100 / 3;
            private Texture2D box;
            private int mX = 0;
            private int mY = 0;
            private float mFullRatio = 1.0f;
            public HealthBarTexture()
            {
                WorldManager.OffsetChanged += updateOffsets;
            }
            public void loadContent(ContentManager content)
            {
                box = content.Load<Texture2D>(Directories.SPRITES + "InGameMenuBackground");
            }
            public void setFullRatio(float fullRatio)
            {
                mFullRatio = fullRatio;
            }
            public void draw(SpriteBatch sb)
            {
                sb.Draw(box, new Rectangle(mX + X_OFFSET, mY + Y_OFFSET, (int)(mFullRatio * BAR_WIDTH), BAR_HEIGHT), Color.Red);
                sb.Draw(box, new Rectangle(mX + X_OFFSET - OUTLINE_THICKNESS, mY + Y_OFFSET - OUTLINE_THICKNESS, BAR_WIDTH + OUTLINE_THICKNESS * 2, OUTLINE_THICKNESS), Color.Black);
                sb.Draw(box, new Rectangle(mX + X_OFFSET - OUTLINE_THICKNESS, mY + Y_OFFSET - OUTLINE_THICKNESS, OUTLINE_THICKNESS, BAR_HEIGHT + OUTLINE_THICKNESS * 2), Color.Black);
                sb.Draw(box, new Rectangle(mX + X_OFFSET + BAR_WIDTH, mY + Y_OFFSET - OUTLINE_THICKNESS, OUTLINE_THICKNESS, BAR_HEIGHT + OUTLINE_THICKNESS * 2), Color.Black);
                sb.Draw(box, new Rectangle(mX + X_OFFSET - OUTLINE_THICKNESS, mY + Y_OFFSET + BAR_HEIGHT, BAR_WIDTH + OUTLINE_THICKNESS * 2, OUTLINE_THICKNESS), Color.Black);
            }
            private void updateOffsets(object sender, CameraOffsetEventArgs e)
            {
                mX += e.dx_offset;
                mY += e.dy_offset;
            }
        }

    }
}
