using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OutlineCardMesh : MonoBehaviour {
    [SerializeField] BaseCardMeshGenerator baseCard;
    Mesh mesh;
    NativeArray<Vertex> vertexBuffer;
    NativeArray<ushort> indexBuffer;
    [StructLayout(LayoutKind.Sequential)]
    struct Vertex {
        public float3 position;
        public float2 texCoord0;
    }
    void Awake() { Build(); }
    [ContextMenu("Build")]
    public void Build() {
        mesh = new Mesh() {name = "Procedural Card"};
        GetComponent<MeshFilter>().sharedMesh = mesh;
        InitializeMesh();
    }
    void InitializeMesh() {
        DisposeBuffers();
        AllocateBuffers();
        RecalculateVertices();

        VertexAttributeDescriptor[] descriptors = new VertexAttributeDescriptor[2] {
            new VertexAttributeDescriptor(dimension: 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, dimension: 2)
        };
        mesh.SetVertexBufferParams(4, descriptors);
        mesh.SetVertexBufferData(vertexBuffer, 0, 0, 4);

        mesh.SetIndexBufferParams(6, IndexFormat.UInt16);
        mesh.SetIndexBufferData(indexBuffer, 0, 0, 6);

        mesh.SetSubMesh(0, new SubMeshDescriptor(0, 6));
        mesh.RecalculateBounds();
    }
    void AllocateBuffers() {
        indexBuffer = new NativeArray<ushort>(6, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        vertexBuffer = new NativeArray<Vertex>(4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        indexBuffer[0] = 0;
        indexBuffer[1] = 1;
        indexBuffer[2] = 2;
        indexBuffer[3] = 2;
        indexBuffer[4] = 3;
        indexBuffer[5] = 0;
    }
    void RecalculateVertices() {
        float halfWidth = baseCard.width / 2 + baseCard.cornerRadius;
        float halfHeight = baseCard.height / 2 + baseCard.cornerRadius;
        vertexBuffer[0] = new Vertex() {
            position = new float3(-halfWidth, -halfHeight, 0),
            texCoord0 = new float2(-halfWidth, -halfHeight),
        };
        vertexBuffer[1] = new Vertex() {
            position = new float3(-halfWidth, halfHeight, 0),
            texCoord0 = new float2(-halfWidth, halfHeight),
        };
        vertexBuffer[2] = new Vertex() {
            position = new float3(halfWidth, halfHeight, 0),
            texCoord0 = new float2(halfWidth, halfHeight),
        };
        vertexBuffer[3] = new Vertex() {
            position = new float3(halfWidth, -halfHeight, 0),
            texCoord0 = new float2(halfWidth, -halfHeight),
        };
    }
    void UpdateMesh() { mesh.SetVertexBufferData(vertexBuffer, 0, 0, 4); }
    void DisposeBuffers() {
        mesh.Clear();
        if (indexBuffer.IsCreated) indexBuffer.Dispose();
        if (vertexBuffer.IsCreated) vertexBuffer.Dispose();
    }
    void OnDisable() => DisposeBuffers();
    void OnDestroy() {
        if (mesh != null) Destroy(mesh);
        DisposeBuffers();
    }
}