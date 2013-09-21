using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;
//using Keys = System.Windows.Forms.Keys;

namespace WindowsGame1
{
    #region 2
    public interface IKeyboardSubscriber
    {
        void RecieveTextInput(char inputChar);
        void RecieveTextInput(string text);
        void RecieveCommandInput(char command);
        void RecieveSpecialInput(Keys key);

        bool Selected { get; set; } //or Focused
    }

    public class KeyboardTextDispatcher
    {

        public KeyboardTextDispatcher(GameWindow window)
        {
            EventTextInput.EventTextInput.Initialize(window);
            EventTextInput.EventTextInput.CharEntered += new EventTextInput.CharEnteredHandler(EventInput_CharEntered);
            EventTextInput.EventTextInput.KeyDown += new EventTextInput.KeyEventHandler(EventInput_KeyDown);
        }

        void EventInput_KeyDown(object sender, EventTextInput.KeyEventArgs e)
        {
            if (_subscriber == null)
                return;
            //Game1.MessageBox(new IntPtr(0), e.KeyCode.ToString(), "", 0);
            _subscriber.RecieveSpecialInput(e.KeyCode);
        }

        void EventInput_CharEntered(object sender, EventTextInput.CharacterEventArgs e)
        {
            if (_subscriber == null)
                return;
            if (char.IsControl(e.Character))
            {
                //Game1.MessageBox(new IntPtr(0), ((int)e.Character).ToString(), "", 0);
                //ctrl-v
                if (e.Character == 0x16)
                {
                    //XNA runs in Multiple Thread Apartment state, which cannot recieve clipboard
                    Thread thread = new Thread(PasteThread);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                    _subscriber.RecieveTextInput(_pasteResult);
                }
                else
                {
                    _subscriber.RecieveCommandInput(e.Character);
                }
            }
            else
            {
                //Game1.MessageBox(new IntPtr(0), ((int)e.Character).ToString(), "", 0);
                _subscriber.RecieveTextInput(e.Character);
            }
        }

        IKeyboardSubscriber _subscriber;
        public IKeyboardSubscriber Subscriber
        {
            get { return _subscriber; }
            set
            {
                if (_subscriber != null)
                    _subscriber.Selected = false;
                _subscriber = value;
                if (value != null)
                    value.Selected = true;
            }
        }

        //Thread has to be in Single Thread Apartment state in order to receive clipboard
        string _pasteResult = "";
        [STAThread]
        void PasteThread()
        {
            if (Clipboard.ContainsText())
            {
                _pasteResult = Clipboard.GetText();
            }
            else
            {
                _pasteResult = "";
            }
        }
    }
    #endregion
}
