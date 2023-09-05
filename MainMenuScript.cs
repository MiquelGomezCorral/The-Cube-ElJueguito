using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public AudioClip ClickSound, OtherClickSound;
    public float ClickVolumen = 1f, OtherClickVolumen = 1f;
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Debug.Log("QUIT...");
        Application.Quit();
    }

    public void EndRound()
    {
        MainCubeScript cube = GameObject.FindGameObjectWithTag("Central Cube").GetComponent<MainCubeScript>();
        cube.KillMainCube();
    }

    public void stopGame()
    {
        MainCubeScript.PlayRunning = false;
        MainCubeScript.EnemysCanMove = false;
    }
    public void continueGame() => MainCubeScript.PlayRunning = true;

    public void clickSound() => Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickSound, ClickVolumen * VolumenManagerScript.GameVolumen);

    public void PlaySound() => Camera.main.GetComponent<AudioSource>().PlayOneShot(OtherClickSound, OtherClickVolumen * VolumenManagerScript.GameVolumen);

}
