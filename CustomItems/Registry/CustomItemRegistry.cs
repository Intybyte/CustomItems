using BepInEx;
using CustomItems.Implementations;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CustomItems
{
    public class CustomItemRegistry
    {
        public static readonly Dictionary<string, BaseCustomItem> addedItems = new Dictionary<string, BaseCustomItem>();
        public static readonly Dictionary<string, bool> enabled = ReadConfigs();

        public static void Init()
        {

            var a = ScriptableObject.CreateInstance<GlassShield>()
                .Identify("glass_shield", "Glass Shield", "When hit deal the same damage to your enemy. Works only on first turn.")
                .Define(ContentBundle.WOODLAND, ItemRarity.COMMON, ItemType.INVENTORY)
                .Tags(ItemTag.STONE)
                .Sprite("Glass Shield")
                .GoldTint(0, 0, -86);

            a.Register();
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
            Dictionary<string, bool> toWrite = enabled;
            string config = Path.Combine(Paths.ConfigPath, "CustomItems_EnabledItems.cfg");

            using (StreamWriter writer = new StreamWriter(config, append: false))
            {
                foreach(BaseCustomItem item in addedItems.Values) 
                {
                    string key = item.nameTag;
                    bool value = enabled.ContainsKey(key) ? enabled[key] : true;
                    writer.WriteLine($"{key}={value}");             
                }
            }
        }
    }
}
