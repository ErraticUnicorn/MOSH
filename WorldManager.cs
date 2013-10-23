using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;

namespace SunsetHigh {

public static class WorldManager {
    private static Dictionary<String, Map> m_maps;
    public static Map m_currentMap { get; private set; }

    public static void loadMaps(ContentManager p_content) {
        m_maps = new Dictionary<String, Map>();
        
        // there's probably a way to do this using loops but listing everything out is safer
        m_maps.Add("map_Library", p_content.Load<Map>("map_Library"));
        m_maps.Add("map_Cafeteria", p_content.Load<Map>("map_Cafeteria"));
        m_maps.Add("map_Hallway", p_content.Load<Map>("map_Hallway"));

        m_currentMap = m_maps["map_Library"];
    }

    public static void setMap(String p_mapName) {
        m_currentMap = m_maps[p_mapName];
    }

    public static void handleWarp(Hero p_hero) {
        Rectangle l_heroBounds = new Rectangle(p_hero.getX(), p_hero.getY(), p_hero.getWidth(), p_hero.getHeight());
        MapObject l_collidedObject = CollisionManager.collisionWithObjectAtRelative(p_hero, CollisionManager.K_ZERO_OFFSET, "Teleport");

        if (l_collidedObject != null && l_collidedObject.Bounds.Contains(l_heroBounds)) {
            setMap((string) l_collidedObject.Properties["warpMap"]);
            p_hero.setX(m_currentMap.TileWidth * (int) l_collidedObject.Properties["warpX"]);
            p_hero.setY(m_currentMap.TileHeight * (int) l_collidedObject.Properties["warpY"]);
        }
    }

    public static Point getCameraOffset(Hero p_hero, GraphicsDevice p_gd, double l_scaleFactor) {
        Point l_cameraOffset = new Point();
        l_cameraOffset.X = (int) (p_hero.getXCenter() - p_gd.Viewport.Width / l_scaleFactor / 2);
        l_cameraOffset.Y = (int) (p_hero.getYCenter() - p_gd.Viewport.Height / l_scaleFactor / 2);

        int l_maxCameraX = (int) (m_currentMap.Width * m_currentMap.TileWidth - p_gd.Viewport.Width / l_scaleFactor);
        int l_maxCameraY = (int) (m_currentMap.Height * m_currentMap.TileHeight - p_gd.Viewport.Height / l_scaleFactor);

        if (l_cameraOffset.X < 0) { l_cameraOffset.X = 0; }
        if (l_cameraOffset.Y < 0) { l_cameraOffset.Y = 0; }
        if (l_cameraOffset.X > l_maxCameraX) { l_cameraOffset.X = l_maxCameraX; }
        if (l_cameraOffset.Y > l_maxCameraY) { l_cameraOffset.Y = l_maxCameraY; }

        return l_cameraOffset;
    }

    public static void drawMap(SpriteBatch p_spriteBatch, Rectangle p_worldArea) {
        m_currentMap.Draw(p_spriteBatch, p_worldArea);
    }
}

}