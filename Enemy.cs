namespace EksamenJuni2010
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Represents an Enemy, of the type DynamicGameObject.
    /// </summary>
    public class Enemy : DynamicGameObject
    {
        /// <summary>
        /// Represents a list of sides that collides.
        /// </summary>
        private List<EnumDirection> collPosition = new List<EnumDirection>();

        /// <summary>
        /// Represents a list of pre-validated moving directions.
        /// </summary>
        private List<EnumDirection> preValidDirection = new List<EnumDirection>();

        /// <summary>
        /// Represents a list of valid moving directions.
        /// </summary>
        private List<EnumDirection> postValidDirection = new List<EnumDirection>();

        /// <summary>
        /// Represents the offset between this Enemy and the Player.
        /// </summary>
        private Point offset;

        /// <summary>
        /// Initializes a new instance of the Enemy class.
        /// </summary>
        /// <param name="posX">The start x-position of the Enemy.</param>
        /// <param name="posY">The start y-position of the Enemy.</param>
        public Enemy(int posX, int posY)
        {
            coords = new Point(posX, posY);
        }

        /// <summary>
        /// Gets the list of sides that collides.
        /// </summary>
        public List<EnumDirection> CollPosition
        {
            get { return this.collPosition; }
        }

        /// <summary>
        /// Sets moveDirection to a valid direction, based on the Player's current position.
        /// </summary>
        /// <param name="player">The Player</param>
        public void DetermineDirection(DynamicGameObject player)
        {
            if (player.Health <= 0)
            {
                // If player is dead, dont attack + stand still.
                atkMelee = false;
                moveDirection = EnumDirection.None;
                return;
            }

            int meleeRange = 10;
            this.ProcessValidDirections();
            bool iamColliding = this.collPosition.Count != 0 ? true : false;

            this.offset = new Point(player.Coords.X - coords.X, player.Coords.Y - coords.Y);

            #region DetermineDirection
            moveDirection = this.offset.X < -meleeRange ? EnumDirection.Left : moveDirection;
            moveDirection = this.offset.X > meleeRange ? EnumDirection.Right : moveDirection;
            if (this.offset.Y < -meleeRange)
            {
                moveDirection = EnumDirection.Up;
                moveDirection = this.offset.Y < -meleeRange && this.offset.X > meleeRange ? EnumDirection.RightUp : moveDirection;
                moveDirection = this.offset.Y < -meleeRange && this.offset.X < -meleeRange ? EnumDirection.LeftUp : moveDirection;
            }

            if (this.offset.Y > meleeRange)
            {
                moveDirection = EnumDirection.Down;
                moveDirection = this.offset.Y > meleeRange && this.offset.X > meleeRange ? EnumDirection.RightDown : moveDirection;
                moveDirection = this.offset.Y > meleeRange && this.offset.X < -meleeRange ? EnumDirection.LeftDown : moveDirection;
            }

            // Set atkMelee to true if we're in range, otherwise do nothing
            atkMelee = ((Math.Abs(this.offset.X) <= meleeRange * 5) && (Math.Abs(this.offset.Y) <= meleeRange * 4)) ? true : atkMelee;
            #endregion

            if (iamColliding && !this.postValidDirection.Contains(moveDirection))
            {
                // We're collision, and dirction is invalid
                #region AlternativeDirection
                EnumDirection tmpDir = moveDirection;
                moveDirection = EnumDirection.None;

                switch (tmpDir)
                {
                    case EnumDirection.LeftUp:
                        moveDirection = this.postValidDirection.Contains(EnumDirection.RightUp) ? EnumDirection.RightUp : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.LeftDown) ? EnumDirection.LeftDown : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.Left) ? EnumDirection.Left : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.Up) ? EnumDirection.Up : moveDirection;
                        break;
                    case EnumDirection.LeftDown:
                        moveDirection = this.postValidDirection.Contains(EnumDirection.LeftUp) ? EnumDirection.LeftUp : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.RightDown) ? EnumDirection.RightDown : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.Left) ? EnumDirection.Left : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.Down) ? EnumDirection.Down : moveDirection;
                        break;
                    case EnumDirection.RightUp:
                        moveDirection = this.postValidDirection.Contains(EnumDirection.RightDown) ? EnumDirection.RightDown : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.LeftUp) ? EnumDirection.LeftUp : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.Right) ? EnumDirection.Right : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.Up) ? EnumDirection.Up : moveDirection;
                        break;
                    case EnumDirection.RightDown:
                        moveDirection = this.postValidDirection.Contains(EnumDirection.RightUp) ? EnumDirection.RightUp : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.LeftDown) ? EnumDirection.LeftDown : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.Right) ? EnumDirection.Right : moveDirection;
                        moveDirection = this.postValidDirection.Contains(EnumDirection.Down) ? EnumDirection.Down : moveDirection;
                        break;
                }
                #endregion
            }
        }

        /// <summary>
        /// Draws the Enemy's valid directions(as lines) if the Enemy is colliding.
        /// </summary>
        /// <param name="dc">Graphics controller.</param>
        public void DrawDebugDirections(Graphics dc)
        {
            foreach (EnumDirection y in this.postValidDirection)
            {
                if (this.collPosition.Count() > 0)
                {
                    // If there is collision
                    Point myRectPos = new Point(MyRectangle().X + (MyRectangle().Width / 2), MyRectangle().Y + (MyRectangle().Height / 2));
                    switch (y)
                    {
                        case EnumDirection.Left:
                            dc.DrawLine(new Pen(Color.Black), myRectPos.X, myRectPos.Y, myRectPos.X - 50, myRectPos.Y);
                            break;
                        case EnumDirection.LeftUp:
                            dc.DrawLine(new Pen(Color.Black), myRectPos.X, myRectPos.Y, myRectPos.X - 35, myRectPos.Y - 35);
                            break;
                        case EnumDirection.LeftDown:
                            dc.DrawLine(new Pen(Color.Black), myRectPos.X, myRectPos.Y, myRectPos.X - 35, myRectPos.Y + 35);
                            break;
                        case EnumDirection.Right:
                            dc.DrawLine(new Pen(Color.Black), myRectPos.X, myRectPos.Y, myRectPos.X + 50, myRectPos.Y);
                            break;
                        case EnumDirection.RightUp:
                            dc.DrawLine(new Pen(Color.Black), myRectPos.X, myRectPos.Y, myRectPos.X + 35, myRectPos.Y - 35);
                            break;
                        case EnumDirection.RightDown:
                            dc.DrawLine(new Pen(Color.Black), myRectPos.X, myRectPos.Y, myRectPos.X + 35, myRectPos.Y + 35);
                            break;
                        case EnumDirection.Up:
                            dc.DrawLine(new Pen(Color.Black), myRectPos.X, myRectPos.Y, myRectPos.X, myRectPos.Y - 50);
                            break;
                        case EnumDirection.Down:
                            dc.DrawLine(new Pen(Color.Black), myRectPos.X, myRectPos.Y, myRectPos.X, myRectPos.Y + 50);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes collisions in collPosition to determine a list of valid directions in postValidDirection.
        /// </summary>
        private void ProcessValidDirections()
        {
            // Stores the valid directions foreach collision.
            this.preValidDirection.Clear();
            foreach (EnumDirection x in this.collPosition)
            {
                this.DetermineValidDirection(x);
            }

            // If there are multiple collisions:
            // Searches through the preValid list and only keeps one copy of the directions there are multiple of, in the postValid list.
            if (this.collPosition.Count() > 1)
            {
                this.postValidDirection.Clear();
                this.preValidDirection.Sort();
                EnumDirection previous = EnumDirection.None;
                foreach (EnumDirection x in this.preValidDirection)
                {
                    if (previous != x)
                    {
                        int i = this.preValidDirection.IndexOf(x);
                        int j = this.preValidDirection.LastIndexOf(x);
                        int total = (j - i) + 1;

                        if (total >= this.collPosition.Count())
                        {
                            this.postValidDirection.Add(x);
                        }

                        previous = x;
                    }
                }
            }
            else
            {
                // If there is only 1 collision
                this.postValidDirection.Clear();
                this.preValidDirection.ForEach(x => this.postValidDirection.Add(x)); // Make the preValidDirections the active(postValidDirections) list.
            }
        }

        /// <summary>
        /// Adds all the possible valid-directions to preValidDirection, based on a collision position.
        /// </summary>
        /// <param name="CollisionPosition">The position of the collision.</param>
        private void DetermineValidDirection(EnumDirection collisionPosition)
        {
            switch (collisionPosition)
            {
                case EnumDirection.Left:
                    this.preValidDirection.Add(EnumDirection.Up);
                    this.preValidDirection.Add(EnumDirection.Down);
                    this.preValidDirection.Add(EnumDirection.Right);
                    this.preValidDirection.Add(EnumDirection.RightUp);
                    this.preValidDirection.Add(EnumDirection.RightDown);
                    break;
                case EnumDirection.LeftUp:
                    this.preValidDirection.Add(EnumDirection.RightUp);
                    this.preValidDirection.Add(EnumDirection.Right);
                    this.preValidDirection.Add(EnumDirection.RightDown);
                    this.preValidDirection.Add(EnumDirection.Down);
                    this.preValidDirection.Add(EnumDirection.LeftDown);
                    break;
                case EnumDirection.LeftDown:
                    this.preValidDirection.Add(EnumDirection.LeftUp);
                    this.preValidDirection.Add(EnumDirection.Up);
                    this.preValidDirection.Add(EnumDirection.RightUp);
                    this.preValidDirection.Add(EnumDirection.Right);
                    this.preValidDirection.Add(EnumDirection.RightDown);
                    break;
                case EnumDirection.Right:
                    this.preValidDirection.Add(EnumDirection.Up);
                    this.preValidDirection.Add(EnumDirection.Down);
                    this.preValidDirection.Add(EnumDirection.Left);
                    this.preValidDirection.Add(EnumDirection.LeftUp);
                    this.preValidDirection.Add(EnumDirection.LeftDown);
                    break;
                case EnumDirection.RightUp:
                    this.preValidDirection.Add(EnumDirection.LeftUp);
                    this.preValidDirection.Add(EnumDirection.RightDown);
                    this.preValidDirection.Add(EnumDirection.Down);
                    this.preValidDirection.Add(EnumDirection.LeftDown);
                    this.preValidDirection.Add(EnumDirection.Left);
                    break;
                case EnumDirection.RightDown:
                    this.preValidDirection.Add(EnumDirection.Up);
                    this.preValidDirection.Add(EnumDirection.RightUp);
                    this.preValidDirection.Add(EnumDirection.LeftDown);
                    this.preValidDirection.Add(EnumDirection.Left);
                    this.preValidDirection.Add(EnumDirection.LeftUp);
                    break;
                case EnumDirection.Up:
                    this.preValidDirection.Add(EnumDirection.Left);
                    this.preValidDirection.Add(EnumDirection.Right);
                    this.preValidDirection.Add(EnumDirection.Down);
                    this.preValidDirection.Add(EnumDirection.RightDown);
                    this.preValidDirection.Add(EnumDirection.LeftDown);
                    break;
                case EnumDirection.Down:
                    this.preValidDirection.Add(EnumDirection.Left);
                    this.preValidDirection.Add(EnumDirection.Right);
                    this.preValidDirection.Add(EnumDirection.Up);
                    this.preValidDirection.Add(EnumDirection.RightUp);
                    this.preValidDirection.Add(EnumDirection.LeftUp);
                    break;
                case EnumDirection.None:
                    this.preValidDirection.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}