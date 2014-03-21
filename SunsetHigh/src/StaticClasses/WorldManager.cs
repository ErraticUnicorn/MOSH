using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;

namespace SunsetHigh {

    /// <summary>
    /// Specifies a certain room (used for linking external text files to objects in the code)
    /// </summary>
    public enum PlaceID
    {
        Nowhere = -1,
        Cafeteria = 0,
        ComputerLab,
        Entrance,
        HallwayEast,
        HallwayWest,
        Library,
        MagazineOffice,
        Math,
        Science,
        StudentLounge,
        Bathroom,
        //TODO: put other rooms here later
    }

    public class CameraOffsetEventArgs : EventArgs
    {
        public int dx_offset { get; set; }
        public int dy_offset { get; set; }
    }

    public static class WorldManager {
        private static Dictionary<PlaceID, Room> m_rooms;

        public static Room m_currentRoom { get; private set; }
        public static PlaceID m_currentRoomID { get; private set; }
        public static Point m_currentCameraOffset { get; private set; }

        public static event EventHandler<CameraOffsetEventArgs> OffsetChanged;

        private static GraphicsDevice m_gd;
        private static double m_scaleFactor;

        public static void init(GraphicsDevice graphicsDevice, double scaleFactor)
        {
            m_gd = graphicsDevice;
            m_scaleFactor = scaleFactor;
        }

        public static void loadMaps(ContentManager p_content) {
            m_rooms = new Dictionary<PlaceID, Room>();
        
            // there's probably a way to do this using loops but listing everything out is safer
            Room bathroom = new Bathroom();
            Room cafeteria = new Cafeteria();
            Room computerLab = new ComputerLab();
            Room entrance = new MainEntrance();
            Room hallwayEast = new HallwayEast();
            Room hallwayWest = new HallwayWest();
            Room library = new Library();
            Room magazineOffice = new MagazineOffice();
            Room math = new MathClassroom();
            Room science = new ScienceClassroom();
            Room studentLounge = new StudentLounge();
            //Room questHall = new Room();
            //Room questHallEnd = new Room();

            bathroom.loadContent(p_content, Directories.MAPS + "map_Bathroom");
            cafeteria.loadContent(p_content, Directories.MAPS + "map_Cafeteria");
            computerLab.loadContent(p_content, Directories.MAPS + "map_ComputerLab");
            entrance.loadContent(p_content, Directories.MAPS + "map_Entrance");
            hallwayEast.loadContent(p_content, Directories.MAPS + "map_HallwayEast");
            hallwayWest.loadContent(p_content, Directories.MAPS + "map_HallwayWest");
            library.loadContent(p_content, Directories.MAPS + "map_Library");
            magazineOffice.loadContent(p_content, Directories.MAPS + "map_MagazineOffice");
            math.loadContent(p_content, Directories.MAPS + "map_Math");
            science.loadContent(p_content, Directories.MAPS + "map_Science");
            studentLounge.loadContent(p_content, Directories.MAPS + "map_StudentLounge");
            //questHall.loadContent(p_content, "map_longhallwaymission");
            //questHallEnd.loadContent(p_content, "map_longhallwayend");

            m_rooms.Add(PlaceID.Bathroom, bathroom);
            m_rooms.Add(PlaceID.Cafeteria, cafeteria);
            m_rooms.Add(PlaceID.ComputerLab, computerLab);
            m_rooms.Add(PlaceID.Entrance, entrance);
            m_rooms.Add(PlaceID.HallwayEast, hallwayEast);
            m_rooms.Add(PlaceID.HallwayWest, hallwayWest);
            m_rooms.Add(PlaceID.Library, library);
            m_rooms.Add(PlaceID.MagazineOffice, magazineOffice);
            m_rooms.Add(PlaceID.Math, math);
            m_rooms.Add(PlaceID.Science, science);
            m_rooms.Add(PlaceID.StudentLounge, studentLounge);
            //m_rooms.Add("map_longhallwaymission", questHall);
            //m_rooms.Add("map_longhallwayend", questHallEnd);

            m_currentRoom = m_rooms[PlaceID.HallwayWest];
            m_currentRoomID = PlaceID.HallwayWest;
        }

        public static void setRoom(PlaceID p_roomName, int p_newX, int p_newY, Direction p_newDirection) {
            if (m_rooms.ContainsKey(p_roomName))
            {
                ScreenTransition.requestTransition(delegate()
                {
                    Hero.instance.reset();
                    m_currentRoomID = p_roomName;
                    m_currentRoom.onExit();
                    m_currentRoom = m_rooms[p_roomName];
                    m_currentRoom.onEnter();
                    Hero.instance.setX(p_newX);
                    Hero.instance.setY(p_newY);
                    Hero.instance.setDirection(p_newDirection);
                    updateCameraOffset(Hero.instance);
                    LocationNamePanel.instance.showNewLocation(SunsetUtils.enumToString<PlaceID>(m_currentRoomID));  //trigger header showing new location name
                });
            }
        }

        public static void setRoom(PlaceID p_roomName)
        {
            setRoom(p_roomName, Hero.instance.getX(), Hero.instance.getY(), Hero.instance.getDirection());
        }

