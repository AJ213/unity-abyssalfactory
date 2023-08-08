using Unity.Mathematics;

public class StorageSilo : IBlockEntity, IInventory
{
    public int ID { get; set; }
    public World.Direction Direction { get; set; }
    
    public int3 placePosition;
    public Item[] items;
    public int capacity = 100;
    float3 startPosition;
    float3 endPosition;
    private int3 outputPosition;

    public bool OnCreate(int ID, int3 globalPosition, World.Direction direction)
    {
        this.ID = ID;
        this.placePosition = globalPosition;
        this.Direction = direction;

        float3 dir = (float3)World.GetDirectionVector(direction);
        startPosition = globalPosition + new float3(0.5f) - dir / 2f;
        endPosition = startPosition + dir;
        outputPosition = placePosition + (int3)dir;
        items = new Item[capacity];

        return true;
    }

    public bool Insert(int3 position, Item item){
        if(!placePosition.Equals(position)){
            return false;
        }

        // No inserting yet
        return true;
    }

    // This is only used for removing specific items
    public bool Remove(int3 position, Item item){
        if (!placePosition.Equals(position))
        {
            return false;
        }
        for (int i = 0; i < capacity; i++){
            if(items[i] == item){
                items[i] = null;
                return true;
            }    
        }
        return false;
    }

    public void Update(float fixedDeltaTime)
    {
        //currentTimer += fixedDeltaTime;
        //if(currentTimer < speedPerCapacity){
        //    return;
        //}

        //Item front = itemsOnConveyor[capacity - 1];

        //if (front != null){
        //    IBlockEntity e = Program.CurrentWorld.GetBlockEntity(outputPosition);
        //    if(e != null && e is TubeConveyor){
        //        TubeConveyor outputConveyor = (TubeConveyor)e;
        //        if(outputConveyor.Insert(outputPosition, front)){
        //            itemsOnConveyor[capacity - 1] = null;
        //        }
        //    }
        //}
        //for (int i = capacity-2; i >= 0; i--){
        //    if(itemsOnConveyor[i] == null){
        //        continue;
        //    }

        //    if(itemsOnConveyor[i+1] != null){
        //        continue;
        //    }

        //    itemsOnConveyor[i + 1] = itemsOnConveyor[i];
        //    itemsOnConveyor[i] = null;
        //}
    }
    public void Render(float timeDelta){

    }
}