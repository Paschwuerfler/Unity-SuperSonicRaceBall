﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class TestGrid : MonoBehaviour
{
    Mesh mesh;

    public int xSize = 20;
    public int zSize = 20;

//    public float[] octaveFrequencies = { 10,9,8,7,6,5,4,3,2,1 };
//    public float[] octaveAmplitudes = { 1,2,3,4,5,6,7,8,9,10 };

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

    public int ccalls = 0;
    public int fcalls = 0;
    public int cfcalls = 0;
    public int endedfcalls = 0;

    public int iterations;
    public float delay;


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

        UpdateMesh();
    }



    void Start()
    {
        mesh = new Mesh();
        Debug.Log("Created Mesh");
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();

        StartCoroutine(fillco(fillstartx,fillstartz,iterations));
        //StartCoroutine(fillnum());
        Debug.LogError("Coroutine was called");


        UpdateMesh();
        Debug.Log("Mesh was Updated");
    }


    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];


        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = 0;
                /*
                for (int o = 0; o < octaveFrequencies.Length; o++)
                    y += octaveAmplitudes[o] * Mathf.PerlinNoise(
                         octaveFrequencies[o] * (x + seed) + .3f,
                         octaveFrequencies[o] * (z + seed) + .3f) * 2f;

                */
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


                    float height = Mathf.InverseLerp(0, zSize * xSize, (float)(convertxy(x, z)));
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


    private void Update()
    {

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        fillstartx += (int)moveHorizontal;
        fillstartz += (int)moveVertical;

        colors[convertxy(fillstartx, fillstartz)] = fillstart;




        UpdateMesh();


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
        //meshCollider = new MeshCollider();
        //meshCollider.sharedMesh = mesh;
        //Debug.Log("UpdateMesh()");
    }


    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        for (int i = 0; i < vertices.Length; i++)
        {
            
            Gizmos.color = colors[i];
            Gizmos.DrawSphere(vertices[i], .3f);
            Debug.Log("Draw Sphere at" + vertices[i] + "with color " + colors[i]);
        }
    }




    IEnumerator fillco(int x, int z, int g)
    {
        fcalls++;
        int conxy = convertxy(x, z);
        if (colors[conxy] != even)
        {
            cfcalls++;
            // Debug.LogError("coroutine ended");
            yield return null;
        }
        else
        {

            colors[conxy] = fillstart;
            //Debug.LogError(convertxy(x, z) + "was colored! x: " + x + " z: " + z);

            if (g == 0)
            {

                // Debug.LogError("coroutine ended");
                yield return null;
            }
            else

            if ((x == 0) || (x == xSize - 1))
            {
                //Debug.LogError("coroutine ended");
                yield return null;
            }

            else
            if ((z == 0) || (z == zSize - 1))
            {
                //Debug.LogError("coroutine ended");
                yield return null;
            }
            else
            {



                if (colors[conxy + 1] == even)
                {
                    yield return new WaitForSeconds(delay);
                    colors[conxy] = fillgra.Evaluate(Mathf.InverseLerp(0f, (float)iterations, (float)g));
                    StartCoroutine(fillco(z, x + 1, g - 1));
                }


                if (colors[conxy - 1] == even)
                {
                    yield return new WaitForSeconds(delay);
                    colors[conxy] = fillgra.Evaluate(Mathf.InverseLerp(0f, (float)iterations, (float)g));
                    StartCoroutine(fillco(z, x - 1, g - 1));
                }

                if (colors[convertxy(x, z + 1)] == even)
                {
                    yield return new WaitForSeconds(delay);
                    colors[conxy] = fillgra.Evaluate(Mathf.InverseLerp(0f, (float)iterations, (float)g));
                    StartCoroutine(fillco(z + 1, x, g - 1));
                }

                if (colors[convertxy(x, z - 1)] == even)
                {
                    yield return new WaitForSeconds(delay);
                    colors[conxy] = fillgra.Evaluate(Mathf.InverseLerp(0f, (float)iterations, (float)g));
                    StartCoroutine(fillco(z - 1, x, g - 1));
                }

            }
        }
        endedfcalls++;
    }




        IEnumerator fillnum()
        {

            for (int z = fillstartx; z < zSize; z++)
            {
                for (int x = fillstartz; x < xSize; x++)
                {



                    colors[convertxy(x, z)] = fillstart;
                    
                    yield return new WaitForSeconds(1f);


                }
            }






        }


    int convertxy(int x , int z)
    {
        //Debug.Log("converted x:" + x + " z:" + z + " to:" + (z * (zSize + 1)) + x + "with xSize:" + xSize + " zSize:" + zSize  );
        ccalls++;
        return (z * (zSize + 1)) + x;
        

    }

    
}
