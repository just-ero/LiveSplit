using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Fetze.WinFormsColor
{
    public class ColorShowBox : UserControl
    {
        private Color upperColor = Color.Transparent;
        private Color lowerColor = Color.Transparent;

        public event EventHandler UpperClick = null;
        public event EventHandler LowerClick = null;

        public Color Color
        {
            get { return upperColor; }
            set
            {
                upperColor = lowerColor = value;
                Invalidate();
            }
        }
        public Color UpperColor
        {
            get { return upperColor; }
            set
            {
                upperColor = value;
                Invalidate();
            }
        }
        public Color LowerColor
        {
            get { return lowerColor; }
            set
            {
                lowerColor = value;
                Invalidate();
            }
        }

        public ColorShowBox()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        protected void OnUpperClick()
        {
            if (UpperClick != null)
            {
                UpperClick(this, null);
            }
        }
        protected void OnLowerClick()
        {
            if (LowerClick != null)
            {
                LowerClick(this, null);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Y > (ClientRectangle.Top + ClientRectangle.Bottom) / 2)
            {
                OnLowerClick();
            }
            else
            {
                OnUpperClick();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(new HatchBrush(HatchStyle.LargeCheckerBoard, Color.LightGray, Color.Gray), ClientRectangle);

            e.Graphics.FillRectangle(new SolidBrush(upperColor),
                ClientRectangle.X,
                ClientRectangle.Y,
                ClientRectangle.Width,
                (ClientRectangle.Height / 2) + 1);
            e.Graphics.FillRectangle(new SolidBrush(lowerColor),
                ClientRectangle.X,
                ClientRectangle.Y + ClientRectangle.Height - (ClientRectangle.Height / 2),
                ClientRectangle.Width,
                ClientRectangle.Height / 2);
        }
    }
}
