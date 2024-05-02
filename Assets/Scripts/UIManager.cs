using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
    public LevelEnemyConfig levelEnemyConfig;
    public Image progressBar;
    [Header("Panel")]
    public GameObject levelCompletePanel;
    public GameObject levelFailPanel, exitPanel, mainPanel, gamePanel, settingPanel,abilityPanel,customizePanel,shopePanel;
    [Header("Texts")]
    public TMP_Text levelNoTxt;
    public TMP_Text nextLevelTxt;
    public Button playButton;
    public GameObject musicOff;
    public GameObject soundOff;
    public LevelButton levelButton;
    public GameObject levelParent;
    public ScrollRect scrollRect;
    public RectTransform contentPanel;

    private void OnEnable()
    {
        EventManager.onGamePlay.AddListener(StartGame);
        EventManager.OnLevelComplete.AddListener(LevelComplete);
        EventManager.OnLevelFail.AddListener(LevelFail);
    }
    private void OnDisable()
    {
        EventManager.onGamePlay.RemoveListener(StartGame);
        EventManager.OnLevelComplete.RemoveListener(LevelComplete);
        EventManager.OnLevelFail.RemoveListener(LevelFail);
    }
    private void StartGame()
    {
        GamePreference.selectedLevel = GamePreference.openLevels;
        OnEnable();
        //OnDisable();
        SpawnLevels();
        InitUIStates();
    }
    public void Start()
    {
        StartGame();
    }
    public void InitUIStates()
    {
        MusicButton(GamePreference.music);
        SoundButton(GamePreference.sound);
    }
    public void MusicButton(bool on)
    {
        musicOff.SetActive(!on);
        GamePreference.music = on;
        AudioManager.Instance.SetSFXSetting(on);
    }
    public void SoundButton(bool on)
    {
        soundOff.SetActive(!on);
        GamePreference.sound = on;
        AudioManager.Instance.SetBGSetting(on);
    }
    public void SettingButton(bool on)
    {
        if (on) Time.timeScale = 0;
        else Time.timeScale = 1;
        settingPanel.SetActive(on);
    }
    public void PlayButton()
    {
        LevelStart();
    }
    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteAnimation.PlayAnimation();
        //levelComplete.Play();
        // levelCompletePanel.transform.DOScale (0,0);
        // levelCompletePanel.transform.DOScale(1, 1);
    }
    public void LevelFail()
    {
        levelFailPanel.SetActive(true);
        levelFailAnimation.PlayAnimation();
        // levelFailPanel.transform.DOScale (0,0);
        // levelFailPanel.transform.DOScale(1, 1);
    }
    public void LevelStart()
    {
        mainPanel.SetActive(false);
        gamePanel.SetActive(true);
        levelCompletePanel.SetActive(false);
        levelFailPanel.SetActive(false);
        levelNoTxt.text = $"" + (GamePreference.selectedLevel + 1);
        nextLevelTxt.text = $"" + (GamePreference.selectedLevel + 2);
        EventManager.onGamePlay.Invoke();
    }
    public void NextLevel()
    {
        GamePreference.selectedLevel = GamePreference.openLevels;
        LevelStart();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public List<LevelButton> buttons;
    public void SpawnLevels()
    {
        buttons = new List<LevelButton>();
        for (int i = 0; i < levelEnemyConfig.levels.Count; i++)
        {
            levelButton = Instantiate(levelButton, levelParent.transform);
            buttons.Add(levelButton);
            levelButton.SetLevelNo(i,levelEnemyConfig.levels[i].hasBossLevel);
        }
        SnapTo();
    }
    public void SelectLevel(LevelButton levelButton)
    {
        foreach (var item in buttons)
        {
            item.button.image.color = Color.white;
        }
        levelButton.button.image.color = Color.green;
        //SnapTo();
    }
    public void SnapTo(RectTransform target=null)
    {
        target=contentPanel.GetChild(GamePreference.selectedLevel).GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();

        contentPanel.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
    }
    public void UpdateProgress(float progress)
    {
        Debug.Log("current progress" + progress);
        progressBar.fillAmount = progress;
    }
    public void AbilityButton(bool active)
    {
        abilityPanel.SetActive(active);
        if(active)abilityPanelAnimation.PlayAnimation();
    }
    public void CustomizeButton(bool active)
    {
        customizePanel.SetActive(active);
        if(active)customizePanelAnimation.PlayAnimation();
    }
    public void ShopeButton(bool active)
    {
        shopePanel.SetActive(active);
        if(active)shopePanelAnimation.PlayAnimation();
    }
    public void ExitButton()
    {
        exitPanel.SetActive(true);
        exitPanelAnimation.PlayAnimation();
    }
    public void ExitGame(bool exit)
    {
        if (exit) Application.Quit();
        else exitPanel.SetActive(false);
    }
    public UIAnimation levelCompleteAnimation;
    public UIAnimation levelFailAnimation;
    public UIAnimation abilityPanelAnimation;
    public UIAnimation customizePanelAnimation;
    public UIAnimation shopePanelAnimation;
    public UIAnimation exitPanelAnimation;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation();
        }
    }
    public void PlayAnimation()
    {
        levelCompleteAnimation.PlayAnimation();
    }
}
[Serializable]
public class UIAnimation
{
    #region VariableDeclaration
    public List<AnimationProps> animationProps;
    public List<AnimationProps> animationPropsWithSequance;
    public DG.Tweening.Sequence sequence;
#endregion

