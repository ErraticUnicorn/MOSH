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
    public interface IInteractable
    {
        /// <summary>
        /// Invoked when Hero presses "action" key close to this object
        /// </summary>
        void onInteract();
        /// <summary>
        /// Invoked when Hero collides with this object (separate from collision detection!)
        /// </summary>
        void onCollide();
        /// <summary>
        /// Returns the bounding rectangle of this object, for collision/interaction purposes
        /// </summary>
        /// <returns></returns>
        Rectangle getBoundingRect();
    }

    public class Room
    {
        public const int TILE_SIZE = 32;

        public Map background;
        public List<Sprite> Sprites { private set; get; }
        public List<Character> CharList { private set; get; }
        public List<IInteractable> Interactables { private set; get; }

        public Room()
        {
            Sprites = new List<Sprite>();
            CharList = new List<Character>();
            Interactables = new List<IInteractable>();
        }

        public virtual void loadContent(ContentManager content, String filename)
        {
            background = content.Load<Map>(filename);
        }

        public virtual void updateState()
        {

        }

        public virtual void update(float elapsed)
        {
            foreach (Sprite a in Sprites)
            {
                a.update(elapsed);
            }
        }

        public virtual void draw(SpriteBatch sb)
        {
            foreach (Sprite a in Sprites)
            {
                a.draw(sb);
            }
        }

        /// <summary>
        /// Adds the object o to any appropriate lists if
        /// it is an instance of Sprite, Character, or IInteractable.
        /// Use this method to modify the lists.
        /// </summary>
        /// <param name="o">The object this room contains.</param>
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
        public void removeObject(object o)
        {
            if (o is Sprite)
                Sprites.Remove((Sprite)o);
            if (o is Character)
                CharList.Remove((Character)o);
            if (o is IInteractable)
                Interactables.Remove((IInteractable)o);
        }
    }
}
