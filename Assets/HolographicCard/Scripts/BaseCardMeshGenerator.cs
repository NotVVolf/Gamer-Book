using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Mathematics.math;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BaseCardMeshGenerator : MonoBehaviour {
    [SerializeField] [Range(0, 50)] public float cornerRadius;
    [SerializeField] [Range(0, 100)] public float width;
    [SerializeField] [Range(0, 100)] public float height;
    Mesh mesh;
    NativeArray<float3> vertexBuffer;
    NativeArray<ushort> indexBuffer;
    void Awake() { Build(); }
    [ContextMenu("Build")]
    public void Build() {
        mesh = new Mesh() {name = "Procedural Card"};
        GetComponent<MeshFilter>().sharedMesh = mesh;
        InitializeMesh();
    }
    public void SetWidth(float newWidth) {
        width = newWidth;
        RecalculateVertices();
        UpdateMesh();
    }
    public void SetHeight(float newHeight) {
        height = newHeight;
        RecalculateVertices();
        UpdateMesh();
    }
    public void SetCurveRadius(float newCornerRadius) {
        cornerRadius = newCornerRadius;
        RecalculateVertices();
        UpdateMesh();
    }
    // C for corner, Q for quad
    // C Q C
    // Q Q Q
    // C Q C

    //Increase this number to increase/decrease complexity.
    const int CornerDetail = 10;
    //1 is for the center, 2 is for the bare minimum vertex, CurveDetail is extra
    const int VerticesPerCorner = 1 + 2 + CornerDetail;
    //Just for ease of use
    const int VerticesPerCurve = 2 + CornerDetail;

    //Curve vertex count * (4 total corners) + 4 vertices per quad * (5 total quads)
    const int VertexCount = VerticesPerCorner * 4 + 4 * 5;
    //Triangles per corner * (4 total corners) + 2 triangles per quad * (5 total quads)
    const int TriangleCount = (CornerDetail - 1) * 4 + 2 * 5;
    const int IndicesPerCorner = (CornerDetail - 1) * 3;
    //3 indices per triangle
    const int IndexCount = TriangleCount * 3;
    void InitializeMesh() {
        DisposeBuffers();
        AllocateBuffers();
        RecalculateVertices();

        mesh.SetVertexBufferParams(VertexCount, new VertexAttributeDescriptor(dimension: 3));
        mesh.SetVertexBufferData(vertexBuffer, 0, 0, VertexCount);

        mesh.SetIndexBufferParams(IndexCount, IndexFormat.UInt16);
        mesh.SetIndexBufferData(indexBuffer, 0, 0, IndexCount);

        mesh.SetSubMesh(0, new SubMeshDescriptor(0, IndexCount));
        mesh.RecalculateBounds();
    }
    //Buffers
    //Quad Structure:
    //n+1 --- n+2
    //|        |
    //|        |
    //n   --- n+3
    //Vertex order
    //Center Quad, //0 1 2 3
    //Right Quad, //4 5 6 7
    //Top Quad, //8 9 10 11
    //Left Quad, //12 13 14 15
    //Bottom Quad, //16 17 18 19
    //n = detail
    //Top Right Corner, //Corner Vertex, CV + 1, CV + 2, CV + 3... CV + n
    //Top Left Corner, //Corner Vertex, CV + 1, CV + 2, CV + 3... CV + n
    //Bottom Left Corner, //Corner Vertex, CV + 1, CV + 2, CV + 3... CV + n
    //Bottom Right Corner //Corner Vertex, CV + 1, CV + 2, CV + 3... CV + n
    void AllocateBuffers() {
        indexBuffer = new NativeArray<ushort>(IndexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        vertexBuffer = new NativeArray<float3>(VertexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        //Triangle stuff
        //30 indices, 20 vertices
        for (int i = 0; i < 5; i++) {
            //(n, n+1, n+2)
            indexBuffer[i * 6 + 0] = (ushort) (i * 4);
            indexBuffer[i * 6 + 1] = (ushort) (i * 4 + 1);
            indexBuffer[i * 6 + 2] = (ushort) (i * 4 + 2);
            //(n, n+2, n+3)
            indexBuffer[i * 6 + 3] = (ushort) (i * 4);
            indexBuffer[i * 6 + 4] = (ushort) (i * 4 + 2);
            indexBuffer[i * 6 + 5] = (ushort) (i * 4 + 3);
        }
        int baseIndexIndex = 30;
        int baseVertexIndex = 20;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < CornerDetail - 1; j++) {
                indexBuffer[baseIndexIndex + j * 3 + 0] = (ushort) (baseVertexIndex + j + 1);
                indexBuffer[baseIndexIndex + j * 3 + 1] = (ushort) baseVertexIndex;
                indexBuffer[baseIndexIndex + j * 3 + 2] = (ushort) (baseVertexIndex + j + 2);
            }
            baseIndexIndex += IndicesPerCorner;
            baseVertexIndex += VerticesPerCorner;
        }
    }
    void RecalculateVertices() {
        //30 indices, 20 vertices
        //Center
        //(n, n+1, n+2, n+3)
        float halfWidth = width / 2;
        float halfHeight = height / 2;
        AddQuadToVertexBuffer(0, float4(-halfWidth, -halfHeight, halfWidth, halfHeight));
        AddQuadToVertexBuffer(1, float4(halfWidth, -halfHeight, halfWidth + cornerRadius, halfHeight)); //Right
        AddQuadToVertexBuffer(2, float4(-halfWidth, halfHeight, halfWidth, halfHeight + cornerRadius)); //Top
        AddQuadToVertexBuffer(3, float4(-halfWidth - cornerRadius, -halfHeight, -halfWidth, halfHeight)); //Left
        AddQuadToVertexBuffer(4, float4(-halfWidth, -halfHeight - cornerRadius, halfWidth, -halfHeight)); //Bottom
        //
        const int QuadVertexCount = 20;
        AddCornerToVertexBuffer(QuadVertexCount + VerticesPerCorner * 0, float2(halfWidth, halfHeight), radians(0)); //TopRight
        AddCornerToVertexBuffer(QuadVertexCount + VerticesPerCorner * 1, float2(-halfWidth, halfHeight), radians(90)); //TopLeft
        AddCornerToVertexBuffer(QuadVertexCount + VerticesPerCorner * 2, float2(-halfWidth, -halfHeight), radians(180)); //BottomLeft
        AddCornerToVertexBuffer(QuadVertexCount + VerticesPerCorner * 3, float2(halfWidth, -halfHeight), radians(270)); //BottomRight
    }
    //Positions: lowerLeftX, lowerLeftY, upperRightX, upperRightY
    void AddQuadToVertexBuffer(int quadIndex, float4 positions) {
        vertexBuffer[quadIndex * 4 + 0] = float3(positions.xy, 0);
        vertexBuffer[quadIndex * 4 + 1] = float3(positions.xw, 0);
        vertexBuffer[quadIndex * 4 + 2] = float3(positions.zw, 0);
        vertexBuffer[quadIndex * 4 + 3] = float3(positions.zy, 0);
    }
    //startingIndex is also the same as the base corner index
    void AddCornerToVertexBuffer(int startingIndex, float2 cornerPosition, float bonusRotation) {
        //Get direction from position
        vertexBuffer[startingIndex] = float3(cornerPosition, 0);
        const float AddAmount = (Mathf.PI / 2) / (VerticesPerCurve - 3);
        for (int i = 0; i < VerticesPerCurve; i++) {
            float degrees = bonusRotation + i * AddAmount;
            vertexBuffer[startingIndex + 1 + i] = float3(cornerPosition + float2(cos(degrees), sin(degrees)) * cornerRadius, 0);
        }
    }
    void UpdateMesh() { mesh.SetVertexBufferData(vertexBuffer, 0, 0, VertexCount); }
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