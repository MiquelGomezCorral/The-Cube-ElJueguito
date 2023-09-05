using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    private float lastShoot, hitTime, lastParicles, distanceToTarget, lastEnemySpawn = 0;
    public float Damage = 40, Health = 100.0f, Speed = 1.0f, ShootingSpeed = 1f, vieweDistance = 4.0f, RotateSpeed = 1, extraEnemySpawnDelay = 1;
    public float DamageType, HealthType, SpeedType, ShootingSpeedType; //Damage, Health, Speed, Shooting Speed.
    public int Cash = 10, extraEnemySpawnCuantity = 0;
    public bool canMove = true, canShoot = true, canBeMove = true, isBoss;

    Vector2 DireccionEmpuje;
    float fuerzaEmpuje;

    public Transform target;
    MainCubeScript cubeScript = null;
    public GameObject Bullet;
    public GameObject[] ExtraEnemyToSpawn;

    public ParticleSystem DeathParticles;
    public ParticleSystem WalkParticles;
    public ParticleSystem SummonParticles;

    public AudioClip SpawnSound, DeadSound;
    public float SpawnVolumen = 1f, DeadVolumen = 1f;

    Rigidbody2D Rigidbody2D;
    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        
        Damage = (float)(DamageType * (20 + Mathf.Pow(WaveSpawnerScript.WaveNumber, 1.2f) * 1));
        Health = (float)(HealthType * (100 + Mathf.Pow(WaveSpawnerScript.WaveNumber, 1.2f) * 2));
        Speed = (float)(SpeedType *(200 + Mathf.Pow(WaveSpawnerScript.WaveNumber / Mathf.Sqrt(SpeedType), 1.2f) * 0.5));
        ShootingSpeed = 2 / ShootingSpeedType;

        if (isBoss)
        {
            cubeScript = GameObject.FindGameObjectWithTag("Central Cube").GetComponent<MainCubeScript>();
            cubeScript.showBossSlider(this.Health);
            Camera.main.GetComponent<CameraShakeScript>().startShake(1f, 25f);
        }

        if(SpawnSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(SpawnSound, SpawnVolumen * VolumenManagerScript.GameVolumen);
        RotateTowardsTarget();
    }
    // Update is called once per frame
    void Update()
    {
        if (!MainCubeScript.PlayRunning) return;
        if (WalkParticles != null && lastParicles + 0.1 <= Time.time)
        {
            lastParicles = Time.time;
            Instantiate(WalkParticles, transform.position, Quaternion.identity); //Particulas
        }
        if (ExtraEnemyToSpawn != null && lastEnemySpawn + extraEnemySpawnDelay <= Time.time)
        {
            lastEnemySpawn = Time.time;
            spawnExtraEnemy();
        }
        //GetTarget Y ROTAR
        RotateTowardsTarget();

        //DISPARAR--------------------------------------------------------------------------
        float xpow = (target.position.x - transform.position.x) * (target.position.x - transform.position.x);
        float ypow = (target.position.y - transform.position.y) * (target.position.y - transform.position.y);

        distanceToTarget = Mathf.Sqrt(xpow + ypow);
        if (canShoot && distanceToTarget < vieweDistance && Time.time >= lastShoot + ShootingSpeed)
        {
            Shoot();
            lastShoot = Time.time;
        }
    }
    private void FixedUpdate()
    {
        if (!MainCubeScript.PlayRunning && !MainCubeScript.EnemysCanMove)
        {
            Rigidbody2D.velocity = new Vector2(0, 0);
            return;
        }
        if (Time.time <= hitTime + 0.15)
        { //Get KockBack for 0.2 secs
            Rigidbody2D.velocity = DireccionEmpuje * fuerzaEmpuje;
        }
        else 
        { //Mover normal
            if (canShoot && distanceToTarget < vieweDistance)
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
            }
            else Rigidbody2D.velocity = transform.up * Speed;
            
        }
 
    }

    private void RotateTowardsTarget()
    {
        if (!target && GameObject.FindGameObjectWithTag("Central Cube")) //SI NO ESTA DEFINIDO TARGET Y EXISTE UN CENTRAL CUBE
        {
            target = GameObject.FindGameObjectWithTag("Central Cube").transform; //TE QUEDAS CON SU POSICIÓN
        }
        Vector2 targetDirection = target.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, RotateSpeed);
    }
    private void Shoot()
    {
        GameObject bullet = (GameObject)Instantiate(Bullet, transform.position + transform.up * 55f, Quaternion.identity);
        bullet.GetComponent<BulletEnemyScript>().Direction = transform.up;
        bullet.GetComponent<BulletEnemyScript>().Damage = Damage;
    }
    public void Hit(float damage, Vector2 motionDirection, float motionForce = 10)
    {
        hitTime = Time.time;
        DireccionEmpuje = motionDirection; fuerzaEmpuje = motionForce/HealthType;
        Health -= damage;
        if (isBoss && cubeScript != null) cubeScript.setValueBossSlider(Health);
        
        if (Health <= 0)
        {// Matar enemigo
            int enemies = (int)Mathf.Min(30, Mathf.Log(WaveSpawnerScript.WaveNumber, 5) + 2);
            if(isBoss) for (int i = 0; i < enemies; i++) spawnExtraEnemy();
            DestroyEnemy();
        }
    }
    public void DestroyEnemy()
    {
        if (DeadSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(DeadSound, DeadVolumen * VolumenManagerScript.GameVolumen);
        if (isBoss)
        {
            cubeScript.hideBossSlider();
            Camera.main.GetComponent<CameraShakeScript>().startShake(3f, 50f);
        }
        Instantiate(DeathParticles, transform.position, Quaternion.identity); //Particulas
        int waveMultiplier = (int)(WaveSpawnerScript.WaveNumber / 3) * 5 + 1; //Cada 3 rondas aumenta * 5
        LevelManagerScript.instance.addMoney(Cash * waveMultiplier);
        Destroy(gameObject);
    }
    public void spawnExtraEnemy()
    {
        for(int i = 0; i < extraEnemySpawnCuantity; i++)
        {
            float d = 100; //Distancia al enemy

            float alf = UnityEngine.Random.Range(0f, 360f); //Angulo en el que aparece

            float x = d * Mathf.Cos(alf);
            float y = d * Mathf.Sin(alf);

            Vector3 offset = new Vector3(x, y, 0);

            int e = UnityEngine.Random.Range(0, ExtraEnemyToSpawn.Length);
            Instantiate(ExtraEnemyToSpawn[e], transform.position + offset, Quaternion.identity);
            if(SummonParticles != null) Instantiate(SummonParticles, transform.position + offset, Quaternion.identity); //Particulas
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        MainCubeScript Cube = collision.collider.GetComponent<MainCubeScript>();
        if (Cube != null)
        {
            canMove = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!MainCubeScript.PlayRunning) return;
        MainCubeScript Cube = collision.collider.GetComponent<MainCubeScript>();
        if (Cube != null && Time.time >= lastShoot + ShootingSpeed)
        {
            //Instantiate(DeathParticles,collision.GetContact(0).point, Quaternion.identity); //Particulas
            lastShoot = Time.time;
            Cube.Hit(Damage, collision.GetContact(0).point); 
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!MainCubeScript.PlayRunning) return;
        MainCubeScript Cube = collision.collider.GetComponent<MainCubeScript>();
        if (Cube != null && Time.time >= lastShoot + ShootingSpeed)
        {
            //Instantiate(DeathParticles, collision.GetContact(0).point, Quaternion.identity);
            lastShoot = Time.time;
            Cube.Hit(Damage, collision.GetContact(0).point);
        }
        TriangeScript Triangle = collision.collider.GetComponent<TriangeScript>();
        if (Triangle != null)
        {
            //Instantiate(DeathParticles, collision.GetContact(0).point, Quaternion.identity);
        }
    }
}
