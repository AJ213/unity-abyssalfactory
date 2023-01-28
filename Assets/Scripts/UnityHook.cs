using UnityEngine;

public class UnityHook : MonoBehaviour
{
    public FileSystemData Database;
    Program program;

    void Awake()
    {
        program = new Program(Database, this.gameObject);
    }
    
    void Update()
    {
        program.Update();
    }

    private void OnDisable()
    {
        program.OnDisable();
    }
}
