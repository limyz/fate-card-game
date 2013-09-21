using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsGame1
{
    public partial class Debug : Form
    {
        public Debug()
        {
            InitializeComponent();
        }

#region Register
        List<TextBox> Textbox_List = new List<TextBox>();
        List<ImageButton> ImageButton_List = new List<ImageButton>();
        List<Label> Label_List = new List<Label>();

        public void Register_For_Debug(SivForm sv)
        {
            if(sv.original_type == typeof(TextBox))
            {
            Textbox_List.Add((TextBox)sv);
            listBox1.Items.Add(sv.name);
            }

            else if (sv.original_type == typeof(ImageButton))
            {
                ImageButton_List.Add((ImageButton)sv);
                listBox2.Items.Add(sv.name);
            }

            else if (sv.original_type == typeof(Label))
            {
                Label_List.Add((Label)sv);
                listBox3.Items.Add(sv.name);
            }
        }
#endregion

#region TextboxTab
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextBox t = Textbox_List[listBox1.SelectedIndex];
            X_textBox.Text = t.rec.X.ToString();
            Y_textBox.Text = t.rec.Y.ToString();
            Width_textBox.Text = t.rec.Width.ToString();
            Height_textBox.Text = t.rec.Height.ToString();
            scrollable_checkBox.Checked = t.scrollable;
            Text_textBox.Text = t.Text;
        }

        private void Apply_Button_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            try
            {
                TextBox t = Textbox_List[listBox1.SelectedIndex];
                t.rec.X = int.Parse(X_textBox.Text);
                t.rec.Y = int.Parse(Y_textBox.Text);
                t.rec.Width = int.Parse(Width_textBox.Text);
                t.rec.Height = int.Parse(Height_textBox.Text);
                t.scrollable = scrollable_checkBox.Checked;
                t.Text = Text_textBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Reset_Button_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            TextBox t = Textbox_List[listBox1.SelectedIndex];
            X_textBox.Text = t.rec.X.ToString();
            Y_textBox.Text = t.rec.Y.ToString();
            Width_textBox.Text = t.rec.Width.ToString();
            Height_textBox.Text = t.rec.Height.ToString();
            scrollable_checkBox.Checked = t.scrollable;
            Text_textBox.Text = t.Text;
        }
#endregion

#region ButtonTab
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImageButton i = ImageButton_List[listBox2.SelectedIndex];
            X_textBox2.Text = i.rec.X.ToString();
            Y_textBox2.Text = i.rec.Y.ToString();
            Width_textBox2.Text = i.rec.Width.ToString();
            Height_textBox2.Text = i.rec.Height.ToString();
        }

        private void Apply_button2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0) return;
            try
            {
                ImageButton i = ImageButton_List[listBox2.SelectedIndex];
                i.rec.X = int.Parse(X_textBox2.Text);
                i.rec.Y = int.Parse(Y_textBox2.Text);
                i.rec.Width = int.Parse(Width_textBox2.Text);
                i.rec.Height = int.Parse(Height_textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Reset_button2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0) return;
            ImageButton i = ImageButton_List[listBox2.SelectedIndex];
            X_textBox2.Text = i.rec.X.ToString();
            Y_textBox2.Text = i.rec.Y.ToString();
            Width_textBox2.Text = i.rec.Width.ToString();
            Height_textBox2.Text = i.rec.Height.ToString();
        }
#endregion

#region LabelTab
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label l = Label_List[listBox3.SelectedIndex];
            X_textBox3.Text = l.rec.X.ToString();
            Y_textBox3.Text = l.rec.Y.ToString();
            Width_textBox3.Text = l.rec.Width.ToString();
            Text_textBox3.Text = l.text;
            Red_textBox3.Text = l._color.R.ToString();
            Green_textBox3.Text = l._color.G.ToString();
            Blue_textBox3.Text = l._color.B.ToString();
            Alpha_textBox3.Text = l._color.A.ToString();
        }  

        private void Apply_button3_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex < 0) return;
            try
            {
                Label l = Label_List[listBox3.SelectedIndex];
                l.rec.X = int.Parse(X_textBox3.Text);
                l.rec.Y = int.Parse(Y_textBox3.Text);
                l.rec.Width = int.Parse(Width_textBox3.Text);
                l.text = Text_textBox3.Text;
                l._color.R = Convert.ToByte(Red_textBox3.Text);
                l._color.G = Convert.ToByte(Green_textBox3.Text);
                l._color.B = Convert.ToByte(Blue_textBox3.Text);
                l._color.A = Convert.ToByte(Alpha_textBox3.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Reset_button3_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex < 0) return;
            Label l = Label_List[listBox3.SelectedIndex];
            X_textBox3.Text = l.rec.X.ToString();
            Y_textBox3.Text = l.rec.Y.ToString();
            Width_textBox3.Text = l.rec.Width.ToString();
            Text_textBox3.Text = l.text;
            Red_textBox3.Text = l._color.R.ToString();
            Green_textBox3.Text = l._color.G.ToString();
            Blue_textBox3.Text = l._color.B.ToString();
            Alpha_textBox3.Text = l._color.A.ToString();
        }
#endregion

        private void Debug_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
