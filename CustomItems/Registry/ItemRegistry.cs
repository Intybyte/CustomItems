using BepInEx;
using CustomItems.Implementations;
using CustomItems.Items;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CustomItems.Registry
{
    public sealed class ItemRegistry
    {
        private ItemRegistry() { }
        private static ItemRegistry _instance = null;

        public readonly Dictionary<string, BaseCustomItem> addedItems = new Dictionary<string, BaseCustomItem>();
        private readonly Dictionary<string, bool> enabledItems = ReadConfigs();
        public readonly Dictionary<string, InventoryItem> existingItems = new Dictionary<string, InventoryItem>();
        
        public readonly Dictionary<string, BaseCustomSet> addedSets = new Dictionary<string, BaseCustomSet>();

        public event Action OnCustomItemRegistryInit;

        public static ItemRegistry Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = new ItemRegistry();
                return _instance;
            }
        }

        public InventoryItem this[string index, bool added = true]
        {
            get
            {
                if (added) 
                {
                    return addedItems[index];
                }
                else
                {
                    return existingItems[index];
                }
            }

            set
            {
                if (added)
                {
                    addedItems[index] = value as BaseCustomItem;
                }
                else
                {
                    existingItems[index] = value;
                }
            }
        }

        public bool IsEnabled(string index)
        {
            if (!enabledItems.ContainsKey(index))
            {
                // assume enabled
                return true;
            }

            return enabledItems[index];
        }

        public void Init()
        {

            ScriptableObject.CreateInstance<GlassShield>()
                .Identify("glass_shield", "Glass Shield", "When hit deal the same damage to your enemy. Works only on first turn.")
                .Define(ContentBundle.WOODLAND, ItemRarity.COMMON, ItemType.INVENTORY)
                .Tags(ItemTag.STONE)
                .Sprite("Glass Shield")
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
            var instance = ItemRegistry.Instance;
            using (StreamWriter writer = new StreamWriter(config, append: false))
            {
                foreach(BaseCustomItem item in instance.addedItems.Values) 
                {
                    string key = item.nameTag;
                    bool value = instance.IsEnabled(key);
                    writer.WriteLine($"{key}={value}");
                }
            }
        }
    }
}