        public static void setRoomNoTransition(PlaceID p_roomName, int p_newX, int p_newY, Direction p_newDirection)
        {
            m_currentRoomID = p_roomName;
            m_currentRoom.onExit();
            m_currentRoom = m_rooms[p_roomName];
            m_currentRoom.onEnter();
            Hero.instance.setX(p_newX);
            Hero.instance.setY(p_newY);
            Hero.instance.setDirection(p_newDirection);
            Hero.instance.reset();
            updateCameraOffset(Hero.instance);
            LocationNamePanel.instance.showNewLocation(SunsetUtils.enumToString<PlaceID>(m_currentRoomID));  //trigger header showing new location name
        }

        public static void setRoomNoTransition(PlaceID p_roomName)
        {
            setRoomNoTransition(p_roomName, Hero.instance.getX(), Hero.instance.getY(), Hero.instance.getDirection());
        }

        public static void addObjectToRoom(IInteractable p_obj, PlaceID p_room)
        {
            m_rooms[p_room].addObject(p_obj);
        }
        public static void removeObjectFromRoom(IInteractable p_obj, PlaceID p_room)
        {
            m_rooms[p_room].removeObject(p_obj);
        }
        public static void enqueueObjectToCurrentRoom(IInteractable p_obj)
        {
            m_rooms[m_currentRoomID].enqueueObject(p_obj);
        }
        public static void dequeueObjectToCurrentRoom(IInteractable p_obj)
        {
            m_rooms[m_currentRoomID].dequeueObject(p_obj);
        }
        public static void clearMaps()
        {
            foreach (Room room in m_rooms.Values)
            {
                room.clearLists();
            }
        }

        public static void handleWarp(Hero p_hero) {
            Rectangle l_heroBounds = new Rectangle(p_hero.getX(), p_hero.getY(), p_hero.getWidth() - 4, p_hero.getHeight() - 4); //NOTE!! warp collision boxes must be bigger
            MapObject l_collidedObject = CollisionManager.collisionWithObjectAtRelative(p_hero, CollisionManager.K_ZERO_OFFSET, "Teleport");

            if (l_collidedObject != null && l_collidedObject.Bounds.Contains(l_heroBounds)) {
                int l_newX = m_currentRoom.background.TileWidth * (int) l_collidedObject.Properties["warpX"];
                int l_newY = m_currentRoom.background.TileHeight * (int) l_collidedObject.Properties["warpY"];
                string l_newRoomName = (string)l_collidedObject.Properties["warpMap"];
                if (l_newRoomName.StartsWith("map_")) l_newRoomName = l_newRoomName.Substring("map_".Length);
                PlaceID l_newRoomID = SunsetUtils.parseEnum<PlaceID>(l_newRoomName);
                setRoom(l_newRoomID, l_newX, l_newY, Hero.instance.getDirection());
            }
        }

        public static void updateCameraOffset(Hero p_hero) {
            Point l_cameraOffset = new Point();
            l_cameraOffset.X = (int) (p_hero.getXCenter() - m_gd.Viewport.Width / m_scaleFactor / 2);
            l_cameraOffset.Y = (int) (p_hero.getYCenter() - m_gd.Viewport.Height / m_scaleFactor / 2);

            int l_maxCameraX = (int) (m_currentRoom.background.Width * m_currentRoom.background.TileWidth - m_gd.Viewport.Width / m_scaleFactor);
            int l_maxCameraY = (int) (m_currentRoom.background.Height * m_currentRoom.background.TileHeight - m_gd.Viewport.Height / m_scaleFactor);

            if (l_cameraOffset.X < 0) { l_cameraOffset.X = 0; }
            if (l_cameraOffset.Y < 0) { l_cameraOffset.Y = 0; }
            if (l_cameraOffset.X > l_maxCameraX) { l_cameraOffset.X = l_maxCameraX; }
            if (l_cameraOffset.Y > l_maxCameraY) { l_cameraOffset.Y = l_maxCameraY; }

            Point l_previousCameraOffset;
            l_previousCameraOffset.X = m_currentCameraOffset.X;
            l_previousCameraOffset.Y = m_currentCameraOffset.Y;

            int l_dx_offset = l_cameraOffset.X - l_previousCameraOffset.X;
            int l_dy_offset = l_cameraOffset.Y - l_previousCameraOffset.Y;

            //update
            m_currentCameraOffset = l_cameraOffset;
            if (!(l_dx_offset == 0 && l_dy_offset == 0))
            {
                CameraOffsetEventArgs args = new CameraOffsetEventArgs();
                args.dx_offset = l_dx_offset;
                args.dy_offset = l_dy_offset;
                if (OffsetChanged != null)
                {
                    OffsetChanged(null, args);  //call all registered listeners
                }
            }
        }

        public static void drawMap(SpriteBatch p_spriteBatch) {
            Rectangle l_visibleArea = new Rectangle(m_currentCameraOffset.X, m_currentCameraOffset.Y, 
                (int)(m_gd.Viewport.Width / m_scaleFactor),
                (int)(m_gd.Viewport.Height / m_scaleFactor));
            m_currentRoom.background.Draw(p_spriteBatch, l_visibleArea);
            m_currentRoom.draw(p_spriteBatch);
        }

        public static void update(float elapsed)
        {
            m_currentRoom.update(elapsed);
        }
    }
}