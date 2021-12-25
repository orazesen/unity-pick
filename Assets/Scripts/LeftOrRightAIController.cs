using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LeftOrRightAIController : MonoBehaviour
{
    #region
    [Header("UI Components")]
    public GridLayoutGroup grid;
    public GameObject buttonPref;
    public GameObject rePassPanel;
    public Text turnText;
    public Text leftScoreText;
    public Text rightScoreText;
    public GameObject L_F_Buttons;
    public Sprite rightOne;
    public AudioSource buttonSound;

    [Header("Constants")]
    List<GameObject> buttonsList;
    public List<GameObject> activeButtons;
    int count;    
    public int maxGridSize = 20;
    int currentGridSize;
    float time = 3.5f;
    bool isEnded = true;
    public bool pause = true;
    bool passTimePassed = true;
    int turn = 1;
    int leftScore = 0;
    int rightScore = 0;
    bool left = false;
    bool isFirst;
    int mistakes;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rePassPanel.SetActive(true);
        //PoolObjects();
        buttonsList = new List<GameObject>();
        Init();
    }

    void PoolObjects()
    {
        buttonsList = new List<GameObject>();
        for (int i = 0; i < maxGridSize; i++)
        {
            GameObject button = Instantiate(buttonPref, grid.transform);
            button.SetActive(false);
            buttonsList.Add(button);
            buttonsList[i].name = i + "";
        }
    }

    int GenerateCount()
    {
        count = UnityEngine.Random.Range(6, maxGridSize + 1);

        if (count % 2 == 1)
        {
            count++;
        }
        return count;
    }

    public void GenerateGrid()
    {
        StartCoroutine(IGenerateGrid());
    }

    IEnumerator IGenerateGrid()
    {
        grid.enabled = true;
        activeButtons = new List<GameObject>();
        for (int i = 0; i < currentGridSize; i++)
        {
            GameObject obj = Instantiate(buttonPref, grid.transform);
            obj.GetComponentInChildren<Text>().text = UnityEngine.Random.Range(1, 100) + "";
            obj.SetActive(true);
            activeButtons.Add(obj);
            yield return null;
        }
        isEnded = false;
        pause = false;
        passTimePassed = false;
        Calculate();
        //RemoveFromList();
        grid.enabled = false;
        ActiveBtnsBehavior((turn == 2) ? false : true);
    }


    void ResetButtonsPool()
    {
        for (int i = 0; i < buttonsList.Count; i++)
        {
            //Color color = new Color();
            //color.a = 1f;
            //color.b = 1f;
            //color.g = 1f;
            //color.r = 1f;
            //buttonsList[i].GetComponent<Image>().color = color;
            //buttonsList[i].SetActive(false);
            Destroy(buttonsList[i]);
        }
        for (int i = 0; i < activeButtons.Count; i++)
        {
            Destroy(activeButtons[i]);
        }
    }
    public void RemoveButton(GameObject target)
    {
        if (target == null)
        {
            return;
        }
        if (target == activeButtons[0] || target == activeButtons[activeButtons.Count - 1])
        {
            buttonSound.Play();
            turn = (turn == 1) ? 2 : 1;           
            SetScores(target);
            AIDecisionMaking(target);            
            Color color = new Color();
            color.a = 0.5f;
            target.GetComponent<Image>().color = color;
            target.GetComponent<Button>().enabled = false;
            buttonsList.Add(target);
            activeButtons.Remove(target);
            if (turn == 1)
            {
                ActiveBtnsBehavior(true);
            }
            if (activeButtons.Count <= 0)
            {
                StartCoroutine(IGameOver());
            }
            else
            {
                SetTurnText();
                ColorValidButtons();
                if (turn == 2)
                {
                    ActiveBtnsBehavior(false);
                    StartCoroutine(PlayAI());
                }
            }            
        }
        else
        {
            turnText.text = "??????";
            StartCoroutine(IShowTurnText());
        }
        
    }

    void ActiveBtnsBehavior(bool state)
    {
        if (!isEnded)
        {
            for (int i = 0; i < activeButtons.Count; i++)
            {
                activeButtons[i].GetComponent<Button>().enabled = state;
            }
        }        
    }


    void AIDecisionMaking(GameObject target)
    {
        if (turn == 2)
        {
            if (isFirst)
            {
                if (mistakes == 0)
                {
                    if ((target == activeButtons[0] && left) || (target == activeButtons[activeButtons.Count - 1] && !left))
                    {
                        int rand = UnityEngine.Random.Range(1, 3);
                        left = (rand == 1) ? true : false;
                    }
                    else
                    {
                        mistakes++;
                        left = (target == activeButtons[0]) ? false : true;
                        
                    }
                }
                else if (mistakes == 1)
                {
                    if ((target == activeButtons[0] && !left) || (target == activeButtons[activeButtons.Count - 1] && left))
                    {
                        int rand = UnityEngine.Random.Range(1, 3);
                        left = (rand == 1) ? true : false;
                    }
                    else
                    {
                        mistakes++;
                        if (target == activeButtons[0])
                        {
                            left = (Int32.Parse(activeButtons[1].GetComponentInChildren<Text>().text) < Int32.Parse(activeButtons[activeButtons.Count - 1].GetComponentInChildren<Text>().text)) ? false : true;
                        }
                        else
                        {
                            left = (Int32.Parse(activeButtons[0].GetComponentInChildren<Text>().text) < Int32.Parse(activeButtons[activeButtons.Count - 2].GetComponentInChildren<Text>().text)) ? false : true;
                        }
                        
                    }
                    
                }
                else if (mistakes == 2)
                {
                    left = (target == activeButtons[0]) ? true : false;
                }
            }
            else
            {
                left = (turn == 2 && target == activeButtons[0]) ? true : false;
            }
        }

    }

    void SetScores(GameObject target)
    {
        if (target == null)
        {
            return;
        }
        leftScore += (turn == 2) ? Int32.Parse(target.GetComponentInChildren<Text>().text) : 0;
        rightScore += (turn == 1) ? Int32.Parse(target.GetComponentInChildren<Text>().text) : 0;
    }

    void ColorValidButtons()
    {
        if (activeButtons.Count == 0)
        {
            return;
        }
        activeButtons[0].GetComponent<Image>().sprite = activeButtons[activeButtons.Count - 1].GetComponent<Image>().sprite = rightOne;
    }

    IEnumerator IShowTurnText()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitWhile(() => pause);
        if (!isEnded)
        {
            SetTurnText();
        }        
    }

    void RemoveFromList()
    {
        buttonsList.RemoveRange(buttonsList.Count - currentGridSize, currentGridSize);
    }

    void Init()
    {
        turnText.text = "";
        leftScoreText.text = "";
        rightScoreText.text = "";
        L_F_Buttons.SetActive(false);
        ShowRestartPanel();
    }

    public void Play()
    {
        pause = false;
        mistakes = 0;
        currentGridSize = GenerateCount();
        left = false;
        rePassPanel.SetActive(false);
        turn = UnityEngine.Random.Range(1, 3);
        Debug.Log(turn);        
        isFirst = (turn == 1) ? true : false;
        time = 1f;
        GenerateGrid();
        leftScore = 0;
        rightScore = 0;
        StartCoroutine(ISetSubButtons(0f, false));
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause)
        {
            if (!isEnded)
            {
                if (!passTimePassed && turn == 1)
                {
                    time -= Time.deltaTime;
                    if (time <= 0)
                    {
                        ColorValidButtons();
                        passTimePassed = true;
                        rePassPanel.SetActive(false);
                        StartCoroutine(ISetSubButtons(0.3f, true));
                        SetTurnText();
                    }
                }
                else if (!passTimePassed && turn == 2)
                {
                    passTimePassed = true;
                    StartCoroutine(PlayAI());
                }
            }
        }
    }

    public IEnumerator PlayAI()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitWhile(() => pause);
        ColorValidButtons();
        if (!isEnded)
        {
            SetTurnText();
        }        
        yield return new WaitForSeconds(1f);
        RemoveButton(activeButtons[(left) ? 0 : activeButtons.Count - 1]);
    }

    void Calculate()
    {
        int sumOdd = 0;
        int sumEven = 0;
        for (int i = activeButtons.Count - 1; i >= 0; i--)
        {
            int current = Int32.Parse(activeButtons[i].GetComponentInChildren<Text>().text);
            if (i % 2 == 0)
            {
                sumOdd += current;
            }
            else
            {
                sumEven += current;
            }
        }
        left = (sumOdd > sumEven) ? true : false;
    }

    public IEnumerator IGameOver()
    {
        turnText.text = "";
        isEnded = true;
        pause = true;
        passTimePassed = true;
        SetTurnText();
        StartCoroutine(ISetSubButtons(0f, false));
        yield return new WaitForSeconds(2f);
        ResetButtonsPool();
        Init();
    }

    public void EndGame()
    {
        ResetButtonsPool();
        isEnded = true;
        pause = true;
        passTimePassed = true;      
        StartCoroutine(ISetSubButtons(0f, false));
        Init();
    }

    void SetTurnText()
    {
        string turnString = "";
        if (isEnded)
        {
            turnString = (rightScore > leftScore) ? "You lost" : "You won";
        }
        else
        {
            turnString = (turn == 1) ? "Pick" : "Wait";
            if (activeButtons.Count != 0)
            {
                StartCoroutine(ISetSubButtons(0.15f, (turn == 1) ? true : false));
            }            
        }
        turnText.text = turnString;
        leftScoreText.text = leftScore + "";
        rightScoreText.text = rightScore + "";
    }
    

    void ShowRestartPanel()
    {
        rePassPanel.SetActive(true);
    }

    IEnumerator ISetSubButtons(float duration, bool state)
    {
        yield return new WaitForSeconds(duration);
        L_F_Buttons.gameObject.SetActive(state);
    }

    public void TakeFromLeft(bool l)
    {
        if (turn == 1)
        {
            RemoveButton(activeButtons[(l) ? 0 : activeButtons.Count - 1]);
            StartCoroutine(ISetSubButtons(0.15f, false));
        }
    }

}
