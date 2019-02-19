using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare : MonoBehaviour {

    HeightMap heightmap;
    public int resolution = 256;
    public float trunc = 1.0f;
    
    // Use this for initialization
    void Start () {
        heightmap = new HeightMap(resolution, trunc, 0.4f);
        heightmap.DiamondSquare();
        Export("E:\\projects\\Shader Lab\\Assets\\Resources\\texture\\");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Export(string targetDirectory)
    {
        Texture2D texture = new Texture2D(resolution, resolution);
        for (int y = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++)
            {
                float greyValue = heightmap.Get(x, y);
                if(greyValue == 0)
                    texture.SetPixel(x, y, new Color(0.9f, 0.0f, 0.2f));
                else
                texture.SetPixel(x, y, new Color(greyValue, greyValue, greyValue));
                Debug.Log("Write Pixel : " + greyValue);
            }
        }
        texture.Apply();

        var path =  System.IO.Path.Combine(targetDirectory, "Heightmap.png");

        byte[] file = texture.EncodeToPNG();
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
        System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.CreateNew);
        fs.Write(file, 0, file.Length);
        fs.Close();
    }

}

class HeightMap
{
    int resolution;
    float minTruncation;
    float maxTruncation;
    float roughness;

    float[,] heightmap;
    System.Random rand;

    public HeightMap(int resolution, float maxTrunc, float roughness)
    {
        if (!isPowerOfTwo(resolution))
            throw new System.ArgumentException("Heightmap not initialized in power of two");
        this.resolution = resolution + 1;
        this.maxTruncation = maxTrunc;

        this.roughness = roughness;
        this.heightmap = new float[this.resolution, this.resolution];
        rand = new System.Random();
    }

    public float Get(int x, int y) { return heightmap[x, y]; }
    public float GetSaturated(int x, int y) { return Mathf.Clamp( heightmap[x, y], 0.0f, 1.0f); }

    public void DiamondSquare()
    {
        Init();
        int iteration = resolution / 2;
        int level = 1;
        while (iteration != 0)
        {
            for (int y = 0; y < level; y++)
            {
                for (int x = 0; x < level; x++)
                {
                    int cell_corner_x = iteration * x;
                    int cell_corner_y = iteration * y;

                    DiamondStep(cell_corner_x, cell_corner_y, iteration, roughness);
                    SquareStep(cell_corner_x, cell_corner_y, iteration, roughness);
                }
            }
            iteration /= 2;
            roughness /= 2.0f;
            level += 1;
        }
    }

    private void Init()
    {
        //initialize corners with random starting values
        heightmap[0, 0] = Random(maxTruncation);
        heightmap[0, resolution - 1] = Random(maxTruncation);
        heightmap[resolution - 1, 0] = Random(maxTruncation);
        heightmap[resolution - 1, resolution - 1] = Random(maxTruncation);

        Debug.Log("Cornerpoints : " + heightmap[0, 0] + " " + heightmap[0, resolution - 1] + " " + heightmap[resolution - 1, 0] + " " + heightmap[resolution - 1, resolution - 1]);
    }

    private void DiamondStep(int x, int y, int iteration, float r)
    {
        float midvalue = (heightmap[x, y] + heightmap[x + iteration * 2, y] + heightmap[x, y + iteration * 2] + heightmap[x + iteration * 2, y + iteration * 2]) / 4.0f;
        heightmap[x + iteration, y + iteration] = midvalue;
        Debug.Log("Midpoint: " + midvalue);
    }
    
   

    private void SquareStep(int x, int y, int iteration, float r)
    {
        //up
        float midvalue = (heightmap[x, y] + heightmap[x + iteration * 2, y] + heightmap[x + iteration, y + iteration]) / 3.0f ;
        heightmap[x + iteration, y] = midvalue;
        //down
        midvalue = (heightmap[x + iteration * 2, y] + heightmap[x + iteration * 2, y + iteration * 2] + heightmap[x + iteration, y + iteration]) / 3.0f ;
        heightmap[x + iteration, y + iteration * 2] = midvalue;
        //left
        midvalue = (heightmap[x, y] + heightmap[x, y + iteration * 2] + heightmap[x + iteration, y + iteration])  / 3.0f ;
        heightmap[x, y + iteration] = midvalue;
        //right
        midvalue = (heightmap[x, y + iteration * 2] + heightmap[x + iteration * 2, y + iteration * 2] + heightmap[x + iteration, y + iteration]) / 3.0f ;
        heightmap[x + iteration * 2, y + iteration] = midvalue;
    }

    private float Random(float scale)
    {
        return (float) (rand.NextDouble()) * scale;
    }

    private bool isPowerOfTwo(int x)
    {
        // First x in the below expression 
        // is for the case when x is 0 
        return x != 0 && ((x & (x - 1)) == 0);
    }

}
