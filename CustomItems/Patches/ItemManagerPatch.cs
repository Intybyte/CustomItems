﻿using CustomItems.Registry;
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

            var items = GetItems();
            items.AddRange(CustomItemRegistry.addedItems.Values);
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
