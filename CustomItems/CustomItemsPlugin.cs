using BepInEx;
using HarmonyLib;
using CustomItems.Registry;

namespace CustomItems
{
    [BepInPlugin("me.vaan.customitemplugin", "CustomItems", "1.0.0")]
    public class CustomItemsPlugin : BaseUnityPlugin
    {

        public void Awake()
        {
            SpriteRegistry.Load();

            Harmony harmony = new Harmony("me.vaan.customitemplugin");
            harmony.PatchAll();
        }

        public void OnDisable()
        {
            ItemRegistry.SaveConfigs();
        }
    }
}

