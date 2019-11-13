namespace EksamenJuni2010
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Represents the Player, of the type DynamicGameObject.
    /// </summary>
    public class Player : DynamicGameObject
    {
        /// <summary>
        /// Represents the bagground image of the inventory.
        /// </summary>
        private Image inventoryImage = Image.FromFile(string.Format("graphics{0}characterbased{0}inventory.png", dirSeparator));

        /// <summary>
        /// Represents the bagground image of the dialog-box.
        /// </summary>
        private Image dialogImage = Image.FromFile(string.Format("graphics{0}characterbased{0}dialogbox.png", dirSeparator));

        /// <summary>
        /// Represents the GameObject the Player is interacting with.
        /// </summary>
        private GameObject inDialogWith = null;

        /// <summary>
        /// Represents the current x-coordinate of the inventory selection.
        /// </summary>
        private int invSelecElement = 3;

        /// <summary>
        /// Represents the current y-coordinate of the inventory selection.
        /// </summary>
        private int invSelecRow = 0;

        /// <summary>
        /// Represents the inventory slots.
        /// </summary>
        private Item[][] inventory = new Item[3][] { new Item[6], new Item[6], new Item[6] };

        /// <summary>
        /// Represents the on-screen coordinates, of the different inventory slots.
        /// </summary>
        private Point[][] inventoryPositions = new Point[3][]
        { 
            new Point[6] { new Point(0, 0), new Point(159, 229), new Point(2, 0), new Point(371, 229), new Point(441, 229), new Point(511, 229) },
            new Point[6] { new Point(89, 299), new Point(159, 299), new Point(229, 299), new Point(371, 299), new Point(441, 299), new Point(511, 299) },
            new Point[6] { new Point(0, 2), new Point(159, 369), new Point(2, 2), new Point(371, 369), new Point(441, 369), new Point(511, 369) }
        };

        /// <summary>
        /// Represents the DateTime in which the Player is invulnerable untill.
        /// </summary>
        private DateTime invulnerableUntill;
        
        /// <summary>
        /// Represents a new instance of the ColorMatrix class.
        /// </summary>
        private ColorMatrix colorM = new ColorMatrix();

        /// <summary>
        /// Represents a new instance of the ImageAttributes class.
        /// </summary>
        private ImageAttributes imgA = new ImageAttributes();

        /// <summary>
        ///  Initializes a new instance of the Player class.
        /// </summary>
        /// <param name="posX">The initial x-coordinate position.</param>
        /// <param name="posY">The initial Y-coordinate position.</param>
        public Player(int posX, int posY)
        {
            coords = new Point(posX, posY);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to draw the inventory.
        /// </summary>
        public bool DrawInventory { get; set; }

        /// <summary>
        /// Gets or sets the GameObject, which the Player is interacting with.
        /// </summary>
        public GameObject InDialogWith
        {
            get { return this.inDialogWith; }
            set { this.inDialogWith = value; }
        }

        /// <summary>
        /// Gets or sets the current x-coordinate of the inventory selection.
        /// </summary>
        public int InvSelecElement
        {
            get 
            {
                return this.invSelecElement;
            }

            set
            {
                if (((value == 0 || value == 6 || value == 2) && this.invSelecRow == 0) ||
                     ((value == 0 || value == 6 || value == 2) && this.invSelecRow == 2))
                {
                    return;
                }

                if (value == this.invSelecElement + 1)
                {
                    // Right
                    if (this.invSelecElement != 5)
                    {
                        this.invSelecElement = value;
                    }
                    else
                    {
                        this.invSelecElement = 0;
                    }
                }
                else
                {
                    // Left
                    if (this.invSelecElement != 0)
                    {
                        this.invSelecElement = value;
                    }
                    else
                    {
                        this.invSelecElement = 5;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the current y-coordinate of the inventory selection.
        /// </summary>
        public int InvSelecRow
        {
            get
            {
                return this.invSelecRow;
            }

            set
            {
                if (((value == 0 || value == 2) && this.invSelecElement == 0) ||
                     ((value == 0 || value == 2) && this.invSelecElement == 2))
                {
                    return;
                }

                if (value == this.invSelecRow + 1)
                {
                    // Down
                    if (this.invSelecRow != 2)
                    {
                        this.invSelecRow = value;
                    }
                    else
                    {
                        this.invSelecRow = 0;
                    }
                }
                else
                {
                    // Up
                    if (this.invSelecRow != 0)
                    {
                        this.invSelecRow = value;
                    }
                    else
                    {
                        this.invSelecRow = 2;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the inventory slots.
        /// </summary>
        public Item[][] Inventory
        {
            get { return this.inventory; }
            set { this.inventory = value; }
        }
        
        /// <summary>
        /// Gets or sets the DateTime in which the Player is invulnerable untill.
        /// </summary>
        public DateTime InvulnerableUntill
        {
            get { return this.invulnerableUntill; }
            set { this.invulnerableUntill = value; }
        }

        /// <summary>
        /// Prevents the Player from moving into a colliding object.
        /// </summary>
        /// <param name="collisionPosition">The player-orientet position of the collision.</param>
        /// <param name="collisionRect">The rectangle of the collision between the player and the colliding object.</param>
        public void HandleCollision(EnumDirection collisionPosition, Rectangle collisionRect)
        {
            switch (collisionPosition)
            {
                case EnumDirection.Left:
                    coords.X = coords.X + collisionRect.Width;
                    break;
                case EnumDirection.LeftUp:
                    if (collisionRect.Width > collisionRect.Height)
                    {
                        coords.Y = coords.Y + collisionRect.Height;
                    }
                    else
                        coords.X = coords.X + collisionRect.Width;
                    break;
                case EnumDirection.LeftDown:
                    if (collisionRect.Width > collisionRect.Height)
                    {
                        coords.Y = coords.Y - collisionRect.Height;
                    }
                    else
                        coords.X = coords.X + collisionRect.Width;
                    break;
                case EnumDirection.Right:
                    coords.X = coords.X - collisionRect.Width;
                    break;
                case EnumDirection.RightUp:
                    if (collisionRect.Width > collisionRect.Height)
                    {
                        coords.Y = coords.Y + collisionRect.Height;
                    }
                    else
                        coords.X = coords.X - collisionRect.Width;
                    break;
                case EnumDirection.RightDown:
                    if (collisionRect.Width > collisionRect.Height)
                    {
                        coords.Y = coords.Y - collisionRect.Height;
                    }
                    else
                        coords.X = coords.X - collisionRect.Width;
                    break;
                case EnumDirection.Up:
                    coords.Y = coords.Y + collisionRect.Height;
                    break;
                case EnumDirection.Down:
                    coords.Y = coords.Y - collisionRect.Height;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Depending on wether the Player is Invulnerable, sets the alpha of the Players current Image and renders it to the Graphics controller.
        /// </summary>
        /// <param name="dc">Graphics controller.</param>
        public override void Render(Graphics dc)
        {
            if (this.InvulnerableUntill > DateTime.Now)
            {
                // Set colormatrix33(alpha) if player is invulnerable
                this.colorM.Matrix33 = (this.colorM.Matrix33 == 1.0f) ? 0.2f : 1.0f;
            }
            else
            {
                this.colorM.Matrix33 = 1.0f;
            }

            this.imgA.SetColorMatrix(this.colorM);

            dc.DrawImage(picture, new Rectangle(new Point(coords.X - (picture.Width / 2), coords.Y - (picture.Height / 2)), picture.Size), 0, 0, picture.Width, picture.Height, GraphicsUnit.Pixel, this.imgA);
        }

        /// <summary>
        /// Based on the current orientation and position, determines wether a specific GameObject is attackable.
        /// </summary>
        /// <param name="enemy">The GameObject to determine if is attackable.</param>
        /// <returns>Returns true if the GameObject is attackable.</returns>
        public bool Attackable(GameObject enemy)
        {
            bool returnVal = false;
            Point offset = new Point(enemy.Coords.X - coords.X, enemy.Coords.Y - coords.Y);
            int meleeRange = 55;

            // Sets the returnValue to true, if the enemy is attackable.
            if (offset.X <= 0 && offset.Y <= 0 && Math.Abs(offset.X) >= Math.Abs(offset.Y) && offset.X >= -meleeRange)
            {
                // Left up
                returnVal = (lookDirection == EnumDirection.Left || lookDirection == EnumDirection.Up) ? true : false;
            }
            else if (offset.X <= 0 && offset.Y >= 0 && Math.Abs(offset.X) >= Math.Abs(offset.Y) && offset.X >= -meleeRange)
            {
                // Left down
                returnVal = (lookDirection == EnumDirection.Left || lookDirection == EnumDirection.Down) ? true : false;
            }
            else if (offset.X >= 0 && offset.Y >= 0 && Math.Abs(offset.X) >= Math.Abs(offset.Y) && offset.X <= meleeRange)
            {
                // Right down
                returnVal = (lookDirection == EnumDirection.Right || lookDirection == EnumDirection.Down) ? true : false;
            }
            else if (offset.X >= 0 && offset.Y <= 0 && Math.Abs(offset.X) >= Math.Abs(offset.Y) && offset.X <= meleeRange)
            {
                // Right up
                returnVal = (lookDirection == EnumDirection.Right || lookDirection == EnumDirection.Up) ? true : false;
            }
            else if (offset.X >= 0 && offset.Y <= 0 && Math.Abs(offset.X) <= Math.Abs(offset.Y) && offset.Y >= -meleeRange)
            {
                // Up right
                returnVal = (lookDirection == EnumDirection.Up || lookDirection == EnumDirection.Right) ? true : false;
            }
            else if (offset.X <= 0 && offset.Y <= 0 && Math.Abs(offset.X) <= Math.Abs(offset.Y) && offset.Y >= -meleeRange)
            {
                // Up left
                returnVal = (lookDirection == EnumDirection.Up || lookDirection == EnumDirection.Left) ? true : false;
            }
            else if (offset.X >= 0 && offset.Y >= 0 && Math.Abs(offset.X) <= Math.Abs(offset.Y) && offset.Y <= meleeRange)
            {
                // Down right
                returnVal = (lookDirection == EnumDirection.Down || lookDirection == EnumDirection.Right) ? true : false;
            }
            else if (offset.X <= 0 && offset.Y >= 0 && Math.Abs(offset.X) <= Math.Abs(offset.Y) && offset.Y <= meleeRange)
            {
                // Down left
                returnVal = (lookDirection == EnumDirection.Down || lookDirection == EnumDirection.Left) ? true : false;
            }
            
            return returnVal;
        }

        /// <summary>
        /// If a inventory slot is empty, sets a specific Item to that slot. Also changes the state of the Item to ItemState.Inventory.
        /// </summary>
        /// <param name="itemToLoot">The Item to set to a inventory slot.</param>
        /// <returns>Returns true if the Item was set to a inventory slot.</returns>
        public bool Lootable(Item itemToLoot)
        {
            for (int row = 0; row < this.inventory.Length; row++)
            {
                for (int element = 1; element < this.inventory[row].Length; element++)
                {
                    if (element > 2)
                    {
                        // We are in a inventory slot.
                        if (this.inventory[row][element] == null)
                        {
                            // The slot is empty.
                            itemToLoot.Coords = this.inventoryPositions[row][element];
                            this.inventory[row][element] = itemToLoot;
                            itemToLoot.MyState = ItemState.Inventory;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines wether an item in the inventory is of a specific ItemType.
        /// </summary>
        /// <param name="it">The ItemType to search for.</param>
        /// <returns>Returns true if the inventory contains an Item with the ItemType.</returns>
        public bool ScanInventoryForType(ItemType it)
        {
            for (int row = 0; row < this.inventory.Length; row++)
            {
                for (int element = 1; element < this.inventory[row].Length; element++)
                {
                    if (element > 2)
                    {
                        // We are in a inventory slot.
                        if (this.inventory[row][element] != null && this.inventory[row][element].MyType == it)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines wether the selected Item is equuippable.
        /// If the ItemType of the Item is already equipped, it attempts to unequip the selected item.
        /// </summary>
        public void UseInventoryItem()
        {
            Item tmpSelectedItem = this.inventory[this.invSelecRow][this.invSelecElement];

            if (this.inventory[this.invSelecRow][this.invSelecElement] != null)
            {
                // The selected inventory slot is not empty.
                if (this.inventory[this.invSelecRow][this.invSelecElement].MyState == ItemState.Inventory)
                {
                    // The selected item's ItemState is ItemState.Iventory.
                    switch (this.inventory[this.invSelecRow][this.invSelecElement].MyType)
                    {
                        // Equip + add bonus'
                        case ItemType.Head:
                            if (this.inventory[0][1] == null)
                            {
                                // ItemType is equippable(nothing in its place).
                                tmpSelectedItem.Coords = this.inventoryPositions[0][1];
                                this.inventory[0][1] = tmpSelectedItem;
                                this.EquipInventoryItem(tmpSelectedItem);
                            }
                            else { NotAbleToEquip(tmpSelectedItem); }
                            break;
                        case ItemType.Weapon:
                            if (this.inventory[1][0] == null) 
                            {
                                // ItemType is equippable(nothing in its place).
                                tmpSelectedItem.Coords = this.inventoryPositions[1][0];
                                this.inventory[1][0] = tmpSelectedItem;
                                this.EquipInventoryItem(tmpSelectedItem);
                            }
                            else { NotAbleToEquip(tmpSelectedItem); }
                            break;
                        case ItemType.Chest:
                            if (this.inventory[1][1] == null) 
                            {
                                // ItemType is equippable(nothing in its place).
                                tmpSelectedItem.Coords = this.inventoryPositions[1][1];
                                this.inventory[1][1] = tmpSelectedItem;
                                this.EquipInventoryItem(tmpSelectedItem);
                            }
                            else { NotAbleToEquip(tmpSelectedItem); }
                            break;
                        case ItemType.Gloves:
                            if (this.inventory[1][2] == null) 
                            {
                                // ItemType is equippable(nothing in its place).
                                tmpSelectedItem.Coords = this.inventoryPositions[1][2];
                                this.inventory[1][2] = tmpSelectedItem;
                                this.EquipInventoryItem(tmpSelectedItem);
                            }
                            else { NotAbleToEquip(tmpSelectedItem); }
                            break;
                        case ItemType.Boots:
                            if (this.inventory[2][1] == null) 
                            {
                                // ItemType is equippable(nothing in its place).
                                tmpSelectedItem.Coords = this.inventoryPositions[2][1];
                                this.inventory[2][1] = tmpSelectedItem;
                                this.EquipInventoryItem(tmpSelectedItem);
                            }
                            else { NotAbleToEquip(tmpSelectedItem); }
                            break;
                        case ItemType.HealthCake:
                            health += 20;
                            health = health > 100 ? 100 : health;
                            this.NotAbleToEquip(tmpSelectedItem);
                            break;
                    }
                }
                else 
                {
                    // Item is equipped - Unequip
                    this.NotAbleToEquip(tmpSelectedItem);
                }
            }
        }
        
        /// <summary>
        /// Draws the inventory, the Player's stats, the selected Item's stats and all of the Item icons.
        /// </summary>
        /// <param name="dc">Graphics controller.</param>
        public void DrawCharacterSheet(Graphics dc)
        {
            dc.FillRectangle(new SolidBrush(Color.FromArgb(190, Color.Black)), 0, 0, 700, 700);
            dc.DrawImage(this.inventoryImage, 0, 0, this.inventoryImage.Width, this.inventoryImage.Height);

            // Draw the Player's stats.
            dc.DrawString(string.Format("Damage: {0}-{1}{4}Crit%: {2}{4}Block%: {3}", (damage / 3), damage, crit, block, Environment.NewLine), new Font(new FontFamily("Arial"), 12, FontStyle.Bold), new SolidBrush(Color.White), 58, 405);

            Item selectedItem = this.inventory[this.invSelecRow][this.invSelecElement];
            if (selectedItem != null)
            {
                // Draw stats for the selected item.
                dc.DrawString(string.Format("Damage: {0}{3}Crit%: {1}{3}Block%: {2}", selectedItem.DamageBonus, selectedItem.CritBonus, selectedItem.BlockBonus, Environment.NewLine), new Font(new FontFamily("Arial"), 12, FontStyle.Bold), new SolidBrush(Color.White), 340, 405);
            }

            // Draw selection rectangle
            dc.DrawRectangle(new Pen(Color.Yellow, 5), new Rectangle(this.inventoryPositions[this.invSelecRow][this.invSelecElement].X - 32, this.inventoryPositions[this.invSelecRow][this.invSelecElement].Y - 32, 64, 64));

            foreach (Item[] row in this.inventory)
            {
                foreach (Item element in row)
                {
                    if (element != null)
                    {
                        element.Render(dc);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the dialogbox and the text of the Obstacle, the player is interacting with.
        /// </summary>
        /// <param name="dc">Graphics controller.</param>
        public void DrawDialogBox(Graphics dc)
        {
            Rectangle collisionRect = Rectangle.Intersect(this.MyRectangle(), this.inDialogWith.MyRectangle());
            if (collisionRect.Width == 0 && collisionRect.Height == 0)
            {
                this.inDialogWith = null;
            }
            else
            {
                Obstacle tmpDialogObj = (Obstacle)this.inDialogWith;
                dc.DrawImage(this.dialogImage, 0, 0, this.dialogImage.Width, this.dialogImage.Height);
                dc.DrawString(tmpDialogObj.ActiveResponse, new Font(new FontFamily("Arial"), 18, FontStyle.Bold), new SolidBrush(Color.White), 50, 430);           
            }
        }

        /// <summary>
        /// Writes Player specific attributes to the Xml writer.
        /// </summary>
        /// <param name="writer">An XmlWriter to write attributes to.</param>
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("StatDamage", this.damage.ToString());
            writer.WriteAttributeString("StatBlock", this.block.ToString());
            writer.WriteAttributeString("StatCrit", this.crit.ToString());
            writer.WriteAttributeString("drawCharSheet", this.DrawInventory.ToString());
        }

        /// <summary>
        /// Reads Player specific attributes from the Xml reader.
        /// </summary>
        /// <param name="reader">An XmlReader to read the attributes from.</param>
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            base.ReadXml(reader);
            this.damage = int.Parse(reader.GetAttribute("StatDamage"));
            this.block = int.Parse(reader.GetAttribute("StatBlock"));
            this.crit = int.Parse(reader.GetAttribute("StatCrit"));
            this.DrawInventory = reader.GetAttribute("drawCharSheet") == "True" ? true : false;
        }

        /// <summary>
        /// Equips a specific Item to the selected inventory slot.
        /// </summary>
        /// <param name="itemToEquip">The Item to equip</param>
        private void EquipInventoryItem(Item itemToEquip)
        {
            this.inventory[this.invSelecRow][this.invSelecElement] = null; // The selected slot is cleared.
            itemToEquip.MyState = ItemState.Equipped;
            this.damage += itemToEquip.DamageBonus;
            this.block += itemToEquip.BlockBonus;
            this.crit += itemToEquip.CritBonus;
        }

        /// <summary>
        /// Clears the selected inventory slot and unequips the Item if it is currently equipped.
        /// </summary>
        /// <param name="it">The Item to unequip.</param>
        private void NotAbleToEquip(Item it)
        {
            if (it.MyState == ItemState.Equipped)
            {
                damage -= it.DamageBonus;
                block -= it.BlockBonus;
                crit -= it.CritBonus;
                this.Lootable(it);
            }

            this.inventory[this.invSelecRow][this.invSelecElement] = null; // The selected slot is cleared.
        }
    }
}