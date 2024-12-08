using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(LineRenderer))]
public class Cannon : MonoBehaviour
{
    [Header("Cannon Settings")]
    [SerializeField] private GameObject enemyTarget;
    [SerializeField] private Transform _cannonBarrel;

    [SerializeField] private GameObject _Bomb;

    [SerializeField]
    [Range(1, 100)]
    private int speed=10;
    private LineRenderer _lineRenderer;
   
    [Header("Display Controls")]
    [SerializeField]
    [Range(0, 180)]
    private float launchAngle = 45f;
    [SerializeField]
    [Range(10, 100)]
    private int LinePoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float TimeBetweenPoints = 0.1f;

    private bool isPositioning = false;
    private bool isFiring = false;

    private Rigidbody _grenadeRB;
    void Start()
    {
        _grenadeRB = _Bomb.GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Fire()
    {
        var bullet = Instantiate(_Bomb, _cannonBarrel.position, Quaternion.identity)
                            .GetComponent<Rigidbody>();
        bullet.AddForce(_cannonBarrel.forward * speed, ForceMode.Impulse);

        Destroy(bullet.gameObject, 6f);
    }
    void Update()
    {
        Vector3 directionToTarget = (enemyTarget.transform.position - _cannonBarrel.position).normalized;


        transform.rotation = Quaternion.LookRotation(directionToTarget);
       

        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;

        Vector3 startPosition = _cannonBarrel.position;
        Vector3 startVelocity = speed * directionToTarget / _grenadeRB.mass;

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
                (point - lastPosition).magnitude))
            {
               
                _lineRenderer.SetPosition(i, hit.point);
                _lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }
    
}
