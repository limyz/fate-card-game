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
        public RectangleF Rect;
        public object Value;
        public bool Visible = true;
        public float Priority = 0.5f;
        public float Active_Priority = 0.5f;
        public bool Activable = false;

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
        public FormEventHandler OnActive;
        public FormEventHandler OnDeactive;

        public SivForm(string _name, Screen parent, Type original_type,RectangleF rec)
        {
            Name = _name;
            this.Parent = parent;
            this.original_type = original_type;
            this.Rect = rec;
            Parent.FormAddindList.Add(this);

            //Debugger
            //parent.main_game.debugger.Register_For_Debug(this);
        }
        public SivForm(string _name, Screen parent, Type original_type, RectangleF rec, float priority, float active_priority)
        {
            Name = _name;
            this.Parent = parent;
            this.original_type = original_type;
            this.Rect = rec;
            this.Priority = priority;
            this.Active_Priority = active_priority;
            Parent.FormAddindList.Add(this);

            //Debugger
            //parent.main_game.debugger.Register_For_Debug(this);
        }

        public void Delete(){
            Parent.FormDeletingList.Add(this);       
        }

        Vector2 NewXY;
        float MoveSpeed = 0f;
        public void MoveBySpeed(float NewX, float NewY, float Speed/*move_time*/)
        {
            this.NewXY.X = NewX;
            this.NewXY.Y = NewY;
            this.MoveSpeed = Speed;

            Parent.FormsUpdate -= this.Mover;
            Parent.FormsUpdate += this.Mover;
        }
        public void MoveBySecond(float NewX, float NewY, float Second/*move_time*/)
        {
            this.NewXY.X = NewX;
            this.NewXY.Y = NewY;
            this.MoveSpeed = (NewXY - Rect.getXY()).Length() / Second;
            //this.MoveSecond = Second;

            Parent.FormsUpdate -= this.Mover;
            Parent.FormsUpdate += this.Mover;
        }
        private void Mover(GameTime gameTime)
        {
            Vector2 move_speed = (NewXY - Rect.getXY());
            move_speed.Normalize();
            if (float.IsNaN(move_speed.X) || float.IsNaN(move_speed.Y))
                move_speed = Vector2.One;
            float move_time = MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 move_distance = move_speed * move_time;
            Console.WriteLine(move_distance.ToString());
            if ((Math.Abs(Rect.X - NewXY.X) < Math.Abs(move_distance.X))
                && (Math.Abs(Rect.Y - NewXY.Y) < Math.Abs(move_distance.Y)))
            {
                Rect.X = NewXY.X;
                Rect.Y = NewXY.Y;
                Parent.FormsUpdate -= this.Mover;
                return;
            }
            this.Rect.X += move_distance.X;
            this.Rect.Y += move_distance.Y;
            /*double direction = (float)(Math.Atan2(NewY- Rect.Y, NewX - Rect.X) * 180 / Math.PI);
            this.Rect.X += (float)Math.Cos(direction * Math.PI / 180) * MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.Rect.Y += (float)Math.Sin(direction * Math.PI / 180) * MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;*/
        }

        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }
    }
}
