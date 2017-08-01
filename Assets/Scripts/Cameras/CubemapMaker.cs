using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubemapMaker : MonoBehaviour
{
    Camera cam;
    public Cubemap cubemap;
    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.RenderToCubemap(cubemap);
    }
}