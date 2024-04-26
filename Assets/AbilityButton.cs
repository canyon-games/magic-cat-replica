using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public AbilityType abilityType;
    public Button button;
    public Image image;
    public Ability ability;
    private void Start() 
    {
        image.fillAmount = 0;
        //image.gameObject.SetActive(false);
        ability=EnemyManager.instance.AbilityButtonClick(abilityType);
        if(GamePreference.selectedLevel>=ability.levelRequire)
        {
            image.fillAmount=1;
            button.interactable = true;
        }
        else
        {
            image.fillAmount=0;
            button.interactable = false;
        }
    }
    public void OnClick()
    {
        image.fillAmount=1;
        ability = EnemyManager.instance.AbilityButtonClick(abilityType);
        StartCoroutine(DecreaseFillAmountOverTime());
    }
    private IEnumerator DecreaseFillAmountOverTime()
    {
        image.gameObject.SetActive(true);
        float elapsedTime = 0f;
        float startFillAmount = image.fillAmount;
        float targetFillAmount = 0f;

        while (elapsedTime < ability.duration)
        {
            elapsedTime -= Time.deltaTime;
            image.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / ability.duration);
            yield return null;
        }

        image.fillAmount = targetFillAmount;
    }
}
