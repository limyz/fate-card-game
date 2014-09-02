using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class CardForm : SivForm
    {
        private CardDeck card;
        public CardDeck CardDeck
        {
            get { return card; }
            set
            {
                card = value;
            }
        }
        private Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
        }
        public Nullable<Rectangle> Source_Rectangle = null;//A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.
        public Color color = Color.White;//The color to tint the sprite. Use Color.White for full color with no tinting
        public Single Rotation = 0f;//Specifies the angle (in radians) to rotate the sprite about its center.
        public Vector2 Origin = new Vector2(0, 0);//The sprite origin; the default is (0,0) which represents the upper-left corner.
        public Vector2 Scale = new Vector2(1, 1);//Scale factor.
        public SpriteEffects effects = SpriteEffects.None;//Effects to apply.
        public float DrawOrder = 0.5f;//The depth of a layer. By default, 0 represents the front layer and 1 represents a back layer. Use SpriteSortMode if you want sprites to be sorted during drawing.
        public Boolean Border = false;
        public float BorderWidth = 3;
        public Color BorderColor = Color.Blue;

        public CardForm(CardDeck card, RectangleF rec, float draw_order, ContentManager content, Screen parent)
            : base(card.Card.CardName, parent, typeof(CardForm), rec)
        {
            CardDeck = card;
            DrawOrder = draw_order;
            this.LoadTexture(content);
        }

        public void LoadTexture(ContentManager content)
        {
            string type = "";
            if (card.Card.CardType == CardType.Basic) type = "Basic";
            else if (card.Card.CardType == CardType.Tool) type = "Tool";
            else if (card.Card.CardType == CardType.Weapon) type = "Equip";
            else if (card.Card.CardType == CardType.Armour) type = "Equip";
            else if (card.Card.CardType == CardType.PlusVehicle) type = "Equip";
            else if (card.Card.CardType == CardType.MinusVehicle) type = "Equip";
            this.texture = content.Load<Texture2D>("Resource/Cards/" + type + "/" + card.Card.CardAsset);
        }

        public override void Update(GameTime gameTime)
        {
        }

        #region Draw's function
        private void Draw_Horizontal_Line(SpriteBatch sb, float y)
        {
            sb.Draw(Game1.whiteTexture, new RectangleF(Rect.X - BorderWidth, y, Rect.Width + BorderWidth * 2, BorderWidth), Source_Rectangle, BorderColor, Rotation, Origin, effects, DrawOrder);
        }
        private void Draw_Vertical_Line(SpriteBatch sb, float x)
        {
            sb.Draw(Game1.whiteTexture, new RectangleF(x, Rect.Y - BorderWidth, BorderWidth, Rect.Height + BorderWidth * 2), Source_Rectangle, BorderColor, Rotation, Origin, effects, DrawOrder);
        }
        #endregion

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, Rect, Source_Rectangle, color, Rotation, Origin, Scale, effects, DrawOrder);
            if (this.Border)
            {
                Draw_Horizontal_Line(spriteBatch, Rect.Y - BorderWidth);
                Draw_Horizontal_Line(spriteBatch, Rect.Y + Rect.Height);
                Draw_Vertical_Line(spriteBatch, Rect.X - BorderWidth);
                Draw_Vertical_Line(spriteBatch, Rect.X + Rect.Width);
            }
            //string suit = "";
            //Color suit_color = Color.Red;
            //if (card.CardSuit == Suit.Heart)
            //{
            //    suit = "♥";
            //}
            //else if (card.CardSuit == Suit.Diamond)
            //{
            //    suit = "♦";
            //}
            //else if (card.CardSuit == Suit.Club)
            //{
            //    suit = "♣";
            //    suit_color = Color.Black;
            //}
            //else if (card.CardSuit == Suit.Spade)
            //{
            //    suit = "♠";
            //    suit_color = Color.Black;
            //}
            //spriteBatch.DrawString(Game1.arial14Bold, suit, Rect.getXY() + new Vector2(5), suit_color, Rotation, Origin, new Vector2(1.0f), effects, DrawOrder /*- 0.001f*/);
            //string number = "";
            //if((int)(card.CardNumber) >= 2 && (int)(card.CardNumber) <= 10)
            //{
            //    number = ((int)(card.CardNumber)).ToString();
            //}
            //else if (card.CardNumber == Number.Jack)
            //{
            //    number = "J";
            //}
            //else if (card.CardNumber == Number.Queen)
            //{
            //    number = "Q";
            //}
            //else if (card.CardNumber == Number.King)
            //{
            //    number = "K";
            //}
            //else if (card.CardNumber == Number.Ace)
            //{
            //    number = "A";
            //}
            //spriteBatch.DrawString(Game1.arial14Bold, number, Rect.getXY() + new Vector2(20,5), Color.Black, Rotation, Origin, new Vector2(1.5f), effects, DrawOrder /*- 0.001f*/);
        }
    }
}
