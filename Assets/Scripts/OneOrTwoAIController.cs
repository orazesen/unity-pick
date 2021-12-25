using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class OneOrTwoAIController : MonoBehaviour
{
    #region
    [Header("GameObjects")]
    //public GameObject particles;
    //public GameObject[] wastes;
    public GameObject oneTwoButton;
    public GameObject re_passPanel;
    public AudioSource buttonSound;

    [Header("Text")]
    public Text numberText;
    public Text turnText;
    public Text timerText;
    public Text subText;

    [Header("Constants")]
    private float timeToPass = 3.5f;
    private int randomNumber;
    private int turn = 1;
    private int lastSubNum;
    private bool isEnded = true;
    private bool passTimePassed = false;
    public bool pause = true;
    public int minValue;
    public int maxValue;

    [Header("Scripts")]
    AnimatorController animController;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SetGameUI();
        animController = GetComponent<AnimatorController>();
    }
    public void Play()
    {
        if (!re_passPanel.transform.GetChild(0).gameObject.activeSelf && re_passPanel.activeSelf)
        {
            re_passPanel.SetActive(false);
            randomNumber = UnityEngine.Random.Range(14, 30);
            turn = UnityEngine.Random.Range(1, 3);
            timeToPass = 3.5f;
            pause = false;
            StartCoroutine(ISetUIText());
        }                
    }

    IEnumerator ISetUIText()
    {        
        int i = 0;
        while (i++ < randomNumber)
        {
            yield return null;
            numberText.text = i + "";
        }

        isEnded = false;
        passTimePassed = false;
        SetTurnText();
    }

    void Update()
    {
        if (!pause)
        {
            if (!isEnded)
            {
                if (!passTimePassed && turn == 1)
                {
                    ShowRePassPanel();
                    timeToPass -= Time.deltaTime;
                    timerText.text = (int)timeToPass + "";                   
                    if (timeToPass <= 0)
                    {
                        passTimePassed = true;
                        StartCoroutine(IPlayAnimation(re_passPanel, 0.5f, false, "PassPanel"));
                        StartCoroutine(ISetSubButtons(0.3f, true));
                    }
                }
                else if (!passTimePassed && turn == 2)
                { 
                    StartCoroutine(PlayAI());                    
                }
            }
        }
    }

    IEnumerator IPlayAnimation(GameObject target, float duration, bool state, string animName)
    {
        if (!state)
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

    public IEnumerator IPause()
    {
        yield return new WaitForSeconds(1f);
        pause = true;
    }

    public IEnumerator PlayAI()
    {
        SetTurnText();
        int tempTurn = 1;
        if (!passTimePassed)
        {
            lastSubNum = 0;
            passTimePassed = true;

            if (randomNumber % 3 == 2)
            {
                lastSubNum = 1;                
            }
            else if (randomNumber % 3 == 0)
            {
                lastSubNum = 2;
            }
            else
            {
                lastSubNum = 0;
            }
            yield return new WaitForSeconds(.7f);
        }
        else
        {
            if ((randomNumber - 1) % 3 == 1)
            {
                lastSubNum = 1;
            }
            else if ((randomNumber - 2) % 3 == 1)
            {
                lastSubNum = 2;
            }
            else
            {
                if (randomNumber > 1)
                {
                    int y = UnityEngine.Random.Range(1, 3);
                    lastSubNum = y;

                    if (randomNumber == lastSubNum)
                    {
                        StartCoroutine(IGameOver());
                        tempTurn = 2;
                    }
                }
                else
                {
                    lastSubNum = 1;
                    StartCoroutine(IGameOver());
                    tempTurn = 2;
                }
            }
            if (randomNumber == 0)
            {
                //for (int i = 0; i < 5; i++)
                //{
                //    wastes[i] = Instantiate(particles);
                //    int x = Random.Range(-5,6);
                //    int y = Random.Range(-5, 6);
                //    wastes[i].transform.position += new Vector3(x, y, 0f);
                //}
                //StartCoroutine(IDestroyParticles());
            }
        }
        yield return new WaitForSeconds(.5f);
        yield return new WaitWhile(() => pause);
        yield return new WaitForSeconds(.5f);
        turn = tempTurn;      
        if (turn == 1)
        {
            UpdateUI();
        }
    }

    public IEnumerator IDestroyParticles()
    {
        yield return new WaitForSeconds(2f);
        //for (int i = 0; i < wastes.Length; i++)
        //{
        //    Destroy(wastes[i]);
        //}
        StartCoroutine(IGameOver());
    }
    
    public IEnumerator IGameOver()
    {
        numberText.text = "0";
        isEnded = true;
        SetTurnText();
        yield return new WaitForSeconds(2f);       
        SetGameUI();  
    }
    public void SetGameUI()
    {
        turnText.text = "";
        isEnded = true;
        pause = true;
        passTimePassed = true;
        StartCoroutine(ISetSubButtons(0f, false));
        ShowRePassPanel();
    }

    void SetTurnText()
    {
        string turnString = "";
        if (isEnded)
        {
            turnString = (turn == 1) ? "You lost" : "You won";
            if (passTimePassed)
            {
                StartCoroutine(ISetSubButtons(0.3f, false));
            }
        }
        else
        {
            turnString = (turn == 1) ? "Pick" : "Wait";
            if (passTimePassed)
            {
                StartCoroutine(ISetSubButtons(0.3f, (turn == 1) ? true : false));
            }
        }
        turnText.text = turnString;        
    }
    void UpdateUI()
    {
        SetTurnText();
        randomNumber -= lastSubNum;
        numberText.text = randomNumber + "";
        buttonSound.Play();       
        if (lastSubNum != 0)
        {
            subText.text = "-" + lastSubNum + "";
        }
        if (!isEnded)
        {
            if (turn == 2)
            {
                StartCoroutine(PlayAI());
            }
        }

    }
    public void SubtractOne()
    {
        if (turn == 1)
        {
            
            lastSubNum = 1;
            
            if (randomNumber >= 2)
            {
                turn = 2;
            }
            else if (randomNumber == 1)
            {                
                StartCoroutine(IGameOver());
            }
            if (turn == 2)
            {
                UpdateUI();
            }            
        }
    }

    public void SubtractTwo()
    {
        if (turn == 1)
        {
            if (randomNumber > 1)
            {
                lastSubNum = 2;
            }
            else
            {
                lastSubNum = 1;
            }
            if (randomNumber > 2)
            {
                turn = 2;
            }
            else
            {
                StartCoroutine(IGameOver());
            }
            if (turn == 2)
            {
                UpdateUI();
            }
        }
    }
    public void Pass(bool state)
    {
        passTimePassed = true;
        //re_passPanel.SetActive(false);
        oneTwoButton.SetActive(!state);
        if (state)
        {
            turn = 2;
            StartCoroutine(PlayAI());
        }
        StartCoroutine(IPlayAnimation(re_passPanel, 0.5f, false, "PassPanel"));
    }

    void ShowRePassPanel()
    {
        re_passPanel.SetActive(true);
        re_passPanel.transform.GetChild(0).gameObject.SetActive(!passTimePassed);
        re_passPanel.transform.GetChild(1).gameObject.SetActive(passTimePassed);
    }

    IEnumerator ISetSubButtons(float duration, bool state)
    {
        yield return new WaitForSeconds(duration);
        oneTwoButton.gameObject.SetActive(state);
    }
}
