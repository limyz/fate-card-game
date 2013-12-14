using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    #region Delegates and Events
    public delegate void FormEventHandler(object sender = null, FormEventData e = null);
    public class FormEventData: EventArgs
    {
        public FormEventData()
        {

        }
        public FormEventData(Type args_type,object args)
        {
            this.args_type = args_type;
            this.args = args;
        }

        public Type args_type;
        public object args;
    }
    #endregion

    public class SivForm
    {
        public string Name;
        public Screen Parent;
        public Type original_type;
        public Rectangle Rect;
        public object Value;
        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value) return;
                _visible = value;
                if (_visible)
                {
                    Parent.FormsUpdate += Update;
                    Parent.FormsDraw += Draw;
                }
                else
                {
                    Parent.FormsUpdate -= Update;
                    Parent.FormsDraw -= Draw;
                }
            }
        }

        public FormEventHandler OnClick;
        public FormEventHandler OnMouseUp;
        public FormEventHandler OnMouseDown;
        public FormEventHandler OnRightMouseClick;
        public FormEventHandler OnRightMouseUp;
        public FormEventHandler OnRightMouseDown;
        public FormEventHandler OnMiddleMouseClick;
        public FormEventHandler OnMiddleMouseUp;
        public FormEventHandler OnMiddleMouseDown;
        public FormEventHandler OnMouseScroll;
        public FormEventHandler OnMouseMove;
        public FormEventHandler OnMouseHover;
        public FormEventHandler OnMouseEnter;
        public FormEventHandler OnMouseLeave;
        public FormEventHandler OnMouseDrag;
        public FormEventHandler OnMouseDrop;
        public FormEventHandler OnKeyPress;
        public FormEventHandler OnKeyDown;
        public FormEventHandler OnKeyUp;

        public SivForm(string _name, Screen parent, Type original_type,Rectangle rec)
        {
            Name = _name;
            this.Parent = parent;
            this.original_type = original_type;
            this.Rect = rec;
            parent.Form_list.Add(this);
            parent.FormsUpdate += Update;
            parent.FormsDraw += Draw;

            //Debugger
            parent.main_game.debugger.Register_For_Debug(this);
        }

        public void Delete(){
            Parent.FormsUpdate -= Update;
            Parent.FormsDraw -= Draw;
            Parent.FormsUpdate += Remove_From_Forms_List;           
        }

        private void Remove_From_Forms_List(GameTime gameTime)
        {
            //Game1.MessageBox(new IntPtr(0), parent.Form_list.Count.ToString(), "before", 0);
            Parent.Form_list.Remove(this);        
            Parent.FormsUpdate -= Remove_From_Forms_List;
            //Game1.MessageBox(new IntPtr(0), parent.Form_list.Count.ToString(), "after", 0);
        }

        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }
    }
}
