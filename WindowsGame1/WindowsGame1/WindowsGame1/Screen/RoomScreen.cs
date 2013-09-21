﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    class RoomScreen : Screen
    {
        #region variable decleration
        SpriteFont font;
        #endregion

        #region Load Content
        public RoomScreen(GraphicsDeviceManager graphics, ContentManager Content, SivEventHandler theEvent, Game1 parent)
            : base("HostScreen",theEvent, parent)
        {
            font = Content.Load<SpriteFont>("SpriteFont1");

            #region RoomScreen_RegisterHandler
            OnKeysDown += RoomScreen_OnKeysDown;
            #endregion
        }
         #endregion

        #region HANDLER
        private void RoomScreen_OnKeysDown(Keys[] keys)
        {
            foreach (Keys k in keys)
            {
                if (k == Keys.Escape)
                    ScreenEvent.Invoke(this, new SivEventArgs(0));
                return;
            }
        }
        #endregion

        #region Update
        public override void Update(GameTime theTime)
        {
            base.Update(theTime);
        }
        #endregion

        #region Draw
        public override void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, main_game.mouse_pos.ToString(), new Vector2(0, 0), Color.White);
            base.Draw(graphics, spriteBatch, gameTime);
            spriteBatch.End();
        }
        #endregion
    }
}
