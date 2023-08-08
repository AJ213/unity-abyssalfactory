using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;


public class ItemTests
{
    World world;
    public static Database GlobalDatabase;
    public static World CurrentWorld;
    
    [SetUp]
    public void Setup()
    {
        
    }
    
    [UnityTest]
    public IEnumerator StorageToStorageTest()
    {
        Program program = new Program();
        yield return null;
        Debug.Log("Done");
    }
}
