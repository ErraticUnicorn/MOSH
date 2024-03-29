﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledPipelineExtensions;

namespace SunsetHigh {

    public static class CollisionManager {
        public static readonly Point K_ZERO_OFFSET = new Point(0, 0);
        private static HashSet<IInteractable> m_excluded = new HashSet<IInteractable>();

        /// <summary>
        /// Returns whether the specified sprite is currently colliding with a solid object
        /// </summary>
        /// <param name="p_sprite"></param>
        /// <returns></returns>
        public static bool collisionWithSolid(IInteractable p_sprite) {
            return collisionWithSolidAtRelative(p_sprite, K_ZERO_OFFSET);
        }

        /// <summary>
        /// Returns whether the specified sprite collides with a solid object if its position were changed
        /// </summary>
        /// <param name="p_sprite"></param>
        /// <param name="p_offset"></param>
        /// <returns></returns>
        public static bool collisionWithSolidAtRelative(IInteractable p_sprite, Point p_offset) {
            return collisionWithObjectAtRelative(p_sprite, p_offset, "Solid") != null;
        }

        /// <summary>
        /// Returns whether the specified sprite is currently colliding with a teleport zone
        /// </summary>
        /// <param name="p_sprite"></param>
        /// <returns></returns>
        public static MapObject collisionWithTeleport(IInteractable p_sprite)
        {
            return collisionWithTeleportAtRelative(p_sprite, K_ZERO_OFFSET);
        }

        /// <summary>
        /// Returns whether the specified sprite collides with a teleport zone if its position were changed
        /// </summary>
        /// <param name="p_sprite"></param>
        /// <param name="p_offset"></param>
        /// <returns></returns>
        public static MapObject collisionWithTeleportAtRelative(IInteractable p_sprite, Point p_offset)
        {
            return collisionWithObjectAtRelative(p_sprite, p_offset, "Teleport");
        }

        // returns an object in the specified Tiled map object layer that the given sprite collides with
        // throws exception if the specified layer does not exist!
        public static MapObject collisionWithObjectAtRelative(IInteractable p_interactable, Point p_offset, String p_layer) {
            MapObject l_collidedObject = null;
            
            foreach (MapObject m in ((MapObjectLayer) WorldManager.m_currentRoom.background.GetLayer(p_layer)).Objects) {
                Rectangle l_copy = p_interactable.getBoundingRect();    //preserve this just in case
                Rectangle l_spriteBounds;
                l_spriteBounds.X = l_copy.X + p_offset.X;
                l_spriteBounds.Y = l_copy.Y + p_offset.Y;
                l_spriteBounds.Width = l_copy.Width;
                l_spriteBounds.Height = l_copy.Height;

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

        /// <summary>
        /// Returns an interactable in the current world's scene that p_interactable collides with if its position were changed.
        /// if p_interactable is colliding with p_exclude, that collision doesn't count
        /// (i.e. it doesn't make sense to check whether something is colliding with itself)
        /// </summary>
        /// <param name="p_interactable">Interactable to check</param>
        /// <param name="p_offset">The proposed movement of the Interactable</param>
        /// <param name="p_exclude">An interactable to exclude from collision checking</param>
        /// <returns>An interactable that p_interactable collides with; null if no collisions are detected</returns>
        public static IInteractable collisionWithInteractableAtRelative(IInteractable p_interactable, Point p_offset, IInteractable p_exclude)
        {
            IInteractable l_collidedObject = null;
            foreach (IInteractable i in WorldManager.m_currentRoom.Interactables)
            {
                if (i == p_exclude || m_excluded.Contains(i))
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

        public static void checkProjectileCollisions()
        {
            //DEBUG
            IInteractable interactable = CollisionManager.collisionWithInteractableAtRelative(Hero.instance, Point.Zero, Hero.instance);
            if (interactable != null)
            {
                interactable.onCollide(Hero.instance);
            }
            //end DEBUG
            foreach (Character character in WorldManager.m_currentRoom.CharList)
            {
                IInteractable interactable2 = null;
                interactable2 = CollisionManager.collisionWithInteractableAtRelative(character, Point.Zero, character);
                if (interactable2 != null)
                {
                    interactable2.onCollide(character);
                }
            }
        }

        public static void excludeInteractableCollision(IInteractable p_exclude)
        {
            m_excluded.Add(p_exclude);
        }
        public static void includeInteractableCollision(IInteractable p_include)
        {
            m_excluded.Remove(p_include);
        }
        public static void resetInteractableCollisions()
        {
            m_excluded.Clear();
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