using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
        int3 startingSiloPosition = new int3(-5, 0, 0);
        int3[] conveyorsPos = new[]
        {
            new int3(-2, 0, 0), 
            new int3(-1, 0, 0),
            new int3(0, 0, 0), 
            new int3(1, 0, 0)
        };
        int3 endingSiloPosition = new int3(2, 0, 0);
        Program.CurWorld.SetBlock(startingSiloPosition, Voxel.STORAGE, World.Direction.Right);
        Program.CurWorld.SetBlock(conveyorsPos[0], Voxel.CONVEYOR, World.Direction.Right);
        Program.CurWorld.SetBlock(conveyorsPos[1], Voxel.CONVEYOR, World.Direction.Right);
        Program.CurWorld.SetBlock(conveyorsPos[2], Voxel.CONVEYOR, World.Direction.Right);
        Program.CurWorld.SetBlock(conveyorsPos[3], Voxel.CONVEYOR, World.Direction.Right);
        Program.CurWorld.SetBlock(endingSiloPosition, Voxel.STORAGE, World.Direction.Right);

        StorageSilo startingSilo = Program.CurWorld.GetBlockEntity<StorageSilo>(startingSiloPosition);
        StorageSilo endingSilo = Program.CurWorld.GetBlockEntity<StorageSilo>(endingSiloPosition);
        TubeConveyor[] conveyors = new TubeConveyor[]
        {
            Program.CurWorld.GetBlockEntity<TubeConveyor>(conveyorsPos[0]), 
            Program.CurWorld.GetBlockEntity<TubeConveyor>(conveyorsPos[1]),
            Program.CurWorld.GetBlockEntity<TubeConveyor>(conveyorsPos[2]), 
            Program.CurWorld.GetBlockEntity<TubeConveyor>(conveyorsPos[3]),
        };
        
        Item myItem = new Item(0);
        startingSilo.Insert(int3.zero, myItem);
        
        Program.CurWorld.AddAllChunksToRendering();
        Program.CurWorld.UpdateRendering();
        Assert.IsTrue(startingSilo.Contains(myItem), "Starting silo doesn't contain expected item");
        Debug.Log("Time delta time is " + Time.fixedDeltaTime);
        Program.CurWorld.UpdateAllEntities(Time.fixedDeltaTime);
        yield return null;


        for (int j = 0; j < conveyors.Length; j++)
        {
            for (int k = 1; k < conveyors.Length; k++)
            {
                int index = (j + k) % conveyors.Length;
                Assert.IsFalse(conveyors[index].Contains(myItem), $"Conveyor{index} contains unexpected item");
            }
            for (int i = 0; i < conveyors[j].Capacity; i++)
            {
                Assert.IsTrue(conveyors[j].Contains(myItem), $"Conveyor{j} doesn't contain expected item");
                int counter = 0;
                while (counter < conveyors[j].Speed)
                {
                    Program.CurWorld.UpdateAllEntities(Time.fixedDeltaTime);
                    counter += 1;
                }
            }
        }



        Assert.IsTrue(endingSilo.Contains(myItem), "Ending silo doesn't contain expected item");
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
