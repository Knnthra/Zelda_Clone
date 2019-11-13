namespace EksamenJuni2010
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    /// <summary>
    /// Types of directions / positions
    /// </summary>
    public enum EnumDirection
    {
        Left,
        LeftUp,
        LeftDown,
        Right,
        RightUp,
        RightDown,
        Up,
        Down,

        /// <summary>
        /// Represents no direction or position
        /// </summary>
        None
    }
    
    /// <summary>
    /// Represents the main form of the game.
    /// </summary>
    public partial class GameForm : Form
    {
        #region Fields
        /// <summary>
        /// Represents a random-number generator.
        /// </summary>
        private Random rnd = new Random();

        /// <summary>
        /// Represents the character used to seperate directory levels.
        /// </summary>
        private static char dirSeparator = Path.DirectorySeparatorChar;

        /// <summary>
        /// Represents the game-score.
        /// </summary>
        private float score;

        /// <summary>
        /// Represents the list of highscores.
        /// </summary>
        private List<string> highscores = new List<string>();

        /// <summary>
        /// Indicates whether the highscores should be shown.
        /// </summary>
        private bool showHighscores;

        /// <summary>
        /// Represents the limit of zones in the x-axis.
        /// </summary>
        private static byte wxLimit = 5;

        /// <summary>
        /// Represents the limit of zones in the y-axis.
        /// </summary>
        private static byte wyLimit = 4;

        /// <summary>
        /// Represents all the world-zone-images.
        /// </summary>
        private Image[,] worldImages = new Image[wxLimit, wyLimit];

        /// <summary>
        /// Represents the current zone. (x-axis).
        /// </summary>
        private int worldX;

        /// <summary>
        /// Represents the current zone. (y-axis).
        /// </summary>
        private int worldY;

        /// <summary>
        /// Indicates whether a zone-change has occurred.
        /// </summary>
        private bool zoneChange = true;

        /// <summary>
        /// Represents the soundplayer for the background-music.
        /// </summary>
        private SoundPlayer backgroundMusic;

        /// <summary>
        /// Represents a list of contents(enemies/items) for all zones.
        /// </summary>
        private GameObject[,][] zoneContent = new GameObject[wxLimit, wyLimit][];

        /// <summary>
        /// Represents a list of obstacles(collisions/dialogs/teleports) for all zones.
        /// </summary>
        private GameObject[,][] obstacleContainer = new GameObject[wxLimit, wyLimit][];

        /// <summary>
        /// Represents the game-world. A list with the current GameObjects.
        /// </summary>
        private List<GameObject> gameWorld = new List<GameObject>();

        /// <summary>
        /// Represents the Player.
        /// </summary>
        private Player player;

        /// <summary>
        /// Indicates whether the attack-button is released.
        /// </summary>
        private bool atkRls;

        /// <summary>
        /// Represents a list of the current ScrollingCombatText.
        /// </summary>
        private List<ScrollingCombatText> scText = new List<ScrollingCombatText>();

        /// <summary>
        /// Represents the DateTime of the start of a gameloop.
        /// </summary>
        private DateTime nowTime;

        /// <summary>
        /// Represents the milliseconds it takes to complete a gameloop.
        /// </summary>
        private float frameTimeMili;

        /// <summary>
        /// Represents the time, in seconds, it takes to complete a gameloop.
        /// </summary>
        private float factor;

        /// <summary>
        /// Represents the current speed of the game.
        /// </summary>
        private int gameSpeed;

        /// <summary>
        /// Represents the overlay-image at the start of the game(Overlay 1).
        /// </summary>
        private Image startOverlay;

        /// <summary>
        /// Represents the overlay-image at the end of the game(Overlay 2).
        /// </summary>
        private Image finishOverlay;

        /// <summary>
        /// Represents the overlay-image when the player is dead(Overlay 3).
        /// </summary>
        private Image deadOverlay;

        /// <summary>
        /// Represents the current overlay.
        /// </summary>
        private int currentOverlay;

        /// <summary>
        /// Indicates whether an overlay-image should be drawn.
        /// </summary>
        private bool drawOverlay;

        /// <summary>
        /// Indicates whether the game is pauesd.
        /// </summary>
        private bool gamePaused;

        /// <summary>
        /// Indicates whether the collision boxes and enemy directions, should be drawn.
        /// </summary>
        private bool drawDebug;
        #endregion

        /// <summary>
        /// Initializes a new instance of the GameForm class.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();

            this.LoadGameFiles();

            this.ResetGame();
        }
        
        /// <summary>
        /// Loads all the required Images, Sounds and reads the gameobjects.txt.
        /// </summary>
        private void LoadGameFiles()
        {
            this.startOverlay = Image.FromFile(string.Format("graphics{0}startscreen.png", dirSeparator));
            this.finishOverlay = Image.FromFile(string.Format("graphics{0}finishscreen.png", dirSeparator));
            this.deadOverlay = Image.FromFile(string.Format("graphics{0}deadscreen.png", dirSeparator));
            this.backgroundMusic = new SoundPlayer(string.Format("sounds{0}background.wav", dirSeparator));

            foreach (string worldImgDir in Directory.GetFiles(string.Format("graphics{0}world{0}", dirSeparator)))
            {
                string fileName = Path.GetFileName(worldImgDir);
                int tmpX = int.Parse(fileName.Split(';')[0]);
                int tmpY = int.Parse(fileName.Split(';')[1]);
                this.worldImages[tmpX, tmpY] = Image.FromFile(worldImgDir);
            }

            this.ClientSize = this.worldImages[0, 0].Size;

            List<GameObject> tmpZoneContent = new List<GameObject>();
            List<GameObject> tmpObsContainer = new List<GameObject>();
            int indexX = 0;
            int indexY = 0;

            foreach (string x in File.ReadAllLines("gameobjects.txt"))
            {
                string[] lines = x.Split(';');
                switch (lines[0])
                {
                    case "zone":
                        if (tmpObsContainer.Count > 0)
                        {
                            this.obstacleContainer[indexX, indexY] = tmpObsContainer.ToArray();
                            tmpObsContainer.Clear();
                        }

                        if (tmpZoneContent.Count() > 0)
                        {
                            this.zoneContent[indexX, indexY] = tmpZoneContent.ToArray();
                            tmpZoneContent.Clear();
                        }

                        indexX = int.Parse(lines[1]);
                        indexY = int.Parse(lines[2]);
                        break;
                    case "collision":
                        tmpObsContainer.Add(new Obstacle(int.Parse(lines[1]), int.Parse(lines[2]), int.Parse(lines[3]), int.Parse(lines[4])));
                        break;
                    case "dialog":
                        tmpObsContainer.Add(new Obstacle(int.Parse(lines[1]), int.Parse(lines[2]), int.Parse(lines[3]), int.Parse(lines[4]), lines[5].ToLower()));
                        break;
                    case "teleport": 
                        tmpObsContainer.Add(new Obstacle(int.Parse(lines[1]), int.Parse(lines[2]), int.Parse(lines[3]), int.Parse(lines[4]), new Point(int.Parse(lines[5]), int.Parse(lines[6])), new Point(int.Parse(lines[7]), int.Parse(lines[8]))));
                        break;
                    case "enemy":
                        tmpZoneContent.Add(new Enemy(int.Parse(lines[1]), int.Parse(lines[2])));
                        break;
                    case "item":
                        string tmpType = lines[3].ToLower();
                        switch (tmpType)
                        {
                            case "questitemmap":
                                tmpZoneContent.Add(new Item(new Point(int.Parse(lines[1]), int.Parse(lines[2])), ItemType.QuestItemMap));
                                break;
                            case "questitemkey":
                                tmpZoneContent.Add(new Item(new Point(int.Parse(lines[1]), int.Parse(lines[2])), ItemType.QuestItemKey));
                                break;
                            case "questitemcrystal":
                                tmpZoneContent.Add(new Item(new Point(int.Parse(lines[1]), int.Parse(lines[2])), ItemType.QuestItemCrystal));
                                break;
                        }

                        break;
                }
            }

            this.zoneContent[indexX, indexY] = tmpZoneContent.ToArray();
            this.obstacleContainer[indexX, indexY] = tmpObsContainer.ToArray();
        }
        
        /// <summary>
        /// Resets the game.
        /// </summary>
        private void ResetGame()
        {
            this.backgroundMusic.PlayLooping();
            toggleMusicToolStripMenuItem.Checked = true;

            //Startzone position
            this.worldX = 0;
            this.worldY = 3;

            //Attack i released(ready)
            this.atkRls = true;

            this.gamePaused = true;

            this.score = 0;
            this.showHighscores = false;

            this.drawOverlay = true;
            this.currentOverlay = 1;

            this.drawDebug = false;
            toggleDebugToolStripMenuItem.Checked = false;

            this.player = new Player(246, 419);

            this.scText.Clear();

            this.gameWorld.Clear();
            this.gameWorld.Add(this.player);
        }
        
        /// <summary>
        /// Runs one gameloop tick.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void game_loop_Tick(object sender, EventArgs e)
        {
            #region DetermineGameSpeed
            this.frameTimeMili = DateTime.Now.Subtract(this.nowTime).Milliseconds; //Stores the time since the last gameloob/frame was complete
            this.nowTime = DateTime.Now;                                      //Stores the current time

            float fps = 1000 / this.frameTimeMili;           //Calculate fps
            this.factor = this.frameTimeMili / 1000;              //Calculates the time, in seconds, it takes to complete 1 gameloop/frame. Used to calculate gameSpeed aswell as other speeds
            this.gameSpeed = (int)(100 * this.factor);            //gameSpeed is used to determine how many pixels a gameobject needs to move each loop/frame to maintain a constant speed, regardless of cpu-power or loop-interval
            #endregion

            this.Input();
            if (!this.gamePaused)
            {
                this.Gamelogic();
                this.Animation();
                this.Physics();
            }

            this.Render();
        }

        /// <summary>
        /// Reads the Player input and determines a direction for each Enemy.
        /// </summary>
        private void Input()
        {
            #region GetKeyboardState
            // Set player to attack if space is pressed(and not already pressed)
            this.player.AtkMelee = (Keyboard.IsKeyDown(Keys.Space) && this.atkRls == true) ? true : this.player.AtkMelee;
            this.atkRls = (!Keyboard.IsKeyDown(Keys.Space)) ? true : false;

            this.player.MoveDirection = EnumDirection.None;
            this.player.MoveDirection = Keyboard.IsKeyDown(Keys.A) ? EnumDirection.Left : this.player.MoveDirection;
            this.player.MoveDirection = Keyboard.IsKeyDown(Keys.D) ? EnumDirection.Right : this.player.MoveDirection; 
            if (Keyboard.IsKeyDown(Keys.W))
            {
                this.player.MoveDirection = EnumDirection.Up;
                this.player.MoveDirection = Keyboard.IsKeyDown(Keys.D) ? EnumDirection.RightUp : this.player.MoveDirection;
                this.player.MoveDirection = Keyboard.IsKeyDown(Keys.A) ? EnumDirection.LeftUp : this.player.MoveDirection;
            }

            if (Keyboard.IsKeyDown(Keys.S))
            {
                this.player.MoveDirection = EnumDirection.Down;
                this.player.MoveDirection = Keyboard.IsKeyDown(Keys.D) ? EnumDirection.RightDown : this.player.MoveDirection;
                this.player.MoveDirection = Keyboard.IsKeyDown(Keys.A) ? EnumDirection.LeftDown : this.player.MoveDirection;
            }
            #endregion

            foreach (Enemy x in this.gameWorld.FindAll(x => x is Enemy))
            {
                Enemy tmpEnemy = (Enemy)x;
                tmpEnemy.DetermineDirection(this.player);
            }
        }

        /// <summary>
        /// Handles the Game's Logic. Moves GameObjects, handles AI and zone-change.
        /// </summary>
        private void Gamelogic()
        {
            this.score += this.frameTimeMili / 1000;

            #region CombatText
            List<ScrollingCombatText> tmpScText = new List<ScrollingCombatText>(this.scText);
            foreach (ScrollingCombatText x in tmpScText)
            {
                if (x.TextColor.A <= 5) //Hvis tekstens alpha er 5 eller under
                {
                    this.scText.Remove(x);
                }
            }
            #endregion

            List<GameObject> tmpWorld = new List<GameObject>(this.gameWorld.FindAll(x => x is DynamicGameObject));
            foreach (DynamicGameObject x in tmpWorld)
            {
                if (x is Enemy)
                {
                    #region HandleEnemy
                    if (x.Health <= 0)
                    {
                        //Oh he dead
                        this.gameWorld.Remove(x);

                        //Drop item
                        this.gameWorld.Add(new Item(x.Coords, this.rnd));
                    }

                    x.HandleInput(this.gameSpeed);

                    if (x.AtkRdy && x.AtkMelee)//Hvis enemy attacker og er i meleeRange
                    {
                        if (this.player.InvulnerableUntill < DateTime.Now) //Hvis player ikke er invulnernable
                        {
                            this.scText.Add(x.CalcAndApplyDamage(this.player, this.rnd));
                            this.player.InvulnerableUntill = DateTime.Now.AddSeconds(1); //Gør player invulnerable i 1 sek
                        }
                    }
                    #endregion
                }
                else if (x is Player)
                {
                    #region HandlePlayer
                    if (x.Health <= 0)
                    {
                        //Oh he dead
                        this.gamePaused = true;
                        this.drawOverlay = true;
                        this.currentOverlay = 3;
                    }

                    x.HandleInput(this.gameSpeed);

                    if (x.AtkRdy && x.AtkMelee) //Hvis attack
                    {
                        foreach (DynamicGameObject y in this.gameWorld.FindAll(y => y is Enemy))
                        {
                            //For hver enemy som er attackable, reduce health
                            if (this.player.Attackable(y))
                            {
                                this.scText.Add(x.CalcAndApplyDamage(y, this.rnd));
                            }
                        }
                    }
                    #endregion

                    this.ChangeZone();
                }
            }
        }

        /// <summary>
        /// Ensures that if the Player reaches the edge of the screen, the zone-image is changed, and relevant objects are loaded.
        /// </summary>
        private void ChangeZone()
        {
            if (this.player.Coords.X < 0) //Venstre
            {
                if (this.worldX != 0)
                {
                    this.player.Coords = new Point(this.ClientSize.Width, this.player.Coords.Y); //Flytter player på den anden side af skærmen
                    this.worldX--;                                               //Justerer verdens-koordinatet
                    this.zoneChange = true;                                      //Indikerer at der er skiftet zone
                }
                else
                {
                    this.player.Coords = new Point(0, this.player.Coords.Y);
                }
            }
            else if (this.player.Coords.X > this.ClientSize.Width) //Højre
            {
                if (this.worldX != wxLimit - 1)
                {
                    this.player.Coords = new Point(0, this.player.Coords.Y);
                    this.worldX++;
                    this.zoneChange = true;
                }
                else
                {
                    this.player.Coords = new Point(this.ClientSize.Width, this.player.Coords.Y);
                }
            }
            else if (this.player.Coords.Y < 0) //Op
            {
                if (this.worldY != 0)
                {
                    this.player.Coords = new Point(this.player.Coords.X, this.ClientSize.Height);
                    this.worldY--;
                    this.zoneChange = true;
                }
                else
                {
                    this.player.Coords = new Point(this.player.Coords.X, 0);
                }
            }
            else if (this.player.Coords.Y > this.ClientSize.Height) //Ned
            {
                if (this.worldY != wyLimit - 1)
                {
                    this.player.Coords = new Point(this.player.Coords.X, 0);
                    this.worldY++;
                    this.zoneChange = true;
                }
                else
                {
                    this.player.Coords = new Point(this.player.Coords.X, this.ClientSize.Height);
                }
            }

            if (this.zoneChange)
            {
                this.LoadZoneObjects();
            }
        }

        /// <summary>
        /// Loads the relevant Objects for the current zone.
        /// </summary>
        private void LoadZoneObjects()
        {
            this.gameWorld.RemoveAll(y => !(y is Player));
            this.scText.Clear();

            if (this.zoneContent[this.worldX, this.worldY] != null)
            {
                foreach (GameObject x in this.zoneContent[this.worldX, this.worldY])
                {
                    if (x is Item)
                    {
                        Item tmpItem = (Item)x;

                        if (!this.player.ScanInventoryForType(tmpItem.MyType))
                        {
                            this.gameWorld.Add(new Item(x.Coords, tmpItem.MyType));
                        }
                    }

                    if (x is Enemy)
                    {
                        this.gameWorld.Add(new Enemy(x.Coords.X, x.Coords.Y));
                    }
                }
            }

            this.zoneChange = false;
        }

        /// <summary>
        /// Handles the Animation by calling the relevant method in the DynamicGameObjects and the ScrollingCombatText.
        /// </summary>
        private void Animation()
        {
            foreach (DynamicGameObject x in this.gameWorld.FindAll(x => x is DynamicGameObject))
            {
                x.Animate(this.gameSpeed);
            }

            foreach (ScrollingCombatText x in this.scText)
            {
                x.Animate();
            }
        }

        /// <summary>
        /// For each DynamicGameObject, searches for colliding objects and handles its Physics.
        /// </summary>
        private void Physics()
        {
            List<GameObject> tmpWorld = new List<GameObject>(this.gameWorld);
            tmpWorld.AddRange(this.obstacleContainer[this.worldX, this.worldY]); //Add the current obstacles to the tmpWorld, for at ingå i collision-detection
            foreach (DynamicGameObject x in tmpWorld.FindAll(x => x is DynamicGameObject))
            {
                if (x is Enemy)
                {
                    #region Enemy
                    Enemy tmpEnemy = (Enemy)x;          //Convert object to an enemy
                    tmpEnemy.CollPosition.Clear();      //Clears list with collisions
                    foreach (GameObject y in tmpWorld.FindAll(y => !(y is Player || y is Item || x == y)))  //Searches for objects that collide(not player, not an item and not itself)
                    {
                        Rectangle colRect = Rectangle.Intersect(x.MyRectangle(), y.MyRectangle());
                        if (colRect.Width != 0 || colRect.Height != 0) //Hvis der er collision, er dens størrelse størrere end 0
                        {
                            if (colRect == x.MyRectangle())
                            {
                                this.gameWorld.Remove(x);
                            }
                            else
                            {
                                this.DetermineCollisionPosition(colRect, tmpEnemy);
                            }
                        }
                    }
                    #endregion
                }
                else if (x is Player)
                {
                    #region Player
                    foreach (GameObject y in tmpWorld.FindAll(y => y is Obstacle || y is Item)) //Hvis player collider med obstacle eller item
                    {
                        Rectangle colRect = Rectangle.Intersect(x.MyRectangle(), y.MyRectangle());
                        if (colRect.Width != 0 || colRect.Height != 0)      //Hvis der er collision, er dens størrelse størrere end 0
                        {
                            if (y is Item)
                            {
                                this.HandleLoot((Item)y);
                            }
                            else if (y is Obstacle)
                            {
                                #region HandleObstacle
                                Obstacle tmpObs = (Obstacle)y;
                                if (tmpObs.MyType == ObstacleType.Teleport) //Collision with teleport
                                {
                                    this.player.Coords = new Point(tmpObs.PlayerRelocation.X, tmpObs.PlayerRelocation.Y);
                                    this.worldX = tmpObs.ZoneDestination.X;
                                    this.worldY = tmpObs.ZoneDestination.Y;
                                    this.LoadZoneObjects();
                                }
                                else if (tmpObs.MyType == ObstacleType.Dialog) //Collision with dialog
                                {
                                    if (this.player.InDialogWith == null && Keyboard.IsKeyDown(Keys.Space))
                                    {
                                        if (tmpObs.MyTag == "vendor")
                                        {
                                            Item healthCake = new Item(this.player.Coords, ItemType.HealthCake);
                                            this.HandleLoot(healthCake);
                                        }
                                        else if (tmpObs.MyTag == "guards")
                                        {
                                            if (this.player.ScanInventoryForType(ItemType.QuestItemCrystal) &&
                                                this.player.ScanInventoryForType(ItemType.QuestItemKey) &&
                                                this.player.ScanInventoryForType(ItemType.QuestItemMap))
                                            {
                                                this.gamePaused = true;
                                                this.drawOverlay = true;
                                                this.currentOverlay = 2;
                                            }
                                        }

                                        tmpObs.RandomizeResponse(this.rnd);
                                        this.player.InDialogWith = y;
                                    }
                                }
                                else if (tmpObs.MyType == ObstacleType.Collision)//Collision with solid-object
                                {
                                    this.DetermineCollisionPosition(colRect, this.player);
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Handles a Player colliding with an Item. Calling Player.Lootable() and creating a ScrollingCombatText.
        /// </summary>
        /// <param name="loot">The Item which the Player collides with.</param>
        private void HandleLoot(Item loot)
        {
            Point tmpItemPos = loot.Coords;
            if (this.player.Lootable((Item)loot))
            {
                this.scText.Add(new ScrollingCombatText(tmpItemPos, "item looted", SctType.Normal));
                this.gameWorld.Remove(loot); //Remove item from world
            }
            else
            {
                if (this.scText.FindAll(sct => sct.Text == "inventory full").Count == 0) //If scText doesnt already contain a "inventory full" message
                {
                    this.scText.Add(new ScrollingCombatText(tmpItemPos, "inventory full", SctType.Block));
                }
            }
        }

        /// <summary>
        /// Determines which side of the DynamicGameObject is colliding.
        /// </summary>
        /// <param name="collisionRect">The collision rectangle between the two Objects.</param>
        /// <param name="yourself">The DynamicGameObject which we are working on.</param>
        private void DetermineCollisionPosition(Rectangle collisionRect, DynamicGameObject yourself)
        {
            if (collisionRect.Width <= collisionRect.Height || collisionRect.Height == yourself.MyRectangle().Height)
            {
                #region LEFT
                if (collisionRect.X <= yourself.MyRectangle().X) //Left
                {
                    if (collisionRect.Height != yourself.MyRectangle().Height) //Collision er et hjørne
                    {
                        if (collisionRect.Y <= yourself.MyRectangle().Y) //left top
                        {
                            this.AddCollisionPosition(EnumDirection.LeftUp, collisionRect, yourself);
                        }
                        else            //left bottom
                        {
                            this.AddCollisionPosition(EnumDirection.LeftDown, collisionRect, yourself);
                        }
                    }
                    else //Collisionen er hele siden
                    {
                        this.AddCollisionPosition(EnumDirection.Left, collisionRect, yourself);
                    }
                }
                #endregion
                #region RIGHT
                else //Right
                {
                    if (collisionRect.Height != yourself.MyRectangle().Height) //Collision er et hjørne
                    {
                        if (collisionRect.Y <= yourself.MyRectangle().Y) //Right Top
                        {
                            this.AddCollisionPosition(EnumDirection.RightUp, collisionRect, yourself);
                        }
                        else    //Right Bottom
                        {
                            this.AddCollisionPosition(EnumDirection.RightDown, collisionRect, yourself);
                        }
                    }
                    else //Collisionen er hele siden
                    {
                        this.AddCollisionPosition(EnumDirection.Right, collisionRect, yourself);
                    }
                }
                #endregion
            }
            else if (collisionRect.Width >= collisionRect.Height || collisionRect.Width == yourself.MyRectangle().Width)
            {
                #region TOP
                if (collisionRect.Y <= yourself.MyRectangle().Y) //Top
                {
                    if (collisionRect.Width != yourself.MyRectangle().Width) //Collision er et hjørne
                    {
                        if (collisionRect.X <= yourself.MyRectangle().X) //Top left
                        {
                            this.AddCollisionPosition(EnumDirection.LeftUp, collisionRect, yourself);
                        }
                        else        //Top right
                        {
                            this.AddCollisionPosition(EnumDirection.RightUp, collisionRect, yourself);
                        }
                    }
                    else //Collisionen er hele siden
                    {
                        this.AddCollisionPosition(EnumDirection.Up, collisionRect, yourself);
                    }
                }
                #endregion
                #region BOTTOM
                else //Bottom
                {
                    if (collisionRect.Width != yourself.MyRectangle().Width) //Collision er et hjørne
                    {
                        if (collisionRect.X <= yourself.MyRectangle().X) //bottom left
                        {
                            this.AddCollisionPosition(EnumDirection.LeftDown, collisionRect, yourself);
                        }
                        else        //Bottomright
                        {
                            this.AddCollisionPosition(EnumDirection.RightDown, collisionRect, yourself);
                        }
                    }
                    else //Collisionen er hele siden
                    {
                        this.AddCollisionPosition(EnumDirection.Down, collisionRect, yourself);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Calls the relevent method to prevent further collision, whether the GameObject is a Player or an Enemy.
        /// </summary>
        /// <param name="direction">Det position(side) of the collision.</param>
        /// <param name="collisionRect">The collision rectangle between the two Objects.</param>
        /// <param name="yourself">The DynamicGameObject which we are working on.</param>
        private void AddCollisionPosition(EnumDirection direction, Rectangle collisionRect, DynamicGameObject yourself)
        {
            if (yourself is Player)
            {
                this.player.HandleCollision(direction, collisionRect);
            }
            else if(yourself is Enemy)
            {
                Enemy tmpEnemy = (Enemy)yourself;
                tmpEnemy.CollPosition.Add(direction);
            }
        }

        /// <summary>
        /// Draws the current background, all GameObjects in the gameWorld, including a health-bar and all ScrollingCombatText's.
        /// Also capable of drawing a dialog-box, an inventory, an overlay and the highscore, if needed.
        /// </summary>
        private void Render()
        {
            Bitmap tmpBitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height); //Creates a new, empty, bitmap, with equal size to the form's client-size
            Graphics gphBitmap = Graphics.FromImage(tmpBitmap); //Creates a graphics controller from the new bitmap

            gphBitmap.DrawImage(this.worldImages[this.worldX, this.worldY], new Point(0, 0)); //Draw appropriate background
            
            #region DrawDebug
            if (this.drawDebug)
            {
                gphBitmap.DrawString(this.worldX.ToString() + ";" + this.worldY.ToString() + Environment.NewLine + "Score: " + ((int)this.score).ToString(), new Font(new FontFamily("Arial"), 20, FontStyle.Bold), new SolidBrush(Color.Blue), 250, 100);
                this.gameWorld.ForEach(x => gphBitmap.DrawRectangle(new Pen(Color.Red), x.MyRectangle())); //Draws collision boxes
                foreach (Enemy x in this.gameWorld.FindAll(x => x is Enemy))
                {
                    x.DrawDebugDirections(gphBitmap);
                }

                foreach (GameObject x in this.obstacleContainer[this.worldX, this.worldY])
                {
                    gphBitmap.DrawRectangle(new Pen(Color.Red), x.MyRectangle()); //Draws collision boxes
                }
            }
            #endregion

            List<GameObject> sortedWorld = new List<GameObject>(this.gameWorld);
            sortedWorld.Sort(new SorterRender());

            foreach (GameObject x in sortedWorld)
            {
                x.Render(gphBitmap);
                #region Draw Health-bar
                if (x is DynamicGameObject)
                {
                    // Draw HP bar
                    DynamicGameObject tmpObj = (DynamicGameObject)x;
                    int healthLength = (int)((float)tmpObj.Health * 0.3f);
                    gphBitmap.DrawLine(new Pen(Color.White, 2), x.Coords.X + 1 - 15, x.Coords.Y + 2 - 29, x.Coords.X - 1 - 15 + 30, x.Coords.Y + 2 - 29);
                    gphBitmap.DrawLine(new Pen(Color.Red, 2), x.Coords.X + 1 - 15, x.Coords.Y + 2 - 29, x.Coords.X - 1 - 15 + healthLength, x.Coords.Y + 2 - 29);
                    gphBitmap.DrawRectangle(new Pen(Color.FromArgb(255, 32, 40, 32), 2), x.Coords.X - 15, x.Coords.Y - 29, 30, 4);
                }
                #endregion
            }

            foreach (ScrollingCombatText x in this.scText) { x.Render(gphBitmap); }
            if (this.player.InDialogWith != null) { this.player.DrawDialogBox(gphBitmap); }
            if (this.player.DrawInventory) { this.player.DrawCharacterSheet(gphBitmap); }
            #region DrawOverlay
            if (this.drawOverlay)
            {
                if (this.currentOverlay == 1)
                {
                    gphBitmap.DrawImage(this.startOverlay, 0, 0, this.startOverlay.Width, this.startOverlay.Height);
                }
                else if (this.currentOverlay == 2)
                {
                    gphBitmap.DrawImage(this.finishOverlay, 0, 0, this.finishOverlay.Width, this.finishOverlay.Height);
                }
                else if (this.currentOverlay == 3)
                {
                    gphBitmap.DrawImage(this.deadOverlay, 0, 0, this.deadOverlay.Width, this.deadOverlay.Height);
                }
            }
            #endregion
            #region DrawHighscores
            if (this.showHighscores)
            {
                gphBitmap.FillRectangle(new SolidBrush(Color.FromArgb(190, Color.Black)), new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));

                StringBuilder strHighscore = new StringBuilder();
                this.highscores.ForEach(x => strHighscore.Append(x + Environment.NewLine));

                Font tmpFont = new Font(new FontFamily("Arial"), 30, FontStyle.Bold);
                SizeF scoreSize = gphBitmap.MeasureString(strHighscore.ToString(), tmpFont);
                Point drawPoint = new Point((this.ClientSize.Width / 2) - ((int)scoreSize.Width / 2), (this.ClientSize.Height / 2) - ((int)scoreSize.Height / 2));

                gphBitmap.DrawString(strHighscore.ToString().Replace(";", ": "), tmpFont, new SolidBrush(Color.White), drawPoint);
            }
            #endregion
            
            Graphics dc = CreateGraphics(); // Creates a graphics controller
            dc.DrawImage(tmpBitmap, 0, 0);  // Draws the 'newly' created bitmap on the form. Thus eliminating any 'flicker' problem

            gphBitmap.Dispose();
            tmpBitmap.Dispose();
            dc.Dispose();
        }

        /// <summary>
        /// Reads the highscores from a .txt file, sorting them and trimming it down to 10. Adding default highscores, if the list is below 10.
        /// </summary>
        private void GetCurrentHighscores()
        {
            this.highscores.Clear();

            if (File.Exists("highscore.txt"))
            {
                foreach (string line in File.ReadAllLines("highscore.txt"))
                {
                    this.highscores.Add(line);
                }

                this.highscores.Sort(new SorterScores());
                if (this.highscores.Count() > 10)
                {
                    this.highscores.RemoveRange(10, this.highscores.Count() - 10);
                }
            }

            while (this.highscores.Count() < 10)
            {
                this.highscores.Add("999;AAA");
            }
        }

        /// <summary>
        /// Compares the current score to the highscore-list, allowing the Player to enter a name,
        /// if the score is going on the top-10, then saves the highscore to a .txt file.
        /// </summary>
        private void CompareAndSaveScore()
        {
            this.GetCurrentHighscores();
            if (int.Parse(this.highscores[9].Split(';')[0]) > this.score)
            {
                EnterName formEnterName = new EnterName();
                if (formEnterName.ShowDialog() == DialogResult.OK)
                {
                    string name = formEnterName.EnteredName;
                    File.AppendAllText("highscore.txt", (int)this.score + ";" + name + Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// Only works if debug is enabled.
        /// Leftclick: Spawns an Enemy, removing it if clicked on.
        /// Rightclick: Spawns a random item. If shift is down, spawns a HealthCake.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">MouseEventArgs</param>
        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.drawDebug)
            {
                Point localCursor = this.PointToClient(Cursor.Position);
                if (e.Button == MouseButtons.Left)
                {
                    foreach (GameObject x in this.gameWorld.FindAll(x => x is Enemy))
                    {
                        if (x.MyRectangle().IntersectsWith(new Rectangle(localCursor, new Size(1, 1))))
                        {
                            this.gameWorld.Remove(x);
                            return;
                        }
                    }

                    this.gameWorld.Add(new Enemy(localCursor.X, localCursor.Y - 15));
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (Keyboard.IsKeyDown(Keys.ShiftKey))
                    {
                        this.gameWorld.Add(new Item(new Point(localCursor.X - 8, localCursor.Y - 8), ItemType.HealthCake));
                    }
                    else
                    {
                        this.gameWorld.Add(new Item(new Point(localCursor.X - 8, localCursor.Y - 8), this.rnd));
                    }
                }
            }
        }

        /// <summary>
        /// Based on which overlay is active, starts or resets the game when Enter is pressed.
        /// If no overlay is active, draws the player inventory when Enter is pressed.
        /// If the player inventory is being drawn, allows the player to move the inventory selection and use an item.
        /// If the highscore is being drawn, it always hides it again.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">KeyEventArgs</param>
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.drawOverlay)
                {
                    if (this.currentOverlay == 1)
                    {
                        this.drawOverlay = false;
                        this.gamePaused = false;
                    }
                    else if (this.currentOverlay == 2)
                    {
                        this.CompareAndSaveScore();
                        this.ResetGame();
                    }
                    else if (this.currentOverlay == 3)
                    {
                        this.ResetGame();
                    }
                }
                else if (this.player.DrawInventory)
                {
                    this.gamePaused = false;
                    this.player.DrawInventory = false;
                }
                else
                {
                    this.gamePaused = true;
                    this.player.DrawInventory = true;
                }
            }

            if (this.showHighscores)
            {
                this.showHighscores = false;
                this.player.DrawInventory = false;
                this.gamePaused = false;
            }

            if (this.player.DrawInventory)
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        this.player.InvSelecElement++;
                        break;
                    case Keys.Left:
                        this.player.InvSelecElement--;
                        break;
                    case Keys.Up:
                        this.player.InvSelecRow--;
                        break;
                    case Keys.Down:
                        this.player.InvSelecRow++;
                        break;
                    case Keys.Space:
                        this.player.UseInventoryItem();
                        break;
                }
            }
        }

        /// <summary>
        /// Writes all relevant information about the current state of the game to a .XML file.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Save to .XML
            XmlWriter writer = XmlWriter.Create(@"savedgame.xml");
            writer.WriteStartElement("GameWorld");

            #region Fields
            writer.WriteStartElement("Fields");
            writer.WriteAttributeString("worldX", this.worldX.ToString());
            writer.WriteAttributeString("worldY", this.worldY.ToString());
            writer.WriteAttributeString("score", ((int)this.score).ToString());
            writer.WriteAttributeString("gamePaused", this.gamePaused.ToString());
            writer.WriteAttributeString("drawOverlay", this.drawOverlay.ToString());
            writer.WriteAttributeString("currentOverlay", this.currentOverlay.ToString());
            writer.WriteAttributeString("drawDebug", this.drawDebug.ToString());
            writer.WriteAttributeString("showHighscores", this.showHighscores.ToString());
            writer.WriteEndElement();
            #endregion

            #region Inventory Items
            // Inventory items
            for (int row = 0; row < this.player.Inventory.Length; row++)
            {
                for (int element = 0; element < this.player.Inventory[row].Length; element++)
                {
                    if (this.player.Inventory[row][element] != null)
                    {
                        writer.WriteStartElement("Item");
                        writer.WriteAttributeString("elementPos", element.ToString());
                        writer.WriteAttributeString("rowPos", row.ToString());
                        this.player.Inventory[row][element].WriteXml(writer);
                        writer.WriteEndElement();
                    }
                }
            }
            #endregion

            foreach (GameObject go in this.gameWorld)
            {
                writer.WriteStartElement(go.GetType().Name);
                go.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();

            this.scText.Add(new ScrollingCombatText(new Point((this.ClientSize.Width / 2) - 100, this.ClientSize.Height / 2), "Game Saved", SctType.Critical));
        }

        /// <summary>
        /// Reads all information about the state of a previously saved game, from a .XML file.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Load from .XML

            if (File.Exists(@"savedgame.xml"))
            {
                this.ResetGame();
                this.zoneChange = false;
                XmlReader reader = XmlReader.Create(@"savedgame.xml");
                reader.MoveToContent();

                while (reader.Read())
                {
                    switch (reader.Name)
                    {
                        case "Fields":
                            this.worldX = int.Parse(reader.GetAttribute("worldX"));
                            this.worldY = int.Parse(reader.GetAttribute("worldY"));
                            this.score = int.Parse(reader.GetAttribute("score"));
                            this.gamePaused = reader.GetAttribute("gamePaused") == "True" ? true : false;
                            this.drawOverlay = reader.GetAttribute("drawOverlay") == "True" ? true : false;
                            this.currentOverlay = int.Parse(reader.GetAttribute("currentOverlay"));                            
                            if (reader.GetAttribute("drawDebug") == "True")
                            {
                                this.drawDebug = true;
                                toggleDebugToolStripMenuItem.Checked = true;
                            }   
                            else
                            {
                                drawDebug = false;
                                toggleDebugToolStripMenuItem.Checked = false;
                            }

                            if (reader.GetAttribute("showHighscores") == "True")
                            {
                                this.GetCurrentHighscores();
                                this.showHighscores = true;
                            }
                            break;
                        case "Player":
                            this.player.ReadXml(reader);
                            break;
                        case "Item":
                            Item item = new Item(reader);
                            if (item.MyState != ItemState.Dropped)
                            {
                                this.player.Inventory[int.Parse(reader.GetAttribute("rowPos"))][int.Parse(reader.GetAttribute("elementPos"))] = item;
                            }
                            else
                            {
                                gameWorld.Add(item);
                            }
                            break;
                        case "Enemy":
                            Enemy tmpEnemy = new Enemy(0, 0);
                            tmpEnemy.ReadXml(reader);
                            this.gameWorld.Add(tmpEnemy); // Add enemy to the game world
                            break;
                    }
                }
                this.Physics();
                reader.Close();
                this.scText.Add(new ScrollingCombatText(new Point((this.ClientSize.Width / 2) - 100, this.ClientSize.Height / 2), "Game Loaded", SctType.Critical));
            }
            else
            {
                this.scText.Add(new ScrollingCombatText(new Point((this.ClientSize.Width / 2) - 100, this.ClientSize.Height / 2), "No game saved...", SctType.Critical));
            }
        }

        /// <summary>
        /// Changes the indication of whether to draw debug.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void toggleDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toggleDebugToolStripMenuItem.Checked)
            {
                toggleDebugToolStripMenuItem.Checked = false;
                this.drawDebug = false;
            }
            else
            {
                toggleDebugToolStripMenuItem.Checked = true;
                this.drawDebug = true;
            }
        }

        /// <summary>
        /// Calls the ResetGame() method.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ResetGame();
        }

        /// <summary>
        /// Shows the highscores, hiding it, if it is already shown.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void highscoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.showHighscores)
            {
                this.showHighscores = false;
                this.gamePaused = false;
            }
            else
            {
                this.showHighscores = true;
                this.gamePaused = true;
                this.GetCurrentHighscores();
            }
        }
        
        /// <summary>
        /// Starts or stops the background music.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void toggleMusicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toggleMusicToolStripMenuItem.Checked)
            {
                this.backgroundMusic.Stop();
                this.backgroundMusic.Dispose();
                toggleMusicToolStripMenuItem.Checked = false;
            }
            else
            {
                this.backgroundMusic.PlayLooping();
                toggleMusicToolStripMenuItem.Checked = true;
            }
        }
        
        /// <summary>
        /// Quits the game/application.
        /// </summary>
        /// <param name="sender">Object</param>
        /// <param name="e">EventArgs</param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}