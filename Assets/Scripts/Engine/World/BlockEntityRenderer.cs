using System;
using System.Collections.Generic;
using UnityEngine;
public class BlockEntityRenderer {
    Dictionary<int, BlockEntityRenderData> renderTargets = new Dictionary<int, BlockEntityRenderData>();
    const int matrixStride = sizeof(float) * 16;
    const int uintStride = sizeof(uint);
    Bounds bounds;

    public void AddBlockEntity(int ID, Matrix4x4 transform)
    {
        if(!renderTargets.ContainsKey(ID)){
            BlockEntityRenderData renderData = new BlockEntityRenderData();
            renderData.Voxel = Program.GlobalDatabase.GetVoxel(ID);
            renderData.Transforms.Add(transform);
            renderTargets.Add(ID, renderData);
        } else {
            BlockEntityRenderData renderData = renderTargets[ID];
            renderData.Transforms.Add(transform);
        }
    }

    public void RemoveBlockEntity(int ID, Matrix4x4 transform)
    {
        if (renderTargets.ContainsKey(ID))
        {
            BlockEntityRenderData renderData = renderTargets[ID];
            bool worked = renderData.Transforms.Remove(transform);
            if(!worked){
                throw new SystemException($"Bro, for {ID}, this matrix {transform} doesn't exist wtf is wrong");
            }
            if(renderData.Transforms.Count == 0){
                renderTargets.Remove(ID);
            }
        } else {
            throw new SystemException($"Bro, you can't remove an ID {ID} that doesn't exist");
        }
    }
    
    public void Render(){
        foreach(BlockEntityRenderData renderData in renderTargets.Values) {
            int instanceCount = renderData.Transforms.Count;

            renderData.Release();

            renderData.argsBuffer = new ComputeBuffer(5, uintStride, ComputeBufferType.IndirectArguments);

            renderData.argsData[0] = renderData.Voxel.EntityMesh.GetIndexCount(0);
            renderData.argsData[1] = (uint)instanceCount;
            renderData.argsData[2] = renderData.Voxel.EntityMesh.GetIndexStart(0);
            renderData.argsData[3] = renderData.Voxel.EntityMesh.GetBaseVertex(0);
            renderData.argsData[4] = 0;

            renderData.argsBuffer.SetData(renderData.argsData);

            Matrix4x4[] matrices = renderData.Transforms.ToArray();

            foreach(Matrix4x4 mat in matrices){
                Vector3 position = mat.GetPosition();
                Vector3 forward = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1)) * Vector3.forward;
                Vector3 right = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1)) * Vector3.right;
                Vector3 left = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1)) * Vector3.left;

                Vector3 endPoint = position + forward * 0.3f;

                Debug.DrawLine(position, endPoint);
                Debug.DrawLine(endPoint, endPoint - forward * 0.1f + right * 0.1f);
                Debug.DrawLine(endPoint, endPoint - forward * 0.1f + left * 0.1f);
            }

            renderData.matricesBuffer = new ComputeBuffer(instanceCount, matrixStride);
            renderData.matricesBuffer.SetData(matrices);
            renderData.Voxel.EntityMaterial.SetBuffer("positionBuffer", renderData.matricesBuffer);
            bounds = new Bounds(new Vector3(Chunk.SIZE, Chunk.SIZE, Chunk.SIZE) / 2, new Vector3(Chunk.SIZE, Chunk.SIZE, Chunk.SIZE)); // need to change position
            Graphics.DrawMeshInstancedIndirect(renderData.Voxel.EntityMesh, 0, renderData.Voxel.EntityMaterial, bounds, renderData.argsBuffer);
        }
    }
 
    public void OnDisable(){
        foreach (BlockEntityRenderData renderData in renderTargets.Values){
            renderData.Release();
        }
    }
}

class BlockEntityRenderData {
    public Voxel Voxel;
    public List<Matrix4x4> Transforms = new List<Matrix4x4>();
    public ComputeBuffer argsBuffer;
    public ComputeBuffer matricesBuffer;
    public uint[] argsData = new uint[5];

    public void Release(){
        argsBuffer?.Release();
        matricesBuffer?.Release();
    }
}