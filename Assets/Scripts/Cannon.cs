using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(LineRenderer))]
public class Cannon : MonoBehaviour
{
    public Animator animator;
    [Header("Cannon Settings")]
    [SerializeField] private GameObject enemyTarget;
    [SerializeField] private Transform _cannonBarrel;
    [SerializeField] private Transform _cannonBarrelBone;
    [SerializeField] private GameObject _Bomb;

    [SerializeField]
    [Range(0, 180)]
    private float firingAngle = 45f;
    [SerializeField]
    [Range(1, 100)]
    private int speed=10;
    private float gravity = -9.81f;
    private LineRenderer _lineRenderer;
   
    [Header("Display Controls")]
    
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
        Vector3 velocity = CalculateLaunchVelocity();
        var bomb = Instantiate(_Bomb, _cannonBarrel.position, Quaternion.identity)
                                .GetComponent<Rigidbody>();
        bomb.velocity = velocity;
        //Reference of Velocity
        bomb.GetComponent<Bomb>().velocity = velocity;
        Destroy(bomb.gameObject, 6f);
    }

    void Update()
    {

        Vector3 directionToTarget = (enemyTarget.transform.position - _cannonBarrel.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToTarget);
        //_cannonBarrelBone.rotation = Quaternion.Euler(firingAngle-transform.rotation.x,transform.rotation.y, transform.rotation.z);
        Vector3 velocity = CalculateLaunchVelocity();
        RenderTrajectory(velocity);
    }

    // Function to calculate the velocity needed to hit the target
    private Vector3 CalculateLaunchVelocity()
    {
        Vector3 directionToTarget = enemyTarget.transform.position - _cannonBarrel.position;

        float distanceToTarget = directionToTarget.magnitude;
        float targetHeightDifference = enemyTarget.transform.position.y - _cannonBarrel.position.y;

        // Convert the firing angle to radians
        float angleInRadians = firingAngle * Mathf.Deg2Rad;

        // Calculate the initial velocity using projectile motion formula
        float velocitySquared = (distanceToTarget * -gravity) / (Mathf.Sin(2 * angleInRadians));
        float initialVelocity = Mathf.Sqrt(Mathf.Abs(velocitySquared));

        // Calculate the velocity components in x and y directions
        Vector3 velocity = directionToTarget.normalized * initialVelocity * Mathf.Cos(angleInRadians);
        velocity.y = initialVelocity * Mathf.Sin(angleInRadians);

        return velocity;
    }

  
    private void RenderTrajectory(Vector3 initialVelocity)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;

        Vector3 startPosition = _cannonBarrel.position;

        int i = 0;
        _lineRenderer.SetPosition(i, startPosition);

        //Simulate the trajectory
        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;

            // Calculate the position of the projectile at the given time
            Vector3 point = startPosition + initialVelocity * time;
            point.y = startPosition.y + initialVelocity.y * time + (0.5f * gravity * time * time);

            _lineRenderer.SetPosition(i, point);

            // Check for collision using a raycast
            Vector3 lastPosition = _lineRenderer.GetPosition(i - 1);
            if (Physics.Raycast(lastPosition, (point - lastPosition).normalized, out RaycastHit hit, (point - lastPosition).magnitude))
            {
                _lineRenderer.SetPosition(i, hit.point);
                _lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }

    }
