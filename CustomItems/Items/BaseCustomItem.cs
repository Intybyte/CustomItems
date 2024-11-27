using CustomItems.Registry;

namespace CustomItems.Items
{
    public class BaseCustomItem : InventoryItem
    {

        public virtual void Register() {
            var registry = ItemRegistry.Instance;
            if (registry.IsEnabled(nameTag))
            {
                registry[nameTag] = this;
            }
        }
    }
}
