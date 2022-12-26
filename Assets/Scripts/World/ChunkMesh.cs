using System.Collections.Concurrent;
using UnityEngine;

public class ChunkMesh
{
    public ConcurrentQueue<Vector3> vertices = new ConcurrentQueue<Vector3>();
    public ConcurrentQueue<int> triangles = new ConcurrentQueue<int>();
    public ConcurrentQueue<Vector3> uvs = new ConcurrentQueue<Vector3>();
    public ConcurrentQueue<Vector3> normals = new ConcurrentQueue<Vector3>();

    public void Clear(){
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        normals.Clear();
    }
}
