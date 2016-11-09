using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        public Hoofdscherm()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "Schets++";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }
        private void maakFileMenu()
        {
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("New", null, this.nieuw).MergeIndex = 0;
            menu.DropDownItems.Add("-").MergeIndex = 1;
            menu.DropDownItems.Add("Open", null, Open).MergeIndex = 2;
            menu.DropDownItems.Add("-").MergeIndex = 3;
            menu.DropDownItems.Add("Exit", null, this.afsluiten).MergeIndex = 8;
            menuStrip.Items.Add(menu);
        }

        private void Open(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "Images (*.sppp;*.png;*.jpg;*.bmp)|*.sppp;*.png;*.jpg;*.bmp";
            if (f.ShowDialog() == DialogResult.OK && f.FileName.Length > 0)
            {
                SchetsWin s = new SchetsWin();
                if (Path.GetExtension(f.FileName) == ".sppp")
                    s.LoadProject(File.ReadAllBytes(f.FileName));
                else
                    s.LoadBitmap(new Bitmap(new MemoryStream(File.ReadAllBytes(f.FileName))));
                s.MdiParent = this;
                s.Show();
            }
        }

        private void maakHelpMenu()
        {
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("About", null, this.about);
            menuStrip.Items.Add(menu);
        }
        private void about(object o, EventArgs ea)
        {
            MessageBox.Show("Schets versie 2.0\n(c) UU Informatica 2016"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        private void nieuw(object sender, EventArgs e)
        {
            SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show();
        }
        private void afsluiten(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Hoofdscherm));
            this.SuspendLayout();
            // sky
            // Hoofdscherm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Hoofdscherm";
            this.ResumeLayout(false);

        }
    }
}
