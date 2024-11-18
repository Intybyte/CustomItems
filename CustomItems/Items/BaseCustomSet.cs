using System.Linq;

namespace CustomItems.Items
{
    /// <summary>
    /// A class used to group multimple items, like cauldron recipes
    /// and sets.
    /// </summary>
    public class BaseCustomGroup : BaseCustomItem
    {
        public BaseCustomGroup Ingredients(params InventoryItem[] ingredients)
        { 
            this.itemComponents = ingredients.ToList();

            return this;
        }
    }
}
