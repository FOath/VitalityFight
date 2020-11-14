using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class TerrainGenerator : MonoBehaviour
{
    Noise noise;

    int[,] heightMap;

    public int width = 25;
    public GameObject[] normalCube;
    public GameObject[] specialCube;
    // Start is called before the first frame update
    void Start()
    {
        heightMap = new int[width, width];

        // 设置山峰数量(2 - 6)
        int peakNum = Random.Range(2, 8);
        int[] peaks = new int[peakNum];
        for(int i = 0; i < peakNum; ++i)
        {
            // 决定山峰高度
            int min = 11 - peakNum;
            int max = 13 - peakNum < 9 ? 13 - peakNum : 9;
            int height = Random.Range(min, max);
            int posX = Random.Range(0, width - 3);
            int posY = Random.Range(0, width - 3);

            // 更改高度图
            for(int h = 1; h <= height; ++h)
            {
                
                int x = posX + (h - 1), y = posY + (h - 1);
                if (x >= width || y >= width)
                    break;
                // 下
                for(; x <= posX + 2 * height - h - 1  && x < width; ++x)
                {
                    if (heightMap[x, y] == 0)
                        heightMap[x, y] = h;
                }

                --x;
                ++y;
                // 右
                for(; y <= posY + 2 * height - h - 1 && y < width; ++y)
                {                   
                    if (heightMap[x, y] == 0)
                        heightMap[x, y] = h;
                }
                --y;
                --x;
                // 上
                for(; x >= posX + h - 1; --x)
                {
                    if (heightMap[x, y] == 0)
                        heightMap[x, y] = h;
                }
                ++x;
                --y;
                // 左
                for(; y > posY + h - 1; --y)
                {
                    if (heightMap[x, y] == 0)
                        heightMap[x, y] = h;
                }
            }
        }
        StartCoroutine(GenerateTerrain());
    }
    IEnumerator GenerateTerrain()
    {
        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j  < width; ++j)
            {
                // 处理地面
                GameObject floor = (GameObject)Instantiate(normalCube[Random.Range(0, normalCube.Length)], new Vector3(i, -1, j), transform.rotation);
                floor.GetComponent<Rigidbody>().useGravity = false;
                floor.GetComponent<Rigidbody>().isKinematic = true;
                floor.transform.parent = transform;
                for (int k = 0; k < heightMap[i, j]; ++k)
                {
                    GameObject temp = null;
                    if (Random.value < 0.8)
                        temp = (GameObject)Instantiate(normalCube[Random.Range(0, normalCube.Length)], new Vector3(i, k, j), transform.rotation);
                    else
                        temp = (GameObject)Instantiate(specialCube[Random.Range(0, specialCube.Length)], new Vector3(i, k, j), transform.rotation);
                    temp.transform.parent = transform;
                }
            }
            yield return null;
        }
        //OptimizeMesh();
        yield break;
    }
    void OptimizeMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();       //获取自身和所有子物体中所有MeshFilter组件
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];    //新建CombineInstance数组

        print(meshFilters.Length);
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);       //合并
        //transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;
        transform.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
