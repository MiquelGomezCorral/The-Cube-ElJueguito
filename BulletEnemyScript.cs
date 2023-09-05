using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemyScript : MonoBehaviour
{
    public AudioClip ShootSound, HitSound;
    public float ShootVolumen = 1f, HitVolumen = 1f;
    Rigidbody2D Rigidbody2D;
    public float Speed = 1.0f, Damage = 40.0f, destroyDelay = 1f;
    public Vector2 Direction { get; set; }

    public ParticleSystem HitParticles;

    void Start()
    {

        if(ShootSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(ShootSound, ShootVolumen * VolumenManagerScript.GameVolumen);
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Destroy(gameObject, destroyDelay);
        //if (Direction.Equals(Vector3.right)) transform.localScale = new Vector2(1.0f, 1.0f);
        //else transform.localScale = new Vector2(-1.0f, 1.0f);
    }

    public void DestroyBullet()
    {
        Instantiate(HitParticles, transform.position, Quaternion.identity); //Particulas
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Rigidbody2D.velocity = Direction * Speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(HitSound, HitVolumen * VolumenManagerScript.GameVolumen);

        MainCubeScript Cube = collision.collider.GetComponent<MainCubeScript>();
        if (Cube != null)
        {
            Cube.Hit(Damage, collision.GetContact(0).point);
        }
        TriangeScript otherBullet = collision.collider.GetComponent<TriangeScript>();
        if (otherBullet != null)
        {
            
        }
        DestroyBullet();
    }
}
