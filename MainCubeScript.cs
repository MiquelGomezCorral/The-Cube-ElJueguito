using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainCubeScript : MonoBehaviour
{
    [SerializeField] public Slider HealthSlider;
    [SerializeField] public Slider BossHealthSlider;
    [SerializeField] TextMeshProUGUI HealthText;
    //public GameObject[] Enemys;
    private float Health, lastShoot;
    public float SpawnTime = 3f, ShootDistance = 700f, MAXHEALTH = 1000.0f, CubeRegen = 1, Points;
    public float Damage, AtackSpeed, KnockBack, BulletSpeed, BulletRegen, BulletDistance;
    public static bool PlayRunning, EnemysCanMove, initialize;
    public bool inmortal = false;
    public int numberOfBullets, numberOfBounces;

    public ParticleSystem DeathParticles;
    public ParticleSystem HitParticles;

    public AudioClip DeadSound, ShootSound;
    public float DeadVolumen = 1f, ShootVolumen = 1f;

    public GameObject GameMusic;
    public GameObject LobbyMusic;
    public GameObject Bullet;

    public SortedSet<float> closserEnemySort;
    public Dictionary<float, GameObject> closserEnemyMap;

    void Start()
    {
        initialize = true;

        Points = 0;
        PlayRunning = true;
    }
    private void Initialize()
    {
        initialize = false;

        MAXHEALTH = Mathf.Pow(LevelManagerScript.instance.Data.HealthLVL, 1.2f) * 10 + 100; //Base 100
        CubeRegen = Mathf.Pow(LevelManagerScript.instance.Data.CubeRegenLVL, 1.2f) * 1 + 2; //Base 2.5

        Damage = Mathf.Pow(LevelManagerScript.instance.Data.DamageLVL, 1.2f) * 1 + 20;
        AtackSpeed = (float)(LevelManagerScript.instance.Data.AtackSpeedLVL * 0.05 + 1); //1 por segundo
        KnockBack = LevelManagerScript.instance.Data.KnockBackLVL * 7.20f + 100; //Base 100 max 3700
        BulletSpeed = LevelManagerScript.instance.Data.BulletSpeedLVL * 40 + 500; //Base 500

        BulletRegen = 0;//Mathf.Pow(LevelManagerScript.instance.Data.BulletRegenLVL, 1.2f) * 1 + 10; //Base 10
        BulletDistance = (float)(LevelManagerScript.instance.Data.CubeViewDistanceLVL * 11 + 250); //Base 250 max 1900
        ShootDistance = (float)(LevelManagerScript.instance.Data.CubeViewDistanceLVL * 9 + 250); //Base 250 max 1500


        numberOfBounces = 0;// LevelManagerScript.instance.Data.BounceBulletLVL;
        numberOfBullets = LevelManagerScript.instance.Data.CubeMultiShootLVL;

        this.Health = MAXHEALTH;
        HealthSlider.maxValue = MAXHEALTH;
        hideBossSlider();
    }
    private void FixedUpdate()
    {
        if (!PlayRunning) return;

        StartCoroutine(playGameMusic(0));
        Heal(CubeRegen * Time.deltaTime);
        HealthText.text = (int)(Health) + " / " + (int)(MAXHEALTH);

        if (Time.time >= lastShoot + (5 / AtackSpeed)) Shoot();
    }
    void Update()
    {
        if (initialize) Initialize();
    }
    IEnumerator playLobbyMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        LobbyMusic.SetActive(true);
        GameMusic.SetActive(false);
        yield break;
    }
    IEnumerator playGameMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameMusic.SetActive(true);
        LobbyMusic.SetActive(false);
        yield break;
    }

    public void Shoot()
    {
        
        closserEnemySort = new SortedSet<float>();
        closserEnemyMap = new Dictionary<float, GameObject>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies) 
        {
            float distance = (float) Math.Sqrt( Math.Pow(enemy.transform.position.x - transform.position.x, 2) +
                                              Math.Pow(enemy.transform.position.y - transform.position.x, 2) );
            closserEnemyMap.TryAdd(distance, enemy);
            closserEnemySort.Add(distance);

            if(closserEnemySort.Count > numberOfBullets)
            {
                closserEnemySort.Remove(closserEnemySort.Max);
            }
        }

        bool hasShoot = false;
        foreach (float d in closserEnemySort)
        {
            if (d <= ShootDistance)
            {
                hasShoot = true;
                GameObject e = closserEnemyMap[d];
                if (e != null)
                {
                    ShootBullet(e);
                }
            }
        }
        if (ShootSound && hasShoot)
        {
            lastShoot = Time.time;
            Camera.main.GetComponent<AudioSource>().PlayOneShot(ShootSound, ShootVolumen * VolumenManagerScript.GameVolumen);
        }
    }
    protected void ShootBullet(GameObject e)
    {
        Vector2 direction = (e.transform.position - transform.position).normalized;
        GameObject bullet = (GameObject)Instantiate(Bullet, (Vector2)transform.position + direction*5, Quaternion.identity);

        bullet.GetComponent<BulletScript>().setValues(direction, Damage, BulletRegen, KnockBack, BulletSpeed, BulletDistance, numberOfBounces, 1 / 1);
    }

    public void Hit(float Damage, Vector2 point)
    {
        Instantiate(HitParticles, point, Quaternion.identity); //Particulas
        if (inmortal) return; //IF inmortal only show that was hit but dont deal damage

        Health -= Damage;
        if (Health <= 0 && PlayRunning)
        {//"KILL" AND GAME OVER
            KillMainCube();;
        }
    }
    public void Heal(float Heal) => this.Health = Mathf.Min(this.Health + Heal, MAXHEALTH);

    public void KillMainCube()
    {
        StartCoroutine(playLobbyMusic(5));

        if (DeadSound) Camera.main.GetComponent<AudioSource>().PlayOneShot(DeadSound, DeadVolumen * VolumenManagerScript.GameVolumen);
        Camera.main.GetComponent<CameraShakeScript>().startShake(5f, 75f);

        gameObject.transform.localScale = new Vector3(0, 0, 0);

        Instantiate(DeathParticles, transform.position, Quaternion.identity); //Particulas

        StartCoroutine(LevelManagerScript.instance.GameOver());
        PlayRunning = false; EnemysCanMove = true;
    }
    private void OnGUI() => HealthSlider.value = Health;
    
    public void addPoints(float Points) => this.Points += Points;

    public void showBossSlider(float health)
    {
        BossHealthSlider.transform.localScale = new Vector3(2.5f, 0.4375001f, 1);
        BossHealthSlider.maxValue = health;
        BossHealthSlider.value = health;
    }
    public void hideBossSlider()
    {
        BossHealthSlider.transform.localScale = new Vector3(0, 0, 0);
    }
    public void setValueBossSlider(float health)
    {
        BossHealthSlider.value = health;
    }


}
