using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1.Class
{
    [Serializable]
    public class Character
    {
        private ContentManager Content;
        public string CharName;
        public string Ability1, Ability2, Ability3, Ability4, Ability5;
        public int MaxHealth;
        public string CharAsset;
        public string CharClass;
        public enum Type
        {
            Master = 0,
            Servant = 1,
            Human = 2,
            Homunculus = 3,
            Monster = 4
        };
        Type CharType;
        public Texture2D CharTexture;
        public Character(ContentManager Content, string charName,
            string charAsset, Type type)
        {
            this.Content = Content;
            this.CharName = charName;
            this.CharAsset = charAsset;
            this.CharType = type;
        }

        public Character(ContentManager Content, string charName, string charClass, int maxHealth,
            string charAsset, Type type, string ability1, string ability2)
        {
            this.Content = Content;
            this.CharName = charName;
            this.CharAsset = charAsset;
            this.MaxHealth = maxHealth;
            this.CharClass = charClass;
            this.CharType = type;
            this.Ability1 = ability1;
            this.Ability2 = ability2;
            this.Ability3 = null;
            this.Ability4 = null;
            this.Ability5 = null;
        }

        public Character(ContentManager Content, string charName, string charClass,
            int maxHealth, string charAsset, Type type, 
            string ability1, string ability2, string ability3, string ability4, string ability5)
        {
            this.Content = Content;
            this.CharName = charName;
            this.CharAsset = charAsset;
            this.MaxHealth = maxHealth;
            this.CharClass = charClass;
            this.CharType = type;
            this.Ability1 = ability1;
            this.Ability2 = ability2;
            this.Ability3 = ability3;
            this.Ability4 = ability4;
            this.Ability5 = ability5;
        }

        public void load_texture()
        {
            CharTexture = Content.Load<Texture2D>("Resource/character/" + CharAsset);
        }
    }
}
