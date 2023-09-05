using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpawnerScript : MonoBehaviour
{
    public GameObject circle;
    public float radius = 1250, angle = 0;

    private void Start()
    {
        spawnCircles();
    }

    public void spawnCircles()
    {
        while (angle < 360)
        {
            /*//float d = UnityEngine.Random.Range(1250f, 1500.0f); //Distancia al enemy
            //float alf = UnityEngine.Random.Range(0f, 360f); //Angulo en el que aparece
            angle++;

            // d/sin(angulo puesto) = a/sin(angulo puesto) = b/sin(angulo puesto)
            float a = (float)(radius * Mathf.Sin(angle)); // entre Math.Sin(90) = 1
            float beta = 180 - 90 - angle; //angulo puesto a b
            float b = (float)(radius * Mathf.Sin(beta)); // entre Math.Sin(90) = 1

            Vector3 direction = new Vector3(b, a, 0);

            Instantiate(circle, new Vector3(0, 0, 0) + direction, Quaternion.identity);*/

            angle++;

            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            Vector3 direction = new Vector3(x, y, 0);
            Instantiate(circle, new Vector3(0, 0, 0) + direction, Quaternion.identity); 
        }
    }
}
