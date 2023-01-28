using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class Program
{
    public static Database GlobalDatabase;
    public static World CurrentWorld;
    public static PlayerSettings PlayerSettings;

    FlyingCamera camera;

    public Program(FileSystemData database, GameObject hook)
    {
        GlobalDatabase = new Database(database);
        Chunk chunk = new Chunk(new int3(0, 0, 0));
        for (int z = 0; z < Chunk.SIZE; z++)
        {
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int x = 0; x < Chunk.SIZE; x++)
                {
                    chunk.SetID(new int3(x, y, z), 0);
                }
            }
        }
        
        CurrentWorld = new World();
        CurrentWorld.chunks[chunk.Coord] = chunk;

        for (int x = 0; x < Chunk.SIZE; x++)
        {
            for (int z = 0; z < Chunk.SIZE; z++)
            {
                chunk.SetID(new int3(x, 0, z), 1);
            }
        }

        int Count = 15;
        for (int i = 0; i < Count; i++)
        {
            chunk.CreateBlockEntity(new int3(i, 1, 0), 3, World.Direction.Right);
            chunk.CreateBlockEntity(new int3(Count, 1, i), 3, World.Direction.Backward);
            chunk.CreateBlockEntity(new int3(i, 1, Count), 3, World.Direction.Left);
            chunk.CreateBlockEntity(new int3(0, 1, i), 3, World.Direction.Forward);
        }

        chunk.CreateBlockEntity(new int3(Count, 1, Count), 4, World.Direction.Up);

        

        CurrentWorld.AddChunkRendering(chunk);
        database.VoxelMaterial.SetTexture("_MainTex", GlobalDatabase.GetTextureArray);

        camera = new FlyingCamera();
    }


    public void Update()
    {
        CurrentWorld.UpdateRendering();
        camera.Update();
    }

    public void OnDisable(){
        CurrentWorld.OnDisable();
    }
}

// Temp implimentation before we implement player and spectate and so on
// // From https://gist.github.com/FreyaHolmer/650ecd551562352120445513efa1d952
public class FlyingCamera
{
    const float acceleration = 50; // how fast you accelerate
    const float accSprintMultiplier = 4; // how much faster you go when "sprinting"
    const float lookSensitivity = 1; // mouse look sensitivity
    const float dampingCoefficient = 5; // how quickly you break to a halt after you stop your input
    Transform transform;
    Vector3 velocity; // current velocity

    static bool Focused
    {
        get => Cursor.lockState == CursorLockMode.Locked;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = value == false;
        }
    }
       
    public FlyingCamera(){
        transform = Camera.main.transform;
    }

    public void Update()
    {
        // Input
        if (Focused)
            UpdateInput();
        else if (Input.GetMouseButtonDown(0))
            Focused = true;

        // Physics
        velocity = Vector3.Lerp(velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    void UpdateInput()
    {
        // Position
        velocity += GetAccelerationVector() * Time.deltaTime;

        // Rotation
        Vector2 mouseDelta = lookSensitivity * new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        Quaternion rotation = transform.rotation;
        Quaternion horiz = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(mouseDelta.y, Vector3.right);
        transform.rotation = horiz * rotation * vert;

        // Leave cursor lock
        if (Input.GetKeyDown(KeyCode.Escape))
            Focused = false;
    }

    Vector3 GetAccelerationVector()
    {
        Vector3 moveInput = default;

        void AddMovement(KeyCode key, Vector3 dir)
        {
            if (Input.GetKey(key))
                moveInput += dir;
        }

        AddMovement(KeyCode.W, Vector3.forward);
        AddMovement(KeyCode.S, Vector3.back);
        AddMovement(KeyCode.D, Vector3.right);
        AddMovement(KeyCode.A, Vector3.left);
        AddMovement(KeyCode.Space, transform.InverseTransformDirection(Vector3.up));
        AddMovement(KeyCode.LeftControl, transform.InverseTransformDirection(Vector3.down));
        Vector3 direction = transform.TransformVector(moveInput.normalized);

        if (Input.GetKey(KeyCode.LeftShift))
            return direction * (acceleration * accSprintMultiplier); // "sprinting"
        return direction * acceleration; // "walking"
    }
}
