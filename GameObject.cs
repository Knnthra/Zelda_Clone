namespace EksamenJuni2010
{
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// An abstract class that represents a GameObject.
    /// </summary>
    public abstract class GameObject
    {
        /// <summary>
        /// Represents the character used to seperate directory levels.
        /// </summary>
        protected static char dirSeparator = Path.DirectorySeparatorChar;

        /// <summary>
        /// Represents the GameObjects Size.
        /// </summary>
        protected Size mySize;

        /// <summary>
        /// Represents the visual representation of the GameObject.
        /// </summary>
        protected Image picture;

        /// <summary>
        /// Represents the Point location of the GameObject.
        /// </summary>
        protected Point coords;

        /// <summary>
        /// Gets or sets the GameObjects Point location.
        /// </summary>
        public Point Coords
        {
            get { return this.coords; }
            set { this.coords = value; }
        }

        /// <summary>
        /// An overrideable method. Creates a rectangle based on the GameObjects location and size.
        /// </summary>
        /// <returns>A Rectangle based on the GameObjects location and size.</returns>
        public virtual Rectangle MyRectangle()
        {
            return new Rectangle(this.coords.X, this.coords.Y, this.mySize.Width, this.mySize.Height);
        }

        /// <summary>
        /// Renders the Image of the GameObject to the Graphics controller.
        /// </summary>
        /// <param name="dc">Graphics controller.</param>
        public virtual void Render(Graphics dc)
        {
            dc.DrawImage(this.picture, new Rectangle(new Point(this.coords.X - (this.picture.Width / 2), this.coords.Y - (this.picture.Height / 2)), this.picture.Size));
        }

        /// <summary>
        /// Writes GameObject specific attributes to the Xml writer.
        /// </summary>
        /// <param name="writer">An XmlWriter to write attributes to.</param>
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("coordsX", this.coords.X.ToString());
            writer.WriteAttributeString("coordsY", this.coords.Y.ToString());
        }

        /// <summary>
        /// Reads GameObject specific attributes from the Xml reader.
        /// </summary>
        /// <param name="reader">An XmlReader to read the attributes from.</param>
        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            this.coords.X = int.Parse(reader.GetAttribute("coordsX"));
            this.coords.Y = int.Parse(reader.GetAttribute("coordsY"));
        }
    }
}