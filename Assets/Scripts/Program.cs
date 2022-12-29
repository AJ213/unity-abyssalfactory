using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class Program
{
    public static Database GlobalDatabase;
    public static World CurrentWorld;
    public static PlayerSettings PlayerSettings;

    public Program(FileSystemData database, GameObject hook)
    {
        GlobalDatabase = new Database(database);
        Chunk chunk = new Chunk(new int3(0, 0, 0));
        for (int z = 0; z < Chunk.SIZE; z++)
        {
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int x = 0; x < Chunk.SIZE; x++)
                {
                    chunk.SetID(x, y, z, UnityEngine.Random.Range(0,4));
                }
            }
        }
        CurrentWorld = new World();
        CurrentWorld.chunks[chunk.Coord] = chunk;
        CurrentWorld.AddChunkRendering(chunk);
        database.VoxelMaterial.SetTexture("_MainTex", GlobalDatabase.GetTextureArray);
        
    }


    // Update is called once per frame
    public void Update(Mesh mesh, Material Material)
    {
        CurrentWorld.UpdateRendering();
        //ChunkMeshGenerationSystem.GenerateMeshCurrentWorld.chunks[new int3(0, 0, 0)]
        //Graphics.DrawMesh(, new float3(16,16,16), Quaternion.identity, Material, 0);
    }
}
