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


public class ChunkRenderer {
    
    Dictionary<Chunk, Mesh> chunks = new Dictionary<Chunk, Mesh> ();
    public void AddChunk(Chunk chunk, BlockEntityRenderer blockEntityRenderer){
        chunks.Add(chunk, null);
        foreach(var keypair in chunk.blockEntities){
            Matrix4x4 mat = Matrix4x4.TRS((float3)keypair.Key + new float3(0.5f), Quaternion.identity, Vector3.one);
            blockEntityRenderer.AddBlockEntity(keypair.Value.ID, mat);
        }
    }

    public void AddMesh(Chunk chunk, Mesh mesh){
        chunks[chunk] = mesh;
    }

    public void Render()
    {
        foreach (var keypair in chunks)
        {
            if(keypair.Value == null){
                continue;
            }
            Graphics.DrawMesh(keypair.Value, (float3)keypair.Key.Coord, Quaternion.identity, Program.GlobalDatabase.VoxelMaterial, 0);
        }
    }
}

public class BlockEntityRenderer {
    Dictionary<int, BlockEntityRenderData> renderTargets = new Dictionary<int, BlockEntityRenderData>();

    public void AddBlockEntity(int ID, Matrix4x4 transform)
    {
        if(!renderTargets.ContainsKey(ID)){
            BlockEntityRenderData renderData = new BlockEntityRenderData();
            renderData.count = 1;
            renderData.Voxel = Program.GlobalDatabase.GetVoxel(ID);
            renderData.Transforms.Add(transform);
            renderTargets.Add(ID, renderData);
        } else {
            BlockEntityRenderData renderData = renderTargets[ID];
            renderData.count++;
            renderData.Transforms.Add(transform);
        }
    }

    public void RemoveBlockEntity(int ID, Matrix4x4 transform)
    {
        if (renderTargets.ContainsKey(ID))
        {
            BlockEntityRenderData renderData = renderTargets[ID];
            renderData.count--;
            bool worked = renderData.Transforms.Remove(transform);
            if(!worked){
                throw new SystemException($"Bro, for {ID}, this matrix {transform} doesn't exist wtf is wrong");
            }
            if(renderData.count == 0){
                renderTargets.Remove(ID);
            }
        } else {
            throw new SystemException($"Bro, you can't remove an ID {ID} that doesn't exist");
        }
    }

    public void Render(){
        foreach(BlockEntityRenderData values in renderTargets.Values) {
            Graphics.DrawMeshInstanced(values.Voxel.EntityMesh, 0, values.Voxel.EntityMaterial, values.Transforms);    
        }
    }
}

 class BlockEntityRenderData {
    public int count;
    public Voxel Voxel;
    public List<Matrix4x4> Transforms = new List<Matrix4x4>();
}

public class PlayerSettings {
    public const int Version = 1;
    public int chunkRenderDistance = 5;
}