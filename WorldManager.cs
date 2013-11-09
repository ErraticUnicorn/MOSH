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
        private static Dictionary<String, Room> m_rooms;
        public static Room m_currentRoom { get; private set; }
        public static string m_currentRoomName { get; private set; }

        public static void loadMaps(ContentManager p_content) {
            m_rooms = new Dictionary<String, Room>();
        
            // there's probably a way to do this using loops but listing everything out is safer
            Room library = new Library();
            library.loadContent(p_content, "map_Library");
            Room cafeteria = new Cafeteria();
            cafeteria.loadContent(p_content, "map_Cafeteria");
            Room hallway = new Room();
            hallway.loadContent(p_content, "map_Hallway");
            m_rooms.Add("map_Library", library);
            m_rooms.Add("map_Cafeteria", cafeteria);
            m_rooms.Add("map_Hallway", hallway);

            m_currentRoom = m_rooms["map_Library"];
            m_currentRoomName = "map_Library";
        }

        public static void setRoom(String p_roomName) {
            if (m_rooms.ContainsKey(p_roomName))
            {
                m_currentRoomName = p_roomName;
                m_currentRoom = m_rooms[p_roomName];
                m_currentRoom.updateState();
            }
        }

        public static void handleWarp(Hero p_hero) {
            Rectangle l_heroBounds = new Rectangle(p_hero.getX(), p_hero.getY(), p_hero.getWidth() - 4, p_hero.getHeight() - 4); //NOTE!! warp collision boxes must be bigger
            MapObject l_collidedObject = CollisionManager.collisionWithObjectAtRelative(p_hero, CollisionManager.K_ZERO_OFFSET, "Teleport");

            if (l_collidedObject != null && l_collidedObject.Bounds.Contains(l_heroBounds)) {
                setRoom((string) l_collidedObject.Properties["warpMap"]);
                p_hero.setX(m_currentRoom.background.TileWidth * (int) l_collidedObject.Properties["warpX"]);
                p_hero.setY(m_currentRoom.background.TileHeight * (int) l_collidedObject.Properties["warpY"]);
            }
        }

        public static Point getCameraOffset(Hero p_hero, GraphicsDevice p_gd, double l_scaleFactor) {
            Point l_cameraOffset = new Point();
            l_cameraOffset.X = (int) (p_hero.getXCenter() - p_gd.Viewport.Width / l_scaleFactor / 2);
            l_cameraOffset.Y = (int) (p_hero.getYCenter() - p_gd.Viewport.Height / l_scaleFactor / 2);

            int l_maxCameraX = (int) (m_currentRoom.background.Width * m_currentRoom.background.TileWidth - p_gd.Viewport.Width / l_scaleFactor);
            int l_maxCameraY = (int) (m_currentRoom.background.Height * m_currentRoom.background.TileHeight - p_gd.Viewport.Height / l_scaleFactor);

            if (l_cameraOffset.X < 0) { l_cameraOffset.X = 0; }
            if (l_cameraOffset.Y < 0) { l_cameraOffset.Y = 0; }
            if (l_cameraOffset.X > l_maxCameraX) { l_cameraOffset.X = l_maxCameraX; }
            if (l_cameraOffset.Y > l_maxCameraY) { l_cameraOffset.Y = l_maxCameraY; }

            return l_cameraOffset;
        }

        public static void drawMap(SpriteBatch p_spriteBatch, Rectangle p_worldArea) {
            m_currentRoom.background.Draw(p_spriteBatch, p_worldArea);
            m_currentRoom.draw(p_spriteBatch);
        }

        public static void update(float elapsed)
        {
            m_currentRoom.update(elapsed);
        }
    }

}