using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "New FileSystemData", menuName = "Data/FileSystemData")]
[ExecuteAlways]
public class FileSystemData : ScriptableObject
{
    public HashSet<Voxel> Voxels = new HashSet<Voxel>();
    [SerializeField] List<Voxel> visualizer = new List<Voxel>();
    [SerializeField] private string voxelDataRelativeFilePath;
    [SerializeField] public Material voxelMaterial;
    public Material VoxelMaterial => voxelMaterial;
    [ContextMenu("Update Database")]
    public void UpdateVoxelDatabase()
    {
        visualizer.Clear();
        int size = Voxels.Count;
        DirectoryInfo directory= new DirectoryInfo(Application.dataPath + "/" + voxelDataRelativeFilePath);
        foreach(FileInfo file in directory.EnumerateFiles())
        {
            if(file.Name.EndsWith(".meta") || !file.Name.EndsWith(".asset")){
                continue;
            }
            Voxel asset = AssetDatabase.LoadAssetAtPath<Voxel>(RelativePath(file.FullName));
            Voxels.Add(asset);
            visualizer.Add(asset);
        }

        if (size != Voxels.Count)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    public static string RelativePath(string path){
        return path.Substring(path.IndexOf("Assets"));
    }
    public void InitializeVoxelsHashSet()
    {
        Voxels = new HashSet<Voxel>(visualizer);
    }
    public static FileSystemData LoadDataBase()
    {
        FileSystemData asset = (FileSystemData)Resources.Load("Database");
        Assert.IsTrue(asset != null, "Failed to load Database asset");
        asset.InitializeVoxelsHashSet();
        return asset;
    }
}