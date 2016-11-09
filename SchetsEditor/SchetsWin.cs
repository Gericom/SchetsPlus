using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using SchetsEditor.DrawingObjects;
using System.IO;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {
        MenuStrip menuStrip;
        SchetsControl schetsControl;
        ISchetsTool currentTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        Dictionary<ISchetsTool, ToolStripButton> mRadioButtons;
        private void sizeChange(object o, EventArgs ea)
        {
            schetsControl.Size = new Size(this.ClientSize.Width - 80
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64 + 10, this.ClientSize.Height - 30);
        }

        private void clickToolMenu(object obj, EventArgs ea)
        {
            var tool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
            this.currentTool = tool;
            mRadioButtons[tool].Select();
        }

        private void clickToolButton(object obj, EventArgs ea)
        {
            foreach (ToolStripButton b in mRadioButtons.Values)
                if (b != obj)
                    b.Checked = false;
            this.currentTool = (ISchetsTool)((ToolStripButton)obj).Tag;
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
                                    , new GumTool(),
                                    new MoveUpTool(),
                                    new MoveDownTool(),
                                    new DragTool()
                                    };

            this.ClientSize = new Size(800, 625);
            currentTool = mTools[0];

            schetsControl = new SchetsControl();
            schetsControl.Location = new Point(64 + 10, 10);
            schetsControl.MouseDown +=
                (o, mea) =>
                {
                    vast = true;
                    currentTool.MouseDown(schetsControl, mea.Location);
                };
            schetsControl.MouseMove += 
                (o, mea) =>
                {
                    if (vast)
                        currentTool.MouseDrag(schetsControl, mea.Location);
                };
            schetsControl.MouseUp += 
                (o, mea) =>
                {
                    if (vast)
                        currentTool.MouseUp(schetsControl, mea.Location);
                    vast = false;
                };
            schetsControl.KeyPress += 
                (o, kpea) =>
                {
                    currentTool.Letter(schetsControl, kpea.KeyChar);
                };
            this.Controls.Add(schetsControl);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.makeFileMenu();
            this.makeToolMenu(mTools);
            this.makeActionMenu();
            this.makeToolButtons(mTools);
            this.makeActionButtons();
            this.Resize += this.sizeChange;
            this.sizeChange(null, null);
            FormClosing += SchetsWin_FormClosing;
            schetsControl.Schets.AcknowledgeChanges();
        }

        private void SchetsWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (schetsControl.Schets.HasUnsavedChanges)
            {
                if (MessageBox.Show("Are you sure you want to close without saving?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                     == DialogResult.No)
                    e.Cancel = true;
            }
        }

        public void LoadBitmap(Bitmap b)
        {
            schetsControl.Schets.VeranderAfmeting(b.Size);
            schetsControl.Schets.BeginAddObject(new BitmapObject(b, false));
            schetsControl.Schets.EndAddObject();
        }

        public void LoadProject(byte[] data)
        {
            schetsControl.Schets.Read(data);
        }

        private void makeFileMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            var it = menu.DropDownItems.Add("Save", null, Save);
            it.MergeIndex = 4;
            it.MergeAction = MergeAction.Insert;
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
            f.Filter = "Schets++ Project(*.sppp)|*.sppp|PNG Images (*.png)|*.png|JPEG Images (*.jpg)|*.jpg|BMP Images (*.bmp)|*.bmp";
            if (f.ShowDialog() == DialogResult.OK && f.FileName.Length > 0)
            {
                switch (f.FilterIndex)
                {
                    case 1:
                        byte[] data = schetsControl.Schets.Write();
                        File.Create(f.FileName).Close();
                        File.WriteAllBytes(f.FileName, data);
                        break;
                    case 2:
                        schetsControl.Schets.ToBitmap().Save(f.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case 3:
                        schetsControl.Schets.ToBitmap().Save(f.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 4:
                        schetsControl.Schets.ToBitmap().Save(f.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
                schetsControl.Schets.AcknowledgeChanges();
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

        private void makeActionMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Action") { MergeIndex = 2, MergeAction = MergeAction.Insert };
            menu.DropDownItems.Add("Clear", null, schetsControl.Clear);
            menu.DropDownItems.Add("Rotate", null, schetsControl.Rotate);
            menu.DropDownItems.Add("Undo", null, schetsControl.Undo);
            menu.DropDownItems.Add("Redo", null, schetsControl.Redo);
            menu.DropDownItems.Add("Choose Color", null, MColorButton_Click);
            menuStrip.Items.Add(menu);
        }

        private void makeToolButtons(ICollection<ISchetsTool> tools)
        {
            mRadioButtons = new Dictionary<ISchetsTool, ToolStripButton>();
            ToolStrip s = new ToolStrip();
            s.Dock = DockStyle.Left;
            s.ImageScalingSize = new Size(32, 32);
            int i = 0;
            foreach (ISchetsTool tool in tools)
            {
                var tb = new ToolStripButton(
                        tool.ToString(),
                        (Image)resourcemanager.GetObject(tool.ToString()),
                        clickToolButton)
                {
                    Tag = tool,
                    TextImageRelation = TextImageRelation.ImageAboveText,
                    CheckOnClick = true,
                    Checked = i == 0
                };
                s.Items.Add(tb);
                mRadioButtons.Add(tool, tb);
                i++;
            }
            this.Controls.Add(s);
        }

        Button mColorButton;

        private void makeActionButtons()
        {
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);

            Button b; Label l, q;
            b = new Button();
            b.Text = "Clear";
            b.Location = new Point(0, 0);
            b.Click += schetsControl.Clear;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Rotate";
            b.Location = new Point(80, 0);
            b.Click += schetsControl.Rotate;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Undo";
            b.Location = new Point(160, 0);
            b.Click += schetsControl.Undo;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Redo";
            b.Location = new Point(240, 0);
            b.Click += schetsControl.Redo;
            paneel.Controls.Add(b);

            q = new Label();
            q.Text = "Pen Size:";
            q.Location = new Point(320, 3);
            q.AutoSize = true;
            paneel.Controls.Add(q);

            NumericUpDown h = new NumericUpDown();
            h.Location = new Point(380, 0);
            h.Size = new Size(50, 10);
            h.Minimum = 1;
            h.Maximum = 50;
            h.Value = 3;
            schetsControl.PenSize = 3;
            h.ValueChanged += (d, f) => { schetsControl.PenSize = (int)h.Value; };
            paneel.Controls.Add(h);

            l = new Label();
            l.Text = "Pen Colour:";
            l.Location = new Point(440, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);

            mColorButton = new Button();
            mColorButton.Location = new Point(510, 0);
            mColorButton.Text = "";
            mColorButton.BackColor = Color.Black;
            schetsControl.PenColor = Color.Black;
            mColorButton.Click += MColorButton_Click;
            paneel.Controls.Add(mColorButton);
        }

        private void MColorButton_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog();
            cd.Color = schetsControl.PenColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                schetsControl.PenColor = cd.Color;
                mColorButton.BackColor = cd.Color;
            }
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
