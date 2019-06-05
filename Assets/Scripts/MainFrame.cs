using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFrame : MonoBehaviour
{
    public Vector3 frameSize;
    void Awake()
    {
        float maxX = 0f;
        float maxZ = 0f;
        foreach (Frame frame in this.GetComponentsInChildren<Frame>())
        {
            if (frame.gameObject.transform.position.x > maxX)
                maxX = frame.gameObject.transform.position.x + frame.gameObject.transform.localScale.x/2;
            if (frame.gameObject.transform.position.z > maxZ)
                maxZ = frame.gameObject.transform.position.z + frame.gameObject.transform.localScale.z/2;
        }
        frameSize = new Vector3 (maxX, 0f, maxZ);
        Debug.Log(frameSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
