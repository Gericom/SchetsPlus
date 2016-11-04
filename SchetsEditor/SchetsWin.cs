using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using SchetsEditor.DrawingObjects;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {
        MenuStrip menuStrip;
        SchetsControl sketchControl;
        ISchetsTool currentTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        Dictionary<ISchetsTool, RadioButton> mRadioButtons;
        private void sizeChange(object o, EventArgs ea)
        {
            sketchControl.Size = new Size(this.ClientSize.Width - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void clickToolMenu(object obj, EventArgs ea)
        {
            var tool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
            this.currentTool = tool;
            mRadioButtons[tool].Select();
        }

        private void clickToolButton(object obj, EventArgs ea)
        {
            this.currentTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void closePanel(object obj, EventArgs ea)
        {
            this.Close();
        }

        public SchetsWin()
        {
            InitializeComponent();
            ISchetsTool[] mTools = { new PenTool()
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new EllipseTool()
                                    , new FillEllipse()
                                    , new TekstTool()
                                    , new GumTool()
                                    };

            //we can add the colour palette to this or extend the color array?
            String[] mColours = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan","Brown","Teal" 
                                 };

            this.ClientSize = new Size(700, 500);
            currentTool = mTools[0];

            sketchControl = new SchetsControl();
            sketchControl.Location = new Point(64, 10);
            sketchControl.MouseDown += (object o, MouseEventArgs mea) =>
                                       {
                                           vast = true;
                                           currentTool.MouseDown(sketchControl, mea.Location);
                                       };
            sketchControl.MouseMove += (object o, MouseEventArgs mea) =>
                                       {
                                           if (vast)
                                               currentTool.MouseDrag(sketchControl, mea.Location);
                                       };
            sketchControl.MouseUp += (object o, MouseEventArgs mea) =>
                                     {
                                         if (vast)
                                             currentTool.MouseUp(sketchControl, mea.Location);
                                         vast = false;
                                     };
            sketchControl.KeyPress += (object o, KeyPressEventArgs kpea) =>
                                      {
                                          currentTool.Letter(sketchControl, kpea.KeyChar);
                                      };
            this.Controls.Add(sketchControl);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.makeFileMenu();
            this.makeToolMenu(mTools);
            this.makeActionMenu(mColours);
            this.makeToolButtons(mTools);
            this.makeActionButtons(mColours);
            this.Resize += this.sizeChange;
            this.sizeChange(null, null);
        }

        public void LoadBitmap(Bitmap b)
        {
            sketchControl.Schets.VeranderAfmeting(b.Size);
            sketchControl.Schets.BeginAddObject(new BitmapObject(b, false));
            sketchControl.Schets.EndAddObject();
        }

        private void makeFileMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            var it = menu.DropDownItems.Add("Save", null, Save);
            it.MergeIndex = 4;
            it.MergeAction = MergeAction.Insert;
            //it = menu.DropDownItems.Add("Save As");
            //it.MergeIndex = 5;
            //it.MergeAction = MergeAction.Insert;
            it = menu.DropDownItems.Add("-");
            it.MergeIndex = 5;
            it.MergeAction = MergeAction.Insert;
            it = menu.DropDownItems.Add("Close", null, this.closePanel);
            it.MergeIndex = 6;
            it.MergeAction = MergeAction.Insert;
            it = menu.DropDownItems.Add("-");
            it.MergeIndex = 7;
            it.MergeAction = MergeAction.Insert;
            menuStrip.Items.Add(menu);
        }

        private void Save(object o, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "PNG Images (*.png)|*.png|JPEG Images (*.jpg)|*.jpg|BMP Images (*.bmp)|*.bmp";
            if (f.ShowDialog() == DialogResult.OK && f.FileName.Length > 0)
            {
                switch (f.FilterIndex)
                {
                    case 1:
                        sketchControl.Schets.ToBitmap().Save(f.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case 2:
                        sketchControl.Schets.ToBitmap().Save(f.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 3:
                        sketchControl.Schets.ToBitmap().Save(f.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
            }
        }

        private void makeToolMenu(ICollection<ISchetsTool> tools)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Tools") { MergeIndex = 1, MergeAction = MergeAction.Insert };
            foreach (ISchetsTool tool in tools)
            {
                ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.clickToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void makeActionMenu(String[] kleuren)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Action") { MergeIndex = 2, MergeAction = MergeAction.Insert };
            menu.DropDownItems.Add("Clear", null, sketchControl.Clear);
            menu.DropDownItems.Add("Rotate", null, sketchControl.Rotate);
            ToolStripMenuItem submenu = new ToolStripMenuItem("Colors");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, sketchControl.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
        }

        private void makeToolButtons(ICollection<ISchetsTool> tools)
        {
            mRadioButtons = new Dictionary<ISchetsTool, RadioButton>();
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.clickToolButton;
                this.Controls.Add(b);
                mRadioButtons.Add(tool, b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void makeActionButtons(String[] kleuren)
        {
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);

            Button b; Label l; ComboBox cbb;
            b = new Button();
            b.Text = "Clear";
            b.Location = new Point(0, 0);
            b.Click += sketchControl.Clear;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Rotate";
            b.Location = new Point(80, 0);
            b.Click += sketchControl.Rotate;
            paneel.Controls.Add(b);

            l = new Label();
            l.Text = "Pen Colour:";
            l.Location = new Point(180, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);

            cbb = new ComboBox(); cbb.Location = new Point(240, 0);
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += sketchControl.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchetsWin));
            this.SuspendLayout();
            // 
            // SchetsWin
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SchetsWin";
            this.ResumeLayout(false);

        }
    }
}
