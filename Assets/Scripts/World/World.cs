using System.Collections.Concurrent;
using Unity.Mathematics;

public class World
{
    public string name;
    public ConcurrentDictionary<int3, Chunk> chunks = new ConcurrentDictionary<int3, Chunk>(); // int3 -> Chunk

    public enum Direction
    {
        Forward,
        Backward,
        Up,
        Down,
        Right,
        Left
    }
}
