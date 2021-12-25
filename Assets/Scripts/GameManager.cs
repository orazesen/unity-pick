using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    public AnimatorController animController;
    public GameObject exitPanel;
    public GameObject fadePanel;
    private int i = 0;
    private float timeOut = 2;
    public Slider slider;
    public Text sliderText;
    OneOrTwoAIController onerTwoAi;
    LeftOrRightAIController leftOrRight;
    public AudioSource backMusic;

    void Awake()
    {
        slider.enabled = false;
        animController = GetComponent<AnimatorController>();
        StartCoroutine(ISetPanel(fadePanel, 0f, true, "CoverPanel"));
        StartCoroutine(ISetPanel(fadePanel, .7f, false, "CoverPanel"));
        if (GameObject.FindGameObjectWithTag("NBM") != null)
        {
            GameObject oldBackMusic = GameObject.FindGameObjectWithTag("BM");
            if (oldBackMusic != null)
            {
                Destroy(oldBackMusic);
            }
        }        
    }
    void Start()
    {
        onerTwoAi = GetComponent<OneOrTwoAIController>();
        leftOrRight = GetComponent<LeftOrRightAIController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                if (!exitPanel.activeSelf)
                {
                    i++;
                    if (i == 2)
                    {
                        Exit();
                        i = 0;
                        timeOut = 2f;
                    }
                }
                else
                {
                    No();
                }
                if (animController.playGame)
                {
                    i = 0;
                    timeOut = 2f;
                    animController.PlayGame();
                }
                if (!animController.settingsOpen)
                {
                    i = 0;
                    timeOut = 2f;
                    animController.MoveSettings();
                }
            }
            else 
            {
                if (!exitPanel.activeSelf)
                {
                    PauseGame();
                }
                else
                {
                    No();
                }
            }
            if (i == 1)
            {
                timeOut -= Time.deltaTime;
                if (timeOut <= 0)
                {
                    timeOut = 2;
                    i = 0;
                }
            }
        }
    }
    
    public void LoadGame(int i)
    {
        StartCoroutine(ILoadGame(i));
    }

    public IEnumerator ILoadGame(int i)
    {
        if (backMusic != null)
        {
            DontDestroyOnLoad(backMusic);
            backMusic.tag = "NBM";
        }
        
        fadePanel.SetActive(true);
        fadePanel.GetComponent<Animator>().SetBool("fade_out", true);        
        yield return new WaitForSeconds(1f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(i);        
        slider.enabled = true;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            sliderText.text = progress + "%";
            yield return null;
        }
    }

    public void Exit()
    {
        float time = (pausePanel.activeSelf) ? .5f : 0f;
        StartCoroutine(ISetPanel(exitPanel, time, true, "ExitPanel"));
    }

    public void Yes()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Application.Quit();
        }
        else
        {
            LoadGame(0);
        }
        
    }

    public void No()
    {
        i = 0;
        timeOut = 2f;
        StartCoroutine(ISetPanel(exitPanel, .5f, false, "ExitPanel"));
    }

    public void PauseGame()
    {
        StartCoroutine(ISetPanel(pausePanel, ((pausePanel.activeSelf) ? 0.5f : 0f), !pausePanel.activeSelf, "PausePanel"));
        if (SceneManager.GetActiveScene().name == "OneOrTwoScene")
        {
            onerTwoAi.pause = !onerTwoAi.pause;
        }
        else
        {
            leftOrRight.pause = !leftOrRight.pause;
        }
    }

    public IEnumerator ISetPanel(GameObject target, float duration, bool state, string animName)
    {
        if (!state && target != fadePanel)
        {
            animController.PlayAnim(animName, !state);
        }
        yield return new WaitForSeconds(duration);
        if (state)
        {
            animController.PlayAnim(animName, !state);
        }
        target.SetActive(state);
    }
}
