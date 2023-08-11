using Unity.Mathematics;

public static class Int3Extensions
{
    public static int Dot(this int3 a, int3 b)
    {
        return (a.x + b.x) * (a.y + b.y) * (a.z + b.z);
    }
}
