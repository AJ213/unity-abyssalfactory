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

    // This needs to check many chunks for this lol, not just one
    // this also needs to be able to request to make new chunks
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
    BlockEntityRenderer blockEntityRenderer;
    public void AddChunk(Chunk chunk, BlockEntityRenderer blockEntityRenderer){
        chunks.Add(chunk, null);
        this.blockEntityRenderer = blockEntityRenderer;
        UpdateBlockEntities(chunk);
    }

    public void UpdateBlockEntities(Chunk chunk){
        foreach (var keypair in chunk.blockEntities)
        {
            float3 offset = (float3)Program.GlobalDatabase.GetVoxel(keypair.Value.ID).Size/2f;
            Matrix4x4 mat = Matrix4x4.TRS((float3)keypair.Key + offset, 
                World.VoxelRotations[(int)keypair.Value.Direction], Vector3.one);
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
            if(keypair.Key.IsDirty){
                UpdateBlockEntities(keypair.Key);
                keypair.Key.ResetDirtyFlag();
            }
            Graphics.DrawMesh(keypair.Value, (float3)keypair.Key.Coord, Quaternion.identity, Program.GlobalDatabase.VoxelMaterial, 0);
        }
    }

    
}

public class BlockEntityRenderer {
    Dictionary<int, BlockEntityRenderData> renderTargets = new Dictionary<int, BlockEntityRenderData>();
    const int matrixStride = sizeof(float) * 16;
    const int uintStride = sizeof(uint);
    Bounds bounds;

    public void AddBlockEntity(int ID, Matrix4x4 transform)
    {
        if(!renderTargets.ContainsKey(ID)){
            BlockEntityRenderData renderData = new BlockEntityRenderData();
            renderData.Voxel = Program.GlobalDatabase.GetVoxel(ID);
            renderData.Transforms.Add(transform);
            renderTargets.Add(ID, renderData);
        } else {
            BlockEntityRenderData renderData = renderTargets[ID];
            renderData.Transforms.Add(transform);
        }
    }

    public void RemoveBlockEntity(int ID, Matrix4x4 transform)
    {
        if (renderTargets.ContainsKey(ID))
        {
            BlockEntityRenderData renderData = renderTargets[ID];
            bool worked = renderData.Transforms.Remove(transform);
            if(!worked){
                throw new SystemException($"Bro, for {ID}, this matrix {transform} doesn't exist wtf is wrong");
            }
            if(renderData.Transforms.Count == 0){
                renderTargets.Remove(ID);
            }
        } else {
            throw new SystemException($"Bro, you can't remove an ID {ID} that doesn't exist");
        }
    }
    
    public void Render(){
        foreach(BlockEntityRenderData renderData in renderTargets.Values) {
            int instanceCount = renderData.Transforms.Count;

            renderData.Release();

            renderData.argsBuffer = new ComputeBuffer(5, uintStride, ComputeBufferType.IndirectArguments);

            renderData.argsData[0] = renderData.Voxel.EntityMesh.GetIndexCount(0);
            renderData.argsData[1] = (uint)instanceCount;
            renderData.argsData[2] = renderData.Voxel.EntityMesh.GetIndexStart(0);
            renderData.argsData[3] = renderData.Voxel.EntityMesh.GetBaseVertex(0);
            renderData.argsData[4] = 0;

            renderData.argsBuffer.SetData(renderData.argsData);

            Matrix4x4[] matrices = renderData.Transforms.ToArray();

            foreach(Matrix4x4 mat in matrices){
                Vector3 position = mat.GetPosition();
                Vector3 forward = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1)) * Vector3.forward;
                Vector3 right = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1)) * Vector3.right;
                Vector3 left = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1)) * Vector3.left;

                Vector3 endPoint = position + forward * 0.3f;

                Debug.DrawLine(position, endPoint);
                Debug.DrawLine(endPoint, endPoint - forward * 0.1f + right * 0.1f);
                Debug.DrawLine(endPoint, endPoint - forward * 0.1f + left * 0.1f);
            }

            renderData.matricesBuffer = new ComputeBuffer(instanceCount, matrixStride);
            renderData.matricesBuffer.SetData(matrices);
            renderData.Voxel.EntityMaterial.SetBuffer("positionBuffer", renderData.matricesBuffer);
            bounds = new Bounds(new Vector3(Chunk.SIZE, Chunk.SIZE, Chunk.SIZE) / 2, new Vector3(Chunk.SIZE, Chunk.SIZE, Chunk.SIZE)); // need to change position
            Graphics.DrawMeshInstancedIndirect(renderData.Voxel.EntityMesh, 0, renderData.Voxel.EntityMaterial, bounds, renderData.argsBuffer);
        }
    }
 
    public void OnDisable(){
        foreach (BlockEntityRenderData renderData in renderTargets.Values){
            renderData.Release();
        }
    }
}

 class BlockEntityRenderData {
    public Voxel Voxel;
    public List<Matrix4x4> Transforms = new List<Matrix4x4>();
    public ComputeBuffer argsBuffer;
    public ComputeBuffer matricesBuffer;
    public uint[] argsData = new uint[5];

    public void Release(){
        argsBuffer?.Release();
        matricesBuffer?.Release();
    }
}

public class PlayerSettings {
    public const int Version = 1;
    public int chunkRenderDistance = 5;
}