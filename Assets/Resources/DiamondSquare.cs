using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare : MonoBehaviour {

    HeightMap heightmap;
    public int resolution = 16;
    public float trunc = 1.0f;

    class HeightMap
    {
        int resolution;
        float minTruncation;
        float maxTruncation;
        float roughness;

        float[,] heightmap;

        public HeightMap (int resolution, float minTrunc, float maxTrunc, float roughness)
        {
            if (!isPowerOfTwo(resolution))
                throw new System.ArgumentException("Heightmap not initialized in power of two");
            this.resolution = resolution + 1;
            this.minTruncation = minTruncation;
            this.maxTruncation = maxTruncation;

            this.roughness = roughness;
            this.heightmap = new float[resolution, resolution];
        }

        public void DiamondSquare()
        {
            Init();
            int iteration = resolution / 2;
            int level = 1;
            while(iteration != 0)
            {
                for(int y = 0; y < level; y++)
                {
                    for(int x = 0; x < level; x++)
                    {
                        int cell_corner_x = iteration * x;
                        int cell_corner_y = iteration * y;

                        SquareStep(cell_corner_x, cell_corner_y, iteration, roughness);
                        SquareStep(cell_corner_x, cell_corner_y, iteration, roughness);
                    }
                }
                iteration /= 2;
                roughness /= 2.0f;
            }
        }

        private void Init()
        {
            //initialize corners with random starting values
            heightmap[0, 0] = random(minTruncation, maxTruncation);
            heightmap[0, resolution - 1] = random(minTruncation, maxTruncation);
            heightmap[resolution - 1, 0] = random(minTruncation, maxTruncation);
            heightmap[resolution - 1, resolution - 1] = random(minTruncation, maxTruncation);
        }

        private void SquareStep(int x, int y, int iteration, float r)
        {
            float midvalue = (heightmap[x, y] + heightmap[x + iteration*2, y] + heightmap[x, y + iteration*2] + heightmap[x + iteration*2, y + iteration*2]) + random(-r,r) / 4.0;
            heightmap[x + iteration, y + iteration] = midvalue;
        }

        private void DiamondStep (int x, int y, int iteration, float r)
        {
            //up
            float midvalue = (heightmap[x, y] + heightmap[x + iteration * 2, y] + heightmap[x + iteration, y + iteration]) + random(-r, r) / 3.0;
            heightmap[x + iteration, y] = midvalue;
            //down
            midvalue = (heightmap[x + iteration * 2, y] + heightmap[x + iteration * 2, y + iteration * 2] + heightmap[x + iteration, y + iteration]) + random(-r, r) / 3.0;
            heightmap[x + iteration, y + iteration * 2] = midvalue;
            //left
            midvalue = (heightmap[x, y] + heightmap[x, y + iteration * 2] + heightmap[x + iteration, y + iteration]) + random(-r, r) / 3.0;
            heightmap[x, y + iteration] = midvalue;
            //right
            midvalue = (heightmap[x , y + iteration * 2] + heightmap[x + iteration * 2, y + iteration * 2] + heightmap[x + iteration, y + iteration]) + random(-r, r) / 3.0;
            heightmap[x + iteration * 2, y + iteration] = midvalue;
        }

    }


	// Use this for initialization
	void Start () {
        heightmap = new HeightMap(resolution, -trunc, maxTrunc);
        heightmap.DiamondSquare();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool isPowerOfTwo(int x)
    {
        // First x in the below expression 
        // is for the case when x is 0 
        return x != 0 && ((x & (x - 1)) == 0);
    }

}
