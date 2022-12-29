using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class Chunk
{
    public const int SIZE = 8;
    public const int SIZEM1 = SIZE-1;
    public const int SIZE_2D = SIZE * SIZE;
    public const int SIZE_3D = SIZE_2D * SIZE;
    public readonly int3 MID_POINT = new int3(SIZE / 2); // come on... size will always be 2 right?
    int[] blocksIDs = new int[SIZE_3D];
    public Dictionary<int3, IBlockEntity> blockEntities = new Dictionary<int3, IBlockEntity>();

    public Chunk(int3 coord){
        this.Coord = coord;
        this.Center = (coord*SIZE) + MID_POINT;
    }

    public IBlockEntity GetBlockEntity(int3 globalPosition) 
    {
        int3 localPosition = World.GetVoxelLocalPositionInChunk(globalPosition);
        blockEntities.TryGetValue(localPosition, out IBlockEntity blockEntity);
        return blockEntity;
    }
    public void AddBlockEntity(int3 globalPosition, IBlockEntity blockEntity)
    {
        int3 localPosition = World.GetVoxelLocalPositionInChunk(globalPosition);
        blockEntities.Add(localPosition, blockEntity);
    }
    public void SetID(int x, int y, int z, int id) {
        Voxel voxel = Program.GlobalDatabase.GetVoxel(id);
        if(voxel.IsEntity){
            IBlockEntity blockEntity = (IBlockEntity)Activator.CreateInstance(Program.GlobalDatabase.GetBlockEntityType(id));
            blockEntity.OnCreate(id, new int3(x, y, z), World.Direction.Forward);
            blockEntities.Add(new int3(x, y, z), blockEntity);
        }
        blocksIDs[(z * SIZE_2D) + (y * SIZE) + x] = id;
    }
        
    public int GetID(int x, int y, int z) => blocksIDs[(z * SIZE_2D) + (y * SIZE) + x];
    public int3 Coord;
    public float3 Center;
    public Chunk[] neighbors = new Chunk[6];
}
