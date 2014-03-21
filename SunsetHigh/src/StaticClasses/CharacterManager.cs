using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SunsetHigh
{
    /// <summary>
    /// Specifies a certain person (used for linking external text files to objects in the code)
    /// </summary>
    public enum PersonID
    {
        None = -1,
        Phil = 0,
        Librarian,
        Bill,
        Artie,
        Claude,
        Jay1,
        Jay2,
        Jay3,
        Jay4,
        BraceFace,
        Fitz,
        ROB,
        Enforcer,
        Tyke,
        Dyke,
        Shaq,
        Herbert,
        Avery,
        Boris,
        Jarvis,
        Zeke
    }

    public struct CharacterBankSave
    {
        public int[] char_x;
        public int[] char_y;
        public PlaceID[] char_room;
    }

    public static class CharacterManager
    {
        public static int NUM_PERSON_IDS = Enum.GetValues(typeof(PersonID)).Length - 1;

        private static Dictionary<PersonID, Character> mCharMap;
        private static Dictionary<PersonID, PlaceID> mRoomMap;

        public static void loadContent(ContentManager content)
        {
            mCharMap = new Dictionary<PersonID, Character>();
            mRoomMap = new Dictionary<PersonID, PlaceID>();

            //initialize all the games characters
            //NOTE: their initial positions are specified in SaveManager's unpackDefaultData class
            Character phil = new Character();
            phil.loadImage(content, Directories.CHARACTERS + "sprite_ffwriter");
            phil.setScript(Directories.INTERACTIONS + "Phil.txt");
            mCharMap[PersonID.Phil] = phil;

            Character librarian = new Character();
            librarian.loadImage(content, Directories.CHARACTERS_TEMP + "teacher");
            librarian.setScript(Directories.INTERACTIONS + "Librarian.txt");
            mCharMap[PersonID.Librarian] = librarian;

            Character bill = new Character();
            bill.loadImage(content, Directories.CHARACTERS_TEMP + "jock");
            bill.setScript(Directories.INTERACTIONS + "Bill.txt");
            mCharMap[PersonID.Bill] = bill;

            Character artie = new Character();
            artie.loadImage(content, Directories.CHARACTERS_TEMP + "prep");
            artie.setScript(Directories.INTERACTIONS + "Artie.txt");
            mCharMap[PersonID.Artie] = artie;

            Character claude = new Character();
            claude.loadImage(content, Directories.CHARACTERS_TEMP + "slacker");
            claude.setScript(Directories.INTERACTIONS + "Claude.txt");
            mCharMap[PersonID.Claude] = claude;

            Character jay1 = new Character();
            jay1.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            jay1.setScript(Directories.INTERACTIONS + "Jay1.txt");
            mCharMap[PersonID.Jay1] = jay1;
            Character jay2 = new Character();
            jay2.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            jay2.setScript(Directories.INTERACTIONS + "Jay2.txt");
            mCharMap[PersonID.Jay2] = jay2;
            Character jay3 = new Character();
            jay3.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            jay3.setScript(Directories.INTERACTIONS + "Jay3.txt");
            mCharMap[PersonID.Jay3] = jay3;
            Character jay4 = new Character();
            jay4.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            jay4.setScript(Directories.INTERACTIONS + "Jay4.txt");
            mCharMap[PersonID.Jay4] = jay4;

            Character braceface = new BraceFace();
            braceface.loadContent(content);
            braceface.loadImage(content, Directories.CHARACTERS + "sprite_brace_face_temp");
            braceface.setScript(Directories.INTERACTIONS + "BraceFace.txt");
            mCharMap[PersonID.BraceFace] = braceface;

            Character fitz = new Character();
            fitz.loadImage(content, Directories.CHARACTERS_TEMP + "nerd");
            fitz.setScript(Directories.INTERACTIONS + "Fitz.txt");
            mCharMap[PersonID.Fitz] = fitz;

            Character rob = new Character();
            rob.loadImage(content, Directories.CHARACTERS_TEMP + "nerd2");
            rob.setScript(Directories.INTERACTIONS + "ROB.txt");
            mCharMap[PersonID.ROB] = rob;

            Character enforcer = new Character();
            enforcer.loadImage(content, Directories.CHARACTERS + "sprite_enforcer");
            enforcer.setScript(Directories.INTERACTIONS + "Enforcer.txt");
            mCharMap[PersonID.Enforcer] = enforcer;

            Character tyke = new Character();
            tyke.loadImage(content, Directories.CHARACTERS_TEMP + "bully");
            tyke.setScript(Directories.INTERACTIONS + "Tyke.txt");
            mCharMap[PersonID.Tyke] = tyke;

            Character dyke = new Character();
            dyke.loadImage(content, Directories.CHARACTERS_TEMP + "bully2");
            dyke.setScript(Directories.INTERACTIONS + "Dyke.txt");
            mCharMap[PersonID.Dyke] = dyke;

            Character shaq = new Character();
            shaq.loadImage(content, Directories.CHARACTERS_TEMP + "jock");
            shaq.setScript(Directories.INTERACTIONS + "Shaq.txt");
            mCharMap[PersonID.Shaq] = shaq;

            Character herbert = new Character();
            herbert.loadImage(content, Directories.CHARACTERS + "sprite_herbert_front");
            herbert.setScript(Directories.INTERACTIONS + "Herbert.txt");
            mCharMap[PersonID.Herbert] = herbert;

            Character avery = new Character();
            avery.loadImage(content, Directories.CHARACTERS_TEMP + "prep");
            avery.setScript(Directories.INTERACTIONS + "Avery.txt");
            mCharMap[PersonID.Avery] = avery;

            Character boris = new Character();
            boris.loadImage(content, Directories.CHARACTERS_TEMP + "prep2");
            boris.setScript(Directories.INTERACTIONS + "Boris.txt");
            mCharMap[PersonID.Boris] = boris;

            Character jarvis = new Character();
            jarvis.loadImage(content, Directories.CHARACTERS_TEMP + "slacker");
            jarvis.setScript(Directories.INTERACTIONS + "Jarvis.txt");
            mCharMap[PersonID.Jarvis] = jarvis;

            Character zeke = new Character();
            zeke.loadImage(content, Directories.CHARACTERS_TEMP + "slacker2");
            zeke.setScript(Directories.INTERACTIONS + "Zeke.txt");
            mCharMap[PersonID.Zeke] = zeke;
        }

        public static Character getCharacter(PersonID id)
        {
            return mCharMap[id];
        }

        public static void moveCharacterToRoom(PersonID person, PlaceID place, int x, int y)
        {
            Character c1 = mCharMap[person];
            WorldManager.removeObjectFromRoom(c1, mRoomMap[person]);
            WorldManager.addObjectToRoom(c1, place);
            mRoomMap[person] = place;
            c1.setX(x);
            c1.setY(y);
            c1.reset();  
        }

        public static void moveCharacterToRoom(PersonID person, PlaceID place)
        {
            moveCharacterToRoom(person, place, mCharMap[person].getX(), mCharMap[person].getY());
        }

        public static CharacterBankSave getSaveStructure()
        {
            CharacterBankSave data = new CharacterBankSave();
            data.char_x = new int[NUM_PERSON_IDS];
            data.char_y = new int[NUM_PERSON_IDS];
            data.char_room = new PlaceID[NUM_PERSON_IDS];
            for (int i = 0; i < NUM_PERSON_IDS; i++)
            {
                Character c1 = mCharMap[(PersonID)i];
                data.char_x[i] = c1.getX();
                data.char_y[i] = c1.getY();
                data.char_room[i] = mRoomMap[(PersonID)i];
            }
            return data;
        }

        public static void loadSaveStructure(CharacterBankSave save)
        {
            for (int i = 0; i < NUM_PERSON_IDS && i < save.char_x.Length; i++)
            {
                Character c1 = mCharMap[(PersonID)i];
                c1.setX(save.char_x[i]);
                c1.setY(save.char_y[i]);
                c1.reset();
                if (mRoomMap.ContainsKey((PersonID)i))
                    WorldManager.removeObjectFromRoom(c1, mRoomMap[(PersonID)i]);
                WorldManager.addObjectToRoom(c1, save.char_room[i]);
                mRoomMap[(PersonID)i] = save.char_room[i];
            }
        }
    }
}
