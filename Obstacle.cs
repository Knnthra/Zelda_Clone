namespace EksamenJuni2010
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Types of Obstacle
    /// </summary>
    public enum ObstacleType
    {
        /// <summary>
        /// Represents a solid collidable Obstacle.
        /// </summary>
        Collision,

        /// <summary>
        /// Represents an Obstacle which teleports on collision.
        /// </summary>
        Teleport,

        /// <summary>
        /// Represents an Obstacle which is interactable.
        /// </summary>
        Dialog
    }

    /// <summary>
    /// Represents a collidable Obstacle of the type GameObject.
    /// </summary>
    public class Obstacle : GameObject
    {
        /// <summary>
        /// Represents this Obstacle tag. Used to search for dialog-responses.
        /// </summary>
        private string myTag;

        /// <summary>
        /// Represents which ObstacleType this is.
        /// </summary>
        private ObstacleType myType;

        /// <summary>
        /// Represents the zone Point-destination to teleport the player to.
        /// </summary>
        private Point zoneDestination;

        /// <summary>
        /// Represents the Pont-location to reposition the player on the screen, after a teleport.
        /// </summary>
        private Point playerRelocation;

        /// <summary>
        /// Represents the current response.
        /// </summary>
        private string activeResponse = string.Empty;

        /// <summary>
        /// Represents all possible responses of this Obstacle.
        /// </summary>
        private List<string> responses = new List<string>();

        /// <summary>
        /// Initializes a new instance of the Obstacle class(Teleport).
        /// </summary>
        /// <param name="posX">The X position of the Obstacle.</param>
        /// <param name="posY">The Y position of the Obstacle.</param>
        /// <param name="width">The width of the Obstacle.</param>
        /// <param name="height">The height of the Obstacle.</param>
        /// <param name="teleDestination">The zone destination of the teleportation.</param>
        /// <param name="newPlayerCoords">The Point position to reposition the player to.</param>
        public Obstacle(int posX, int posY, int width, int height, Point teleDestination, Point newPlayerCoords)
        {
            this.playerRelocation = newPlayerCoords;
            this.zoneDestination = teleDestination;
            this.myType = ObstacleType.Teleport;
            this.mySize = new Size(width, height);
            this.coords = new Point(posX, posY);
        }

        /// <summary>
        /// Initializes a new instance of the Obstacle class(Dialog).
        /// </summary>
        /// <param name="posX">The X position of the Obstacle.</param>
        /// <param name="posY">The Y position of the Obstacle.</param>
        /// <param name="width">The width of the Obstacle.</param>
        /// <param name="height">The height of the Obstacle.</param>
        /// <param name="tag">The tag of the dialog-responses to load.</param>
        public Obstacle(int posX, int posY, int width, int height, string tag)
        {
            this.myTag = tag.ToLower();
            this.myType = ObstacleType.Dialog;
            this.mySize = new Size(width, height);
            this.coords = new Point(posX, posY);
            this.LoadResponses();            
        }

        /// <summary>
        /// Initializes a new instance of the Obstacle class(Collision).
        /// </summary>
        /// <param name="posX">The X position of the Obstacle.</param>
        /// <param name="posY">The Y position of the Obstacle.</param>
        /// <param name="width">The width of the Obstacle.</param>
        /// <param name="height">The height of the Obstacle.</param>
        public Obstacle(int posX, int posY, int width, int height)
        {
            this.myType = ObstacleType.Collision;
            this.mySize = new Size(width, height);
            this.coords = new Point(posX, posY);
        }

        /// <summary>
        /// Gets the current tag of this Obstacle.
        /// </summary>
        public string MyTag
        {
            get { return this.myTag; }
        }

        /// <summary>
        /// Gets the current active reponse.
        /// </summary>
        public string ActiveResponse
        {
            get { return this.activeResponse; }
        }

        /// <summary>
        /// Gets the Point location to relocate the player to.
        /// </summary>
        public Point PlayerRelocation
        {
            get { return this.playerRelocation; }
        }

        /// <summary>
        /// Gets the current ObstacleType.
        /// </summary>
        public ObstacleType MyType
        {
            get { return this.myType; }
        }

        /// <summary>
        /// Gets the current Point zone destination.
        /// </summary>
        public Point ZoneDestination
        {
            get { return this.zoneDestination; }
        }

        /// <summary>
        /// Sets the activeResponse, randomly, from one of the valid responses.
        /// </summary>
        /// <param name="rnd">An already instansiated, random-number generator.</param>
        public void RandomizeResponse(Random rnd)
        {
            this.activeResponse = this.responses[rnd.Next(0, this.responses.Count())];
        }        

        /// <summary>
        /// Loads all responses marked with myTag, from the dialogresponses.txt.
        /// </summary>
        private void LoadResponses()
        {
            foreach (string line in File.ReadAllLines("dialogresponses.txt"))
            {
                if (line.Split(';')[0] == this.myTag)
                {
                    List<char> chars = new List<char>(line.Split(';')[1].ToCharArray());
                    this.responses.Add(this.WrapLine(ref chars));
                }
            }
        }

        /// <summary>
        /// Wraps a line, inserting a line-break each 40th char.
        /// </summary>
        /// <param name="chars">The list of chars, representing a line, which is to be wrapped.</param>
        /// <returns>A string, representing the line, now included with line-breaks.</returns>
        private string WrapLine(ref List<char> chars)
        {
            int charLimit = 40;
            StringBuilder lineBuilder = new StringBuilder();

            if (chars.Count() > charLimit)
            {
                for (int i = charLimit; i != 0; i--)
                {
                    if (chars[i] == ' ')
                    {
                        List<char> tmpline = new List<char>();

                        tmpline.AddRange(chars.GetRange(0, i)); // Add first part of text
                        tmpline.AddRange(Environment.NewLine.ToCharArray()); // Add new line

                        List<char> lastPart = chars.GetRange(i + 1, chars.Count() - (i + 1));
                        if (lastPart.Count() > charLimit)
                        {
                            this.WrapLine(ref lastPart); // Wrap last part of text(recursive)
                        }

                        tmpline.AddRange(lastPart); // Add last part of text

                        chars.Clear();
                        chars.AddRange(tmpline); // Add complete wrapped text

                        break;
                    }
                }
            }

            chars.ForEach(x => lineBuilder.Append(x));
            return lineBuilder.ToString();
        }
    }
}