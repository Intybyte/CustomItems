using BepInEx;
using CustomItems.Implementations;
using CustomItems.Items;
using CustomItems.Utils;
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

            var glassShieldEffects = new EffectBuilder()
                .Identify("glass_shield", "Glass Shield", "When hit deal the same damage to your enemy. Works only on first turn.")
                .ContentBundle(ContentBundle.WOODLAND)
                .Sprite("Glass Shield");

            var glassShield = ScriptableObject.CreateInstance<GlassShield>()
                .DefineKind(ItemRarity.COMMON, ItemType.WEAPON)
                .Tags(ItemTag.STONE);

            glassShieldEffects.BuildOn(glassShield);
            glassShield.Register();

            var goldSwordEffects = new EffectBuilder()
                .Identify("gold_sword", "Gold Sword", "Deals 1 damage for each 10 gold owned.")
                .ContentBundle(ContentBundle.WOODLAND)
                .Stats(0, 1)
                .Sprite("Golden Sword");

            var goldSword = ScriptableObject.CreateInstance<GoldSword>()
                .DefineKind(ItemRarity.HEROIC, ItemType.WEAPON)
                .Tags(ItemTag.UNIQUE);
            
            goldSwordEffects.BuildOn(goldSword);
            goldSword.Register();

            var sanguineSwordEffects = new EffectBuilder()
                .Identify("sanguine_sword", "Sanguine Sword", "Let blood strengthen you.")
                .ContentBundle(ContentBundle.WOODLAND)
                .Stats(10, 1)
                .Sprite("Sanguine Sword");

            var sanguineSword = ScriptableObject.CreateInstance<BaseCustomSet>()
                .DefineKind(ItemRarity.UNOBTAINABLE, ItemType.SET)
                .Ingredients(existingItems["bleeding_edge"], existingItems["heart_drinker"], existingItems["lifeblood_burst"]);

            sanguineSwordEffects.BuildOn(sanguineSword);
            sanguineSword.Register();

            var stoneSkinEffects = new EffectBuilder()
                .Identify("stone_skin", "Stone Skin", "Your skin is turning to stone...")
                .ContentBundle(ContentBundle.WOODLAND)
                .Stats(0, 0, 8, -2)
                .Sprite("Stone Skin");

            var stoneSkin = ScriptableObject.CreateInstance<BaseCustomSet>()
                .DefineKind(ItemRarity.UNOBTAINABLE, ItemType.SET)
                .Ingredients(existingItems["marble_mirror"], existingItems["stone_steak"], existingItems["ironstone_sandals"]);

            stoneSkinEffects.BuildOn(stoneSkin);
            stoneSkin.Register();

            var goldWarriorEffects = new EffectBuilder()
                .ContentBundle(ContentBundle.WOODLAND)
                .Identify("gold_warrior", "Gold Warrior", "Your gold is as important as your life at this point. If you receive a deadly hit you will lose gold instead of dying if possible.");

            var goldWarrior = ScriptableObject.CreateInstance<BaseCustomSet>()
                .DefineKind(ItemRarity.UNOBTAINABLE, ItemType.SET)
                .Ingredients(addedItems["gold_sword"], existingItems["gold_ring"], existingItems["boss_contract"]);

            goldWarriorEffects.BuildOn(goldWarrior);
            goldWarrior.Register();

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
