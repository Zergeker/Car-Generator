using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapCreator : MonoBehaviour
{
    void Start()
    {
        Vector3 size = gameObject.transform.localScale;

        for (float i = 1; i <= size.x; i++)
            for (float j = 1; j <= size.y; j++)
            {
                createCollider(i, j, 1f, size);
                createCollider(i, j, size.z, size);
            }

        
          for (float i = 1; i <= size.x; i++)
            for (float j = 2; j <= (size.z-1); j++)
            {
                createCollider(i, 1f, j, size);
                createCollider(i, size.y, j, size);
            }
          
         for (float i = 2; i <= (size.y-1); i++)
            for (float j = 2; j <= (size.z-1); j++)
            {
                createCollider(1f, i, j, size);
                createCollider(size.x, i, j, size);
            }
    }

    void createCollider(float x, float y, float z, Vector3 size)
    {
        BoxCollider cube = gameObject.AddComponent<BoxCollider>();
        cube.center = new Vector3(CenterCoord(x, size.x), CenterCoord(y, size.y), CenterCoord(z, size.z));
        cube.size = CubeSize(size);
    }

    float CenterCoord(float a, float max)
    {
        return ((2 * a) - 1) / (2 * max) - 0.5f;
    }

    Vector3 CubeSize(Vector3 size)
    {
        return new Vector3(1 / size.x, 1 / size.y, 1 / size.z);
    }

 }