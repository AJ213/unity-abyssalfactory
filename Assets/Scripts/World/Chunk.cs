using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class Chunk
{
    public const int SIZE = 32;
    public const int SIZEM1 = SIZE-1;
    public const int SIZE_2D = SIZE * SIZE;
    public const int SIZE_3D = SIZE_2D * SIZE;
    public readonly int3 MID_POINT = new int3(SIZE / 2); // come on... size will always be 2 right?

    int[] blocksIDs = new int[SIZE_3D];
    public Dictionary<int3, IBlockEntity> blockEntities = new Dictionary<int3, IBlockEntity>();
    bool isDirty = false;

    public Chunk(int3 coord){
        this.Coord = coord;
        this.Center = (coord*SIZE) + MID_POINT;
    }

    public void ResetDirtyFlag() => isDirty = false;
    public bool IsDirty => isDirty;

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
    public bool CreateBlockEntity(int3 globalPositionCorner, int id, World.Direction direction)
    {
        Voxel voxel = Program.GlobalDatabase.GetVoxel(id);
        if (voxel == null) return false;
        if (!voxel.IsEntity) return false; // may just allow anyways, make this create block

        EntityRegion region = new EntityRegion
        {
            Size = voxel.Size,
            Corner = globalPositionCorner
        };
        if (!Program.CurrentWorld.IsRegionEmpty(region)) return false;

        IBlockEntity blockEntity = Program.GlobalDatabase.CreateBlockEntity(id);
        bool worked = blockEntity.OnCreate(id, globalPositionCorner, direction);
        
        if (worked){
            int3 localPosition = World.GetVoxelLocalPositionInChunk(globalPositionCorner);
            SetID(localPosition.x, localPosition.y, localPosition.z, id);
            blockEntities.Add(localPosition, blockEntity);
            isDirty = true;
        }
        return worked;
    }

    public void SetID(int3 globalPosition, int id)
    {
        int3 localPosition = World.GetVoxelLocalPositionInChunk(globalPosition);
        SetID(localPosition.x, localPosition.y, localPosition.z, id);
    }

    private void SetID(int x, int y, int z, int id) {
        blocksIDs[(z * SIZE_2D) + (y * SIZE) + x] = id;
        isDirty = true;
    }
    // best if used once
    public bool IsSpaceEmpty(int3 globalPosition){
        int3 localPosition = World.GetVoxelLocalPositionInChunk(globalPosition);
        Voxel voxel = GetVoxel(localPosition);
        return !voxel.IsSolid;
    }

    public bool IsRegionEmpty(EntityRegion region){
        int3 localPosition = World.GetVoxelLocalPositionInChunk(region.Corner);
        for(int x = localPosition.x; x < localPosition.x + region.Size.x; x++)
        {
            for (int y = localPosition.y; y < localPosition.y + region.Size.y; y++)
            {
                for (int z = localPosition.z; z < localPosition.z + region.Size.z; z++)
                {
                    if(GetVoxel(x,y,z).IsSolid){
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public Voxel GetVoxel(int3 localPosition) => Program.GlobalDatabase.GetVoxel(GetID(localPosition.x, localPosition.y, localPosition.z));
    public Voxel GetVoxel(int x, int y, int z) => Program.GlobalDatabase.GetVoxel(GetID(x, y, z));
    public int GetID(int x, int y, int z) => blocksIDs[(z * SIZE_2D) + (y * SIZE) + x];
    public int3 Coord;
    public float3 Center;
    public Chunk[] neighbors = new Chunk[6];
}
