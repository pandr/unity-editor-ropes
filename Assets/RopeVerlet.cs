using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class RopeVerlet : MonoBehaviour
{
    public int numSegments;
    public Transform endPoint1;
    public Transform endPoint2;
    public float ropeLength;

    Vector3[] segmentPos;
    Vector3[] segmentLastPos;

    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numSegments;
        segmentPos = new Vector3[numSegments];
        segmentLastPos = new Vector3[numSegments];
        var ep1 = endPoint1.transform.position;
        var ep2dir = endPoint2.transform.position - ep1;
        ropeLength = ep2dir.magnitude;
        for (var i = 0; i < numSegments; i++)
        {
            segmentPos[i] = ep1 + ep2dir * (float)i / (numSegments-1);
            segmentLastPos[i] = segmentPos[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Verlet step
        float Dt = 0.02f;
        for(var i = 0; i < numSegments; i++)
        {
            var old = segmentPos[i];
            segmentPos[i] = segmentPos[i] + (segmentPos[i] - segmentLastPos[i]) * 0.98f + 10.0f * Vector3.down * Dt * Dt;
            segmentLastPos[i] = old;
        }

        var segmentLength = ropeLength / (numSegments - 1);
        // Constraints
        segmentPos[0] = endPoint1.position;
        segmentPos[numSegments - 1] = endPoint2.position;
        for (var i = 0; i < numSegments-1; i++)
        {
            Vector3 d = segmentPos[i + 1] - segmentPos[i];
            float dl = d.magnitude;
            if (dl < segmentLength)
                continue;
            float dif = (dl - segmentLength) / dl;
            float b = (i == 0) ? 0.0f : (i==numSegments-1)? 1.0f : 0.5f;
            segmentPos[i] += d * b * dif;
            segmentPos[i + 1] -= d * (1.0f - b) * dif;
        }

        lineRenderer.SetPositions(segmentPos);
    }
}
