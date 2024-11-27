using System.Linq;

namespace CustomItems.Utils
{
    public static class InventoryItemUtils
    {

        public static T DefineKind<T>(this T item, ItemRarity rarirty, ItemType type) where T : InventoryItem
        {
            item.itemRarity = rarirty;
            item.itemType = type;
            return item;
        }

        public static T Tags<T>(this T item, params ItemTag[] tags) where T : InventoryItem
        {
            item.itemTags = tags.ToList();
            return item;
        }
    }
}
