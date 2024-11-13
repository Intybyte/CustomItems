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
            CustomItemRegistry.Init();

            Harmony harmony = new Harmony("me.vaan.customitemplugin");
            harmony.PatchAll();
        }

        public void OnDisable()
        {
            CustomItemRegistry.SaveConfigs();
        }
    }
}

