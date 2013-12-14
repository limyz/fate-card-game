using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public delegate void ImageButtonEvent(ImageButton sender);

    public class ImageButton : SivForm
    {
        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public bool IsHovered = false;
        public float DrawOrder = 0.5f;

        public ImageButton(string name, Texture2D button_texture, Rectangle button_rec
            , Screen parent)
            : base(name, parent, typeof(ImageButton), button_rec)
        {
            texture = button_texture;
            this.Parent = parent;
            imageButton_RegisterHandler();
        }

        public ImageButton(string name, Texture2D button_texture, Rectangle button_rec
            , float p_draw_order, Screen parent)
            : base(name, parent, typeof(ImageButton), button_rec)
        {
            texture = button_texture;
            DrawOrder = p_draw_order;
            this.Parent = parent;
            imageButton_RegisterHandler();
        }

        #region FormEventHandler
        private void imageButton_RegisterHandler()
        {
            OnMouseEnter += imageButton_OnMouseEnter;
            OnMouseLeave += imageButton_OnMouseLeave;
        }
        private void imageButton_OnMouseEnter(object sender, FormEventData e)
        {
            IsHovered = true;
        }
        private void imageButton_OnMouseLeave(object sender, FormEventData e)
        {
            IsHovered = false;
        }
        #endregion

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsHovered)
            {
                spriteBatch.Draw(texture/*main_game.white_texture*/, Rect, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, DrawOrder + 0.0001f);
                spriteBatch.Draw(texture, Rect, null, new Color(0xFF, 0xFF, 0xFF, 0xCC), 0f, new Vector2(0, 0), SpriteEffects.None, DrawOrder);
            }
            else
            {
                spriteBatch.Draw(texture, Rect, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, DrawOrder);
            }
        }
    }
}
