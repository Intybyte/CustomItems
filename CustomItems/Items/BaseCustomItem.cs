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
            if (golden == null) golden = normal;
            if (diamond == null) diamond = normal;

            this.sprite = SpriteRegistry.sprites[normal];
            this.goldenSprite = SpriteRegistry.sprites[golden];
            this.diamondSprite = SpriteRegistry.sprites[diamond];

            return this;
        }

        public BaseCustomItem Sprite(Sprite normal, Sprite golden = null, Sprite diamond = null)
        {
            if (golden == null) golden = normal;
            if (diamond == null) diamond = normal;

            this.sprite = normal;
            this.goldenSprite = golden;
            this.diamondSprite = diamond;

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
            CustomItemRegistry.addedItems[nameTag] = this;
        }
    }
}
