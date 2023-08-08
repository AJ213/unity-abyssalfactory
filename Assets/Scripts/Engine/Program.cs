using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class Program
{
    public static Database GlobalDatabase;
    public static World CurrentWorld;
    public static PlayerSettings PlayerSettings;
    
    public Program()
    {
        FileSystemData database = FileSystemData.LoadDataBase();
        GlobalDatabase = new Database(database);
        CurrentWorld = new World();
        database.VoxelMaterial.SetTexture("_MainTex", GlobalDatabase.GetTextureArray);
    }
    
    


    public void Update()
    {
        CurrentWorld.UpdateRendering();
    }

    public void OnDisable(){
        CurrentWorld.OnDisable();
    }
}