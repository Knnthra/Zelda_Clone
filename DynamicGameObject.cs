namespace EksamenJuni2010
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents an dynamic, animated and movable DynamicGameObject.
    /// </summary>
    public class DynamicGameObject : GameObject
    {
        /// <summary>
        /// Represents the damage this DynamicGameObject can deal.
        /// </summary>
        protected int damage = 10;

        /// <summary>
        /// Represents the block(percent) chance this DynamicGameObject has.
        /// </summary>
        protected int block = 5;

        /// <summary>
        /// Represents the crit(percent) chance this DynamicGameObject has.
        /// </summary>
        protected int crit = 2;

        /// <summary>
        /// Indicates whether this GameObject should attack.
        /// </summary>
        protected bool atkMelee = false;

        /// <summary>
        /// Represents the current health of the DynamicGameObject.
        /// </summary>
        protected int health = 100;

        /// <summary>
        /// Represents the current direction the DynamicGameObject is facing.
        /// </summary>
        protected EnumDirection lookDirection = EnumDirection.Down;

        /// <summary>
        /// Represents the current direction the DynamicGameObject is moving.
        /// </summary>
        protected EnumDirection moveDirection = EnumDirection.Down;

        /// <summary>
        /// Indicates whether this DynamicGameObject is ready to attack.
        /// </summary>
        private bool atkRdy = true;

        /// <summary>
        /// Represents the current image-number to display.
        /// </summary>
        private float imgNr = 0;

        /// <summary>
        /// Represents all run Images.
        /// </summary>
        private Image[][] runSprites = new Image[4][];

        /// <summary>
        /// Represents all attack Images.
        /// </summary>
        private Image[][] atkSprites = new Image[4][];

        /// <summary>
        /// Initializes a new instance of the DynamicGameObject class. Loading run and attack sprites.
        /// </summary>
        public DynamicGameObject()
        {
            this.LoadSprites();
            picture = this.runSprites[0][0];
        }

        /// <summary>
        /// Gets or sets the current moving direction.
        /// </summary>
        public EnumDirection MoveDirection
        {
            get { return this.moveDirection; }
            set { this.moveDirection = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the DynamicGameObject is ready to attack.
        /// </summary>
        public bool AtkRdy
        {
            get { return this.atkRdy; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DynamicGameObject should attack.
        /// </summary>
        public bool AtkMelee
        {
            get { return this.atkMelee; }
            set { this.atkMelee = value; }
        }

        /// <summary>
        /// Gets the current health of this DynamicGameObject.
        /// </summary>
        public int Health
        {
            get { return this.health; }
        }

        /// <summary>
        /// Animates the DynamicGameObject. Looping Images based on the direction.
        /// </summary>
        /// <param name="gameSpeed">The current speed of the game.</param>
        public virtual void Animate(int gameSpeed)
        {
            EnumDirection tmpDir = this.moveDirection;
            this.lookDirection = (this.moveDirection != EnumDirection.None && this.atkRdy) ? tmpDir : this.lookDirection;

            #region ConvertEnumToArrayNr
            byte directionNr = 0;
            switch (this.lookDirection)
            {
                case EnumDirection.Left:
                    directionNr = 2;
                    break;
                case EnumDirection.LeftUp:
                    directionNr = 0;
                    break;
                case EnumDirection.LeftDown:
                    directionNr = 1;
                    break;
                case EnumDirection.Right:
                    directionNr = 3;
                    break;
                case EnumDirection.RightUp:
                    directionNr = 0;
                    break;
                case EnumDirection.RightDown:
                    directionNr = 1;
                    break;
                case EnumDirection.Up:
                    directionNr = 0;
                    break;
                case EnumDirection.Down:
                    directionNr = 1;
                    break;
            }
            #endregion

            if (this.atkMelee == true)
            {
                // Melee animation
                this.imgNr = this.atkRdy ? 0 : this.imgNr;
                this.atkRdy = false;
                picture = this.atkSprites[directionNr][(int)this.imgNr];
                this.imgNr += 0.3f * gameSpeed;
                if (this.imgNr >= this.atkSprites[directionNr].Count() || this.imgNr < 0)
                {
                    this.imgNr = 0;
                    this.atkMelee = false;
                    this.atkRdy = true;
                }
            }
            else
            {
                // Run or standstill animation
                this.imgNr = (this.imgNr >= this.runSprites[directionNr].Count()) || (this.moveDirection == EnumDirection.None || this.imgNr < 0) ? 0 : this.imgNr;
                picture = this.runSprites[directionNr][(int)this.imgNr];
                this.imgNr += 0.2f * gameSpeed;
            }
        }

        /// <summary>
        /// Creates a Rectangle based on the location and a predefined size.
        /// </summary>
        /// <returns>A Rectangle based on the location and a predefined size.</returns>
        public override Rectangle MyRectangle()
        {
            return new Rectangle(this.coords.X - 16, this.coords.Y + 10, 32, 12);
        }

        /// <summary>
        /// Moves the DynamicGameObject based on the current moveDirection.
        /// </summary>
        /// <param name="speed">The speed of the DynamicGameObject.</param>
        public void HandleInput(int speed)
        {
            speed = this is Player ? speed + (speed / 2) : speed;

            if (!this.atkMelee)
            {
                switch (this.moveDirection)
                {
                    case EnumDirection.Left:
                        coords.X -= speed;
                        break;
                    case EnumDirection.LeftUp:
                        coords.X -= speed;
                        coords.Y -= speed;
                        break;
                    case EnumDirection.LeftDown:
                        coords.X -= speed;
                        coords.Y += speed;
                        break;
                    case EnumDirection.Right:
                        coords.X += speed;
                        break;
                    case EnumDirection.RightUp:
                        coords.X += speed;
                        coords.Y -= speed;
                        break;
                    case EnumDirection.RightDown:
                        coords.X += speed;
                        coords.Y += speed;
                        break;
                    case EnumDirection.Up:
                        coords.Y -= speed;
                        break;
                    case EnumDirection.Down:
                        coords.Y += speed;
                        break;
                }
            }
        }
        
        /// <summary>
        /// Calculates and applies the damage of an attack(Based on this' damage, block and crit values).
        /// </summary>
        /// <param name="victim">The DynamicGameObject which health is going to be reduced.</param>
        /// <param name="rnd">An already instansiated, random-number generator.</param>
        /// <returns>A new instance of the ScrollingCombatText class, based upon calculated attack.</returns>
        public ScrollingCombatText CalcAndApplyDamage(DynamicGameObject victim, Random rnd)
        {
            int dmg = this is Player ? rnd.Next(this.damage / 3, this.damage) : rnd.Next(1, this.damage);

            string combatText = dmg.ToString();
            SctType textType = SctType.Normal;

            if (rnd.Next(0, 101) <= victim.block)
            {
                combatText = "Block";
                textType = SctType.Block;
            }
            else if (rnd.Next(0, 101) <= this.crit)
            {
                dmg *= 3;
                combatText = dmg.ToString();
                victim.health = (victim.health - dmg) <= 0 ? 0 : victim.health - dmg;
                textType = SctType.Critical;
            }
            else
            {
                victim.health = (victim.health - dmg) <= 0 ? 0 : victim.health - dmg;
            }

            return new ScrollingCombatText(victim.coords, combatText, textType);
        }

        /// <summary>
        /// Writes DynamicGameObject specific attributes to the Xml writer
        /// </summary>
        /// <param name="writer">An XmlWriter to write attributes to.</param>
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("Health", this.health.ToString());
        }

        /// <summary>
        /// Reads DynamicGameObject specific attributes from the Xml reader
        /// </summary>
        /// <param name="reader">An XmlReader to read the attributes from.</param>
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            this.health = int.Parse(reader.GetAttribute("Health"));
        }

        /// <summary>
        /// Based on the type of DynamicGameObject, loads all the run and attack Images into two lists.
        /// </summary>
        private void LoadSprites()
        {
            List<Image> tmp_RunLeft = new List<Image>();
            List<Image> tmp_RunRight = new List<Image>();
            List<Image> tmp_RunFront = new List<Image>();
            List<Image> tmp_RunUp = new List<Image>();

            List<Image> tmp_AtkLeft = new List<Image>();
            List<Image> tmp_AtkRight = new List<Image>();
            List<Image> tmp_AtkFront = new List<Image>(10);
            List<Image> tmp_AtkUp = new List<Image>();

            foreach (string fileDir in Directory.GetFiles(string.Format("graphics{0}sprites{0}", dirSeparator)))
            {
                string fileName = Path.GetFileName(fileDir);
                string[] spriteTags = fileName.Split('_');

                if ((spriteTags[0] == "player" && this is Player) || (spriteTags[0] == "skelet" && this is Enemy))
                {
                    if (spriteTags[1] == "atk")
                    {
                        switch (spriteTags[2])
                        {
                            case "front":
                                tmp_AtkFront.Add(Image.FromFile(string.Format("graphics{0}sprites{0}", dirSeparator) + fileName));
                                break;
                            case "bk":
                                tmp_AtkUp.Add(Image.FromFile(string.Format("graphics{0}sprites{0}", dirSeparator) + fileName));
                                break;
                            case "left":
                                tmp_AtkLeft.Add(Image.FromFile(string.Format("graphics{0}sprites{0}", dirSeparator) + fileName));
                                break;
                            case "right":
                                tmp_AtkRight.Add(Image.FromFile(string.Format("graphics{0}sprites{0}", dirSeparator) + fileName));
                                break;
                        }
                    }
                    else
                    {
                        switch (spriteTags[2])
                        {
                            case "front":
                                tmp_RunFront.Add(Image.FromFile(string.Format("graphics{0}sprites{0}", dirSeparator) + fileName));
                                break;
                            case "bk":
                                tmp_RunUp.Add(Image.FromFile(string.Format("graphics{0}sprites{0}", dirSeparator) + fileName));
                                break;
                            case "left":
                                tmp_RunLeft.Add(Image.FromFile(string.Format("graphics{0}sprites{0}", dirSeparator) + fileName));
                                break;
                            case "right":
                                tmp_RunRight.Add(Image.FromFile(string.Format("graphics{0}sprites{0}", dirSeparator) + fileName));
                                break;
                        }
                    }
                }
            }

            this.runSprites[0] = tmp_RunUp.ToArray();
            this.runSprites[1] = tmp_RunFront.ToArray();
            this.runSprites[2] = tmp_RunLeft.ToArray();
            this.runSprites[3] = tmp_RunRight.ToArray();

            this.atkSprites[0] = tmp_AtkUp.ToArray();
            this.atkSprites[1] = tmp_AtkFront.ToArray();
            this.atkSprites[2] = tmp_AtkLeft.ToArray();
            this.atkSprites[3] = tmp_AtkRight.ToArray();
        }
    }
}