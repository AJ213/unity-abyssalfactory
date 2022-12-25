using UnityEngine;

public class UnityHook : MonoBehaviour
{
    public FileSystemData Database;
    Program program;
    void Start()
    {
        program = new Program(Database, this.gameObject);
    }
    
    void Update()
    {
        program.Update();
    }
}
