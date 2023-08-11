using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using UnityEngine;

public class TubeConveyor : IBlockEntity, IInventory
{
    public Voxel Voxel { get; set; }
    public EntityRegion EntityRegion { get; set; }
    public World.Direction Direction { get; set; }
    public int3 VecDirection { get; private set; }

    public float Speed => speedPerCapacity;
    public int Capacity => capacity;
    private int3 placePosition;
    private Item[] itemsOnConveyor;
    int itemCount = 0;
    private int capacity = 5;
    private float speed = 5;
    private int speedPerCapacity;
    private float3 startPosition;
    private float3 endPosition;
    private int currentTicks = 0;
    private int3 outputPosition;

    public bool OnCreate(Voxel voxel, EntityRegion region, World.Direction direction)
    {
        this.Voxel = voxel;
        this.placePosition = region.Corner;
        EntityRegion = region;
        this.Direction = direction;

        VecDirection = World.GetDirectionVector(direction);
        float3 dir = VecDirection;
        startPosition = region.Corner + new float3(0.5f) - dir / 2f;
        endPosition = startPosition + dir;
        outputPosition = placePosition + VecDirection;
        itemsOnConveyor = new Item[capacity];

        speedPerCapacity = (int)((speed / capacity)/Time.fixedDeltaTime);
        
        currentTicks = 0;
        return true;
    }

    public bool Insert(int3 position, Item item){
        if(!placePosition.Equals(position)){
            return false;
        }
        if(itemsOnConveyor[0] != null){
            return false;
        }
        itemsOnConveyor[0] = item;
        itemCount++;
        return true;
    }

    // This is only used for removing specific items
    public bool Remove(int3 position, Item item){
        if (!placePosition.Equals(position))
        {
            return false;
        }
        for (int i = 0; i < capacity; i++){
            if(itemsOnConveyor[i].Equals(item)){
                itemsOnConveyor[i] = null;
                itemCount--;
                return true;
            }    
        }
        return false;
    }
    public IEnumerable<Item> GetItems() => itemsOnConveyor;
    public bool Contains(Item item) => itemsOnConveyor.Contains(item);
    
    public void Update(float fixedDeltaTime)
    {
        if (itemCount == 0)
        {
            currentTicks = 0;
            return;
        }
        
        currentTicks += 1;
        if(currentTicks < speedPerCapacity){
            return;
        }
        currentTicks = 0;
        Item front = itemsOnConveyor[capacity - 1];

        if (front != null){
            IBlockEntity e = Program.CurWorld.GetBlockEntity(outputPosition);
            if(e is IInventory inventory){
                if(inventory.Insert(outputPosition, front)){
                    itemsOnConveyor[capacity - 1] = null;
                    itemCount--;
                }
            }
        }
        for (int i = capacity-2; i >= 0; i--){
            if(itemsOnConveyor[i] == null){
                continue;
            }

            if(itemsOnConveyor[i+1] != null){
                continue;
            }

            itemsOnConveyor[i + 1] = itemsOnConveyor[i];
            itemsOnConveyor[i] = null;
        }
    }
    public void Render(float timeDelta){

    }
}