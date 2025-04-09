using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MatchAudio : MonoBehaviour
{
    public AudioClip matchMusic, tackle, damage, death, score, stunned, button;
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
        switch (action)
        {
            case 0: //tackle
                temp = tackle;
                break;
            case 1: //damage
                temp = damage;
                break;
            case 2: //death
                temp = death;
                break;
            case 3: //score
                temp = score;
                break;
            case 4: //stunned
                temp = stunned;
                break;
        }
        if (temp != button)
        {
            sfx.PlayOneShot(temp);
        }
    }

    public void Music(int trigger)
    {
        switch (trigger)
        {
            case 0: //normal match music
                break;
            case 1: //last minute rush
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
