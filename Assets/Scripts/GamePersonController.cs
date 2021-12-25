using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePersonController : MonoBehaviour
{
    public Text number;
    private int randomNumber;
    public int turn = 0;
    public Text turnShower;
    private bool isEnded = true;
    public Text timer;
    private float time = 4f;
    private float passTime = 3.5f;
    private bool passTimePassed = false;
    public Button pass;
    public Button restartButton;
    public Button subOne;
    public Button subTwo;
    public Button subOne1;
    public Button subTwo1;
    public bool animPlayed = false;
    private int lastSubNum;
    public Text subNum;
    public GameObject popUpPanel;
    public Text popUpText;
    public int popUpState = 0;
    public GameObject colliderPanel;
    public bool pause = false;
    public GameObject particles;
    public GameObject[] wastes;

    // Start is called before the first frame update
    void Start()
    {
        GameOver();
    }

    public void Play()
    {
        randomNumber = Random.Range(15, 24);
        number.text = randomNumber + "";
        timer.text = time + "";
        turn = Random.Range(1, 3);
        restartButton.gameObject.SetActive(false);
        pass.gameObject.SetActive(false);
        subOne.gameObject.SetActive(false);
        subTwo.gameObject.SetActive(false);
        subOne1.gameObject.SetActive(false);
        subTwo1.gameObject.SetActive(false);
        number.enabled = false;
        isEnded = false;
        time = 4f;
        animPlayed = false;
        timer.enabled = true;
        if (turn == 1)
        {
            turnShower.text = "Player1 pick";
            turnShower.gameObject.GetComponent<Animator>().SetTrigger("up");
            timer.gameObject.GetComponent<Animator>().SetTrigger("down");
        }
        else
        {
            turnShower.text = "Player2 pick!";
            turnShower.gameObject.GetComponent<Animator>().SetTrigger("down");
            timer.gameObject.GetComponent<Animator>().SetTrigger("up");
        }
        passTimePassed = false;
        passTime = 3.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause)
        {
            if (!isEnded)
            {
                time -= Time.deltaTime;
                timer.text = (int)time + "";
                if (time <= 0)
                {
                    timer.enabled = false;
                    if (!animPlayed)
                    {
                        number.enabled = true;
                        if (turn == 1)
                        {
                            number.gameObject.GetComponent<Animator>().SetTrigger("move");
                            animPlayed = true;
                        }
                        else
                        {
                            number.gameObject.GetComponent<Animator>().SetTrigger("up");
                            animPlayed = true;
                        }
                    }
                    if (!passTimePassed)
                    {
                        EnableOrDisablePassButton();
                        passTime -= Time.deltaTime;
                        timer.text = "Click to pass!" + "\n" + (int)passTime;
                        timer.enabled = true;
                        if (passTime <= 0)
                        {
                            passTimePassed = true;
                            isEnded = true;
                            EnableOrDisablePassButton();
                            EnableOrDisableSubButtons();
                            timer.enabled = false;
                        }
                    }
                }
            }
        }
    }

    public void OpenPopUp(int i)
    {
        popUpPanel.SetActive(true);
        colliderPanel.SetActive(true);
        pause = true;
        if (i == 0)
        {
            popUpState = 0;
            popUpText.text = "Do you want to restart?";
        }
        else if (i == 1)
        {
            popUpState = 1;
            popUpText.text = "Do you want to exit?";
        }
    }
    public void ClosePopUp()
    {
        popUpPanel.SetActive(false);
        colliderPanel.SetActive(false);
        pause = false;
    }

    public void PopUpYesPressed()
    {
        if (popUpState == 0)
        {
            if (turn == 1)
            {
                turnShower.gameObject.SetActive(false);
                timer.gameObject.SetActive(false);
                number.gameObject.SetActive(false);
                turnShower.gameObject.SetActive(true);
                timer.gameObject.SetActive(true);
                number.gameObject.SetActive(true);
            }
            else
            {
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("idle");
                timer.gameObject.GetComponent<Animator>().SetTrigger("idle");
                number.gameObject.GetComponent<Animator>().SetTrigger("idle");
            }
            GameOver();
            popUpPanel.SetActive(false);
            colliderPanel.SetActive(false);
            pause = false;
        }
        else if (popUpState == 1)
        {
            popUpPanel.SetActive(false);
            colliderPanel.SetActive(false);
            pause = false;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void GameOver()
    {
        turnShower.text = "Tap to Play";
        EnableRestartButton();        
        turn = 0;
        number.text = "0";
        isEnded = true;
        timer.text = "";
    }
    public IEnumerator IGameOver()
    {
        yield return new WaitForSeconds(2f);
        turnShower.text = "Tap to Play";
        EnableRestartButton();
        turnShower.gameObject.GetComponent<Animator>().SetTrigger("idle");
        timer.gameObject.GetComponent<Animator>().SetTrigger("idle");
        number.gameObject.GetComponent<Animator>().SetTrigger("idle");        
        isEnded = true;
        timer.text = "";
        for (int i = 0; i < wastes.Length; i++)
        {
            Destroy(wastes[i]);
        }
    }
    public void RestartGame()
    {
        Play();        
    }

    public void EnableRestartButton()
    {
        restartButton.gameObject.SetActive(true);
        subOne.gameObject.SetActive(false);
        subOne1.gameObject.SetActive(false);
        subTwo.gameObject.SetActive(false);
        subTwo1.gameObject.SetActive(false);
    }
    public void SubtractOne()
    {
        if (turn == 1 && randomNumber > 0)
        {
            randomNumber--;
            lastSubNum = 1;
            number.text = randomNumber + "";
            if (randomNumber > 0)
            {
                turn = 2;
                turnShower.text = "Player2 pick!";
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("down");
                timer.gameObject.GetComponent<Animator>().SetTrigger("up");
                number.gameObject.GetComponent<Animator>().SetTrigger("turn");
                subNum.text = "-" + lastSubNum + "";
                subNum.gameObject.GetComponent<Animator>().SetTrigger("up");
                EnableOrDisableSubButtons();
            }
            else if (randomNumber == 0)
            {
                subNum.text = "-" + lastSubNum + "";
                subNum.gameObject.GetComponent<Animator>().SetTrigger("up");
                turnShower.text = "Player2 won!";
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("down");
                timer.gameObject.GetComponent<Animator>().SetTrigger("up");
                number.gameObject.GetComponent<Animator>().SetTrigger("turn");
                DisableSubButtons();
                StartCoroutine(IShowParticles());
                //turn = 2;
            }
        }
        else if (turn == 2 && randomNumber > 0)
        {
            randomNumber--;
            lastSubNum = 1;
            number.text = randomNumber + "";
            if (randomNumber > 0)
            {
                turn = 1;
                turnShower.text = "Player1 pick!";
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("up");
                timer.gameObject.GetComponent<Animator>().SetTrigger("down");
                number.gameObject.GetComponent<Animator>().SetTrigger("turnback");
                subNum.text = "-" + lastSubNum + "";
                subNum.gameObject.GetComponent<Animator>().SetTrigger("down");
                EnableOrDisableSubButtons();
            }
            else if (randomNumber == 0)
            {
                subNum.text = "-" + lastSubNum + "";
                subNum.gameObject.GetComponent<Animator>().SetTrigger("down");
                turnShower.text = "Player1 won!";
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("up");
                timer.gameObject.GetComponent<Animator>().SetTrigger("down");
                number.gameObject.GetComponent<Animator>().SetTrigger("turnback");
                DisableSubButtons();
                StartCoroutine(IShowParticles());
                //turn = 1;
            }
        }        
    }
    public void SubtractTwo()
    {
        if (turn == 1 && randomNumber > 0)
        {
            if (randomNumber > 1)
            {
                randomNumber -= 2;
                lastSubNum = 2;
            }
            else
            {
                randomNumber--;
                lastSubNum = 1;
            }
            number.text = randomNumber + "";
            if (randomNumber > 0)
            {
                turn = 2;
                turnShower.text = "Player2 pick!";
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("down");
                timer.gameObject.GetComponent<Animator>().SetTrigger("up");
                number.gameObject.GetComponent<Animator>().SetTrigger("turn");
                subNum.text = "-" + lastSubNum + "";
                subNum.gameObject.GetComponent<Animator>().SetTrigger("up");
                EnableOrDisableSubButtons();
            }
            else
            {
                turnShower.text = "Player2 won!";
                //turn = 2;
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("down");
                timer.gameObject.GetComponent<Animator>().SetTrigger("up");
                number.gameObject.GetComponent<Animator>().SetTrigger("turn");         
                subNum.text = "-" + lastSubNum + "";
                subNum.gameObject.GetComponent<Animator>().SetTrigger("up");
                DisableSubButtons();
                StartCoroutine(IShowParticles());
            }
        }
        else if (turn == 2 && randomNumber > 0)
        {
            if (randomNumber > 1)
            {
                randomNumber -= 2;
                lastSubNum = 2;
            }
            else
            {
                randomNumber--;
                lastSubNum = 1;
            }
            number.text = randomNumber + "";
            if (randomNumber > 0)
            {
                turn = 1;
                turnShower.text = "Player1 pick!";
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("up");
                timer.gameObject.GetComponent<Animator>().SetTrigger("down");
                number.gameObject.GetComponent<Animator>().SetTrigger("turnback");
                subNum.text = "-" + lastSubNum + "";
                subNum.gameObject.GetComponent<Animator>().SetTrigger("down");
                EnableOrDisableSubButtons();
            }
            else
            {
                turnShower.text = "Player1 won!";
                //turn = 1;
                turnShower.gameObject.GetComponent<Animator>().SetTrigger("up");
                timer.gameObject.GetComponent<Animator>().SetTrigger("down");
                number.gameObject.GetComponent<Animator>().SetTrigger("turnback");
                subNum.text = "-" + lastSubNum + "";
                subNum.gameObject.GetComponent<Animator>().SetTrigger("down");
                DisableSubButtons();
                StartCoroutine(IShowParticles());                
            }
        }        
    }
    public IEnumerator IShowParticles()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            wastes[i] = Instantiate(particles);           
            int x = Random.Range(-5, 6);
            int y = 0;
            if (turn == 2)
            {
                y = Random.Range(-3,-7);
            }
            else
            {
                y = Random.Range(3, 7);
            }            
            wastes[i].transform.position += new Vector3(x, y, 0f);
            if (turn == 1)
            {
                ParticleSystem[] par = wastes[i].GetComponentsInChildren<ParticleSystem>();
                for (int j = 0; j < par.Length; j++)
                {
                    par[j].gravityModifier = -0.3f;
                }
            }
            
        }
        StartCoroutine(IGameOver());
    }
    public void Pass()
    {
        if (turn == 1 && !passTimePassed)
        {
            turn = 2;
            turnShower.text = "Player2 pick!";
            passTimePassed = true;
            turnShower.gameObject.GetComponent<Animator>().SetTrigger("down");
            timer.gameObject.GetComponent<Animator>().SetTrigger("up");
            number.gameObject.GetComponent<Animator>().SetTrigger("turn");
        }
        else if (turn == 2 && !passTimePassed)
        {
            turn = 1;
            turnShower.text = "Player1 pick!";
            passTimePassed = true;
            turnShower.gameObject.GetComponent<Animator>().SetTrigger("up");
            timer.gameObject.GetComponent<Animator>().SetTrigger("down");
            number.gameObject.GetComponent<Animator>().SetTrigger("turnback");
        }
        EnableOrDisablePassButton();
        EnableOrDisableSubButtons();
    }
    public void EnableOrDisablePassButton()
    {
        if (!passTimePassed)
        {
            pass.gameObject.SetActive(true);
        }
        else
        {
            pass.gameObject.SetActive(false);
        }
    }
    public void EnableOrDisableSubButtons()
    {
        if (turn == 1)
        {
            subOne.gameObject.SetActive(true);
            subTwo.gameObject.SetActive(true);
            subOne1.gameObject.SetActive(false);
            subTwo1.gameObject.SetActive(false);
        }
        else
        {
            subOne1.gameObject.SetActive(true);
            subTwo1.gameObject.SetActive(true);
            subOne.gameObject.SetActive(false);
            subTwo.gameObject.SetActive(false);
        }
    }
    public void DisableSubButtons()
    {
            subOne1.gameObject.SetActive(false);
            subTwo1.gameObject.SetActive(false);
            subOne.gameObject.SetActive(false);
            subTwo.gameObject.SetActive(false);
    }
}
