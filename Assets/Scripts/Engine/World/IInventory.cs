using System.Collections.Generic;
using Unity.Mathematics;
public interface IInventory {
    public bool Insert(int3 position, Item item);
    public bool Remove(int3 position, Item item);

    public IEnumerable<Item> GetItems();
    public bool Contains(Item item);
}
