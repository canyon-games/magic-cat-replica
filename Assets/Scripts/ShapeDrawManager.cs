using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapeInputs;
using TMPro;
using UnityEngine.UI;
public class ShapeDrawManager : MonoBehaviour
{
    public Text logText;
    public EnemiesManager enemyManager;
    void Update()
    {
        var shape = ShapeInput.GetShape();

        if (shape.shape == Shapes.NONE)
        {
            //PrintLog(shape);
        }
        else if (ShapeDrawed(shape.shape))
        {
            PrintLog(shape);
        }
        else if (shape.shape != Shapes.NONE) //if there is a queued shape 
        {
            //loss++;
            print("Failed:" + shape.shape);
        }
    }
    public void PrintLog(object log)
    {
        print(log);
        logText.text = log.ToString();
    }
    public bool ShapeDrawed(Shapes shape)
    {
        enemyManager.TakeDamageEnemies(shape);
        return true;
    }
}
