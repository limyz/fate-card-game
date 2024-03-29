﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

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
        List<Border> Border_List = new List<Border>();

        public void Register_For_Debug(SivForm sv)
        {
            if (sv.original_type == typeof(TextBox))
            {
                Textbox_List.Add((TextBox)sv);
                listBox1.Items.Add(sv.Name);
            }

            else if (sv.original_type == typeof(ImageButton))
            {
                ImageButton_List.Add((ImageButton)sv);
                listBox2.Items.Add(sv.Name);
            }

            else if (sv.original_type == typeof(Label))
            {
                Label_List.Add((Label)sv);
                listBox3.Items.Add(sv.Name);
            }

            else if (sv.original_type == typeof(Border))
            {
                Border_List.Add((Border)sv);
                listBox4.Items.Add(sv.Name);
            }
        }
        #endregion

        #region TextboxTab
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextBox t = Textbox_List[listBox1.SelectedIndex];
            ParentScreen_toolStripStatusLabel2.Text = t.Parent.name;
            X_textBox.Text = t.Rect.X.ToString();
            Y_textBox.Text = t.Rect.Y.ToString();
            Width_textBox.Text = t.Rect.Width.ToString();
            Height_textBox.Text = t.Rect.Height.ToString();
            //scrollable_checkBox.Checked = t.scrollable;
            Text_textBox.Text = t.Text;
        }

        private void Apply_Button_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            try
            {
                TextBox t = Textbox_List[listBox1.SelectedIndex];
                t.Rect.X = int.Parse(X_textBox.Text);
                t.Rect.Y = int.Parse(Y_textBox.Text);
                t.Rect.Width = int.Parse(Width_textBox.Text);
                t.Rect.Height = int.Parse(Height_textBox.Text);
                //t.scrollable = scrollable_checkBox.Checked;
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
            X_textBox.Text = t.Rect.X.ToString();
            Y_textBox.Text = t.Rect.Y.ToString();
            Width_textBox.Text = t.Rect.Width.ToString();
            Height_textBox.Text = t.Rect.Height.ToString();
            //scrollable_checkBox.Checked = t.scrollable;
            Text_textBox.Text = t.Text;
        }
        #endregion

        #region ImageButtonTab
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImageButton i = ImageButton_List[listBox2.SelectedIndex];
            ParentScreen_toolStripStatusLabel2.Text = i.Parent.name;
            X_textBox2.Text = i.Rect.X.ToString();
            Y_textBox2.Text = i.Rect.Y.ToString();
            Width_textBox2.Text = i.Rect.Width.ToString();
            Height_textBox2.Text = i.Rect.Height.ToString();
        }

        private void Apply_button2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0) return;
            try
            {
                ImageButton i = ImageButton_List[listBox2.SelectedIndex];
                i.Rect.X = int.Parse(X_textBox2.Text);
                i.Rect.Y = int.Parse(Y_textBox2.Text);
                i.Rect.Width = int.Parse(Width_textBox2.Text);
                i.Rect.Height = int.Parse(Height_textBox2.Text);
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
            X_textBox2.Text = i.Rect.X.ToString();
            Y_textBox2.Text = i.Rect.Y.ToString();
            Width_textBox2.Text = i.Rect.Width.ToString();
            Height_textBox2.Text = i.Rect.Height.ToString();
        }
        #endregion

        #region LabelTab
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label l = Label_List[listBox3.SelectedIndex];
            ParentScreen_toolStripStatusLabel2.Text = l.Parent.name;
            X_textBox3.Text = l.Rect.X.ToString();
            Y_textBox3.Text = l.Rect.Y.ToString();
            Width_textBox3.Text = l.Rect.Width.ToString();
            Text_textBox3.Text = l.Text;
            Red_textBox3.Text = l.Color.R.ToString();
            Green_textBox3.Text = l.Color.G.ToString();
            Blue_textBox3.Text = l.Color.B.ToString();
            Alpha_textBox3.Text = l.Color.A.ToString();
        }

        private void Apply_button3_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex < 0) return;
            try
            {
                Label l = Label_List[listBox3.SelectedIndex];
                l.Rect.X = int.Parse(X_textBox3.Text);
                l.Rect.Y = int.Parse(Y_textBox3.Text);
                l.Rect.Width = int.Parse(Width_textBox3.Text);
                l.Text = Text_textBox3.Text;
                l.Color.R = Convert.ToByte(Red_textBox3.Text);
                l.Color.G = Convert.ToByte(Green_textBox3.Text);
                l.Color.B = Convert.ToByte(Blue_textBox3.Text);
                l.Color.A = Convert.ToByte(Alpha_textBox3.Text);
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
            X_textBox3.Text = l.Rect.X.ToString();
            Y_textBox3.Text = l.Rect.Y.ToString();
            Width_textBox3.Text = l.Rect.Width.ToString();
            Text_textBox3.Text = l.Text;
            Red_textBox3.Text = l.Color.R.ToString();
            Green_textBox3.Text = l.Color.G.ToString();
            Blue_textBox3.Text = l.Color.B.ToString();
            Alpha_textBox3.Text = l.Color.A.ToString();
        }
        #endregion

        #region BorderTab
        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            Border b = Border_List[listBox4.SelectedIndex];
            ParentScreen_toolStripStatusLabel2.Text = b.Parent.name;
            X_textBox4.Text = b.Rect.X.ToString();
            Y_textBox4.Text = b.Rect.Y.ToString();
            Width_textBox4.Text = b.Rect.Width.ToString();
            Height_textBox4.Text = b.Rect.Height.ToString();
            Border_witdh_textBox4.Text = b.Width.ToString();
            Red_textBox4.Text = b.color.R.ToString();
            Green_textBox4.Text = b.color.G.ToString();
            Blue_textBox4.Text = b.color.B.ToString();
            Alpha_textBox4.Text = b.color.A.ToString();
        }

        private void Apply_button4_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0) return;
            try
            {
                Border b = Border_List[listBox4.SelectedIndex];
                b.Rect.X = int.Parse(X_textBox4.Text);
                b.Rect.Y = int.Parse(Y_textBox4.Text);
                b.Rect.Width = int.Parse(Width_textBox4.Text);
                b.Rect.Height = int.Parse(Height_textBox4.Text);
                b.Width = int.Parse(Border_witdh_textBox4.Text);
                Microsoft.Xna.Framework.Color c = new Microsoft.Xna.Framework.Color();
                c.R = Convert.ToByte(Red_textBox4.Text);
                c.G = Convert.ToByte(Green_textBox4.Text);
                c.B = Convert.ToByte(Blue_textBox4.Text);
                c.A = Convert.ToByte(Alpha_textBox4.Text);
                b.color = c;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Reset_button4_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0) return;
            Border b = Border_List[listBox4.SelectedIndex];
            X_textBox4.Text = b.Rect.X.ToString();
            Y_textBox4.Text = b.Rect.Y.ToString();
            Width_textBox4.Text = b.Rect.Width.ToString();
            Height_textBox4.Text = b.Rect.Height.ToString();
            Border_witdh_textBox4.Text = b.Width.ToString();
            Red_textBox4.Text = b.color.R.ToString();
            Green_textBox4.Text = b.color.G.ToString();
            Blue_textBox4.Text = b.color.B.ToString();
            Alpha_textBox4.Text = b.color.A.ToString();
        }
        #endregion

        private void Debug_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(SaveAllToText);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        [STAThread]
        private void SaveAllToText()
        {
            saveFileDialog1.FileName = "output.txt";
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                    {
                        sw.WriteLine("-------------------------------------------------------");
                        sw.WriteLine("TextBox");
                        sw.WriteLine("-------------------------------------------------------");
                        for (int i = 0; i < Textbox_List.Count; i++)
                        {
                            sw.WriteLine("\t" + listBox1.Items[i] + ":");
                            sw.WriteLine("\tParent Screen:" + Textbox_List[i].Parent.name);
                            sw.WriteLine("\tRectangle:" + Textbox_List[i].Rect.ToString());
                            //sw.WriteLine("\tScrollable:" + Textbox_List[i].scrollable);
                            sw.WriteLine("\t-----------------");
                        }
                        sw.WriteLine("-------------------------------------------------------");
                        sw.WriteLine("ImageButton");
                        sw.WriteLine("-------------------------------------------------------");
                        for (int i = 0; i < ImageButton_List.Count; i++)
                        {
                            sw.WriteLine("\t" + listBox2.Items[i] + ":");
                            sw.WriteLine("\tParent Screen:" + ImageButton_List[i].Parent.name);
                            sw.WriteLine("\tRectangle:" + ImageButton_List[i].Rect.ToString());
                            sw.WriteLine("\t-----------------");
                        }
                        sw.WriteLine("-------------------------------------------------------");
                        sw.WriteLine("Label");
                        sw.WriteLine("-------------------------------------------------------");
                        for (int i = 0; i < Label_List.Count; i++)
                        {
                            sw.WriteLine("\t" + listBox3.Items[i] + ":");
                            sw.WriteLine("\tParent Screen:" + Label_List[i].Parent.name);
                            sw.WriteLine("\tRectangle:" + Label_List[i].Rect.ToString());
                            sw.WriteLine("\tColor:" + Label_List[i].Color.ToString());
                            sw.WriteLine("\t-----------------");
                        }
                        sw.WriteLine("-------------------------------------------------------");
                        sw.WriteLine("Border");
                        sw.WriteLine("-------------------------------------------------------");
                        for (int i = 0; i < Border_List.Count; i++)
                        {
                            sw.WriteLine("\t" + listBox4.Items[i] + ":");
                            sw.WriteLine("\tParent Screen:" + Border_List[i].Parent.name);
                            sw.WriteLine("\tRectangle:" + Border_List[i].Rect.ToString());
                            sw.WriteLine("\tBorder Width:" + Border_List[i].Width);
                            sw.WriteLine("\tColor:" + Border_List[i].color.ToString());
                            sw.WriteLine("\t-----------------");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}