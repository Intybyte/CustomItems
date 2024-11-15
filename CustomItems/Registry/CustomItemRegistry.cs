using BepInEx;
using CustomItems.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CustomItems
{
    public sealed class CustomItemRegistry
    {
        public static readonly Dictionary<string, BaseCustomItem> addedItems = new Dictionary<string, BaseCustomItem>();
        public static Dictionary<string, bool> enabledItems = ReadConfigs();
        public static event Action OnCustomItemRegistryInit;

        public static void Init()
        {

            ScriptableObject.CreateInstance<GlassShield>()
                .Identify("glass_shield", "Glass Shield", "When hit deal the same damage to your enemy. Works only on first turn.")
                .Define(ContentBundle.WOODLAND, ItemRarity.COMMON, ItemType.INVENTORY)
                .Tags(ItemTag.STONE)
                .Sprite("Glass Shield")
                .GoldTint(0, -32, -86)
                .DiamondTint(-86, -86, 0)
                .Register();

            ScriptableObject.CreateInstance<GoldSword>()
                .Identify("gold_sword", "Gold Sword", "Deals 1 damage for each 10 gold owned.")
                .Define(ContentBundle.WOODLAND, ItemRarity.HEROIC, ItemType.WEAPON)
                .Tags(ItemTag.UNIQUE)
                .Stats(0, 1)
                .Sprite("Golden Sword")
                .Register();

            OnCustomItemRegistryInit?.Invoke();
        }

        public static Dictionary<string, bool> ReadConfigs()
        {

            Dictionary<string, bool> result = new Dictionary<string, bool>();
            string config = Path.Combine(Paths.ConfigPath, "CustomItems_EnabledItems.cfg");
            if (!File.Exists(config)) 
            {
                return result;
            }

            Debug.Log("Reading config file CustomItems_EnabledItems.cfg");
            string contents = File.ReadAllText(config);
            string[] lines = contents.Split('\n');


            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                string[] keyValue = line.Split(new char[] { '=' }, 2);

                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    Debug.Log($"Reading Key: {key}, Value: {value}");
                    result[key] = bool.Parse(value);
                }
            }

            return result;
        }

        public static void SaveConfigs()
        {
            string config = Path.Combine(Paths.ConfigPath, "CustomItems_EnabledItems.cfg");

            using (StreamWriter writer = new StreamWriter(config, append: false))
            {
                foreach(BaseCustomItem item in addedItems.Values) 
                {
                    string key = item.nameTag;
                    bool value = enabledItems.ContainsKey(key) ? enabledItems[key] : true;
                    writer.WriteLine($"{key}={value}");             
                }
            }
        }
    }
}
