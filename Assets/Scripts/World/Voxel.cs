using UnityEngine;

[CreateAssetMenu(fileName = "New Voxel", menuName = "Data/Voxel")]
public class Voxel : ScriptableObject
{
    [SerializeField] int id;
    [SerializeField] string displayName;
    [SerializeField] bool isSolid;
    [SerializeField] bool isDestructable;
    [SerializeField] bool canRenderFaces;
    [SerializeField] VoxelMeshData voxelMeshData;
    [SerializeField] Texture2D[] textureFaces;

    public int ID => id;
    public VoxelMeshData VoxelMeshData => voxelMeshData;
    public Texture2D GetTexture(World.Direction direction) { 
        if (textureFaces == null || (int)direction >= textureFaces.Length) return null;

        return textureFaces[(int)direction]; 
    }
    public bool IsSolid => isSolid;
    public bool IsDestructable => isDestructable;
    public bool CanRenderFaces => canRenderFaces;
    

    public override bool Equals(object obj)
    {
        Voxel item = obj as Voxel;

        if (item == null)
        {
            return false;
        }

        return this.id.Equals(item.id);
    }

    public override int GetHashCode()=> id;
    
    public override string ToString() => displayName;
}
