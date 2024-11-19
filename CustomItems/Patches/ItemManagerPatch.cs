using CustomItems.Registry;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CustomItems
{
    [HarmonyPatch(typeof(ItemManager))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    internal class ItemManagerPatch
    {
        public static ItemManager itemManager;

        public static void Postfix(ItemManager __instance)
        {
            Debug.Log("Init start");
            itemManager = __instance;
            ItemRegistry.Init();

            var items = GetItems();
            foreach (InventoryItem item in items) 
            {
                /* no way in hell am saving them as item_543 that is useless
                   if some devs ends up reading this PLEASE, consider using an hashmap
                   for storing the data, also consider using a namespacekey like
                   
                   base:effect_type:name
                   
                   so:

                   base:item:hero_shield
                   base:oil:attack
                   base:edge:agile

                   for my use case string -> InventoryItem is good enough
                 */
                var savedKey = item.effectName.Replace(' ', '_').ToLower();
                if(ItemRegistry.existingItems.ContainsKey(savedKey))
                {
                    Debug.LogError($"KEY COLLISION FOR {savedKey}: {item.nameTag} - {ItemRegistry.existingItems[savedKey].nameTag}");
                }
                ItemRegistry.existingItems[savedKey] = item;
                //Debug.Log($"Read tag {item.nameTag} : Display {item.effectName} : Saved as {savedKey}");
            }
            items.AddRange(ItemRegistry.addedItems.Values);
            SetItems(items);
        }

        public static List<InventoryItem> GetItems()
        {
            if (itemManager == null)
            {
                throw new System.Exception("Cannot execute ItemManagerPatch#GetItems() yet");
            }

            FieldInfo itemsField = typeof(ItemManager).GetField("workingItems", BindingFlags.NonPublic | BindingFlags.Instance);
            if (itemsField == null)
            {
                throw new System.Exception("Getting items field of ItemManager returns null");
            }

            List<InventoryItem> items = (List<InventoryItem>) itemsField.GetValue(itemManager);
            if (items == null)
            {
                throw new System.Exception("Getting items of ItemManager field returns null");
            }

            return items;
        }

        public static void SetItems(List<InventoryItem> list)
        {
            if (itemManager == null)
            {
                throw new System.Exception("Cannot execute ItemManagerPatch#GetItems() yet");
            }

            FieldInfo itemsField = typeof(ItemManager).GetField("workingItems", BindingFlags.NonPublic | BindingFlags.Instance);
            if (itemsField == null)
            {
                throw new System.Exception("Getting items field of ItemManager returns null");
            }

            itemsField.SetValue(itemManager, list);
        }
    }
}
