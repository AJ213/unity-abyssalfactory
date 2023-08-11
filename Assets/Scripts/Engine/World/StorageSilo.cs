using System.Collections;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow.Items;
using Unity.Mathematics;

public class StorageSilo : IBlockEntity, IInventory
{
    public Voxel Voxel { get; set; }
    public EntityRegion EntityRegion { get; set; }
    public World.Direction Direction { get; set; }
    
    private int3 placePosition;
    private List<Item> items;
    private int capacity = 100;

    private List<TubeConveyor> linkedExportingConveyors = new List<TubeConveyor>();

    public bool OnCreate(Voxel voxel, EntityRegion region, World.Direction direction)
    {
        this.Voxel = voxel;
        this.EntityRegion = region;
        this.placePosition = region.Corner;
        this.Direction = direction;

        items = new List<Item>(capacity);

        return true;
    }

    public bool Insert(int3 position, Item item){
        if (items.Count >= capacity)
        {
            return false;
        }
        items.Add(item);
        UpdateLinkedConveyorStatus(); // really want to do on owned chunk update
        // No inserting yet
        return true;
    }

    // This is only used for removing specific items
    public bool Remove(int3 position, Item item){
        return items.Remove(item);
    }
    public IEnumerable<Item> GetItems() => items;
    public bool Contains(Item item) => items.Contains(item);

    public void UpdateLinkedConveyorStatus()
    {
        linkedExportingConveyors.Clear();
        foreach ((int3, int3) posDir in EntityRegion.NearbyRegionPositions())
        {
            TubeConveyor conveyor = Program.CurWorld.GetBlockEntity<TubeConveyor>(posDir.Item1);
            if (conveyor == null) continue;
            if (conveyor.VecDirection.Dot(posDir.Item2) < 0) continue;
            
            linkedExportingConveyors.Add(conveyor);
        }
    }

    public void Update(float fixedDeltaTime)
    {
        UpdateLinkedConveyorStatus();
        if (items.Count <= 0) return;
        foreach (TubeConveyor conveyor in linkedExportingConveyors)
        {
            if (items.Count <= 0) break;
            Item item = items[^1];
            if (conveyor.Insert(conveyor.EntityRegion.Corner,item))
            {
                items.Remove(item);
            }
        }
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