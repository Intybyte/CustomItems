using CustomItems.Registry;
using System.Linq;
using UnityEngine;

namespace CustomItems
{
    public class BaseCustomItem : InventoryItem
    {

        public BaseCustomItem Stats(int maxHealth = 0, int attack = 0, int armor = 0, int speed = 0)
        {
            this.maxHealth = maxHealth;
            this.attack = attack;
            this.armor = armor;
            this.speed = speed;
            
            return this;
        }

        public BaseCustomItem Sprite(string normal, string golden = null, string diamond = null)
        {
            this.sprite = SpriteRegistry.sprites[normal];

            this.goldenSprite = golden == null ? null : SpriteRegistry.sprites[golden];

            this.diamondSprite = diamond == null ? null : SpriteRegistry.sprites[diamond];

            return Sprite(this.sprite, this.goldenSprite, this.diamondSprite);
        }

        public BaseCustomItem Sprite(Sprite normal, Sprite golden = null, Sprite diamond = null)
        {
            this.sprite = normal;

            if (golden == null)
            {
                this.goldenSprite = this.sprite.SetColor(203, 130, 25);
            }
            else
            {
                this.goldenSprite = golden;
            }

            if (diamond == null)
            {
                this.diamondSprite = this.sprite.SetColor(146, 226, 235);
            }
            else
            {
                this.diamondSprite = diamond;
            }

            return this;
        }

        public BaseCustomItem GoldTint(int r, int g, int b)
        {
            this.goldenSprite = this.goldenSprite.AddColor(r, g, b);
            return this;
        }

        public BaseCustomItem DiamondTint(int r, int g, int b)
        {
            this.diamondSprite = this.diamondSprite.AddColor(r, g, b);
            return this;
        }

        public BaseCustomItem Tags(params ItemTag[] tags)
        { 
            this.itemTags = tags.ToList();
            return this; 
        }

        public BaseCustomItem Define(ContentBundle contentBundle, ItemRarity rarirty, ItemType type) 
        { 
            this.contentBundle = contentBundle;
            this.itemRarity = rarirty;
            this.itemType = type;
            return this; 
        }

        public BaseCustomItem Identify(string tag, string name, string desc) {
            this.effectName = name;
            this.nameTag = tag;
            this.effectDesc = desc;
            return this;
        }
        public void Register() { 
            if (!CustomItemRegistry.enabledItems.ContainsKey(nameTag))
            {
                // assume enabled
                CustomItemRegistry.addedItems[nameTag] = this;
                return;
            }

            if (!CustomItemRegistry.enabledItems[nameTag])
            {
                return;
            }

            CustomItemRegistry.addedItems[nameTag] = this;
        }
    }
}
