using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class Grid : MonoBehaviour
{
    Mesh mesh;

    public int xSize = 20;
    public int zSize = 20;

    public float[] octaveFrequencies = { 10,9,8,7,6,5,4,3,2,1 };
    public float[] octaveAmplitudes = { 1,2,3,4,5,6,7,8,9,10 };

    public Gradient gradient;
    public Gradient fillgra;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;
    public float maxheight = 0;
    public float minheight = 0;
    public Color even;
    public Color fillstart;
    public int fillstartx;
    public int fillstartz;

    public float tolerance = 1;
    MeshCollider meshCollider;
    public int seed;


   

    /// Add a context menu named "Do Something" in the inspector
    /// of the attached script.
    [ContextMenu("GenMesh")]
    void GenMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
      
       
        Debug.Log("Genned Mesh");
    }


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
      
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];


        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = 0;
                for (int o = 0; o < octaveFrequencies.Length; o++)
                    y += octaveAmplitudes[o] * Mathf.PerlinNoise(
                         octaveFrequencies[o] * (x + seed) + .3f,
                         octaveFrequencies[o] * (z + seed) + .3f) * 2f;


                if (y > maxheight)
                    maxheight = y;
                if (y < minheight)
                    minheight = y;


                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        mesh.vertices = vertices;

        int vert = 0;
        int tris = 0;

        triangles = new int[6 * xSize * zSize];

        for (int z = 0; z < zSize; z++)
        {


            for (int x = 0; x < xSize; x++)
            {

                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;


                vert++;
                tris += 6;
                
            }
            vert++;
        }

        colors = new Color[vertices.Length];



        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //float height = Mathf.InverseLerp(minheight , maxheight, vertices[i].y);
                try
                {


                    float height = Mathf.InverseLerp(0, zSize * xSize, (float)(z * zSize + x));
                    colors[i] = gradient.Evaluate(height);
                }
                catch {
                    return;
                }

                
                i++;

                
            }
        }

        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                if (Mathf.Abs(vertices[z * zSize + x].y - vertices[z * zSize + x + 1].y) < tolerance)
                    if (Mathf.Abs(vertices[z * zSize + x].y - vertices[z * zSize + x - 1].y) < tolerance)
                        if (Mathf.Abs(vertices[z * zSize + x].y - vertices[(z - 1) * zSize + x + 1].y) < tolerance)
                            if (Mathf.Abs(vertices[z * zSize + x].y - vertices[(z + 1) * zSize + x + 1].y) < tolerance)
                            {
                               
                                colors[z * zSize + x] = even;
                          


                                colors[z * zSize + x + 1] = even;
                                colors[z * zSize + x - 1] = even;
                                colors[(z + 1) * zSize + x] = even;
                                colors[(z - 1) * zSize + x] = even;
                            }

                
            }
        }

        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {


                //if (colors[z * zSize + x] == even)
                    //fillco(x, z, 10);                      
            }
        }




    }

    private void Update()
    {
       
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        // add collider! 
        meshCollider = new MeshCollider();
        meshCollider.sharedMesh = mesh;
    }
    /*
    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

    */


    void fillco(int x, int z, int g)
    {

        colors[z * zSize + x] = fillgra.Evaluate(Mathf.InverseLerp(0f,10f,(float)g));
        

        if (g == 0)
            return;

        if ((x == 0) || (x == xSize - 1))
            return;
        if ((z == 0) || (z == zSize - 1))
            return;

        try
        {
            if (colors[z * zSize + x + 1] == even)
                fillco(z, x + 1, g - 1);

            if (colors[z * zSize + x - 1] == even)
                fillco(z, x - 1, g - 1);

            if (colors[(z + 1) * zSize + x] == even)
                fillco(z + 1, x, g - 1);

            if (colors[(z - 1) * zSize + x] == even)
                fillco(z - 1, x, g - 1);
        }
        catch
        {
        }



    }

    
}
