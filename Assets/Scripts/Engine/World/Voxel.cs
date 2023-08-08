using Unity.Mathematics;
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

    [SerializeField] bool isBlockEntity = false;
    [SerializeField] Mesh entityMesh;
    [SerializeField] Material entityMaterial;
    [SerializeField] string blockEntityClassName;
    [SerializeField] int3 size = new int3(1,1,1);

    public int ID => id;
    public VoxelMeshData VoxelMeshData => voxelMeshData;
    public Texture2D GetTexture(World.Direction direction) { 
        if (textureFaces == null || (int)direction >= textureFaces.Length) return null;

        return textureFaces[(int)direction]; 
    }
    public bool HasTexture() => textureFaces is { Length: > 0 };
    public bool IsSolid => isSolid;
    public bool IsDestructable => isDestructable;
    public bool CanRenderFaces => canRenderFaces;
    public bool IsEntity => isBlockEntity;
    public Mesh EntityMesh => entityMesh;
    public Material EntityMaterial => entityMaterial;
    public string BlockEntityClassName => blockEntityClassName;
    public int3 Size => size;
   

    public override bool Equals(object obj)
    {
        Voxel item = obj as Voxel;

        if (item == null)
        {
            return false;
        }

        return this.id.Equals(item.id);
    }

    public override int GetHashCode() => id;
    
    public override string ToString() => displayName;

    public const int AIR = 0;
    public const int STONE = 1;
    public const int STONE2 = 2;
    public const int CONVEYOR = 3;
    public const int STORAGE = 4;
}
