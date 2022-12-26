using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class Program
{
    MeshFilter filter;
    MeshRenderer renderer;
    public Database database;
    public Program(FileSystemData database, GameObject hook)
    {
        this.database = new Database(database);
        Chunk chunk = new Chunk();
        for (int z = 0; z < Chunk.SIZE; z++)
        {
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int x = 0; x < Chunk.SIZE; x++)
                {
                    chunk.SetID(x, y, z, UnityEngine.Random.Range(0,3));
                }
            }
        }

        ChunkMesh chunkMesh = new ChunkMesh();
        ChunkMeshGenerationSystem.GenerateMesh(chunk, chunkMesh, this.database);
        filter = hook.AddComponent<MeshFilter>();
        renderer = hook.AddComponent<MeshRenderer>();
        filter.mesh = ChunkMeshGenerationSystem.CreateMesh(chunkMesh);
        renderer.material = database.VoxelMaterial;
        renderer.sharedMaterial.SetTexture("_MainTex", this.database.GetTextureArray);
    }


    // Update is called once per frame
    public void Update()
    {
        
    }
}
