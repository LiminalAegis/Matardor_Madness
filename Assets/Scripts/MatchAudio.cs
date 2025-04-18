using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class MatchAudio : MonoBehaviour
{
    public AudioClip matchMusic, finalMin, tackle, whistle, death, score, stunned, button, flare;
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


        }
    }

    public void SFX(int action)
    {
        AudioClip temp = button;
        bool fadeOut = false;
        switch (action)
        {
            case 0: //tackle
                temp = tackle;
                break;
            case 1: //whistle
                temp = whistle;
                break;
            case 2: //death
                temp = death;
                break;
            case 3: //score
                temp = score;
                fadeOut = true;
                break;
            case 4: //stunned
                temp = stunned;
                break;
            case 5: //flare
                temp = flare;
                break;
        }
        if (temp != button)
        {
            if (fadeOut)
            {
                sfx.PlayOneShot(temp);
                StartCoroutine(FadeOut("sfx", 4.5f));
            }
            else
            {
                sfx.PlayOneShot(temp);
            }
        }
    }

    public IEnumerator FadeOut(string sourceName, float length)
    {
        
        float temp = 0f, maxvol, volstep;
        switch (sourceName)
        {
            case "sfx":
                
                maxvol = sfx.volume;
                volstep = maxvol / length * 0.1f;
                while (true)
                {
                    temp += 0.1f;
                    if (temp >= length)
                    {
                        sfx.Stop();
                        sfx.volume = maxvol;
                        break;
                    }

                    sfx.volume -= volstep;
                    yield return new WaitForSeconds(0.1f);
                }
                break;
            case "music":
                maxvol = music.volume;
                volstep = maxvol / length / 0.1f;
                while (true)
                {
                    temp += 0.1f;
                    if (temp >= length)
                    {
                        music.Stop();
                        music.volume = maxvol;
                        break;
                    }

                    music.volume -= volstep;
                    yield return new WaitForSeconds(0.1f);
                }
                break;
        }
    }

    public void Music(int trigger)
    {
        switch (trigger)
        {
            case 0: //normal match music
                break;
            case 1: //last minute rush
                music.clip = finalMin;
                music.Play();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
