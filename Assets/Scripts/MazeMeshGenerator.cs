using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator
{

    public Mesh FromData(int[,] data, float width, float height, Vector3 transformPos)
    {
        Mesh maze = new Mesh();

        //3
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        maze.subMeshCount = 2;
        List<int> floorTriangles = new List<int>();
        List<int> wallTriangles = new List<int>();

        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);
        float halfH = height * .5f;

        //4
        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (data[i, j] == 1 && i != 0 && i != rMax &&
                    j != 0 && j != cMax)
                {
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(transformPos.x + j * width, transformPos.y + height, transformPos.z + i * width),
                        Quaternion.LookRotation(Vector3.up),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);
                }
                //ja konkrētā šūna ir blank..
                if (data[i, j] != 1)
                {
                    // floor
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(transformPos.x + j * width, transformPos.y, transformPos.z + i * width),
                        Quaternion.LookRotation(Vector3.up),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);

                    // ceiling
                    //ToDo: Only for obstacles
                    //Te būs tikai 0 value šūnas
                    //Debug.Log(string.Format("{0} {1}: {2}",i ,j, data[i, j]));

                

                    // walls on sides next to blocked grid cells
                    //i-1<0 is corner wall OR is filled 
                    if (i - 1 < 0 || data[i - 1, j] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(transformPos.x + j * width, transformPos.y + halfH, transformPos.z + (i - .5f) * width),
                            Quaternion.LookRotation(Vector3.forward),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (j + 1 > cMax || data[i, j + 1] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(transformPos.x + (j + .5f) * width, transformPos.y + halfH, transformPos.z + i * width),
                            Quaternion.LookRotation(Vector3.left),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (j - 1 < 0 || data[i, j - 1] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(transformPos.x + (j - .5f) * width, transformPos.y + halfH, transformPos.z + i * width),
                            Quaternion.LookRotation(Vector3.right),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (i + 1 > rMax || data[i + 1, j] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(transformPos.x + j * width, transformPos.y + halfH, transformPos.z + (i + .5f) * width),
                            Quaternion.LookRotation(Vector3.back),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }
                }
            }
        }

        maze.vertices = newVertices.ToArray();
        maze.uv = newUVs.ToArray();

        maze.SetTriangles(floorTriangles.ToArray(), 0);
        maze.SetTriangles(wallTriangles.ToArray(), 1);

        //5
        maze.RecalculateNormals();

        return maze;
    }

    //1, 2
    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices,
        ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        int index = newVertices.Count;

        // corners before transforming
        Vector3 vert1 = new Vector3(-.5f, -.5f, 0);
        Vector3 vert2 = new Vector3(-.5f, .5f, 0);
        Vector3 vert3 = new Vector3(.5f, .5f, 0);
        Vector3 vert4 = new Vector3(.5f, -.5f, 0);

        newVertices.Add(matrix.MultiplyPoint3x4(vert1));
        newVertices.Add(matrix.MultiplyPoint3x4(vert2));
        newVertices.Add(matrix.MultiplyPoint3x4(vert3));
        newVertices.Add(matrix.MultiplyPoint3x4(vert4));

        newUVs.Add(new Vector2(1, 0));
        newUVs.Add(new Vector2(1, 1));
        newUVs.Add(new Vector2(0, 1));
        newUVs.Add(new Vector2(0, 0));

        newTriangles.Add(index + 2);
        newTriangles.Add(index + 1);
        newTriangles.Add(index);

        newTriangles.Add(index + 3);
        newTriangles.Add(index + 2);
        newTriangles.Add(index);
    }

}
