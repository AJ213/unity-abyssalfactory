using System;
using Unity.Mathematics;
using UnityEngine;

public class UnityHook : MonoBehaviour
{
    Program program;
    FlyingCamera cam;
    void Awake()
    {
        program = new Program();
        ConveyorTest();
        Program.CurrentWorld.AddAllChunksToRendering();
        cam = new FlyingCamera();
    }

    void ConveyorTest()
    {
        Program.CurrentWorld.SetBlock(new int3(-5, 0, 0), Voxel.STORAGE, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(-2, 0, 0), Voxel.CONVEYOR, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(-1, 0, 0), Voxel.CONVEYOR, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(0, 0, 0), Voxel.CONVEYOR, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(1, 0, 0), Voxel.CONVEYOR, World.Direction.Right);
        Program.CurrentWorld.SetBlock(new int3(2, 0, 0), Voxel.STORAGE, World.Direction.Right);
    }
    
    void Something()
    {
        for (int x = 0; x < Chunk.SIZE; x++)
        {
            for (int z = 0; z < Chunk.SIZE; z++)
            {
                Program.CurrentWorld.SetBlock(new int3(x, 0, z), 1);
            }
        }

        int Count = 15;
        for (int i = 0; i < Count; i++)
        {
            Program.CurrentWorld.SetBlock(new int3(i, 1, 0), 3, World.Direction.Right);
            Program.CurrentWorld.SetBlock(new int3(Count, 1, i), 3, World.Direction.Backward);
            Program.CurrentWorld.SetBlock(new int3(i, 1, Count), 3, World.Direction.Left);
            Program.CurrentWorld.SetBlock(new int3(0, 1, i), 3, World.Direction.Forward);
        }
        
        Program.CurrentWorld.SetBlock(new int3(Count, 1, Count), 4, World.Direction.Forward);
    }
    
    void Update()
    {
        program.Update(Time.deltaTime);
        cam.Update();
    }
    void FixedUpdate()
    {
        program.FixedUpdate(Time.fixedDeltaTime);
    }

    private void OnDisable()
    {
        program.OnDisable();
    }
}
