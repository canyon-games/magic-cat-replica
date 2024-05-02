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
        if(abilityType==AbilityType.SlowMO)ability=EnemiesManager.instance.abilitiesConfig.slowMoAbility;
        if(abilityType==AbilityType.SameShape)ability=EnemiesManager.instance.abilitiesConfig.sameShape;
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
        print("ability button pressed");
        image.fillAmount=1;
        if(abilityType==AbilityType.SlowMO)
        {
            AudioManager.Instance.PlaySFX(SFX.SlowmoAbility);
            StartCoroutine(DecreaseFillAmountOverTime());
        }
        if(abilityType==AbilityType.SameShape)
        {
            AudioManager.Instance.PlaySFX(SFX.MirrorAbility);
            EnemiesManager.instance.SetMirrorAbility(true);
        }
        //ability = EnemiesManager.instance.AbilityButtonClick(abilityType);
    }
    private IEnumerator DecreaseFillAmountOverTime()
    {
        EnemiesManager.instance.SetSlowMoAbility(true);
        image.gameObject.SetActive(true);
        float elapsedTime = ability.duration;
        while (elapsedTime >= 0)
        {
            elapsedTime -= Time.deltaTime;
            image.fillAmount = Mathf.Lerp(0,1, elapsedTime / ability.duration);
            //print(elapsedTime);
            yield return new WaitForEndOfFrame();
        }
        EnemiesManager.instance.SetSlowMoAbility(false);
        //image.fillAmount = targetFillAmount;
    }
}
