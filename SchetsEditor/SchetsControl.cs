using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class SchetsControl : UserControl
    {
        private Schets schets;
        //private Color pencolour;
        //private Color[] palette;


        public Color penColor { get; set; }
        //{
       //     get { return pencolour; }
       // }
        public Schets Schets
        {
            get { return schets; }
        }
        public SchetsControl()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
            DoubleBuffered = true;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }
        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }
        public void Clear(object o, EventArgs ea)
        {
            schets.Clear();
            this.Invalidate();
        }
        public void Rotate(object o, EventArgs ea)
        {
            schets.Rotate();
            this.Invalidate();
        }
        public void Undo(object o, EventArgs ea)
        {
            schets.Undo();
            this.Invalidate();
        }

        public void Redo(object o, EventArgs ea)
        {
            schets.Redo();
            this.Invalidate();
        }
        public void VeranderKleur(object obj, EventArgs ea)
        {
            string colourName = ((ComboBox)obj).Text;
            penColor = Color.FromName(colourName);
        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            string colourName = ((ToolStripMenuItem)obj).Text;
            penColor = Color.FromName(colourName);
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SchetsControl
            // 
            this.DoubleBuffered = true;
            this.Name = "SchetsControl";
            this.ResumeLayout(false);
        }
    }
}
