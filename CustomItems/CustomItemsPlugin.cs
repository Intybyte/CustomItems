using BepInEx;
using CustomItems.Implementations;
using HarmonyLib;
using UnityEngine;
using CustomItems.Registry;

namespace CustomItems
{
    [BepInPlugin("me.vaan.customitemplugin", "CustomItems", "1.0.0")]
    public class CustomItemsPlugin : BaseUnityPlugin
    {

        public void Awake()
        {
            SpriteRegistry.Load();
            var added = ScriptableObject.CreateInstance<GlassShield>()
                .Identify("glass_shield", "Glass Shield", "When hit deal the same damage to your enemy. Works only on first turn.")
                .Define(ContentBundle.WOODLAND, ItemRarity.COMMON, ItemType.INVENTORY)
                .Tags(ItemTag.EXCLUSIVE)
                .Sprite("Glass Shield");

            //region stats
            added.Register();
            //endregion

            Harmony harmony = new Harmony("me.vaan.customitemplugin");
            harmony.PatchAll();
        }
    }
}

