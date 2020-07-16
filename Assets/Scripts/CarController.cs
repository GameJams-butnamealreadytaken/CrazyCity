using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float fSpeed = 2.0f;
    private Vector3 vDir;
    private Vector3 vTargetLocation;

    public List<AudioClip> aHornSound;
    private AudioSource Audio;

    private Rigidbody2D rbody;

    private int explosionCounter = 0;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        Audio = GetComponent<AudioSource>();
    }

    public void Initialize(float InfSpeed, Vector3 InvDir)
    {
        fSpeed = InfSpeed;
        vDir = InvDir;

        vTargetLocation = transform.position + vDir * 600.0f;
    }

    void FixedUpdate()
    {
        rbody.AddForce(vDir * fSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Car"))
        {
            if (Random.Range(0, 100) > 50)
            {
                Audio.clip = aHornSound[Random.Range(0, aHornSound.Count)];
                Audio.Play();
            }

            // TODO detect when car is stuck instead
            if (++explosionCounter > 15 && Random.Range(0, 100) > 50)
                Destroy(collision.collider.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CarDestructor"))
        {
            Destroy(gameObject);
        }
    }
}
