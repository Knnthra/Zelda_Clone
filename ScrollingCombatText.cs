namespace EksamenJuni2010
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    /// <summary>
    /// Types of ScrollingCombatText.
    /// </summary>
    public enum SctType
    {   
        /// <summary>
        /// Represents text with white font and normal size.
        /// </summary>
        Normal,

        /// <summary>
        /// Represents text with enlarged font and different color.
        /// </summary>
        Critical,

        /// <summary>
        /// Represents text with enlarged font and different color.
        /// </summary>
        Block
    }

    /// <summary>
    /// Represents an animated text string.
    /// </summary>
    public class ScrollingCombatText
    {
        /// <summary>
        /// Represents the Point-location of the ScrollingComatText
        /// </summary>
        private Point coords;
        
        /// <summary>
        /// Represents the text-string
        /// </summary>
        private string text;
        
        /// <summary>
        /// Represents a DateTime, when the ScrollingCombatText should start fading away.
        /// </summary>
        private DateTime startFade;
        
        /// <summary>
        /// Represents the text-size
        /// </summary>
        private int textSize = 20;
        
        /// <summary>
        /// Represents which type of ScrollingCombatText this is.
        /// </summary>
        private SctType myType;
        
        /// <summary>
        /// Represents the color of the text.
        /// </summary>
        private Color textColor;

        /// <summary>
        /// Initializes a new instance of the ScrollingCombatText class, based upon the given parameters.
        /// </summary>
        /// <param name="p">The Point location of the ScrollingCombatText</param>
        /// <param name="txt">The string text of the ScrollingCombatText</param>
        /// <param name="type">The SctType type of the ScrollingCombatText</param>
        public ScrollingCombatText(Point p, string txt, SctType type)
        {
            this.myType = type;
            this.startFade = DateTime.Now.AddSeconds(1);
            this.coords.X = p.X - 10;
            this.coords.Y = p.Y - 45;
            this.text = txt;

            switch (this.myType)
            {
                case SctType.Normal:
                    this.textColor = Color.White;
                    break;
                case SctType.Critical:
                    this.textColor = Color.Yellow;
                    this.textSize = 30;
                    this.coords.X -= 5;
                    this.startFade.AddSeconds(1);
                    break;
                case SctType.Block:
                    this.coords.X -= 15;
                    this.textColor = Color.Red;
                    break;
            }
        }

        /// <summary>
        /// Gets the current text-string.
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        /// <summary>
        /// Gets the current font-color.
        /// </summary>
        public Color TextColor
        {
            get { return this.textColor; }
        }

        /// <summary>
        /// Renders the ScrollingCombatText to the Graphics controller.
        /// </summary>
        /// <param name="dc">Graphics controller</param>
        public void Render(Graphics dc)
        {
            GraphicsPath outLine = new GraphicsPath();
            outLine.AddString(this.text, new FontFamily("Arial"), (int)FontStyle.Bold, this.textSize, this.coords, StringFormat.GenericDefault);

            dc.SmoothingMode = SmoothingMode.HighQuality;
            dc.FillPath(new SolidBrush(this.textColor), outLine);
            dc.DrawPath(new Pen(Color.FromArgb((int)this.textColor.A, Color.Black)), outLine);
        }

        /// <summary>
        /// Animates the ScrollingCombatText. Moving and fading it.
        /// </summary>
        public void Animate()
        {
            if (this.startFade < DateTime.Now)
            {
                this.textColor = (this.textColor.A >= 5) ? Color.FromArgb(this.textColor.A - 50, this.textColor.R, this.textColor.G, this.textColor.B) : this.textColor;
            }

            if (this.myType != SctType.Critical)
            {
                this.coords.Y -= 2;
            }
        }
    }
}