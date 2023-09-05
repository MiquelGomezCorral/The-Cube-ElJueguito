using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cashText;

    [SerializeField] TextMeshProUGUI EnemyDamage;
    [SerializeField] TextMeshProUGUI EnemyHealth;
    [SerializeField] TextMeshProUGUI EnemySpeed;
    [SerializeField] TextMeshProUGUI EnemyShooting;

    [SerializeField] TextMeshProUGUI TriangleDamage;
    [SerializeField] TextMeshProUGUI TriangleThorms;
    [SerializeField] TextMeshProUGUI TriangleKockback;
    [SerializeField] TextMeshProUGUI TriangleBulletRegen;
    [SerializeField] TextMeshProUGUI TriangleSpeed;
    [SerializeField] TextMeshProUGUI TriangleDashMult;

    [SerializeField] TextMeshProUGUI EnemiesCount;
    [SerializeField] TextMeshProUGUI RegPerSecond;


    private void OnGUI()
    {
        cashText.text = LevelManagerScript.instance.Cash.ToString();

        //ENEMY
        double aux = 20 + Mathf.Pow(WaveSpawnerScript.WaveNumber, 1.2f) * 1;
        EnemyDamage.text = roundTo2Decimals(aux);

        aux = 100 + Mathf.Pow(WaveSpawnerScript.WaveNumber, 1.2f) * 2;
        EnemyHealth.text = roundTo2Decimals(aux);

        aux = 200 + Mathf.Pow(WaveSpawnerScript.WaveNumber, 1.2f) * 0.5;
        EnemySpeed.text = "x"+roundTo2Decimals(aux/200);

        //aux = Mathf.Max(-0.5f, (float)(2 - WaveSpawnerScript.WaveNumber * 0.005));
        //EnemyShooting.text = roundTo2Decimals(aux);

        //TRIANGLE
        aux = Mathf.Pow(LevelManagerScript.instance.Data.DamageLVL, 1.2f) * 1 + 20; //Base 20
        TriangleDamage.text = roundTo2Decimals(aux);

        aux = Mathf.Pow(LevelManagerScript.instance.Data.ThormsLVL, 1.2f) * 2 + 40; //Base 10
        TriangleThorms.text = roundTo2Decimals(aux);

        aux = LevelManagerScript.instance.Data.KnockBackLVL * 7.20f + 100; //Base 100 max 3700
        TriangleKockback.text = "x"+roundTo2Decimals(aux/100);

        aux = Mathf.Pow(LevelManagerScript.instance.Data.BulletRegenLVL, 1.2f) * 1 + 10; //Base 10
        TriangleBulletRegen.text = roundTo2Decimals(aux);

        aux = LevelManagerScript.instance.Data.MoveSpeedLVL * 8 + 200; //Base 100
        TriangleSpeed.text = "x"+roundTo2Decimals(aux/200);

        aux = (float)(LevelManagerScript.instance.Data.DashMultLVL * 0.1 + 2); //Base 2
        TriangleDashMult.text = "x"+roundTo2Decimals(aux);

        //REGEN
        aux = Mathf.Pow(LevelManagerScript.instance.Data.CubeRegenLVL, 1.2f) * 1 + 2; //Base 2
        RegPerSecond.text = "+" + roundTo2Decimals(aux) + "/sec";


        EnemiesCount.text = WaveSpawnerScript.NumberOfEnemies + "";
    }

    public static string roundTo2Decimals(double aux)
    {
        aux = (int) (aux*100); //Truncar el valor para que se quede solo con 2 decimales

        string x = "";
        int decenas = (int)(aux % 100);
        int unidades = (int)(aux % 10); //Sacar unidades y decenas
        
        if (unidades == 0) //Si termina en 0
        {
            if (decenas == 0) x = ",00"; //Si NO habrán decimales añadir
            else x = "0";
        };

        aux /= 100; //De volver el valor a 
        return aux + x;
    }
}
