using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;


public class ItemTests
{
    Program program;
    [SetUp]
    public void Setup()
    {
        program = new Program();
    }
    [TearDown]
    public void TearDown()
    {
        program.OnDisable();
    }
    [UnityTest]
    public IEnumerator StorageToStorageTest()
    {
        Program.CurrentWorld.SetBlock(new int3(-5, 0, 0), Voxel.STORAGE, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(-2, 0, 0), Voxel.CONVEYOR, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(-1, 0, 0), Voxel.CONVEYOR, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(0, 0, 0), Voxel.CONVEYOR, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(1, 0, 0), Voxel.CONVEYOR, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(2, 0, 0), Voxel.STORAGE, World.Direction.Right);
        Program.CurrentWorld.AddAllChunksToRendering();
        Program.CurrentWorld.UpdateRendering();
        Debug.Log("Should be showing?");
        yield return null;
        Program.CurrentWorld.UpdateRendering();
        yield return null;
        Program.CurrentWorld.UpdateRendering();
        yield return null;
        Program.CurrentWorld.UpdateRendering();
        yield return null;
        Program.CurrentWorld.UpdateRendering();
    }

    [Test]
    public void WorldLocalMathTests()
    {
        void AreEqual(int x, int y, int z, int x1, int y1, int z1)
        {
            Assert.AreEqual(new int3(x, y, z),World.GetVoxelLocalPositionInChunk(new int3(x1, y1, z1)));
        }
        for (int x = 0; x < Chunk.SIZE; x++)
        {
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int z = 0; z < Chunk.SIZE; z++)
                {
                    AreEqual(x, y, z, x, y, z);
                }
            }
        }
        for (int x = 1; x <= Chunk.SIZE; x++)
        {
            for (int y = 1; y <= Chunk.SIZE; y++)
            {
                for (int z = 1; z <= Chunk.SIZE; z++)
                {
                    
                    AreEqual(Chunk.SIZE-x, Chunk.SIZE-y,Chunk.SIZE-z, -x,-y,-z);
                }
            }
        }
        AreEqual(Chunk.SIZEM1, 0,0, -1,0,0);
        
    }
}
