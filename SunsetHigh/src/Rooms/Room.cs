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
    public interface IInteractable
    {
        /// <summary>
        /// Invoked when Hero presses "action" key close to this object
        /// </summary>
        void onInteract();
        /// <summary>
        /// Invoked when something else collides with this object (separate from collision detection!)
        /// </summary>
        void onCollide(IInteractable collider);
        /// <summary>
        /// Returns the bounding rectangle of this object, for collision/interaction purposes
        /// </summary>
        /// <returns></returns>
        Rectangle getBoundingRect();
    }

    public class Room
    {
        public const int TILE_SIZE = 32;
        public static int NUM_PLACE_IDS = Enum.GetValues(typeof(PlaceID)).Length - 1;
        private const int OFFSCREEN_OFFSET = 100;

        public Map background;
        public List<Sprite> Sprites { private set; get; }
        public List<Character> CharList { private set; get; }
        public List<IInteractable> Interactables { private set; get; }
        private List<object> addItemsSink;
        private List<object> removeItemsSink;

        public Room()
        {
            Sprites = new List<Sprite>();
            CharList = new List<Character>();
            Interactables = new List<IInteractable>();
            addItemsSink = new List<object>();
            removeItemsSink = new List<object>();
        }

        public virtual void loadContent(ContentManager content, String filename)
        {
            if (filename.StartsWith(Directories.MAPS))
                filename = filename.Substring(Directories.MAPS.Length);

            //background = content.Load<Map>(Directories.MAPS + filename);
            //System.Diagnostics.Debug.WriteLine(filename);
            background = TmxImporter.Import(Directories.MAPS + filename + ".tmx", content);
        }

        public virtual void updateState()
        {
            drainSinks();
        }

        public virtual void update(float elapsed)
        {
            foreach (Sprite a in Sprites)
            {
                a.update(elapsed);
            }
            //cleanup of projectiles offscreen, somewhat naive removal
            foreach (IInteractable i in new List<IInteractable>(this.Interactables))
            {
                if (i is Projectile)
                {
                    Projectile p = (Projectile)i;
                    if (p.getY() >  this.background.Height * this.background.TileHeight + OFFSCREEN_OFFSET || 
                        p.getY() < 0 - OFFSCREEN_OFFSET ||
                        p.getX() > this.background.Width * this.background.TileWidth + OFFSCREEN_OFFSET ||
                        p.getX() < 0 - OFFSCREEN_OFFSET)
                    {
                        this.removeObject(p);
                    }
                }
            }
            drainSinks();
        }

        public virtual void draw(SpriteBatch sb)
        {
            foreach (Sprite a in Sprites)
            {
                a.draw(sb);
            }
        }

        public virtual void onEnter()
        {
            updateState();
        }

        public virtual void onExit()
        {
            updateState();
        }

        private void drainSinks()
        {
            //drain sinks
            if (addItemsSink.Count > 0)
            {
                foreach (object o in addItemsSink)
                {
                    this.addObject(o);
                }
                addItemsSink.Clear();
            }
            if (removeItemsSink.Count > 0)
            {
                foreach (object o in removeItemsSink)
                {
                    this.removeObject(o);
                }
                removeItemsSink.Clear();
            }
        }

        /// <summary>
        /// Adds the object o to any appropriate lists if
        /// it is an instance of Sprite, Character, or IInteractable.
        /// Use this method to modify the lists.
        /// </summary>
        /// <param name="o">The object this room contains.</param>
        public void enqueueObject(object o)
        {
            addItemsSink.Add(o);
        }

        public void addObject(object o)
        {
            if (o is Sprite)
                Sprites.Add((Sprite)o);
            if (o is Character)
                CharList.Add((Character)o);
            if (o is IInteractable)
                Interactables.Add((IInteractable)o);
        }

        /// <summary>
        /// Removes the object of from any lists it is in
        /// </summary>
        /// <param name="o"></param>
        public void dequeueObject(object o)
        {
            removeItemsSink.Add(o);
        }

        public void removeObject(object o)
        {
            if (o is Sprite)
                Sprites.Remove((Sprite)o);
            if (o is Character)
                CharList.Remove((Character)o);
            if (o is IInteractable)
                Interactables.Remove((IInteractable)o);
        }

        //Used when resetting the game
        public void clearLists()
        {
            Sprites.Clear();
            CharList.Clear();
            Interactables.Clear();
            addItemsSink.Clear();
            removeItemsSink.Clear();
        }

        protected void clearProjectiles()
        {
            foreach (IInteractable i in new List<IInteractable>(Interactables))
            {
                if (i is Projectile)
                {
                    Interactables.Remove(i);
                    Sprites.Remove((Sprite)i);
                }
            }
        }
    }
}
