using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public Image fill;
    public float time=3;
    void Start()
    {
        StartCoroutine(DecreaseFillAmountOverTime());
        //Invoke(nameof(LoadNextScene),2);
    }

    // Update is called once per frame
    void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
    private IEnumerator DecreaseFillAmountOverTime()
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            fill.fillAmount = Mathf.Lerp(0,1, elapsedTime / time);
            //print(elapsedTime);
            yield return new WaitForEndOfFrame();
        }
        LoadNextScene();
        //image.fillAmount = targetFillAmount;
    }
}
