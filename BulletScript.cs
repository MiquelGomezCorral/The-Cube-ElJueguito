using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BulletScript : MonoBehaviour
{
    public AudioClip ShootSound, HitSound;
    public float ShootVolumen = 1f, HitVolumen = 1f;
    Rigidbody2D Rigidbody2D;
    public int numberOfBounces = 0;
    public float Speed = 1.0f, Damage = 40.0f, Heal = 25, maxDistance = 10f, MotionForce = 1f;
    public float destroyDelay = 10f;
    public Vector2 Direction { get; set; }
    private Vector2 lastPosition;

    public ParticleSystem HitParticles;
    public ParticleSystem HealParticles;
    void Start()
    {
        //if (ShootSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(ShootSound, ShootVolumen * VolumenManagerScript.GameVolumen);
        Rigidbody2D = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
        Destroy(gameObject, destroyDelay);
    }
    
    public void DestroyBullet() //true green particles, false blueW
    {
        Destroy(gameObject);
    }
    public void particle(Boolean heal)
    {
        if (heal) Instantiate(HealParticles, transform.position, Quaternion.identity); //Verdes
        else Instantiate(HitParticles, transform.position, Quaternion.identity); //Azules claro
    }

    private void FixedUpdate()
    {
        Rigidbody2D.velocity = Direction * Speed;
        Vector2 dif;
        dif.x = Mathf.Abs(lastPosition.x - transform.position.x);
        dif.y = Mathf.Abs(lastPosition.y - transform.position.y);
        maxDistance -= Mathf.Sqrt(dif.x*dif.x + dif.y*dif.y);
        if (maxDistance <= 0) { 
            DestroyBullet();
            particle(false);
        }
        lastPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(HitSound, HitVolumen * VolumenManagerScript.GameVolumen);

        EnemyScript Enemy = collision.collider.GetComponent<EnemyScript>();
        if (Enemy != null)
        {
            Enemy.Hit(Damage, Direction, MotionForce);
            particle(false);
            if (numberOfBounces <= 0) DestroyBullet();
        }
        BulletScript otherBullet = collision.collider.GetComponent<BulletScript>();
        if (otherBullet != null)
        {
            particle(false);
            if (numberOfBounces <= 0) DestroyBullet();
        }
        MainCubeScript Cube = collision.collider.GetComponent<MainCubeScript>();
        if (Cube != null)
        {
            Cube.Heal(Heal);
            particle(true);
            if (numberOfBounces <= 0) DestroyBullet();
        }
        //Direction = ((Vector2)transform.position - collision.GetContact(0).point).normalized;
        if(numberOfBounces > 0)
        {
            Direction = Vector2.Reflect(Direction, collision.contacts[0].normal);
            numberOfBounces--;
        }
        else
        {
            DestroyBullet();
        }
        
    }

    public void setValues(Vector2 direction, float damage, float heal, float motionForce, float speed, float distance, int numberOfBounces, float HitVolumen)
    {
        this.Direction = direction;
        this.Damage = damage;
        this.Heal = heal;
        this.MotionForce = motionForce;
        this.Speed = speed;
        this.maxDistance = distance;
        this.numberOfBounces = numberOfBounces;
        this.HitVolumen = HitVolumen;
    }
}
