using System.Collections.Concurrent;
using Unity.Mathematics;
using UnityEngine;

public class World
{
    public string name;
    public ConcurrentDictionary<int3, Chunk> chunks = new ConcurrentDictionary<int3, Chunk>(); // int3 -> Chunk

    public Chunk GetChunk(int3 globalPosition) {
        int3 coord = GetChunkCoordFromPosition(globalPosition);
        chunks.TryGetValue(coord, out Chunk chunk);
        return chunk;
    }
    public IBlockEntity GetBlockEntity(int3 globalPosition) {
        Chunk chunk = GetChunk(globalPosition);
        if(chunk == null){ // meaning chunk hasn't been generated
            return null;
        }
        return chunk.GetBlockEntity(globalPosition);
    }

    public T GetEntity<T>(int3 position){
        
        return default(T);
    }

    #region Lookup Tables
    public enum Direction
    {
        Forward,
        Backward,
        Up,
        Down,
        Right,
        Left
    }
    public static readonly int3[] VoxelDirections = new int3[] {
        new int3(0,0,1),    // forward
        new int3(0,0,-1),   // backward
        new int3(0,1,0),    // up
        new int3(0,-1,0),   // down
        new int3(1,0,0),    // right
        new int3(-1,0,0),   // left
    };

    public static int3 GetDirectionVector(Direction dir) => VoxelDirections[(int)dir];
    public static int3 GetDirectionVector(int i) => VoxelDirections[i];
    #endregion



    #region Helpers
    public static int3 GetChunkCoordFromPosition(float3 globalPosition)
    {
        int x = Mathf.FloorToInt(globalPosition.x / Chunk.SIZE);
        int y = Mathf.FloorToInt(globalPosition.y / Chunk.SIZE);
        int z = Mathf.FloorToInt(globalPosition.z / Chunk.SIZE);
        return new int3(x, y, z);
    }

    public static int3 GetVoxelLocalPositionInChunk(float3 globalPosition)
    {
        return new int3(Mod(Mathf.FloorToInt(globalPosition.x), Chunk.SIZE),
                        Mod(Mathf.FloorToInt(globalPosition.y), Chunk.SIZE),
                        Mod(Mathf.FloorToInt(globalPosition.z), Chunk.SIZE));
    }

    public static int3 GetVoxelGlobalPositionFromChunk(float3 localPosition, int3 coord)
    {
        return new int3(Mathf.FloorToInt(localPosition.x) + (Chunk.SIZE * coord.x),
                        Mathf.FloorToInt(localPosition.y) + (Chunk.SIZE * coord.y),
                        Mathf.FloorToInt(localPosition.z) + (Chunk.SIZE * coord.z));
    }

    public static int Mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }

    public static bool IsVoxelGlobalPositionInChunk(float3 globalPosition, int3 coord)
    {
        return ((float3)GetVoxelGlobalPositionFromChunk(GetVoxelLocalPositionInChunk(globalPosition), coord)).Equals(globalPosition);
    }
    #endregion
}
