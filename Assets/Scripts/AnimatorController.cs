using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorController : MonoBehaviour
{
    public AnimatorPrefab[] animators;

    public bool settingsOpen = true;

    public bool playGame = false;

    public GameObject gameSelection;

    public void MoveSettings()
    {
        for (int i = 0; i < 5; i++)
        {
            animators[i].animator.SetBool(animators[i].animations[0], settingsOpen);
        }
        settingsOpen = !settingsOpen;
    }

    public void PlayAnim(string obj, bool state)
    {
        foreach (var animName in animators)
        {
            if (animName.name == obj)
            {
                animName.animator.SetBool(animName.animations[0], state);
            }
        }
    }

    public void PlayGame()
    {
        StartCoroutine(IPlayGame(.8f));
    }

    IEnumerator IPlayGame(float duration)
    {
        if (!gameSelection.activeSelf)
        {
            gameSelection.SetActive(!playGame);
        }
        PlayAnim("GameSelection", playGame);
        if (!settingsOpen)
        {
            MoveSettings();
        }
        yield return new WaitForSeconds(duration);
        Color imgColor = new Color();
        imgColor.r = 1f;
        imgColor.b = 1f;
        imgColor.g = 1f;
        imgColor.a = .6f;
        gameSelection.GetComponent<Image>().color = imgColor;
        gameSelection.SetActive(!playGame);
        playGame = !playGame;
    }
}
