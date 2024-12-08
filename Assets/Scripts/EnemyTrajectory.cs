using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class EnemyTrajectory : MonoBehaviour
{
    public Transform enemyTarget;
    [SerializeField] private Bomb _bomb;
    private LineRenderer _lineRenderer;
    private Rigidbody _grenadeRB;
    [Header("Display Controls")]
    [SerializeField]
    [Range(10, 100)]
    private int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float TimeBetweenPoints = 0.1f;

    //private LayerMask testLayer=5;
    void Start()
    {
        _grenadeRB =_bomb.bombObject.GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionToTarget = (enemyTarget.position - _bomb._cannonBarrel.position).normalized;

        // Optional: You can adjust the cannon rotation to face the target
        transform.rotation=Quaternion.LookRotation(directionToTarget);
        _bomb._cannonBarrel.rotation = Quaternion.LookRotation(directionToTarget);

        // Step 2: Calculate the trajectory
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;

        Vector3 startPosition = _bomb._cannonBarrel.position;
        Vector3 startVelocity = 20f * directionToTarget / _grenadeRB.mass;

        int i = 0;
        _lineRenderer.SetPosition(i, startPosition);

        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            _lineRenderer.SetPosition(i, point);

            Vector3 lastPosition = _lineRenderer.GetPosition(i - 1);

            // Step 3: Check for collisions (Raycasting)
            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude))
            {
                // If hit, end the trajectory at the hit point
                _lineRenderer.SetPosition(i, hit.point);
                _lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }
}
