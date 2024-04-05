using ShapeInputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DemoGameScript : MonoBehaviour
{
    public Mesh z,x,i,v,w;
    public Mesh x_mesh;
    public Mesh o_mesh;
    public Mesh tri_mesh;
    public Mesh star_mesh;
    public Mesh heart_mesh;
    public Mesh square_mesh;
    public GameObject shape_item;
    public Text winLoseText;
    public Transform spawnPoint;
    Shapes currentShape;
    int wins = 0;
    int loss = 0;

    void Start()
    {
        
        currentShape = (Shapes)Random.Range(0, 6);
        if (!spawnPoint) spawnPoint = GetComponent<Transform>();
        BuildShape();
    }
    private void Awake()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 45;
#endif
    }
    private void BuildShape()
    {
        if (spawnPoint.childCount == 0)
        {
            shape_item = Instantiate(shape_item, spawnPoint);
        }
       
        switch (currentShape)
        {
            case Shapes.NONE:
            case Shapes.NONSENSE:
                break;
            // case Shapes.X:
            //     shape_item.GetComponent<MeshFilter>().mesh = x;                
            //     break;
            // case Shapes.Z:
            //     shape_item.GetComponent<MeshFilter>().mesh = z;
            //     break;
            // case Shapes.I:
            //     shape_item.GetComponent<MeshFilter>().mesh = i;
            //     break;
            // case Shapes.W:
            //     shape_item.GetComponent<MeshFilter>().mesh = w;
            //     break;
            // case Shapes.v:
            //     shape_item.GetComponent<MeshFilter>().mesh = v;
            //     break;
            // case Shapes.Square:
            //     shape_item.GetComponent<MeshFilter>().mesh = square_mesh;
            //     break;
        }
    }


    public void ResetGame()
    {
        wins = 0;
        loss = 0;
    }

    void Update()
    {     

        var shape = ShapeInput.GetShape();
   
        if(shape.shape!= Shapes.NONE)
        {
            Debug.Log(shape);
        }
        if(shape.shape == currentShape)
        {
            wins++;
            var lastShape = currentShape;
            while(currentShape == lastShape)
                currentShape = (Shapes)Random.Range(0, 6);
            BuildShape();
        }
        else if(shape.shape != Shapes.NONE) //if there is a queued shape 
        {
            loss++;
            print("Failed:" + shape.shape);
        }


        //rotate this about the Y 
        this.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        

        winLoseText.text = $"WIN: {wins} LOSE: {loss}";

    }


}
