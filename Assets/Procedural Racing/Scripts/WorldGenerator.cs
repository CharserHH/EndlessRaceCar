using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldGenerator : MonoBehaviour
{
    public Material meshMatiaral;
    public float scale;
    public float perlinScale;
    public float offset;
    public float waveHeight;
    public Vector2 dimensions;
    public float globalSpeed;
    public float randomness;
    public int startTransitionLength = 5;
    public int startObstacleChance;
    public int gateChance;
    public GameObject gate;
    public GameObject[] obstacles;

    private Vector3[] beginPoints;
    private GameObject currentCyclinder;

    GameObject[] pieces = new GameObject[2];
    void Start()
    {
        beginPoints = new Vector3[(int) dimensions.x + 1];

        for(int i = 0; i < 2; i++){
            GenerateWorldPiece(i);
        }
    }

    private void LateUpdate() {
        if(pieces[1] && pieces[1].transform.position.z <= -15f){
            StartCoroutine(UpdateWorldPieces());
        }
    }

    IEnumerator UpdateWorldPieces(){
        Destroy(pieces[0]);
        pieces[0] = pieces[1];
        pieces[1] = CreateCylinder();

        // 设置位置
        pieces[1].transform.position = pieces[0].transform.position + (Vector3.forward * dimensions.y * scale * Mathf.PI);
        pieces[1].transform.rotation = pieces[0].transform.rotation;

        // 更新单个piece
        UpdateSinglePiece(pieces[1]);

        yield return 0;
    }
    void GenerateWorldPiece(int index){
        // 生成一个圆柱体 保存到pieces数组中
        pieces[index] = CreateCylinder();

        // 设置位置
        pieces[index].transform.Translate(Vector3.forward * dimensions.y * scale * Mathf.PI * index);

        // 标记尾部的位置 用于将来的生成
        UpdateSinglePiece(pieces[index]);
    }

    void UpdateSinglePiece(GameObject piece){
        // 挂载移动脚本
        BasicMovement movement = piece.AddComponent<BasicMovement>();
        movement.moveSpeed = -globalSpeed;
        movement.rotationSpeed = 30f;

        // 创建结束点
        GameObject endPoint = new GameObject("End Point");
        endPoint.transform.position = piece.transform.position + Vector3.forward * dimensions.y * scale * Mathf.PI;
        endPoint.transform.parent = piece.transform;

        offset += randomness;
    }
    public GameObject CreateCylinder(){
        // 创建GameObject并命名
        GameObject newCylinder = new GameObject("World Piece");
        currentCyclinder = newCylinder;

        // 添加网格MeshFilter和MeshRenderer组件
        MeshFilter meshFilter = newCylinder.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newCylinder.AddComponent<MeshRenderer>();

        meshRenderer.material = meshMatiaral;
        meshFilter.mesh = Generate();

        // 添加MeshCollider组件
        newCylinder.AddComponent<MeshCollider>();

        return newCylinder;
    }

    private Mesh Generate(){
        // 创建一个Mesh
        Mesh mesh = new Mesh();
        mesh.name = "MESH";

        // uv, 顶点, 三角形
        Vector3[] vertices = null;
        Vector2[] uvs = null;
        int[] triangles = null;

        //创建形状
        CreateShape(ref vertices, ref uvs, ref triangles);

        // 赋值
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        // 重新计算法线和边界体积
        mesh.RecalculateNormals();

        return mesh;
    }

    private void CreateShape(ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles){
        // 向z轴方向延伸，x是横截面
        int xCount = (int)dimensions.x;
        int zCount = (int)dimensions.y;

        // 初始化顶点和uv数组 通过xCount和zCount计算顶点数量
        vertices = new Vector3[(xCount + 1) * (zCount + 1)];
        uvs = new Vector2[vertices.Length];

        // 半径计算
        float radius = xCount * scale * 0.5f;

        // 生成顶点和uv
        int index = 0;
        for (int x = 0; x <= xCount; x++)
        {
            for (int z = 0; z <= zCount; z++)
            {
                // 计算顶点的位置
                float angle = x * Mathf.PI * 2f / xCount;
                float xPos = Mathf.Cos(angle) * radius;
                float zPos = Mathf.Sin(angle) * radius;

                // 顶点和uv赋值
                vertices[index] = new Vector3(xPos, zPos, z * scale * Mathf.PI);
                uvs[index] = new Vector2(x * scale, z * scale);

                // 柏林噪声
                float pX = (vertices[index].x * perlinScale) + offset;
                float pZ = (vertices[index].z * perlinScale) + offset;

                // 需要一个中心点与当前顶点做差值后归一化 再计算柏林噪声
                Vector3 center = new Vector3(0, 0, vertices[index].z);
                vertices[index] += (center - vertices[index]).normalized * Mathf.PerlinNoise(pX, pZ) * waveHeight;

                if (z < startTransitionLength && beginPoints[x] != Vector3.zero){
                    float perlinPercentage = z * (1f / startTransitionLength);
                    Vector3 beginPoint = new Vector3(beginPoints[x].x, beginPoints[x].y, vertices[index].z);
                    vertices[index] = (perlinPercentage * vertices[index]) + ((1 - perlinPercentage) * beginPoint);
                } else if (z == zCount){
                    beginPoints[x] = vertices[index];
                }

                if (Random.Range(0, startObstacleChance) == 0 && !(gate == null && obstacles.Length == 0)){
                    CreateItem(vertices[index], x);
                }

                index++;
            }
        }

        // 生成三角形
        triangles = new int[xCount * zCount * 6];

        // 创建一个数组来存储三角形的6个顶点 方便调用
        int[] boxBase = new int[6];
        int current = 0;

        for (int x = 0; x < xCount; x++)
        {
            boxBase = new int[]{
                x * (zCount + 1),
                x * (zCount + 1) + 1,
                (x + 1) * (zCount + 1),
                x * (zCount + 1) + 1,
                (x + 1) * (zCount + 1) + 1,
                (x + 1) * (zCount + 1)
            };

            for (int z = 0; z < zCount; z++)
            {
                // 更新索引
                for (int i = 0; i < 6; i++)
                {
                    boxBase[i] += 1;
                }

                for (int j = 0; j < 6; j++)
                {
                    triangles[current + j] = boxBase[j] - 1;
                }

                current += 6;
            }
        }
    }
    private void CreateItem(Vector3 vert, int x){
        Vector3 zCenter = new Vector3(0, 0, vert.z);

        GameObject newItem = Instantiate(Random.Range(0, gateChance) == 0 ? gate : obstacles[Random.Range(0, obstacles.Length)]);
        newItem.transform.rotation = Quaternion.LookRotation(zCenter - vert, Vector3.up);
        newItem.transform.position = vert;
        newItem.transform.SetParent(currentCyclinder.transform, false);
    }
    public Transform GetWorldPiece(){
        return pieces[0].transform;
    }
}
