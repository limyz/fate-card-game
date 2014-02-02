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
        public FormEventData(object args)
        {
            this.args = args;
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
            //parent.main_game.debugger.Register_For_Debug(this);
        }

        public void Delete(){
            Parent.FormsUpdate -= Update;
            Parent.FormsDraw -= Draw;
            Parent.FormsUpdate += this.Remove_From_Forms_List;           
        }

        private void Remove_From_Forms_List(GameTime gameTime)
        {
            //Game1.MessageBox(new IntPtr(0), Parent.Form_list.Count.ToString(), "before", 0);
            Parent.Form_list.Remove(this);        
            Parent.FormsUpdate -= Remove_From_Forms_List;
            //Game1.MessageBox(new IntPtr(0), Parent.Form_list.Count.ToString(), "after", 0);
        }

        int NewX;
        int NewY;
        float realX;
        float realY;
        float MoveTime = 0;
        public void Move(int NewX, int NewY, float move_time)
        {
            this.NewX = NewX;
            this.NewY = NewY;
            this.MoveTime = move_time;
            this.realX = Rect.X;
            this.realY = Rect.Y;

            Parent.FormsUpdate += this.Mover;
        }
        private void Mover(GameTime gameTime)
        {
            if (MoveTime == 0 || (Rect.X == NewX && Rect.Y == NewY ))
            {
                Parent.FormsUpdate -= this.Mover;
                return;
            }
            this.realX += ((this.NewX - this.realX) / MoveTime);
            this.realY += ((this.NewY - this.realY) / MoveTime);
            this.Rect.X = (int)realX;
            this.Rect.Y = (int)realY;
            this.MoveTime--;
        }

        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }
    }
}
