using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;

namespace SunsetHigh {

    public static class CollisionManager {
        public static readonly Point K_ZERO_OFFSET = new Point(0, 0);

        // returns whether the specified sprite is currently colliding with a solid object
        public static bool collisionWithSolid(Sprite p_sprite) {
            return collisionWithSolidAtRelative(p_sprite, K_ZERO_OFFSET);
        }

        // returns whether the specified sprite collides with a solid object if its position were changed
        public static bool collisionWithSolidAtRelative(Sprite p_sprite, Point p_offset) {
            return collisionWithObjectAtRelative(p_sprite, p_offset, "Solid") != null;
        }

        // returns an object in the specified Tiled map object layer that the given sprite collides with
        // throws exception if the specified layer does not exist!
        public static MapObject collisionWithObjectAtRelative(Sprite p_sprite, Point p_offset, String p_layer) {
            MapObject l_collidedObject = null;
            
            foreach (MapObject m in ((MapObjectLayer) WorldManager.m_currentRoom.background.GetLayer(p_layer)).Objects) {
                Rectangle l_spriteBounds = new Rectangle(
                    p_sprite.getX() + p_offset.X,
                    p_sprite.getY() + p_offset.Y,
                    p_sprite.getWidth(),
                    p_sprite.getHeight());
                if (l_spriteBounds.Intersects(m.Bounds)) {
                    l_collidedObject = m;
                    break;
                }
            }

            return l_collidedObject;
        }

        public static MapObject collisionWithObjectAtRelative(Sprite p_sprite, Point p_offset)
        {
            return collisionWithObjectAtRelative(p_sprite, p_offset, "Solid");
        }

        // returns a Character that p_sprite collides with if its position were changed
        // if p_sprite is colliding with p_exclude, that collision doesn't count
        // (i.e. it doesn't make sense to check whether something is colliding with itself)
        public static Character collisionWithCharacterAtRelative(Sprite p_sprite, Point p_offset, Character p_exclude) {
            Character l_collidedObject = null;

            foreach (Character c in WorldManager.m_currentRoom.CharList) {
                if (c == p_exclude) {
                    continue;
                }
                Rectangle l_spriteBounds = new Rectangle(
                    p_sprite.getX() + p_offset.X,
                    p_sprite.getY() + p_offset.Y,
                    p_sprite.getWidth(),
                    p_sprite.getHeight());
                Rectangle l_charBounds = new Rectangle(c.getX(), c.getY(), c.getWidth(), c.getHeight());
                if (l_spriteBounds.Intersects(l_charBounds)) {
                    l_collidedObject = c;
                    break;
                }
            }

            return l_collidedObject;
        }

        //Clone of Character collision method, with Sprite instead, for testing
        public static Sprite collisionWithSpriteAtRelative(Sprite p_sprite, Point p_offset, Sprite p_exclude)
        {
            Sprite l_collidedObject = null;

            foreach (Sprite s in WorldManager.m_currentRoom.CharList)
            {
                if (s == p_exclude)
                {
                    continue;
                }
                Rectangle l_spriteBounds = new Rectangle(
                    p_sprite.getX() + p_offset.X,
                    p_sprite.getY() + p_offset.Y,
                    p_sprite.getWidth(),
                    p_sprite.getHeight());
                Rectangle l_charBounds = new Rectangle(s.getX(), s.getY(), s.getWidth(), s.getHeight());
                if (l_spriteBounds.Intersects(l_charBounds))
                {
                    l_collidedObject = s;
                    break;
                }
            }

            return l_collidedObject;
        }

        // clone of Character collision method with Interactable
        public static IInteractable collisionWithInteractableAtRelative(IInteractable p_interactable, Point p_offset, IInteractable p_exclude)
        {
            IInteractable l_collidedObject = null;
            foreach (IInteractable i in WorldManager.m_currentRoom.Interactables)
            {
                if (i == p_exclude)
                {
                    continue;
                }
                Rectangle l_copy = p_interactable.getBoundingRect();    //preserve this just in case
                Rectangle l_spriteBounds;
                l_spriteBounds.X = l_copy.X + p_offset.X;
                l_spriteBounds.Y = l_copy.Y + p_offset.Y;
                l_spriteBounds.Width = l_copy.Width;
                l_spriteBounds.Height = l_copy.Height;

                Rectangle l_charBounds = i.getBoundingRect();
                if (l_spriteBounds.Intersects(l_charBounds))
                {
                    l_collidedObject = i;
                    break;
                }
            }
            return l_collidedObject;
        }

        /// <summary>
        /// Sets the given Sprite one pixel off the closest side of the given Rectangle
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="rect"></param>
        /// <returns>True if the sprite's position changed, false otherwise</returns>
        public static bool setSpriteOutsideRectangle(Sprite sprite, Rectangle rect)
        {
            if (sprite.getY() + sprite.getHeight() < rect.Y && sprite.getY() != rect.Y - sprite.getHeight())
            {
                sprite.setY(rect.Y - sprite.getHeight());
                return true;
            }
            else if (sprite.getY() > rect.Y + rect.Height && sprite.getY() != rect.Y + rect.Height)
            {
                sprite.setY(rect.Y + rect.Height);
                return true;
            }

            if (sprite.getX() + sprite.getWidth() < rect.X && sprite.getX() != rect.X - sprite.getWidth())
            {
                sprite.setX(rect.X - sprite.getWidth());
                return true;
            }
            else if (sprite.getX() > rect.X + rect.Width && sprite.getX() != rect.X + rect.Width)
            {
                sprite.setX(rect.X + rect.Width);
                return true;
            }
            return false;
        }

    }
}