    public void PlayAnimation()
    {
        foreach (var item in animationProps)
        {
            item.GetAnimationTweenToPlay().Play();
        }
            sequence = DOTween.Sequence();
        foreach (var item in animationPropsWithSequance)
        {
            //sequence.AppendInterval(1);
            sequence.Append(item.GetAnimationTweenToPlay());
        }
    }
}
[Serializable]
public class AnimationProps
{
    #region VariableDeclaration
    public Tween tween;
    public bool activateBeforePlay=true;
    public DOTweenAnimation.AnimationType animationType;
    public Ease ease;
    public bool relative;
    public int loops = 1;
    public bool setInitialValue;
    public Vector3 initial, final;
    public float duration;
    public float delay;
    public Transform itemTransform;
    public Action onComplete;
    #endregion

    public Tween GetAnimationTweenToPlay()
    {
        //itemTransform.gameObject.SetActive(!activateBeforePlay);
        switch (animationType)
        {
            case DOTweenAnimation.AnimationType.Scale:
                //itemTransform.DOScale(Vector3.zero,0);
                itemTransform.localScale=initial;
                tween = itemTransform.DOScale(final, duration).SetDelay(delay).SetEase(ease).SetRelative(relative).SetLoops(loops);
                break;
            case DOTweenAnimation.AnimationType.Rotate:
                itemTransform.eulerAngles=initial;
                tween = itemTransform.DORotate(final, duration).SetDelay(delay).SetEase(ease).SetRelative(relative).SetLoops(loops);
                break;
            case DOTweenAnimation.AnimationType.LocalRotate:
                itemTransform.localEulerAngles=initial;
                tween = itemTransform.DOLocalRotate(final, duration).SetDelay(delay).SetEase(ease).SetRelative(relative).SetLoops(loops);
                break;
            case DOTweenAnimation.AnimationType.Move:
                itemTransform.position=initial;
                tween = itemTransform.DOMove(final, duration).SetDelay(delay).SetEase(ease).SetRelative(relative).SetLoops(loops);
                break;
            case DOTweenAnimation.AnimationType.LocalMove:
                itemTransform.localPosition=initial;
                tween = itemTransform.DOLocalMove(final, duration).SetDelay(delay).SetEase(ease).SetRelative(relative).SetLoops(loops);
                break;
        }
        return tween;
    }
}