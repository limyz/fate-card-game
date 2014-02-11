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
    public delegate void MenuItemSelectedEvent(Menu sender, int Selected_Index);

    public class Menu : SivForm
    {
        public SpriteFont Font;
        private bool Shown = false;
        public List<string> ItemsList;

        public Menu(string name, SpriteFont font, List<string> items, Screen parent)
            : base(name, parent, typeof(Menu), new RectangleF(), 0.99f, 0.01f)
        {
            Font = font;
            ItemsList = items;
            this.Activable = true;

            this.OnDeactive += Menu_OnDeactive;
            this.OnMouseLeave += Menu_OnMouseLeave;
            this.OnMouseMove += Menu_OnMouseMove;
            this.OnClick += Menu_OnClick;
        }

        public void Show(float x, float y)
        {
            float sumheight = ItemsList.Count * Font.LineSpacing + Font.Spacing;
            float maxwidth = 0;
            foreach (string s in ItemsList)
            {
                int width = (int)Font.MeasureString(s).X;
                if (width > maxwidth)
                {
                    maxwidth = width;
                }
            }
            if (maxwidth > 0)
                maxwidth += Font.LineSpacing * 2;
            this.Rect = new RectangleF(x, y, maxwidth, sumheight);
            Shown = true;
            Parent.ActiveForm = this;
        }
        public void Hide()
        {
            this.Rect = new RectangleF();
            Shown = false;
        }

        public int Hoverring_Item_Index = -1;
        #region FormEventHandler
        private void Menu_OnDeactive(object sender, FormEventData e)
        {
            this.Hide();
        }
        private void Menu_OnMouseLeave(object sender, FormEventData e)
        {
            Hoverring_Item_Index = -1;
        }
        private void Menu_OnMouseMove(object sender, FormEventData e)
        {
            MouseState ms = (MouseState)e.args;
            Hoverring_Item_Index = (int)((ms.Y - Rect.Y) / Font.LineSpacing);
        }
        private void Menu_OnClick(object sender, FormEventData e)
        {
            if (Hoverring_Item_Index != -1 && MenuItemSelected != null)
            {
                MenuItemSelected.Invoke(this, Hoverring_Item_Index);
            }
        }
        #endregion
        public event MenuItemSelectedEvent MenuItemSelected;

        public override void Update(GameTime theTime)
        {
        }
        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            if (Shown)
            {
                if (ItemsList.Count > 0)
                {
                    var cc = System.Drawing.SystemColors.Menu;
                    sb.Draw(Game1.whiteTexture, Rect, null, new Color(cc.R, cc.G, cc.B, cc.A), 0f, new Vector2(0, 0), SpriteEffects.None, 0.013f);
                    if (Hoverring_Item_Index != -1)
                    {
                        var hc = System.Drawing.SystemColors.MenuHighlight;
                        RectangleF rec = new RectangleF(Rect.X
                            , Rect.Y + Hoverring_Item_Index * Font.LineSpacing
                            , Rect.Width, Font.LineSpacing + Font.Spacing);
                        sb.Draw(Game1.whiteTexture, rec, null, new Color(hc.R, hc.G, hc.B, hc.A), 0f, new Vector2(0, 0), SpriteEffects.None, 0.012f);
                    }
                    for (int i = 0; i < ItemsList.Count; i++)
                    {
                        Vector2 pos = new Vector2(Rect.X + Font.LineSpacing, Rect.Y + i * Font.LineSpacing + Font.Spacing);
                        sb.DrawString(Font, ItemsList[i], pos, Color.Black, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.011f);
                    }
                }
            }
        }
    }
}
