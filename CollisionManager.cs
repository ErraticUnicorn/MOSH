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

    public static void CollisionWithCharacter(Hero h, Character c)
    {

        if (h.inRange(c, 0))
        {
            h.move(h.getDirection(), -4);
        }
    }
    
    public static void CollisionWithProjectiles(Hero h, Character c)
    {

        List<Projectile> shots = h.getProjectiles();

        for (int i = 0; i < shots.Count; i++)
        {
            if (shots[i].inRange(c, 0))
            {
                shots.Remove(shots[i]);
            }

        }
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
}

}