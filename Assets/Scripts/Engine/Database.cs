using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Database
{
    Voxel[] voxels;
    Dictionary<int, List<Texture2D>> uniqueTextures;
    Dictionary<Texture2D, int> textureIndexes;
    Dictionary<int, Type> blockEntityTypes;
    Texture2DArray textureArray;
    FileSystemData fileSystemData;
    public Database(FileSystemData fileSystemData)
    {
        this.fileSystemData = fileSystemData;
        voxels = new Voxel[fileSystemData.Voxels.Count];
        foreach (Voxel voxel in fileSystemData.Voxels)
        {
            voxels[voxel.ID] = voxel;
        }

        ////////////////////////////// Create Texture Array //
        
        uniqueTextures = new Dictionary<int, List<Texture2D>>();
        int texCount = 0;
        HashSet<Texture2D> temp = new HashSet<Texture2D>();
        Texture2D randomTex = null;
        foreach (Voxel voxel in fileSystemData.Voxels)
        {
            temp.Clear();
            uniqueTextures.Add(voxel.ID, new List<Texture2D>());
            for (int i = 0; i < 6; i++)
            { // too damn lazy to do enum counting for all directions
                Texture2D cur = voxel.GetTexture((World.Direction)i);
                if (cur == null)
                {
                    continue;
                }
                if (!temp.Contains(cur))
                {
                    uniqueTextures[voxel.ID].Add(cur);
                    temp.Add(cur);
                    texCount++;
                    randomTex = cur;
                }
            }
        }
        if (randomTex == null)
        {
            Debug.LogError($"random texture is null for some reason. is it because of voxel list being nothing? {fileSystemData.Voxels.Count}");
        }
        textureArray = new
            Texture2DArray(randomTex.width,
            randomTex.height, texCount,
            TextureFormat.RGBA32, true, false);
        textureArray.filterMode = FilterMode.Bilinear;
        textureArray.wrapMode = TextureWrapMode.Repeat;
        int count = 0;
        textureIndexes = new Dictionary<Texture2D, int>();
        for (int i = 0; i < fileSystemData.Voxels.Count; i++)
        {
            List<Texture2D> texs = uniqueTextures[i];
            for (int j = 0; j < texs.Count; j++)
            {
                textureIndexes[texs[j]] = count;
                textureArray.SetPixels(texs[j].GetPixels(0), count, 0);
                count++;
            }
        }
        textureArray.Apply();

        ////////////////////////////// Create Entity Type Dictionary //
        blockEntityTypes = new Dictionary<int, Type>();
        foreach (Voxel voxel in voxels)
        {
            if (voxel.IsEntity)
            {
                blockEntityTypes.Add(voxel.ID, Type.GetType(voxel.BlockEntityClassName));
            }
        }
    }
    public int GetTextureID(Texture2D tex) => textureIndexes[tex];
    public IBlockEntity CreateBlockEntity(int id) => (IBlockEntity)Activator.CreateInstance(blockEntityTypes[id]);
    public Texture2DArray GetTextureArray => textureArray;
    public Voxel GetVoxel(int id) => voxels[id];
    public Material VoxelMaterial => fileSystemData.VoxelMaterial;
}
