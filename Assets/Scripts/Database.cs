using System.Collections.Generic;

[System.Serializable]
public class Database
{
    Dictionary<int, Voxel> voxelMeshDataMap;
    public Database(FileSystemData fileSystemData)
    {
        voxelMeshDataMap = new Dictionary<int, Voxel>();
        foreach (Voxel voxel in fileSystemData.Voxels){
            voxelMeshDataMap.Add(voxel.ID, voxel);
        }
    }

    public Voxel GetVoxel(int id) => voxelMeshDataMap[id];
}
