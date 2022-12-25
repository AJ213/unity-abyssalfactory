using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class Chunk
{
    public const int SIZE = 32;
    public const int SIZEM1 = SIZE-1;
    public const int SIZE_2D = SIZE * SIZE;
    public const int SIZE_3D = SIZE_2D * SIZE;
    int[] blocksIDs = new int[SIZE_3D];

    public void SetID(int x, int y, int z, int id) => 
        blocksIDs[(z * SIZE_2D) + (y * SIZE) + x] = id;
    public int GetID(int x, int y, int z) => blocksIDs[(z * SIZE_2D) + (y * SIZE) + x];
    public int3 Coord;
    public Chunk[] neighbors = new Chunk[6];
}
