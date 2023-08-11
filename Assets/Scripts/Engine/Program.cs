using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class Program
{
    public static Database GlobalDatabase;
    public static World CurWorld;
    public static PlayerSettings PlayerSettings;
    
    public Program()
    {
        FileSystemData database = FileSystemData.LoadDataBase();
        GlobalDatabase = new Database(database);
        CurWorld = new World();
        database.VoxelMaterial.SetTexture("_MainTex", GlobalDatabase.GetTextureArray);
    }

    public void Update(float deltaTime)
    {
        CurWorld.UpdateRendering();
    }

    public void FixedUpdate(float deltaTime)
    {
        CurWorld.UpdateAllEntities(deltaTime);
    }

    public void OnDisable(){
        CurWorld.OnDisable();
    }
}