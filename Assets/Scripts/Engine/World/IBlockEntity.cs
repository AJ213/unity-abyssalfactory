using Unity.Mathematics;
public interface IBlockEntity {
    public Voxel Voxel { get; set; }
    public EntityRegion EntityRegion { get; set;  }
    public World.Direction Direction { get; set; }
    public bool OnCreate(Voxel ID, EntityRegion region, World.Direction direction);
    public void Update(float fixedDeltaTime);
    public void Render(float timeDelta);
}
