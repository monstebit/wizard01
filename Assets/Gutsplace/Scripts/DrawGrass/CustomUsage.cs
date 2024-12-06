using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

class CustomUsage : MonoBehaviour
{
    struct DrawData
    {
        public Vector3 Pos;
        public Quaternion Rot;
        public Vector3 Scale;
    }

    public Mesh mesh;
    public Material material;
    public float planeWidth = 10f; // Ширина плоскости
    public float planeHeight = 10f; // Высота плоскости

    List<DrawData> instances;

    ComputeBuffer drawDataBuffer;
    ComputeBuffer argsBuffer;
    uint[] args = new uint[5];
    MaterialPropertyBlock mpb;

    void Awake()
    {
        instances = new List<DrawData>();

        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
        // Meshes with sub-meshes needs more structure, this assumes a single sub-mesh
        args[0] = mesh.GetIndexCount(0);

        mpb = new MaterialPropertyBlock();

        SpawnExample();
    }

    void OnDestroy()
    {
        argsBuffer?.Release();
        drawDataBuffer?.Release();
    }

    void LateUpdate()
    {
        // Only needs to be called if "instances" changed
        PushDrawData();
        mpb.SetBuffer("_DrawData", drawDataBuffer); //  StructuredBuffer<DrawData> _DrawData;

        args[1] = (uint)instances.Count;
        argsBuffer.SetData(args);

        Graphics.DrawMeshInstancedIndirect(
          mesh, 0, material,
          new Bounds(Vector3.zero, Vector3.one * 1000f),
          argsBuffer, 0,
          mpb
        );
    }

    void PushDrawData()
    {
        if (drawDataBuffer == null || drawDataBuffer.count < instances.Count)
        {
            drawDataBuffer?.Release();
            drawDataBuffer = new ComputeBuffer(instances.Count, Marshal.SizeOf<DrawData>());
        }
        drawDataBuffer.SetData(instances);
    }

    void SpawnExample()
    {
        instances.Clear();
        // for (int i = 0; i < 4096; i++)
        for (int i = 0; i < 30000; i++)
        // for (int i = 0; i < 50000; i++)
        {
            instances.Add(new DrawData()
            {
                Pos = new Vector3(
                    Random.Range(-planeWidth / 2, planeWidth / 2),
                    0,
                    Random.Range(-planeHeight / 2, planeHeight / 2)
                ),
                Rot = Quaternion.identity, // Стандартное вращение
                Scale = Vector3.one // Стандартный масштаб
            });
        }
    }
}