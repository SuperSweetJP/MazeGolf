using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator
{
    public Mesh FromData(int[,] data, float width, float height, Vector3 transformPos)
    {
        Mesh maze = new Mesh();

        // Lists to hold vertices, UVs, and triangles for different materials
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        maze.subMeshCount = 3; // Three sub-meshes: floor, walls, and roofs
        List<int> floorTriangles = new List<int>();
        List<int> wallTriangles = new List<int>();
        List<int> roofTriangles = new List<int>();

        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);
        float halfH = height * 0.5f;

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                // Floor generation
                if (data[i, j] != 1)
                {
                    // Floor
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(transformPos.x + j * width, transformPos.y, transformPos.z + i * width),
                        Quaternion.LookRotation(Vector3.up),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);

                    // Walls on sides next to blocked cells
                    if (i - 1 < 0 || data[i - 1, j] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(transformPos.x + j * width, transformPos.y + halfH, transformPos.z + (i - 0.5f) * width),
                            Quaternion.LookRotation(Vector3.forward),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (j + 1 > cMax || data[i, j + 1] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(transformPos.x + (j + 0.5f) * width, transformPos.y + halfH, transformPos.z + i * width),
                            Quaternion.LookRotation(Vector3.left),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (j - 1 < 0 || data[i, j - 1] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(transformPos.x + (j - 0.5f) * width, transformPos.y + halfH, transformPos.z + i * width),
                            Quaternion.LookRotation(Vector3.right),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (i + 1 > rMax || data[i + 1, j] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(transformPos.x + j * width, transformPos.y + halfH, transformPos.z + (i + 0.5f) * width),
                            Quaternion.LookRotation(Vector3.back),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }
                }

                // Roof generation (above obstacles)
                if (data[i, j] == 1 && i != 0 && i != rMax &&
                    j != 0 && j != cMax)
                {
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(transformPos.x + j * width, transformPos.y + height, transformPos.z + i * width),
                        Quaternion.LookRotation(Vector3.up),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref roofTriangles);
                }
            }
        }

        // Assign vertices and triangles to the mesh
        maze.vertices = newVertices.ToArray();
        maze.uv = newUVs.ToArray();

        maze.SetTriangles(floorTriangles.ToArray(), 0); // Floor material
        maze.SetTriangles(wallTriangles.ToArray(), 1); // Wall material
        maze.SetTriangles(roofTriangles.ToArray(), 2); // Roof material

        // Recalculate normals for correct lighting
        maze.RecalculateNormals();

        return maze;
    }

    // Utility method to add a quad
    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices,
        ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        int index = newVertices.Count;

        // Corners before transformation
        Vector3 vert1 = new Vector3(-0.5f, -0.5f, 0);
        Vector3 vert2 = new Vector3(-0.5f, 0.5f, 0);
        Vector3 vert3 = new Vector3(0.5f, 0.5f, 0);
        Vector3 vert4 = new Vector3(0.5f, -0.5f, 0);

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
