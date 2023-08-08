using Unity.Mathematics;
using UnityEngine;
public class DebugDrawer {
    public static void ChunkDrawer(float3 position)
    {
        float3 cornerCoord = World.GetChunkCoordFromPosition(position) * Chunk.SIZE;
        float3 rightCoord = cornerCoord + (float3)Vector3.right * Chunk.SIZE;
        float3 forwardCoord = cornerCoord + (float3)Vector3.forward * Chunk.SIZE;
        float3 forwardRightCoord = cornerCoord + (float3)Vector3.forward * Chunk.SIZE + (float3)Vector3.right * Chunk.SIZE;
        float3 upCoord = cornerCoord + (float3)Vector3.up * Chunk.SIZE;
        float3 upRightCoord = upCoord + (float3)Vector3.right * Chunk.SIZE;
        float3 upForwardCoord = upCoord + (float3)Vector3.forward * Chunk.SIZE;
        float3 upForwardRightCoord = upCoord + (float3)Vector3.forward * Chunk.SIZE + (float3)Vector3.right * Chunk.SIZE;

        Color color = Color.yellow;
        Debug.DrawLine(cornerCoord, rightCoord, color);
        Debug.DrawLine(cornerCoord, forwardCoord, color);
        Debug.DrawLine(rightCoord, forwardRightCoord, color);
        Debug.DrawLine(forwardCoord, forwardRightCoord, color);

        Debug.DrawLine(upCoord, upRightCoord, color);
        Debug.DrawLine(upCoord, upForwardCoord, color);
        Debug.DrawLine(upRightCoord, upForwardRightCoord, color);
        Debug.DrawLine(upForwardCoord, upForwardRightCoord, color);

        Debug.DrawLine(cornerCoord, upCoord, color);
        Debug.DrawLine(rightCoord, upRightCoord, color);
        Debug.DrawLine(forwardCoord, upForwardCoord, color);
        Debug.DrawLine(forwardRightCoord, upForwardRightCoord, color);
    }
}
