using System.Collections.Generic;
public class Item {
    public int ID;
    public Dictionary<string, object> data;

    public Item(int id)
    {
        ID = id;
    }

    // Note: Does not implement any NBT data comparision and so forth yet
    public override bool Equals(object obj)
    {
        return obj is Item item &&
            ID == item.ID;
    }

    public override int GetHashCode() => ID;
}
