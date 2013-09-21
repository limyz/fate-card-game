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
        public Texture2D texture;
        public bool isHovered = false;
        float draw_order=0.5f;

        public ImageButton(string name, Texture2D button_texture, Rectangle button_rec
            , Screen parent)
            : base(name, parent, typeof(ImageButton), button_rec)
        {
            texture = button_texture;
            this.parent = parent;
            imageButton_RegisterHandler();
        }

        public ImageButton(string name, Texture2D button_texture, Rectangle button_rec
            , float p_draw_order, Screen parent)
            : base(name, parent, typeof(ImageButton), button_rec)
        {
            texture = button_texture;
            draw_order = p_draw_order;
            this.parent = parent;
            imageButton_RegisterHandler();
        }

        public override void Update(GameTime gameTime)
        {
        }
        #region FormEventHandler
        private void imageButton_RegisterHandler()
        {
            OnMouseEnter += imageButton_OnMouseEnter;
            OnMouseLeave += imageButton_OnMouseLeave;
        }
        private void imageButton_OnMouseEnter(object sender, FormEventData e)
        {
            isHovered = true;
        }
        private void imageButton_OnMouseLeave(object sender, FormEventData e)
        {
            isHovered = false;
        }
        #endregion

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (isHovered)
            {
                spriteBatch.Draw(texture/*main_game.white_texture*/, rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, draw_order + 0.0001f);
                spriteBatch.Draw(texture, rec, null, new Color(0xFF, 0xFF, 0xFF, 0xCC), 0f, new Vector2(0, 0), SpriteEffects.None, draw_order);
            }
            else
            {
                spriteBatch.Draw(texture, rec, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, draw_order);
            }
        }     
    }
}
