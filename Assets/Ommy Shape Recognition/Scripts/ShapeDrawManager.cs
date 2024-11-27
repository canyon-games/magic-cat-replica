
using UnityEngine;
using UnityEngine.UI;
namespace OmmyShapeML
{
public class ShapeDrawManager : MonoBehaviour
{
    public Text logText;
    public EnemyManager enemyManager;
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
}