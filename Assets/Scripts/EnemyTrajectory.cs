using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class EnemyTrajectory : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField]
    private Transform ReleasePosition;
    private Rigidbody _grenadeRB;
    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    private int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float TimeBetweenPoints = 0.1f;

    private LayerMask testLayer=5;
    void Start()
    {
        _grenadeRB =GetComponentInChildren<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;
        Vector3 startPosition = ReleasePosition.position;
        Vector3 startVelocity = 20f * ReleasePosition.forward / _grenadeRB.mass;
        int i = 0;
        _lineRenderer.SetPosition(i, startPosition);
        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            _lineRenderer.SetPosition(i, point);

            Vector3 lastPosition = _lineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude),testLayer)
            {
                _lineRenderer.SetPosition(i, hit.point);
                _lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }
}
