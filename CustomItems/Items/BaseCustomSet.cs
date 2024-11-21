using CustomItems.Registry;
using System.Linq;

namespace CustomItems.Items
{
    public class BaseCustomSet : SetBase
    {
        public BaseCustomSet Ingredients(params InventoryItem[] ingredients)
        { 
            this.itemComponents = ingredients.ToList();

            return this;
        }

        public void Register() 
        {
            var registry = ItemRegistry.Instance;
            if (registry.IsEnabled(nameTag))
            {
                registry.addedSets[nameTag] = this;
            }
        }
    }
}
