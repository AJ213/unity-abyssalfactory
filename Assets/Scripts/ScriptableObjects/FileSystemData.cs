using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New FileSystemData", menuName = "Data/FileSystemData")]
[ExecuteAlways]
public class FileSystemData : ScriptableObject
{
    public HashSet<Voxel> Voxels = new HashSet<Voxel>();
    [SerializeField] private string voxelDataRelativeFilePath;
    [SerializeField] public Material voxelMaterial;
    void OnValidate()
    {
        if (!Application.IsPlaying(this))
        {
            int size = Voxels.Count;
            DirectoryInfo directory= new DirectoryInfo(Application.dataPath + "/" + voxelDataRelativeFilePath);
            foreach(FileInfo file in directory.EnumerateFiles())
            {
                if(file.Name.EndsWith(".meta") || !file.Name.EndsWith(".asset")){
                    continue;
                }
                Voxel asset = AssetDatabase.LoadAssetAtPath<Voxel>(RelativePath(file.FullName));
                Voxels.Add(asset);
            }

            if (size != Voxels.Count)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
    public Material VoxelMaterial => voxelMaterial;
    public static string RelativePath(string path){
        return path.Substring(path.IndexOf("Assets"));
    }
    

}