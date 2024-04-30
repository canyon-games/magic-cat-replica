using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public GameObject levelFailPanel, exitPanel, mainPanel, gamePanel, settingPanel;
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
        levelComplete.Play();
        // levelCompletePanel.transform.DOScale (0,0);
        // levelCompletePanel.transform.DOScale(1, 1);
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
    public void LevelFail()
    {
        levelFailPanel.SetActive(true);
        levelFailPanel.transform.DOScale (0,0);
        levelFailPanel.transform.DOScale(1, 1);
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
    public void ExitButton()
    {
        exitPanel.SetActive(true);
    }
    public void ExitGame(bool exit)
    {
        if (exit) Application.Quit();
        else exitPanel.SetActive(false);
    }
    public UIAnimtion levelComplete;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
    }
    public void Play()
    {
        levelComplete.Play();
    }
}
[Serializable]
public class UIAnimtion
{
    public DG.Tweening.Sequence sequence;
    public void Play()
    {
        foreach (var item in animationProps)
        {
            item.Play().Play();
        }
        foreach (var item in animationPropsWithSequance)
        {
            if(sequence != null ) sequence.Kill();
            sequence = DOTween.Sequence();
            //item.Play();

            //sequence.AppendInterval(1);
            sequence.Join(item.Play());
            sequence.Play();
        }
    }
    public List<AnimationProps> animationProps;
    public List<AnimationProps> animationPropsWithSequance;
}
[Serializable]
public class AnimationProps
{
    public Tween tween=null;
    public Tween Play()
    {
        switch (this.animationType)
        {
            case DOTweenAnimation.AnimationType.Scale:
                //if(tween != null)tween.Kill();
                //transform.DOScale(initial,0);
                transform.localScale=Vector3.zero;
                tween= transform.DOScale(final,duration);
            break;
            case DOTweenAnimation.AnimationType.Rotate:
                            transform.DORotate(initial,0);
                tween= transform.DORotate(final,duration).SetDelay(delay).SetEase(ease).OnComplete(onComplete.Invoke).SetRelative(relative).SetLoops(loops);
            break;
            case DOTweenAnimation.AnimationType.LocalRotate:
                            transform.DOLocalRotate(initial,0);
                tween= transform.DOLocalRotate(final,duration).SetDelay(delay).SetEase(ease).OnComplete(onComplete.Invoke).SetRelative(relative).SetLoops(loops);
            break;
            case DOTweenAnimation.AnimationType.Move:
                            transform.DOMove(initial,0);
                tween= transform.DOMove(final,duration).SetDelay(delay).SetEase(ease).OnComplete(onComplete.Invoke).SetRelative(relative).SetLoops(loops);
            break;
            case DOTweenAnimation.AnimationType.LocalMove:
                            transform.DOLocalMove(initial,0);
                tween= transform.DOLocalMove(final,duration).SetDelay(delay).SetEase(ease).OnComplete(onComplete.Invoke).SetRelative(relative).SetLoops(loops);
            break;
        }
        return tween;
    }
    public DOTweenAnimation.AnimationType animationType;
    public Ease ease;
    public bool relative;
    public int loops=1;
    public bool setInitialValue;
    public Vector3 initial,final;
    public float duration;
    public float delay;
    public Transform transform;
    public Action onComplete;
}