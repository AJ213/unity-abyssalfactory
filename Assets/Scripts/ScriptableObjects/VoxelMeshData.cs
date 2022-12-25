using UnityEngine;

[CreateAssetMenu(fileName = "New Voxel Mesh Data", menuName = "Data/Voxel Mesh Data")]
public class VoxelMeshData : ScriptableObject
{
    public FaceMeshData[] faces;
}

[System.Serializable]
public class FaceMeshData
{
    public string direction;
    public Vector3 normal;
    public VertData[] vertData;
    public int[] triangles;
}

[System.Serializable]
public class VertData
{
    public Vector3 position;
    public Vector2 uv;
}