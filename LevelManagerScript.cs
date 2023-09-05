using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Diagnostics;

public class LevelManagerScript : MonoBehaviour
{
    public static LevelManagerScript instance;

    public GameObject deathScreen;
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI HighestWaveText;
    public TextMeshProUGUI TotalCashTextScore;
    public TextMeshProUGUI TotalCashTextShop;

    public SaveData Data;
    private string fileName = "ToBuild000";

    public int Cash, Wave;
    void Start()
    {
        instance = this;
        Cash = 0;
        Wave = 0;

        SaveSystem.Initialize();
        string loadedData = SaveSystem.Load(fileName);
        if (loadedData != null) Data = JsonUtility.FromJson<SaveData>(loadedData);
        else
        {
            Data = new SaveData();
            string saveData = JsonUtility.ToJson(Data);
            SaveSystem.Save(fileName, saveData);
        }
    }

    public void addMoney(int amount) => Cash += amount;
    public bool spendMoney(int amount)
    {
        if(amount <= Data.TotalCash)
        {
            instance.Data.TotalCash -= amount;
            TotalCashTextScore.text = Data.TotalCash.ToString() + "$";
            TotalCashTextShop.text = Data.TotalCash.ToString() + "$";
            //Debug.Log("Comprar");

            string saveData = JsonUtility.ToJson(Data);
            SaveSystem.Save(fileName, saveData);
            return true;
        }
        else
        {
            //Debug.Log("No pots comprar");
            return false;
        }
    }

    public int getLevel(string name)
    {
        switch (name)
        {
            case "DAMAGE": return Data.DamageLVL;
            case "ATACK SPEED": return Data.AtackSpeedLVL;
            case "THORMS": return Data.ThormsLVL;
            case "KNOCKBACK": return Data.KnockBackLVL;

            case "MOVE SPEED": return Data.MoveSpeedLVL;
            case "DASH MULT": return Data.DashMultLVL;
            case "DASH RECOVER": return Data.DashRecoverLVL;
            case "BULLET SPEED": return Data.BulletSpeedLVL;

            case "HEALTH": return Data.HealthLVL;
            case "CUBE REGEN": return Data.CubeRegenLVL;
            case "BULLET REGEN": return Data.BulletRegenLVL;
            case "BULLET DISTANCE": return Data.BulletDistanceLVL;

            case "BOUNCE BULLET": return Data.BounceBulletLVL;
            case "NUMBER OF BULLETS": return Data.NumberOfBulletsLVL;
            case "CUBE MULTISHOOT": return Data.CubeMultiShootLVL;
            case "CUBE VIEW DISTANCE": return Data.CubeViewDistanceLVL;


            default: return 0;
        }
    }
    public void addLevel(string name)
    {
        switch (name)
        {
            case "DAMAGE": Data.DamageLVL++;
                break;
            case "ATACK SPEED": Data.AtackSpeedLVL++;
                break;
            case "THORMS": Data.ThormsLVL++;
                break;
            case "KNOCKBACK": Data.KnockBackLVL++;
                break;

            case "MOVE SPEED": Data.MoveSpeedLVL++;
                break;
            case "DASH MULT": Data.DashMultLVL++;
                break;
            case "DASH RECOVER": Data.DashRecoverLVL++;
                break;
            case "BULLET SPEED": Data.BulletSpeedLVL++;
                break;


            case "HEALTH": Data.HealthLVL++;
                break;
            case "CUBE REGEN": Data.CubeRegenLVL++;
                break;
            case "BULLET REGEN": Data.BulletRegenLVL++;
                break;
            case "BULLET DISTANCE": Data.BulletDistanceLVL++;
                break;

            case "BOUNCE BULLET": Data.BounceBulletLVL++;
                break;
            case "NUMBER OF BULLETS": Data.NumberOfBulletsLVL++;
                break;
            case "CUBE MULTISHOOT": Data.CubeMultiShootLVL++;
                break;
            case "CUBE VIEW DISTANCE": Data.CubeViewDistanceLVL++;
                break;
        }
        string saveData = JsonUtility.ToJson(Data);
        SaveSystem.Save(fileName, saveData);
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(5);
        deathScreen.SetActive(true);
        Wave = WaveSpawnerScript.WaveNumber;
        WaveText.text = Wave.ToString();

        if(Data.HighestWave < Wave)
        {
            Data.HighestWave = Wave;
        }
        Data.TotalCash += Cash;

        HighestWaveText.text = Data.HighestWave.ToString();
        TotalCashTextScore.text = Data.TotalCash.ToString() + "$";
        TotalCashTextShop.text = Data.TotalCash.ToString() + "$";

        string saveData = JsonUtility.ToJson(Data);
        SaveSystem.Save(fileName,saveData);

        yield break;
    }
    public void ReplayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void ChangeScene(string name) => SceneManager.LoadScene(name);
}

[System.Serializable]
public class SaveData
{
    public int HighestWave;
    public int TotalCash;

    public int DamageLVL;
    public int AtackSpeedLVL;
    public int ThormsLVL;
    public int KnockBackLVL;

    public int MoveSpeedLVL;
    public int DashMultLVL;
    public int DashRecoverLVL;
    public int BulletSpeedLVL;

    public int HealthLVL;
    public int CubeRegenLVL;
    public int BulletRegenLVL;
    public int BulletDistanceLVL;

    public int BounceBulletLVL;
    public int NumberOfBulletsLVL;
    public int CubeMultiShootLVL;
    public int CubeViewDistanceLVL;


    public SaveData(int HighWave = 0, int totalCash = 0, int Damage = 0, int AtackSpeed = 0, int Thorms = 0, int KnockBack = 0, int MoveSpeed = 0,
              int DashMult = 0, int DashRecover = 0, int BulletSpeed = 0, int Health = 0, int CubeRegen = 0, int BulletRegen = 0, int BulletDistance = 0,
              int BounceBullet = 0, int NumberOfBullets = 0, int CubeMultiShoot = 0, int CubeViewDistance = 0)
    {
        HighestWave = HighWave;
        TotalCash = totalCash;

        DamageLVL = Damage;
        AtackSpeedLVL = AtackSpeed;
        ThormsLVL = Thorms;
        KnockBackLVL = KnockBack;

        MoveSpeedLVL = MoveSpeed;
        DashMultLVL = DashMult;
        DashRecoverLVL = DashRecover;
        BulletSpeedLVL = BulletSpeed;

        HealthLVL = Health;
        CubeRegenLVL = CubeRegen;
        BulletRegenLVL = BulletRegen;
        BulletDistanceLVL = BulletDistance;

        BounceBulletLVL = BounceBullet;
        NumberOfBulletsLVL = NumberOfBullets;
        CubeMultiShootLVL = CubeMultiShoot;
        CubeViewDistanceLVL = CubeViewDistance;

    }
}
