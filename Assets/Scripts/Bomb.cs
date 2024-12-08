using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bombObject;
    public Transform _cannonBarrel;
    public float speed = 20f;
    void Start()
    {
        
    }

    // Update is called once per frame
   public void Test()
    {
        var bullet = Instantiate(bombObject, _cannonBarrel.position, Quaternion.identity)
                            .GetComponent<Rigidbody>();
        bullet.AddForce(_cannonBarrel.forward * speed, ForceMode.Impulse);

        Destroy(bullet.gameObject, 6f);
    }
}
