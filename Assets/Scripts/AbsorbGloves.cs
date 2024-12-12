using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbsorbGloves : MonoBehaviour
{

    public Portal _portal;
    [SerializeField] private float GravityRadius = 2f;
    [SerializeField] private float GravityForce = 1f;

    [SerializeField] private GameObject Portal;
    //[SerializeField] private Transform PortalPosition;
    



    public InputActionReference ActivateAction;


    private bool isAbsorbing = false;
    private void Awake()
    {
        ActivateAction.action.performed += ActivateSpatialMove;
        ActivateAction.action.canceled += DeactivateSpatialMove;

        ActivateAction.action.Enable();

    }
    private void ActivateSpatialMove(InputAction.CallbackContext ctx)
    {

        Portal.SetActive(true);
        Debug.Log("Absorbing");
        isAbsorbing = true;


    }
    private void DeactivateSpatialMove(InputAction.CallbackContext ctx)
    {
        Portal.SetActive(false);
        isAbsorbing = false;



    }
    private void Update()
    {
        if (isAbsorbing)
        {
            AbsorbAbility();
        }


    }
    public void AbsorbAbility()
    {
        Collider[] colliders = Physics.OverlapSphere(Portal.transform.position, GravityRadius);
        foreach (var obj in colliders)
        {
            Rigidbody objRB = obj.GetComponent<Rigidbody>();
            if (!objRB && obj.gameObject.layer != LayerMask.NameToLayer("Spatial")) continue;
            objRB.useGravity = false;
            objRB.AddForce((Portal.transform.position - obj.transform.position) * GravityForce, ForceMode.Acceleration);
            //objRB.velocity = Vector3.zero;

        }
    }

}
