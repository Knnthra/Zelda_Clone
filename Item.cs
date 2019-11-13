namespace EksamenJuni2010
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Types of Items.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Represents an equippable head Item.
        /// </summary>
        Head,

        /// <summary>
        /// Represents an equippable weapon Item.
        /// </summary>
        Weapon, 

        /// <summary>
        /// Represents an equippable chest Item.
        /// </summary>
        Chest,

        /// <summary>
        /// Represents an equippable glove Item.
        /// </summary>
        Gloves,

        /// <summary>
        /// Represents an equippable boot item.
        /// </summary>
        Boots,

        /// <summary>
        /// Represents a non-equippable, consumable.
        /// </summary>
        HealthCake,

        /// <summary>
        /// Represents a non-equippable, non-consumable quest map-Item.
        /// </summary>
        QuestItemMap,

        /// <summary>
        /// Represents a non-equippable, non-consumable quest key-Item.
        /// </summary>
        QuestItemKey,

        /// <summary>
        /// Represents a non-equippable, non-consumable quest crystal-Item.
        /// </summary>
        QuestItemCrystal
    }

    /// <summary>
    /// Types of states an Item can be in.
    /// </summary>
    public enum ItemState
    {
        /// <summary>
        /// Represents an equipped Item.
        /// </summary>
        Equipped,

        /// <summary>
        /// Represents a non-equipped inventory Item.
        /// </summary>
        Inventory,

        /// <summary>
        /// Represents a non-looted Item.
        /// </summary>
        Dropped
    }

    /// <summary>
    /// Represents a collidable Item of the type GameObject.
    /// </summary>
    public class Item : GameObject
    {
        /// <summary>
        /// Represents the color(quality) of the Item.
        /// </summary>
        private Color myQuality;

        /// <summary>
        /// Represents the ItemType of the Item.
        /// </summary>
        private ItemType myType;

        /// <summary>
        /// Represents the Items current ItemState.
        /// </summary>
        private ItemState myState = ItemState.Dropped;

        /// <summary>
        /// Represents the damage bonus of the Item.
        /// </summary>
        private int damageBonus;

        /// <summary>
        /// Represents the block bonus(procent) of the Item.
        /// </summary>
        private int blockBonus;

        /// <summary>
        /// Represents the crit bonus(procent) of the item.
        /// </summary>
        private int critBonus;

        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        /// <param name="reader">An XmlReader to read and set the attributes from.</param>
        public Item(System.Xml.XmlReader reader)
        {
            picture = Image.FromFile(string.Format("graphics{0}icons{0}loot.png", dirSeparator));

            this.ReadXml(reader);

            #region Get Itemtype
            switch (reader.GetAttribute("itemType"))
            {
                case "Head":
                    this.myType = ItemType.Head;
                    break;
                case "Weapon":
                    this.myType = ItemType.Weapon;
                    break;
                case "Boots":
                    this.myType = ItemType.Boots;
                    break;
                case "Chest":
                    this.myType = ItemType.Chest;
                    break;
                case "Gloves":
                    this.myType = ItemType.Gloves;
                    break;
                case "HealthCake":
                    this.myType = ItemType.HealthCake;
                    break;
                case "QuestItemCrystal":
                    this.myType = ItemType.QuestItemCrystal;
                    break;
                case "QuestItemKey":
                    this.myType = ItemType.QuestItemKey;
                    break;
                case "QuestItemMap":
                    this.myType = ItemType.QuestItemMap;
                    break;
            }
            #endregion

            #region Get ItemQuality
            switch (reader.GetAttribute("itemQuality"))
            {
                case "Color [Green]":
                    this.myQuality = Color.Green;
                    break;
                case "Color [Blue]":
                    this.myQuality = Color.Blue;
                    break;
                case "Color [Purple]":
                    this.myQuality = Color.Purple;
                    break;
            }
            #endregion

            #region Get ItemState
            switch (reader.GetAttribute("itemState"))
            {
                case "Dropped":
                    this.myState = ItemState.Dropped;
                    break;
                case "Inventory":
                    this.myState = ItemState.Inventory;
                    break;
                case "Equipped":
                    this.myState = ItemState.Equipped;
                    break;
            }
            #endregion

            this.damageBonus = int.Parse(reader.GetAttribute("damageBonus"));
            this.critBonus = int.Parse(reader.GetAttribute("critBonus"));
            this.blockBonus = int.Parse(reader.GetAttribute("blockBonus"));
        }

        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        /// <param name="position">The Point position of the Item.</param>
        /// <param name="type">The ItemType type of the Item.</param>
        public Item(Point position, ItemType type)
        {
            this.myType = type;
            coords = position;
            picture = Image.FromFile(string.Format("graphics{0}icons{0}loot.png", dirSeparator));
        }

        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        /// <param name="position">The Point position of the Item.</param>
        /// <param name="rnd">An already instansiated, random-number generator.</param>
        public Item(Point position, Random rnd)
        {
            #region Randomize type and stat
            int rndType = rnd.Next(0, 5);
            switch (rndType)
            {
                case 0:
                    this.myType = ItemType.Boots;
                    break;
                case 1:
                    this.myType = ItemType.Chest;
                    break;
                case 2:
                    this.myType = ItemType.Gloves;
                    break;
                case 3:
                    this.myType = ItemType.Head;
                    break;
                case 4:
                    this.myType = ItemType.Weapon;
                    break;
            }

            int lootQuality = rnd.Next(0, 101);
            if (lootQuality <= 55)
            {
                this.damageBonus = rnd.Next(2, 5);
                this.blockBonus = rnd.Next(1, 1);
                this.critBonus = rnd.Next(0, 1);
                this.myQuality = Color.Green;
            }
            else if (lootQuality <= 90)
            {
                this.damageBonus = rnd.Next(2, 10);
                this.blockBonus = rnd.Next(2, 5);
                this.critBonus = rnd.Next(0, 3);
                this.myQuality = Color.Blue;
            }
            else if (lootQuality <= 100)
            {
                this.damageBonus = rnd.Next(5, 10);
                this.blockBonus = rnd.Next(6, 8);
                this.critBonus = rnd.Next(3, 5);
                this.myQuality = Color.Purple;
            }
            #endregion

            coords = position;
            coords.Y += rnd.Next(5, 21);
            coords.X += rnd.Next(5, 21);

            picture = Image.FromFile(string.Format("graphics{0}icons{0}loot.png", dirSeparator));
        }

        /// <summary>
        /// Gets the ItemType of this Item.
        /// </summary>
        public ItemType MyType
        {
            get { return this.myType; }
        }

        /// <summary>
        /// Gets or sets the ItemState of this Item.
        /// </summary>
        public ItemState MyState
        {
            get { return this.myState; }
            set { this.myState = value; }
        }

        /// <summary>
        /// Gets the damage bonus of this Item.
        /// </summary>
        public int DamageBonus
        {
            get { return this.damageBonus; }
        }

        /// <summary>
        /// Gets the block bonus of this Item.
        /// </summary>
        public int BlockBonus
        {
            get { return this.blockBonus; }
        }

        /// <summary>
        /// Gets the crit bonus of this Item.
        /// </summary>
        public int CritBonus
        {
            get { return this.critBonus; }
        }

        /// <summary>
        /// Renders a specific Image, based on the ItemType.
        /// </summary>
        /// <param name="dc">Graphics controller.</param>
        public override void Render(Graphics dc)
        {
            if (this.myState != ItemState.Dropped)
            {
                switch (this.myType)
                {
                    case ItemType.Head:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}head.png", dirSeparator));
                        break;
                    case ItemType.Weapon:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}weapon.png", dirSeparator));
                        break;
                    case ItemType.Chest:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}chest.png", dirSeparator));
                        break;
                    case ItemType.Gloves:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}gloves.png", dirSeparator));
                        break;
                    case ItemType.Boots:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}boots.png", dirSeparator));
                        break;
                    case ItemType.HealthCake:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}healthcake.png", dirSeparator));
                        break;
                    case ItemType.QuestItemCrystal:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}questitem_crystal.png", dirSeparator));
                        break;
                    case ItemType.QuestItemKey:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}questitem_key.png", dirSeparator));
                        break;
                    case ItemType.QuestItemMap:
                        picture = Image.FromFile(string.Format("graphics{0}icons{0}questitem_map.png", dirSeparator));
                        break;
                }
            }
            
            base.Render(dc);

            if (this.myState != ItemState.Dropped)
            {
                dc.DrawRectangle(new Pen(this.myQuality, 2), new Rectangle(coords.X - 32, coords.Y - 32, 64, 64));
            }
        }

        /// <summary>
        /// Creates a rectangle based on the GameObjects location and Image size.
        /// </summary>
        /// <returns>A Rectangle based on the GameObjects location and Image size.</returns>
        public override Rectangle MyRectangle()
        {
            return new Rectangle(coords.X - (picture.Width / 2), coords.Y - (picture.Width / 2), picture.Width, picture.Height);
        }

        /// <summary>
        /// Writes Item specific attributes to the Xml writer.
        /// </summary>
        /// <param name="writer">An XmlWriter to write attributes to.</param>
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("itemQuality", this.myQuality.ToString());
            writer.WriteAttributeString("itemType", this.myType.ToString());
            writer.WriteAttributeString("itemState", this.myState.ToString());
            writer.WriteAttributeString("damageBonus", this.damageBonus.ToString());
            writer.WriteAttributeString("blockBonus", this.blockBonus.ToString());
            writer.WriteAttributeString("critBonus", this.critBonus.ToString());
        }
    }
}