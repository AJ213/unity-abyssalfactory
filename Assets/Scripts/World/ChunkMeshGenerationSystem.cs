using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEngine;

public class ChunkMeshGenerationSystem 
{
    public static void GenerateMesh([NotNull] Chunk chunk, [NotNull] ChunkMesh outputMesh, Database database)
    {
        if(chunk == null)
        {
            throw new System.Exception("A null chunk was passed into the mesh generation system");
        }
        outputMesh.Clear();
        int vertIndex = 0;
        for(int z = 0; z < Chunk.SIZE; z++)
        {
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int x = 0; x < Chunk.SIZE; x++)
                {
                    int voxelID = chunk.GetID(x, y, z);
                    int3 localPosition = new int3(x, y, z);
  
                    for (int p = 0; p < World.VoxelDirections.Length; p++)
                    {
                        Voxel voxel = database.GetVoxel(voxelID);
                        Texture2D texture = voxel.GetTexture((World.Direction)p);
                        if (texture == null)
                        {
                            continue;
                        }

                        int3 localNeighborPosition = localPosition + World.GetDirectionVector(p);
                        int neighborID;
                        bool neighborNull = chunk.neighbors[p] == null;
                        bool gX0 = localNeighborPosition.x > -1;
                        bool gY0 = localNeighborPosition.y > -1;
                        bool gZ0 = localNeighborPosition.z > -1;
                        bool lXM = localNeighborPosition.x < Chunk.SIZE;
                        bool lYM = localNeighborPosition.y < Chunk.SIZE;
                        bool lZM = localNeighborPosition.z < Chunk.SIZE;
                        bool inBounds = gX0 && gY0 && gZ0 && lXM && lYM && lZM;
                        if(!inBounds){
                            if (neighborNull)
                                neighborID = 0;
                            else if (gX0)
                                neighborID = chunk.neighbors[p].GetID(Chunk.SIZEM1, y, z);
                            else if (gY0)
                                neighborID = chunk.neighbors[p].GetID(x, Chunk.SIZEM1, z);
                            else if (gZ0)
                                neighborID = chunk.neighbors[p].GetID(x, y, Chunk.SIZEM1);
                            else if (lXM)
                                neighborID = chunk.neighbors[p].GetID(0, y, z);
                            else if (lYM)
                                neighborID = chunk.neighbors[p].GetID(x, 0, z);
                            else // lZM
                                neighborID = chunk.neighbors[p].GetID(x, y, 0);
                        } else
                            neighborID = chunk.GetID(localNeighborPosition.x, localNeighborPosition.y, localNeighborPosition.z);  

                        if (!database.GetVoxel(neighborID).CanRenderFaces)
                        {
                            continue;
                        }
                        
                        FaceMeshData faceMeshData = voxel.VoxelMeshData.faces[p];
                        VertData[] vertData = faceMeshData.vertData;
                        for (int i = 0; i < vertData.Length; i++)
                        {
                            outputMesh.vertices.Enqueue(localPosition +
                            (float3)vertData[i].position);
                            outputMesh.normals.Enqueue((float3)faceMeshData.normal);

                            int textureID = database.GetTextureID(texture);
                            outputMesh.uvs.Enqueue(new Vector3(vertData[i].uv.x, vertData[i].uv.y, textureID));
                        }
                        int[] triangleData = faceMeshData.triangles;
                        for (int i = 0; i < triangleData.Length; i++)
                        {
                            outputMesh.triangles.Enqueue(vertIndex + triangleData[i]);
                        }
                        vertIndex += vertData.Length;
                    }
                }
            }
        }
    }

    public static Mesh CreateMesh(ChunkMesh chunkMesh)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = chunkMesh.vertices.ToArray();
        mesh.triangles = chunkMesh.triangles.ToArray();
        mesh.normals = chunkMesh.normals.ToArray();
        mesh.SetUVs(0,chunkMesh.uvs.ToArray());
        return mesh;
    }

    
}