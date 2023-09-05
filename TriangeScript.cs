using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TriangeScript : MonoBehaviour
{
    private float Horizontal, lastHorizontal, Vertical, lastVertical, lastDash, lastShoot, lastThorms;
    private bool SpaceDash, RigthClickDash, initialize, resetThorms = false;
    Camera cam;
    public Object Bullet;
    [SerializeField] public Slider DashSlider;
    public float Speed = 1.0f, AtackSpeed = 1f, Damage = 40, Thorms = 20, BulletRegen = 25, BulletSpeed = 1, BulletDistance = 1f;
    public float DashDelay = 5, DashMult = 5f, DashMultMovement, ThormsDelay = 0.5f, KnockBack = 1, RotationSpeed = 0.1f;
    public int numberOfBounces = 0, numberOfBullets = 1;
    Rigidbody2D Rigidbody2D;
    Vector3 Direction;

    public ParticleSystem ThormsParticles;

    public AudioClip DashSound, ThormsSound, ThormsDashSound, ShootSound;
    public float DashVolumen = 1f, ThormsVolumen = 1f, ThormsDashVolumen = 1f, ShootVolumen = 1f;

    public List<EnemyScript> enemyHitByThormsList;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        SpaceDash = false; initialize = true;
        lastDash = Time.time;
        lastShoot = Time.time;
        lastThorms = Time.time;
        
    }

    private void Initialize()
    {
        initialize = false;
        //Damage = LevelManagerScript.instance.Data.DamageLVL * 1 + 20; //Base 20 
        Damage = Mathf.Pow(LevelManagerScript.instance.Data.DamageLVL, 1.2f) * 1 + 20;
        AtackSpeed = (float)(LevelManagerScript.instance.Data.AtackSpeedLVL * 0.05 + 1); //1 por segundo
        //Thorms = (float)(LevelManagerScript.instance.Data.ThormsLVL * 2 + 40); //Base 40
        Thorms = Mathf.Pow(LevelManagerScript.instance.Data.ThormsLVL, 1.2f) * 2 + 40;
        KnockBack = LevelManagerScript.instance.Data.KnockBackLVL * 7.20f + 100; //Base 100 max 3700

        Speed = LevelManagerScript.instance.Data.MoveSpeedLVL * 8 + 200; //Base 100
        DashMult = (float)(LevelManagerScript.instance.Data.DashMultLVL * 0.1 + 2); //Base 2
        DashMultMovement = DashMult * 4 + 6;
        DashDelay = (float)(LevelManagerScript.instance.Data.DashRecoverLVL * -0.019 + 5); //Base 5 --> min 0.25
        BulletSpeed = LevelManagerScript.instance.Data.BulletSpeedLVL * 40 + 500; //Base 500

        //BulletRegen = LevelManagerScript.instance.Data.BulletRegenLVL * 1 + 10; //Base 10
        BulletRegen = Mathf.Pow(LevelManagerScript.instance.Data.BulletRegenLVL, 1.2f) * 1 + 10; //Base 10
        BulletDistance = (float)(LevelManagerScript.instance.Data.BulletDistanceLVL * 11 + 250); //Base 250 max 3000

        numberOfBounces = LevelManagerScript.instance.Data.BounceBulletLVL;
        numberOfBullets = LevelManagerScript.instance.Data.NumberOfBulletsLVL+1;

        DashSlider.maxValue = DashDelay;
    }
    void Update()
    {
        if (!MainCubeScript.PlayRunning) return;
        if(initialize) Initialize();
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");

        Horizontal /= Mathf.Sqrt(2);
        Vertical /= Mathf.Sqrt(2);
    
        if (Horizontal != 0 && Vertical != 0) {
            lastHorizontal = Horizontal;
            lastVertical = Vertical;
        }else if (Horizontal == 0 && !(Vertical == 0))
        {
            lastHorizontal = 0;
            lastVertical = Vertical;
        }
        else if (Vertical == 0 && !(Horizontal == 0))
        {
            lastHorizontal = Horizontal;
            lastVertical = 0;
        }

        //Girar--------------------------------------------------------
        if (cam != null && Input.mousePosition != null) {
            Direction = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
            transform.up = Vector2.MoveTowards(transform.up, Direction, RotationSpeed * Time.deltaTime);
        }


        //Shoot---------------------------------------------------------
        if (Input.GetMouseButton(0) && Time.time > lastShoot+(1/ AtackSpeed))
        {
            lastShoot = Time.time;
            Shoot();
        }

        //Dash----------------------------------------------------------
        SpaceDash = Input.GetKey(KeyCode.Space);
        RigthClickDash = Input.GetMouseButton(1);
        if ((SpaceDash || RigthClickDash))
        {
            if (!resetThorms)
            {
                if (DashSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(DashSound, DashVolumen * VolumenManagerScript.GameVolumen);
                enemyHitByThormsList = new List<EnemyScript>();
                resetThorms = true;
            }    
            lastThorms = 0;
        }
    }

    private void FixedUpdate()
    {
        if (SpaceDash && Time.time >= lastDash + DashDelay)
        {//Space Dash
            SpaceDash = false; resetThorms = false;
            lastDash = Time.time;
            Rigidbody2D.velocity = new Vector2(lastHorizontal * Speed * DashMultMovement, lastVertical * Speed * DashMultMovement);
        }else if (RigthClickDash && Time.time >= lastDash + DashDelay) {
            //RigthClick Dash
            RigthClickDash = false; resetThorms = false;
            lastDash = Time.time;
            Rigidbody2D.velocity = Direction.normalized * Speed * DashMultMovement * 0.75f;
        }
        else if(Time.time > lastDash + 0.05)
        {//Normal
            Rigidbody2D.velocity = new Vector2(Horizontal * Speed, Vertical * Speed);
        }
        
    }
    private void OnGUI()
    {
        DashSlider.value = Mathf.Min(Time.time - lastDash,DashDelay);
    }
    private void Shoot()
    {
        if (ShootSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(ShootSound, ShootVolumen * VolumenManagerScript.GameVolumen);

        GameObject[] bullets = new GameObject[numberOfBullets];
        float numOffset = 10f;
        Vector3 offset = new Vector2(transform.up.y, -transform.up.x);
        /*switch (numberOfBullets){
                
            case 2:
                bullets[0] = (GameObject) Instantiate(Bullet, transform.position + transform.up * 50f + offset * numOffset, Quaternion.identity);
                bullets[1] = (GameObject) Instantiate(Bullet, transform.position + transform.up * 50f + offset * -numOffset, Quaternion.identity);
                break;

            case 3:
                ;
                bullets[0] = (GameObject) Instantiate(Bullet, transform.position + transform.up * 50f + offset * numOffset, Quaternion.identity);
                bullets[1] = (GameObject) Instantiate(Bullet, transform.position + transform.up * 50f, Quaternion.identity);
                bullets[2] = (GameObject) Instantiate(Bullet, transform.position + transform.up * 50f + offset * -numOffset, Quaternion.identity);
                break;

            default: //1 bullet
                bullets[0] = (GameObject)Instantiate(Bullet, transform.position + transform.up * 50f, Quaternion.identity);
                break;
        }*/

        int i = 0, x = 0;
        if (bullets.Length % 2 == 0)
        {
            i = 1; x = 1;
        }
        
        for (int k = 0; i < bullets.Length + x; i++, k++)
        {
            int j = (i + 1) / 2;
            j *= (i % 2 == 0) ? 1 : -1; //Si es par positivo si es impar negativo

            Vector3 pos = transform.position + transform.up * (50f - numOffset * Mathf.Abs(j)) + offset * numOffset * j;
            bullets[k] = (GameObject)Instantiate(Bullet, pos, Quaternion.identity);

            Vector3 direction = transform.up.normalized + offset.normalized * j * 0.025f;
            bullets[k].GetComponent<BulletScript>().setValues(direction, Damage, BulletRegen, KnockBack, BulletSpeed, BulletDistance, numberOfBounces, 1 / numberOfBullets);
        }

        /*foreach (GameObject g in bullets)
        {
            g.GetComponent<BulletScript>().setValues(transform.up.normalized, Damage, BulletRegen, KnockBack, BulletSpeed, BulletDistance, numberOfBounces, 1/numberOfBullets);
        }*/
        
    }

    public void Hit(float Damage)
    {
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        EnemyScript Enemy = collision.collider.GetComponent<EnemyScript>();
        if (Enemy != null)
        {
            if (Time.time >= lastThorms + ThormsDelay )
            {
                if ((SpaceDash || RigthClickDash) && enemyHitByThormsList.Contains(Enemy)) return; //SI ESTÁ DASH Y YA HA GOLPEADO AL ENEMIGO

                enemyHitByThormsList.Add(Enemy);
                ThormsDamage(Enemy, collision);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyScript Enemy = collision.collider.GetComponent<EnemyScript>();
        if (Enemy != null)
        {
            if(Time.time >= lastThorms + ThormsDelay)
            {
                if ((SpaceDash || RigthClickDash) && enemyHitByThormsList.Contains(Enemy)) return; //SI ESTÁ DASH Y YA HA GOLPEADO AL ENEMIGO

                enemyHitByThormsList.Add(Enemy);
                ThormsDamage(Enemy, collision);
            }

        }
    }
    private void ThormsDamage(EnemyScript Enemy, Collision2D collision)
    {
        
        lastThorms = Time.time;
        if(SpaceDash || RigthClickDash) //SUPER THROMS
        {
            if (ThormsDashSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(ThormsDashSound, ThormsDashVolumen * VolumenManagerScript.GameVolumen);
            for (int i = 0; i < 5; i++) Instantiate(ThormsParticles, collision.GetContact(0).point, Quaternion.identity); //Particulas

            //Vector2 direccion = collision.GetContact(0).point - (Vector2)transform.position;
            Vector2 direccion = Rigidbody2D.velocity;

            Enemy.Hit(Thorms * DashMult, direccion.normalized, KnockBack * DashMult);
        }
        else
        {
            if (ThormsSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(ThormsSound, ThormsVolumen * VolumenManagerScript.GameVolumen);
            Instantiate(ThormsParticles, collision.GetContact(0).point, Quaternion.identity); //Particulas

            Vector2 direccion = collision.GetContact(0).point - (Vector2)transform.position;

            Enemy.Hit(Thorms, direccion.normalized, KnockBack);
        }
        
    }
}
