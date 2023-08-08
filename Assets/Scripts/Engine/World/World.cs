using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class World
{
    public string name;
    public ConcurrentDictionary<int3, Chunk> chunks = new ConcurrentDictionary<int3, Chunk>(); // int3 -> Chunk
    public static Action<PlayerSettings> OnPlayerUpdateSettings;

    ChunkRenderer chunkRenderer = new ChunkRenderer();
    BlockEntityRenderer blockEntityRenderer = new BlockEntityRenderer();

    public void AddChunkRendering(Chunk chunk){
        chunkRenderer.AddChunk(chunk, blockEntityRenderer);
        ChunkMesh chunkMesh = new ChunkMesh();
        ChunkMeshGenerationSystem.GenerateMesh(chunk, chunkMesh);
        chunkRenderer.AddMesh(chunk, ChunkMeshGenerationSystem.CreateMesh(chunkMesh));
    }

    public void UpdateRendering(){
        chunkRenderer.Render();
        blockEntityRenderer.Render();
    }

    public void AddAllChunksToRendering()
    {
        foreach (Chunk chunk in chunks.Values)
        {
            AddChunkRendering(chunk);
        }
    }
    
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

    // This needs to check many chunks for this lol, not just one
    // this also needs to be able to request to make new chunks
    // This is inefficient lmao
    public bool IsRegionEmpty(EntityRegion region){
        int3 coord = GetChunkCoordFromPosition(region.Corner);
        chunks.TryGetValue(coord, out Chunk chunk);
        if (chunk == null) return false;
        if (region.IsSingleSize) return chunk.IsSpaceEmpty(region.Corner);

        return chunk.IsRegionEmpty(region);
    }

    public T GetEntity<T>(int3 position){
        return default(T);
    }

    public void OnDisable()
    {
        blockEntityRenderer.OnDisable();
    }

    public void SetBlock(int3 position, int id, Direction direction = Direction.Forward)
    {
        int3 coord = GetChunkCoordFromPosition(position);
        if (!chunks.TryGetValue(coord, out Chunk chunk))
        {
            chunk = new Chunk(coord);
            chunks[coord] = chunk;
        }
        chunk.SetBlock(position, id, direction);
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
    public static readonly Quaternion[] VoxelRotations = new Quaternion[] {
        Quaternion.LookRotation(Vector3.forward, Vector3.up),
        Quaternion.LookRotation(Vector3.back, Vector3.up),
        Quaternion.LookRotation(Vector3.up, Vector3.up),
        Quaternion.LookRotation(Vector3.down, Vector3.up),
        Quaternion.LookRotation(Vector3.right, Vector3.up),
        Quaternion.LookRotation(Vector3.left, Vector3.up)
    };

    public static int3 GetDirectionVector(Direction dir) => VoxelDirections[(int)dir];
    public static int3 GetDirectionVector(int i) => VoxelDirections[i];
    #endregion

    #region Helpers
    public static int3 GetChunkCoordFromPosition(float3 position)
    {
        int x = Mathf.FloorToInt(position.x / Chunk.SIZE);
        int y = Mathf.FloorToInt(position.y / Chunk.SIZE);
        int z = Mathf.FloorToInt(position.z / Chunk.SIZE);
        return new int3(x, y, z);
    }

    public static int3 GetVoxelLocalPositionInChunk(float3 position)
    {
        return new int3(Mod(Mathf.FloorToInt(position.x), Chunk.SIZE),
                        Mod(Mathf.FloorToInt(position.y), Chunk.SIZE),
                        Mod(Mathf.FloorToInt(position.z), Chunk.SIZE));
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

public class PlayerSettings {
    public const int Version = 1;
    public int chunkRenderDistance = 5;
}