using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
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
            float3 offset = (float3)keypair.Value.Voxel.Size/2f;
            Matrix4x4 mat = Matrix4x4.TRS((float3)keypair.Key + offset + chunk.Coord*Chunk.SIZE, 
                World.VoxelRotations[(int)keypair.Value.Direction], Vector3.one);
            blockEntityRenderer.AddBlockEntity(keypair.Value.Voxel.ID, mat);
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
