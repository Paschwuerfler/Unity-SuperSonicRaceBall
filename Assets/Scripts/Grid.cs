using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class Grid : MonoBehaviour
{
    Mesh mesh;

    public int xSize = 20;
    public int zSize = 20;

    public float[] octaveFrequencies;  //terrain generation. should be filled with example values in future
    public float[] octaveAmplitudes;

    public Gradient gradient; //gradient based on height
    public Gradient fillgra; //gradient for filling in path sections

    Vector3[] vertices; //mesh gen
    int[] triangles;
    Color[] colors;
    public float maxheight = 0;
    public float minheight = 0;
    public Color even; //color for paths
    public Color fillstart; //debugging (not really in use rn)
    public int fillstartx;
    public int fillstartz;

    public float tolerance = 1; //tolerance for paths to appear
    MeshCollider meshCollider; //this doesnt fucking work, but sohuld establish collision with player in future
    public int seed; //ofset for perlin noise for "random" terrain




    /// Add a context menu named "GenMesh" in the inspector
    /// of the attached script.
    /// Has a tendency to crash the whole editor, use with cation, only after establishing stuff works in playmode
    [ContextMenu("GenMesh")]
    void GenMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();

        UpdateMesh();
    }



    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape(); 

        UpdateMesh();
    }

    void CreateShape()//creates (and colors) random terrain
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)]; //creates size + 1 for roght number of "squares"


        for (int i = 0, z = 0; z <= zSize; z++)  //creates perlin-noise terran heights)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = 0;
                for (int o = 0; o < octaveFrequencies.Length; o++) //octave generation
                    y += octaveAmplitudes[o] * Mathf.PerlinNoise(
                         octaveFrequencies[o] * (x + seed) + .3f,
                         octaveFrequencies[o] * (z + seed) + .3f) * 2f;


                if (y > maxheight)  //sets min and maxheight for right coloring
                    maxheight = y;
                if (y < minheight)
                    minheight = y;


                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        mesh.vertices = vertices; //also done in update so why here ?

        int vert = 0;
        int tris = 0;

        triangles = new int[6 * xSize * zSize];

        for (int z = 0; z < zSize; z++) //fills in connectios btween verts
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

        colors = new Color[vertices.Length]; //sure this is the right length ?



        for (int i = 0, z = 0; z <= zSize; z++)  //coloring based on height of map
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minheight , maxheight, vertices[i].y);
                try
                {


                    //float height = Mathf.InverseLerp(0, zSize * xSize, (float)(convertxy(x, z)));
                    colors[i] = gradient.Evaluate(height);
                }
                catch {
                    return;
                }

                
                i++;

                
            }
        }

        for (int z = 1; z < zSize; z++)  //path coloring based on height of 4 adjcent quares
        {
            for (int x = 1; x < xSize; x++)
            {
                if (Mathf.Abs(vertices[convertxy(x, z)].y - vertices[convertxy(x, z) + 1].y) < tolerance)
                    if (Mathf.Abs(vertices[convertxy(x, z)].y - vertices[convertxy(x, z) - 1].y) < tolerance)
                        if (Mathf.Abs(vertices[convertxy(x, z)].y - vertices[convertxy(x, z - 1)+ 1].y) < tolerance)
                            if (Mathf.Abs(vertices[convertxy(x, z)].y - vertices[convertxy(x, z - 1)+ 1].y) < tolerance) 
                            {
                               
                                colors[convertxy(x, z)] = even;
                          


                                colors[convertxy(x, z) + 1] = even;
                                colors[convertxy(x, z) - 1] = even;
                                colors[(z + 1) * zSize + x] = even;
                                colors[(z - 1) * zSize + x] = even;
                            }

                
            }
        }

        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {


                //if (colors[convertxy(x, z)] == even)
                    //fillco(x, z, 10);                      
            }
        }




    }



    void UpdateMesh() //carries over changes to actual mesh 
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

    private void OnDrawGizmos() 
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .3f);
        }
    }




    void fillco(int x, int z, int g)
    {
        return;
        colors[convertxy(x, z)] = fillgra.Evaluate(Mathf.InverseLerp(0f,10f,(float)g));
        

        if (g == 0)
            return;

        if ((x == 0) || (x == xSize - 1))
            return;
        if ((z == 0) || (z == zSize - 1))
            return;

        try
        {
            if (colors[convertxy(x, z) + 1] == even)
                fillco(z, x + 1, g - 1);

            if (colors[convertxy(x, z) - 1] == even)
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


    int convertxy(int x, int z) //converty xy coordinates to verticy position in mesh 
    {
        Debug.Log("converted x:" + x + " z:" + z + " to:" + (z * (zSize + 1)) + x + "with xSize:" + xSize + " zSize:" + zSize);
        return (z * (zSize + 1)) + x;

    }


}
