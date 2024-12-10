using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpatialGloves : MonoBehaviour
{
    public enum SpatialMove
    {
        Absorb = 0,
        Counter = 1
    };

    public SpatialMove SpatialAbility = SpatialMove.Absorb;

    public List<GameObject> TeleportedObjects;
    [SerializeField] private float GravityRadius = 2f;
    [SerializeField] private float GravityForce = 1f;

    [SerializeField] private GameObject Portal;
    //[SerializeField] private Transform PortalPosition;
    [SerializeField] private Transform CounterReleasePosition;

    private AudioSource _source;
    [SerializeField] private AudioClip _absorbSfx;
    [SerializeField] private AudioClip _launchSfx;
    public InputActionReference ActivateAction;
    private GameObject livePortal;

    private LineRenderer lineRenderer;
    private Bomb _bomb;

    private bool isAbsorbing = false;
    private bool isCountering = false;
    private bool showTrajectory = false;

    private float gravity = -9.81f;
    [Range(10, 100)]
    private int LinePoints = 25;
    
    [Range(0.01f, 0.25f)]
    private float TimeBetweenPoints = 0.1f;
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        ActivateAction.action.performed += ActivateSpatialMove;
        ActivateAction.action.canceled += DeactivateSpatialMove;

        ActivateAction.action.Enable();

        if(SpatialAbility==SpatialMove.Counter)
        {
            lineRenderer = GetComponent<LineRenderer>();
 
        }
    }
    private void ActivateSpatialMove(InputAction.CallbackContext ctx)
    {
       
        Portal.SetActive(true);
        switch (SpatialAbility)
        {
            case SpatialMove.Absorb:
                Debug.Log("Absorbing");
                isAbsorbing = true;
                break;
            case SpatialMove.Counter:
                Debug.Log("Pushing");
                showTrajectory = true;
                break;
            default:
                break;
        }
    }
    private void DeactivateSpatialMove(InputAction.CallbackContext ctx)
    {
        Portal.SetActive(false);
        isAbsorbing = false;
        showTrajectory= false;
        CounterAbility();
    
        
    }
    private void Update()
    {
        if (isAbsorbing)
        {
            AbsorbAbility();
        }
        if (showTrajectory)
        {
            RenderTrajectory();
        }
       
    }
    public void AbsorbAbility()
    {
        Collider[] colliders = Physics.OverlapSphere(Portal.transform.position, GravityRadius);
        foreach (var obj in colliders)
        {
            Rigidbody objRB = obj.GetComponent<Rigidbody>();
            if (!objRB && obj.gameObject.layer!=LayerMask.NameToLayer("Spatial")) continue;
            objRB.useGravity = false;
            objRB.velocity = Vector3.zero;
            objRB.AddForce((Portal.transform.position - obj.transform.position) * GravityForce, ForceMode.Acceleration);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("PlayerBomb");
            other.gameObject.SetActive(false);
            _source.clip = _absorbSfx; _source.Play();
            TeleportedObjects.Add(other.gameObject);
        }
    }
    public void CounterAbility()
    {
        if (TeleportedObjects.Count == 0) return;
        GameObject obj = TeleportedObjects[0];
        obj.transform.position = CounterReleasePosition.position;
        obj.SetActive(true);
        obj.GetComponent<Rigidbody>().AddForce(obj.GetComponent<Rigidbody>().velocity, ForceMode.Force);
        _source.clip = _launchSfx;_source.Play();
        TeleportedObjects.Remove(obj);
    }



    private void RenderTrajectory()
    {
        Vector3 initialVelocity = TeleportedObjects[0].GetComponent<Rigidbody>().velocity;
        lineRenderer.enabled = true;
        lineRenderer.positionCount = Mathf.CeilToInt(LinePoints / TimeBetweenPoints) + 1;

        Vector3 startPosition = CounterReleasePosition.position;

        int i = 0;
        lineRenderer.SetPosition(i, startPosition);

        //Simulate the trajectory
        for (float time = 0; time < LinePoints; time += TimeBetweenPoints)
        {
            i++;

            // Calculate the position of the projectile at the given time
            Vector3 point = startPosition + initialVelocity * time;
            point.y = startPosition.y + initialVelocity.y * time + (0.5f * gravity * time * time);

            lineRenderer.SetPosition(i, point);

            // Check for collision using a raycast
            Vector3 lastPosition = lineRenderer.GetPosition(i - 1);
            if (Physics.Raycast(lastPosition, (point - lastPosition).normalized, out RaycastHit hit, (point - lastPosition).magnitude))
            {
                lineRenderer.SetPosition(i, hit.point);
                lineRenderer.positionCount = i + 1;
                return;
            }
        }

    }

}
