using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestsPerlin : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Vector3[] posArr = new Vector3[100];
        for (int i = 0; i < posArr.Length; i++)
        {
            posArr[i] = new Vector3(i * 0.1f, Mathf.PerlinNoise(i * 0.1f, 0), 0);
        }
        _lineRenderer.positionCount = posArr.Length;
        _lineRenderer.SetPositions(posArr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
