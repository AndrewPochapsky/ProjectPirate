using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                //ignores the right and bottom edges of the verticies
                if(x < width-1 && y < height - 1)
                {
                    meshData.AddTriagle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriagle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }

}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triagles;

    public Vector2[] uvs;

    int triagleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triagles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriagle(int a, int b, int c)
    {
        triagles[triagleIndex] = a;
        triagles[triagleIndex+1] = b;
        triagles[triagleIndex+2] = c;

        triagleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triagles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        return mesh;
    }

}
