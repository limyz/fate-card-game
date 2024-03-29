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
    public class Screen
    {
        public string name;
        protected SivEventHandler ScreenEvent;        
        public Game1 main_game;
        public List<SivForm> Form_list=new List<SivForm>();
        private SivForm activeform;
        public SivForm ActiveForm
        {
            get { return activeform; }
            set
            {
                if (activeform == value) return;
                if (value != null)
                    if (!value.Activable) return;
                if (activeform != null)
                    if (activeform.OnDeactive != null)
                        activeform.OnDeactive.Invoke(activeform, null);
                activeform = value;
                if (activeform != null)
                    if (activeform.OnActive != null)
                        activeform.OnActive.Invoke(activeform, null);
            }
        }

        public Screen(String _name, SivEventHandler theScreenEvent, Game1 parent)
        {
            name = _name;
            main_game = parent;
            ScreenEvent = theScreenEvent;

            OnClick += On_Click_Dispatcher;
            OnMouseUp += On_Mouse_Up_Dispatcher;
            OnMouseDown += On_Mouse_Down_Dispatcher;
            OnRightMouseClick += On_Right_Mouse_Click_Dispatcher;
            OnRightMouseUp += On_Right_Mouse_Up_Dispatcher;
            OnRightMouseDown += On_Right_Mouse_Down_Dispatcher;
            OnMiddleMouseClick += On_Middle_Mouse_Click_Dispatcher;
            OnMiddleMouseUp += On_Middle_Mouse_Up_Dispatcher;
            OnMiddleMouseDown += On_Middle_Mouse_Down_Dispatcher;
            OnMouseHover += On_Mouse_Hover_Dispatcher;
            OnMouseScroll += On_Mouse_Scroll_Dispatcher;
            OnMouseMove += On_Mouse_Move_Dispatcher;
            OnKeysPress += On_Keys_Pressed_Dispatcher;
            OnKeysDown += On_Keys_Down_Dispatcher;
            OnKeysUp += On_Keys_Up_Dispatcher;
        }

        #region Handler
        public MouseEventHandler OnClick;
        public MouseEventHandler OnMouseUp;
        public MouseEventHandler OnMouseDown;
        public MouseEventHandler OnRightMouseClick;
        public MouseEventHandler OnRightMouseUp;
        public MouseEventHandler OnRightMouseDown;
        public MouseEventHandler OnMiddleMouseClick;
        public MouseEventHandler OnMiddleMouseUp;
        public MouseEventHandler OnMiddleMouseDown;
        public MouseScrollEventHandler OnMouseScroll;
        public MouseEventHandler OnMouseMove;
        public MouseEventHandler OnMouseHover;
        public MouseEventHandler OnMouseEnter;
        public MouseEventHandler OnMouseLeave;
        public MouseEventHandler OnMouseDrag;
        public MouseEventHandler OnMouseDrop;
        public KeyboardEventHandler OnKeysPress;
        public KeyboardEventHandler OnKeysDown;
        public KeyboardEventHandler OnKeysUp;
        #endregion
        #region HANDLER
        private void On_Click_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnClick != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnClick.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }

            List<SivForm> lsf = Form_list.OrderBy(sv => sv.Active_Priority).ToList();
            foreach (SivForm form in lsf)
            {
                if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                {
                    if (form.Activable)
                    {
                        ActiveForm = form;
                        return;
                    }
                }
            }
            ActiveForm = null;
        }
        private void On_Mouse_Up_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnMouseUp != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnMouseUp.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Mouse_Down_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnMouseDown != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnMouseDown.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Right_Mouse_Click_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnRightMouseClick != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnRightMouseClick.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Right_Mouse_Up_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnRightMouseUp != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnRightMouseUp.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Right_Mouse_Down_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnRightMouseDown != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnRightMouseDown.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Middle_Mouse_Click_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnMiddleMouseClick != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnMiddleMouseClick.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Middle_Mouse_Up_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnMiddleMouseUp != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnMiddleMouseUp.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Middle_Mouse_Down_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnMiddleMouseDown != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnMiddleMouseDown.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Mouse_Scroll_Dispatcher(int change)
        {
            if (ActiveForm != null)
            {
                if (ActiveForm.Interactable)
                {
                    if (ActiveForm.OnMouseScroll != null)
                    {
                        ActiveForm.OnMouseScroll.Invoke(ActiveForm, new FormEventData(change));
                    }
                }
            }
        }
        private void On_Mouse_Hover_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.OnMouseHover != null)
                    {
                        if (form.Rect.Contains(new Point(ms.X, ms.Y)))
                        {
                            form.OnMouseHover.Invoke(form, new FormEventData(typeof(MouseState), ms));
                        }
                    }
                }
            }
        }
        private void On_Mouse_Move_Dispatcher(MouseState ms, MouseState last_ms)
        {
            foreach (SivForm form in Form_list)
            {
                if (form.Interactable)
                {
                    if (form.Rect.Contains(new Point(ms.X, ms.Y)) && form.OnMouseMove != null)
                    {
                        form.OnMouseMove.Invoke(form, new FormEventData(typeof(MouseState), ms));
                    }
                    if (!form.Rect.Contains(new Point(last_ms.X, last_ms.Y))
                        && form.Rect.Contains(new Point(ms.X, ms.Y))
                        && form.OnMouseEnter != null)
                    {
                        form.OnMouseEnter.Invoke(form, new FormEventData(typeof(MouseState), ms));
                    }
                    else if (!form.Rect.Contains(new Point(ms.X, ms.Y))
                        && form.Rect.Contains(new Point(last_ms.X, last_ms.Y))
                        && form.OnMouseLeave != null)
                    {
                        form.OnMouseLeave.Invoke(form, new FormEventData(typeof(MouseState), ms));
                    }
                }
            }
        }                                                                         
        private void On_Keys_Pressed_Dispatcher(Keys[] keys)
        {
            if (ActiveForm != null)
            {
                if (ActiveForm.Interactable)
                {
                    if (ActiveForm.OnKeyPress != null)
                    {
                        ActiveForm.OnKeyPress.Invoke(ActiveForm, new FormEventData(keys));
                    }
                }
            }
        }
        private void On_Keys_Down_Dispatcher(Keys[] keys)
        {
            if (ActiveForm != null)
            {
                if (ActiveForm.Interactable)
                {
                    if (ActiveForm.OnKeyDown != null)
                    {
                        ActiveForm.OnKeyDown.Invoke(ActiveForm, new FormEventData(keys));
                    }
                }
            }
        }        
        private void On_Keys_Up_Dispatcher(Keys[] keys)
        {
            if (ActiveForm != null)
            {
                if (ActiveForm.Interactable)
                {
                    if (ActiveForm.OnKeyUp != null)
                    {
                        ActiveForm.OnKeyUp.Invoke(ActiveForm, new FormEventData(keys));
                    }
                }
            }
        }
        #endregion

        public virtual void Start(Command command)
        {
        }
        public virtual void End(Command command)
        {
        }

        public UpdateDelegate FormsUpdate;
        public DrawDelegate FormsDraw;

        public List<SivForm> FormAddindList = new List<SivForm>();
        public List<SivForm> FormDeletingList = new List<SivForm>();
        public virtual void Update(GameTime theTime)
        {
            if (FormAddindList.Count > 0)
            {
                foreach (SivForm sv in FormAddindList)
                    this.Form_list.Add(sv);
                FormAddindList.Clear();
                Form_list = Form_list.OrderBy(sv => sv.Priority).ToList();
            }
            if (FormDeletingList.Count > 0)
            {
                foreach (SivForm sv in FormDeletingList)
                    this.Form_list.Remove(sv);
                FormDeletingList.Clear();
            }

            foreach (SivForm sv in Form_list)
            {
                if (sv.Visible)
                {
                    sv.Update(theTime);
                }
            }
            if (FormsUpdate != null)
            {
                FormsUpdate.Invoke(theTime);
            }
        }
        public virtual void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Console.WriteLine("------------Start Drawing--------------");
            foreach (SivForm sv in Form_list)
            {
                if (sv.Visible)
                {
                    //Console.WriteLine("Drawing: " + sv.Name);
                    sv.Draw(spriteBatch, gameTime);
                }
            }
            //Console.WriteLine("------------End Drawing--------------");
            if (FormsDraw != null)
            {
                FormsDraw.Invoke(spriteBatch, gameTime);
            }
        }
    }
}
