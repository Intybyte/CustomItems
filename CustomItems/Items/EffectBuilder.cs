using CustomItems.Registry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomItems.Items
{
    internal class EffectBuilder
    {
        private int maxHealth = 0, attack = 0, armor = 0, speed = 0;

        //these are required
        private Sprite sprite, goldenSprite, diamondSprite;
        private List<ItemTag> itemTags;
        private ContentBundle contentBundle;
        private ItemRarity itemRarity;
        private ItemType itemType;
        private string effectName, nameTag, effectDesc;

        public EffectBuilder Stats(int maxHealth = 0, int attack = 0, int armor = 0, int speed = 0)
        {
            this.maxHealth = maxHealth;
            this.attack = attack;
            this.armor = armor;
            this.speed = speed;

            return this;
        }

        public EffectBuilder Sprite(string normal, string golden = null, string diamond = null)
        {
            this.sprite = SpriteRegistry.sprites[normal];

            this.goldenSprite = golden == null ? null : SpriteRegistry.sprites[golden];

            this.diamondSprite = diamond == null ? null : SpriteRegistry.sprites[diamond];

            return Sprite(this.sprite, this.goldenSprite, this.diamondSprite);
        }

        public EffectBuilder Sprite(Sprite normal, Sprite golden = null, Sprite diamond = null)
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

        public EffectBuilder GoldTint(int r, int g, int b)
        {
            this.goldenSprite = this.goldenSprite.AddColor(r, g, b);
            return this;
        }

        public EffectBuilder DiamondTint(int r, int g, int b)
        {
            this.diamondSprite = this.diamondSprite.AddColor(r, g, b);
            return this;
        }

        public EffectBuilder Tags(params ItemTag[] tags)
        {
            this.itemTags = tags.ToList();
            return this;
        }

        public EffectBuilder Define(ContentBundle contentBundle, ItemRarity rarirty, ItemType type)
        {
            this.contentBundle = contentBundle;
            this.itemRarity = rarirty;
            this.itemType = type;
            return this;
        }

        public EffectBuilder Identify(string tag, string name, string desc)
        {
            this.effectName = name;
            this.nameTag = tag;
            this.effectDesc = desc;
            return this;
        }

        public void BuildOn(EffectBase effect)
        {
            effect.maxHealth = this.maxHealth;
            effect.attack = this.attack;
            effect.armor = this.armor;
            effect.speed = this.speed;

            effect.sprite = this.sprite;
            if (effect is InventoryItem item)
            {
                item.goldenSprite = this.goldenSprite;
                item.diamondSprite= this.diamondSprite;
            }


        }
    }
}
