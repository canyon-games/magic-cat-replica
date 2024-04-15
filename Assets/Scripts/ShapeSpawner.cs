using System.Collections;
using System.Collections.Generic;
using ShapeInputs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSpawner : MonoBehaviour
{
    public int noOfShapes;
    public int resetTimer;
    public Enemy[] shapes;
    public List<Enemy> currentshapes;
    public Transform spawnPoints;
    private void Start() 
    {
        StartCoroutine(SpawnShapesTimer());
    }
    IEnumerator SpawnShapesTimer()
    {
        while(true)
        {
            SpawnShapes();
            yield return new WaitForSeconds(resetTimer);
        }
    }
    public void SpawnShapes()
    {
        foreach (var item in currentshapes)
        {
            Destroy(item.gameObject);
        }
        currentshapes=new List<Enemy>();
        for (int i = 0; i < noOfShapes; i++)
        {
            var shape=Instantiate(shapes[Random.Range(0,shapes.Length)]);
            shape.transform.SetParent(spawnPoints);
            currentshapes.Add(shape);
        }
    }
    public bool ShapeDrawed(Shapes shape)
    {
        foreach (var item in currentshapes)
        {
            if(item.shapeType==shape)
            {
                currentshapes.Remove(item);
                Destroy(item.gameObject);
                return true;
            }
        }
        return false;
    }
    public void CheckShapeAvilable(Shapes shape)
    {
        
    }
    public Text log;
    void Update()
    {     

        var shape = ShapeInput.GetShape();
   
        if(shape.shape!= Shapes.NONE)
        {
            Debug.Log(shape);
            log.text=shape.ToString();
        }
        // if(shape.shape == currentShape)
        // {
        //     wins++;
        //     var lastShape = currentShape;
        //     while(currentShape == lastShape)
        //         currentShape = (Shapes)Random.Range(0, 6);
        //     BuildShape();
        // }
        if(ShapeDrawed(shape.shape))
        {

        }
        else if(shape.shape != Shapes.NONE) //if there is a queued shape 
        {
            //loss++;
            print("Failed:" + shape.shape);
        }
    }
}
