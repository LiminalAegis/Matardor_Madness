using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public AudioSource music, sfx;

    // Start is called before the first frame update
    void Start()
    {
        if (this.CompareTag("Audio")) DontDestroyOnLoad(this.gameObject);
        else
        {
            AudioSource[] temp = FindObjectsOfType<AudioSource>();
            if (temp[0].name == "Music")
            {
                music = temp[0];
                sfx = temp[1];
            }
            else
            {
                sfx = temp[0];
                music = temp[1];

            }
        }
    }

    public void ChangeMusicVolume(float val)
    {
        music.volume = val;
    }

    public void ChangeSFXVolume(float val)
    {
        sfx.volume = val;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
