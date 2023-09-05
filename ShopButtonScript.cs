using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopButtonScript : MonoBehaviour
{
    public int level, price, maxLevel, BasePrice = 0, PriceIncrease;
    public float MultValue, BaseValue, DivValue = 1;
    public bool lineal = true, Condition = false;
    public int ConditionLVL = 0;
    public string ConditionName = "", prefix = "", sufix = "";
    public TextMeshProUGUI ValueText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI PriceText;
    private Button Button;
    void Start()
    {
        Button = GetComponent<Button>();
    }
    public void reFreshPrice()
    {
        if(BasePrice != 0) price = BasePrice + level* PriceIncrease;
        else price = (int)Mathf.Pow(level, 1.55f - (maxLevel / 50000)); // 1.35f
    }
    private void OnGUI()
    {
        if (!MainCubeScript.PlayRunning)
        {
            level = LevelManagerScript.instance.getLevel(name);
            reFreshPrice();
            Button.interactable = (LevelManagerScript.instance.Data.TotalCash >= price)
                               && (level <= maxLevel)
                               && (!Condition || LevelManagerScript.instance.getLevel(ConditionName) > 0);
            //Suficiente dinero, NO ha llegado al nivel máximo y si tienes condition: sea mayor que nivelCond. Sino tiene, esa parte es siempre true
            if (lineal) ValueText.text = prefix + roundTo2Decimals((level * MultValue + BaseValue)/DivValue) + sufix;
            else
            {
                ValueText.text = prefix + roundTo2Decimals((Mathf.Pow(level,1.2f) * MultValue + BaseValue) / DivValue) + sufix;
            }
            LevelText.text = level.ToString();
            if (level <= maxLevel) PriceText.text = price.ToString() + "$";
            else PriceText.text = "MAX LEVEL";
        }
    }

    public void Click()
    {
        if(LevelManagerScript.instance.Data.TotalCash >= price) //redundante pero por si aca
        {
            LevelManagerScript.instance.spendMoney(price);
            LevelManagerScript.instance.addLevel(name);
        }
    }

    public string roundTo2Decimals(double aux)
    {
        aux = (int)(aux * 100); //Truncar el valor para que se quede solo con 2 decimales

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
