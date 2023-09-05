using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WaveSpawnerScript;

public class WaveSpawnerScript : MonoBehaviour
{
    public TextMeshProUGUI waveText, SpecialWaveText;
    [SerializeField] public Slider LevelSlider;
    [SerializeField] public Image[] EnemysImage;

    public GameObject circle;
    public GameObject[] Enemys;
    public Wave CurrentWave, NextWave;

    public float timeBetweenWaves = 10, timeBetweenEnemyes = 1;
    public float waveCountDown;
    private float searchCountDown = 1f;
    public static int WaveNumber = 0;
    public static int NumberOfEnemies = 0;
    public bool continuatedSpawn = false; 
    public enum SpawnState { SPAWINING, WAITING, COUNTING };
    private SpawnState state = SpawnState.COUNTING;
    public enum EnemyNumber {ENEMY = 0, FAST = 1, RANGED = 2, TANK = 3, NANO = 4, SUMMONER = 5, BOSS = 6};

    //private static object lockObj = new object();

    void Start()
    {
        waveText.text = "1";
        LevelSlider.maxValue = timeBetweenWaves;
        waveCountDown = timeBetweenWaves;

        WaveNumber = 0;

        NextWave = null;
        generateNextWave(); //Generar 2 rondas y poner la actual

        hideSpecialWaveText();
    }
    void FixedUpdate()
    {
        NumberOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (state == SpawnState.WAITING)
        {
            if (continuatedSpawn || !EnemyIsAlive() )
            {
                continuatedSpawn = true;
                hideSpecialWaveText();

                state = SpawnState.COUNTING;
                waveCountDown = timeBetweenWaves;
            }
            else return;
        }
        if (waveCountDown <= 0) //Counting
        {
            if (state != SpawnState.SPAWINING) //Counting
            {
                increaseWave();
                StartCoroutine(SpawnWave(CurrentWave));
            }
        }
        else if(MainCubeScript.PlayRunning) //Counting
        {
            if (EnemyIsAlive())
            {
                waveCountDown -= Time.deltaTime;
                LevelSlider.value = timeBetweenWaves - waveCountDown;
            }
            else
            {
                waveCountDown -= (timeBetweenWaves)*Time.deltaTime;
                LevelSlider.value = timeBetweenWaves - waveCountDown;
            }
            
        }
    }
    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;
        if (searchCountDown <= 0)
        {
            searchCountDown = 0.01f;
            return GameObject.FindGameObjectsWithTag("Enemy").Length != 0;
        }
        return true;
    }

    public void generateNextWave()
    {
        if (NextWave == null)
        {
            NextWave = new Wave(Enemys, 2, WaveNumber+1, 1); 
            //WaveNumber++;
        }
        CurrentWave = NextWave;
        NextWave = new Wave(Enemys, WaveNumber, WaveNumber, timeBetweenEnemyes);
    }
    IEnumerator SpawnWave(Wave wave)
    {
        LevelSlider.value = 0;
        state = SpawnState.SPAWINING;

        if (wave.special) continuatedSpawn = false;

        generateNextWave();

        if (wave.special) //Show Text
        {
            SpecialWaveText.transform.localScale = new Vector3(1f, 1f, 1f);
            float aux = getImageScale(wave.enemyType);
            EnemysImage[wave.enemyType].transform.localScale = new Vector3(aux, aux, aux);
        }

        for (int i = 0; i < wave.enemysCount; i++)
        {
            while(!MainCubeScript.PlayRunning) yield return new WaitForSeconds(wave.spawnDelay);
            SpawnEnemy(wave, i);
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    private void SpawnEnemy(Wave wave, int e)
    {
        /*float d = UnityEngine.Random.Range(1250f, 1500.0f); //Distancia al enemy
        float alf = UnityEngine.Random.Range(0f, 360f); //Angulo en el que aparece

        // d/sin(angulo puesto) = a/sin(angulo puesto) = b/sin(angulo puesto)
        float a = (float)(d * Mathf.Sin(alf)); // entre Math.Sin(90) = 1
        float beta = 180 - 90 - alf; //angulo puesto a b
        float b = (float)(d * Mathf.Sin(beta)); // entre Math.Sin(90) = 1

        Vector3 direction = new Vector3(b, a, 0);

        Instantiate(wave.toSpawnEnemys[e], new Vector3(0, 0, 0) + direction, Quaternion.identity);*/

        float d = UnityEngine.Random.Range(1250f, 1500.0f); //Distancia al enemy
       
        float alf = UnityEngine.Random.Range(0f, 360f); //Angulo en el que aparece

        float x = d * Mathf.Cos(alf);
        float y = d * Mathf.Sin(alf);

        Vector3 direction = new Vector3(x, y, 0);
        Instantiate(wave.toSpawnEnemys[e], new Vector3(0, 0, 0) + direction, Quaternion.identity);
        //Instantiate(circle, new Vector3(0, 0, 0) + direction, Quaternion.identity); //Circle to see where did it spawned
    }

    private void increaseWave()
     {
        WaveNumber++;
        waveText.text = WaveNumber.ToString();
        LevelManagerScript.instance.addMoney(10 * WaveNumber);
    }

    public float getImageScale(int i)
    {
        switch (i)
        {
            case (int)EnemyNumber.ENEMY: return 0.5f;
            case (int)EnemyNumber.FAST: return 0.5f;
            case (int)EnemyNumber.RANGED: return 0.5f;
            case (int)EnemyNumber.TANK: return 0.65f;
            case (int)EnemyNumber.NANO: return 0.2f;
            case (int)EnemyNumber.SUMMONER: return 0.5f;
            case (int)EnemyNumber.BOSS: return 0.75f;
            default: return 0.5f;
        }
    }

    public void hideSpecialWaveText()
    {
        foreach (Image i in EnemysImage)
        {
            i.transform.localScale = new Vector3(0f, 0f, 0f);
        }
        SpecialWaveText.transform.localScale = new Vector3(0f, 0f, 0f);
    }

}

    [System.Serializable]
    public class Wave
    {
        public GameObject[] Enemys, toSpawnEnemys;
        public int enemysCount, WaveNumber;
        public float spawnDelay;
        public bool special;
        public int enemyType;

        public Wave(GameObject[] Enemys, int enemysCount = 1, int WaveNumber = 1, float spawnDelay = 3)
        {
            this.Enemys = Enemys;
            this.enemysCount = (int)((enemysCount + 1) / 5) + 2;
            this.spawnDelay = Mathf.Max(0.1f, (float)(1.5 - (WaveNumber/100)));
            this.WaveNumber = WaveNumber;
            special = false;

            
            GenerateEnemyes();
        }

    private void GenerateEnemyes()
    {
        bool boos = false;
        if((WaveNumber+2) % 10 == 0) {
            enemysCount++;
            boos = true;
        }else if ((WaveNumber + 2) % 5 == 0)
        {
            special = true;
        }
        
        if (!special)
        {
            toSpawnEnemys = new GameObject[enemysCount];
            int x;
            for (int i = 0; i < enemysCount; i++)
            {
                x = UnityEngine.Random.Range(0, 100);
                if (i == enemysCount / 2 && boos) toSpawnEnemys[i] = Enemys[(int)WaveSpawnerScript.EnemyNumber.BOSS];

                else if (x <= 50) toSpawnEnemys[i] = Enemys[(int)WaveSpawnerScript.EnemyNumber.ENEMY]; //50%

                else if (x <= 75) toSpawnEnemys[i] = Enemys[(int)WaveSpawnerScript.EnemyNumber.FAST]; //25%

                //else if (x <= 70) toSpawnEnemys[i] = Enemys[(int)WaveSpawnerScript.EnemyNumber.NANO];

                else if (x <= 80) toSpawnEnemys[i] = Enemys[(int)WaveSpawnerScript.EnemyNumber.SUMMONER]; //5%

                else if (x <= 90) toSpawnEnemys[i] = Enemys[(int)WaveSpawnerScript.EnemyNumber.RANGED]; //10%

                else if (x <= 100) toSpawnEnemys[i] = Enemys[(int)WaveSpawnerScript.EnemyNumber.TANK]; //10%
            }
        }
        else //CADA 5 RONDAS
        {
            spawnDelay = Mathf.Max(0.025f, 0.25f - (WaveNumber / 100));;
            enemyType = UnityEngine.Random.Range(1, Enemys.Length-1); //TODOS MENOS EL NORMAL Y EL BOSS
            //enemyType = (int)WaveSpawnerScript.EnemyNumber.SUMMONER;
            switch (enemyType)
            {
                //case (int)WaveSpawnerScript.EnemyNumber.NANO: enemysCount = (int) (enemysCount * 2); //NORMAL
                //break;
                case (int)WaveSpawnerScript.EnemyNumber.FAST: 
                    enemysCount = (int)(enemysCount * 3); //FAST
                    break;
                case (int)WaveSpawnerScript.EnemyNumber.RANGED: 
                    enemysCount = (int)(enemysCount * 2); //RANGED
                    break;
                case (int)WaveSpawnerScript.EnemyNumber.TANK: 
                    enemysCount = (int)(enemysCount * 1); //TANK
                    break;
                case (int)WaveSpawnerScript.EnemyNumber.NANO: 
                    enemysCount = (int)(enemysCount * 5); //NANO
                    break;
                case (int)WaveSpawnerScript.EnemyNumber.SUMMONER:
                    enemysCount = (int)(enemysCount * 0.75); //SUMMONER
                    break;

            }
            toSpawnEnemys = new GameObject[enemysCount];
            for (int i = 0; i < enemysCount; i++)
            {
                toSpawnEnemys[i] = Enemys[enemyType];
            }
        }
        
    }
}

