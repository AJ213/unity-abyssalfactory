using System.Collections.Generic;
using Unity.Mathematics;
public struct EntityRegion {
    public int3 Corner; // Always bottom left corner
    public int3 Size; // Always the width/height/depth of object in full, not half
    public EntityRegion(int3 corner, int3 size) {
        Corner = corner;
        Size = size;
    }
    public bool IsSingleSize => Size.x == 1 && Size.y == 1 && Size.z == 1;

    // First int3 is position, second is vector of which side its on
    public IEnumerable<(int3, int3)> NearbyRegionPositions()
    {
        // bottom/top
        int downY = Corner.y - 1;
        int topY = Corner.y + Size.y;
        for (int x = 0; x < Size.x; x++)
        {
            for (int z = 0; z < Size.z; z++)
            {
                yield return (new int3(Corner.x + x, downY, Corner.z + z), World.Down);
                yield return (new int3(Corner.x + x, topY, Corner.z + z), World.Up);
            }
        }
        
        int leftX = Corner.x - 1;
        int rightX = Corner.x + Size.x;
        for (int y = 0; y < Size.y; y++)
        {
            for (int z = 0; z < Size.z; z++)
            {
                yield return (new int3(leftX, Corner.y + y, Corner.z + z ), World.Left);
                yield return (new int3(rightX, Corner.y + y, Corner.z + z), World.Right);
            }
        }
        
        int backZ = Corner.z - 1;
        int forwardZ = Corner.z + Size.z;
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                yield return (new int3(Corner.x + x, Corner.y + y, backZ), World.Backward);
                yield return (new int3(Corner.x + x, Corner.y + y, forwardZ), World.Forward);
            }
        }
    }
}
