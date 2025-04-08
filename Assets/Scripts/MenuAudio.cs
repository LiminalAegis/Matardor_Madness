using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    public AudioClip menuMusic, buttonAffirm, buttonDeny;
    public AudioSource music, sfx;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] temp = FindObjectsOfType<AudioSource>();
        if (temp.Length > 0)
        {
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
            if (!music.isPlaying)
            {
                music.clip = menuMusic;
                music.Play();
            }
        }
    }

    public void ButtonHit(bool affirm)
    {
        if (affirm)
        {
            sfx.PlayOneShot(buttonAffirm);
        }
        else
        {
            sfx.PlayOneShot(buttonDeny);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
