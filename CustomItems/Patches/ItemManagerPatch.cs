using CustomItems.Registry;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CustomItems
{
    [HarmonyPatch(typeof(ItemManager))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    internal class ItemManagerPatch
    {
        public static ItemManager itemManager;

        public static void Postfix(
            ItemManager __instance, 
            List<InventoryItem> ___workingItems,
            List<SetBase> ___workingSets)
        {
            Debug.Log("Init start");
            itemManager = __instance;
            var registry = ItemRegistry.Instance;

            foreach (InventoryItem item in ___workingItems) 
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
                if(registry.existingItems.ContainsKey(savedKey))
                {
                    Debug.LogError($"KEY COLLISION FOR {savedKey}: {item.nameTag} - {registry.existingItems[savedKey].nameTag}");
                }
                registry.existingItems[savedKey] = item;
                Debug.Log($"Read tag {item.nameTag} : Display {item.effectName} : Saved as {savedKey}");
            }
            
            registry.Init();
            ___workingItems.AddRange(registry.addedItems.Values);
            ___workingSets.AddRange(registry.addedSets.Values);
        }
    }
}
