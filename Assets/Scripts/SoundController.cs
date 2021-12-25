using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public AudioSource music;
    public AudioSource buttonClick;
    public bool sound = true;
    public GameObject banImage;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        GameObject mObj = GameObject.FindGameObjectWithTag("NBM");
        if (mObj == null)
        {
            mObj = GameObject.FindGameObjectWithTag("BM");
            SetSound(PlayerPrefs.GetInt("sound", 1));
        }
        music = mObj.GetComponent<AudioSource>();
        sound = (PlayerPrefs.GetInt("sound", 1) == 1) ? true : false;
        if (banImage != null)
        {
            banImage.SetActive((sound) ? false : true);
        }
        
    }
    public void SetSound()
    {
        sound = !sound;
        SetSound((sound) ? 1 : 0);
        if (banImage != null)
        {
            banImage.SetActive((sound) ? false : true);
        }
    }
    public void SetSound(int state)
    {
        AudioListener.volume = state;
        if (state == 1)
        {
            music.Play();
        }
        else
        {
            music.Stop();
        }
        PlayerPrefs.SetInt("sound", state);
        PlayerPrefs.Save();
    }

    public void PlaySound()
    {
        buttonClick.Play();
    }
}
