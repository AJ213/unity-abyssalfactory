using UnityEngine;

public class UnityHook : MonoBehaviour
{
    public FileSystemData Database;
    Program program;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;


    void Awake()
    {
        program = new Program(Database, this.gameObject);
    }
    
    void Update()
    {
        program.Update(mesh, material);
    }
}
