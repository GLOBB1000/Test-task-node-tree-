using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLineRenderer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    private Transform[] points;

    public void SetUpLine(Transform[] points)
    {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    private void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}
