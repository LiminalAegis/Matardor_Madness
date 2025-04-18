using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    //dynamic
    public GameMaster gameMaster;

    //set in editor
    public TextMeshProUGUI timeVal, team1Points, team2Points;
    public Image puSlot;
    public Sprite xIcon, sewingKit, mask, food, glove, flare;
    public Image h1, h2, h3;
    MatchAudio matchAudio;
    bool lastMin = false;

    //mirrors of game master vals
    public float totalTime, currentTime;

    public IEnumerator StartMatchUI(float matchLength)
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(true);
        totalTime = matchLength;
        currentTime = matchLength;
        if (matchLength < 60) lastMin = true;
        timeVal.text = currentTime.ToString("N").Replace(".", ":");
        matchAudio.Music(0);
        //countdown timer here
        yield return new WaitForSeconds(3);
        //trigger the GameRunningUI coroutine here
        StartCoroutine(GameRunningUI());
    }

    public IEnumerator GameRunningUI()
    {
        
        if (matchAudio != null)
        {
            matchAudio.Music(0);
        }
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            currentTime -= 0.1f;
            timeVal.text = currentTime <= 0 ? "00:00" : currentTime.ToString("N").Replace(".", ":");
            if (currentTime <= 60 && !lastMin)
            {
                lastMin = true;
                
            }
            if(currentTime <= 0)
            {
                //game end UI coroutine start here
                StopCoroutine(nameof(GameRunningUI));
            }
        }
    }

    
    public void ScoreUpdate(int team, int score)
    {
        switch (team)
        {
            case 1:
                team1Points.text = score.ToString();
                break;
            case 2:
                team2Points.text = score.ToString();
                break;
        }
    }

    public void PowerUpVisual(int image)
    {
        switch (image)
        {
            case 0: // no PU
                puSlot.sprite = xIcon;
                break;
            case 1:
                puSlot.sprite = sewingKit;
                break;
            case 2:
                puSlot.sprite = mask;
                break;
            case 3:
                puSlot.sprite = food;
                break;
            case 4:
                puSlot.sprite = glove;
                break;
            case 5:
                puSlot.sprite = flare;
                break;

        }
    }

    public void HealthChange(int state)
    {
        switch (state)
        {
            case 3:
                h1.color = Color.white;
                h2.color = Color.white;
                h3.color = Color.white;
                break;
            case 2:
                h1.color = Color.white;
                h2.color = Color.white;
                h3.color = Color.gray;
                break;
            case 1:
                h1.color = Color.white;
                h2.color = Color.gray;
                h3.color = Color.gray;
                break;
            case 0:
                h1.color = Color.gray;
                h2.color = Color.gray;
                h3.color = Color.gray;
                break;
        }
        Debug.Log("Health changed to"+state);
    }

    // Start is called before the first frame update
    void Start()
    {
        //grab the game master from the scene
        gameMaster = FindAnyObjectByType<GameMaster>();
        matchAudio = FindObjectOfType<MatchAudio>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
