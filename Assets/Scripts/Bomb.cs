using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;
    [HideInInspector] public Vector3 velocity;
    private AudioSource audioSource;
    [SerializeField] private AudioClip _boomClip;
    [SerializeField] private AudioClip _PlayerHitClip;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        velocity = rb.velocity;
    }
    private void Update()
    {
      
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer==LayerMask.NameToLayer("Cannon"))
        {
            audioSource.clip = _boomClip;
            audioSource.Play();
        }
        if(collision.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            audioSource.clip = _boomClip;
            audioSource.spread.CompareTo(velocity);
            audioSource.Play();
            Destroy(gameObject,0.1f);
        }
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            audioSource.clip = _PlayerHitClip;
            audioSource.Play();
        }
    }
}
