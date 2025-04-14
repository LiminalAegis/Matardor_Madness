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
    public Image puSlot, xIcon, sewingKit, mask, food;

    //mirrors of game master vals
    public float totalTime, currentTime;

    public IEnumerator StartMatchUI(float matchLength)
    {
        totalTime = matchLength;
        currentTime = matchLength;
        timeVal.text = currentTime.ToString("N").Replace(".", ":");
        //countdown timer here
        yield return new WaitForSeconds(3);
        //trigger the GameRunningUI coroutine here
        StartCoroutine(GameRunningUI());
    }

    public IEnumerator GameRunningUI()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            currentTime -= 0.1f;
            timeVal.text = currentTime <= 0 ? "00:00" : currentTime.ToString("N").Replace(".", ":");
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
                puSlot =xIcon;
                break;
            case 1:
                puSlot = sewingKit;
                break;
            case 2:
                puSlot = mask;
                break;
            case 3:
                puSlot = food;
                break;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //grab the game master from the scene
        gameMaster = FindAnyObjectByType<GameMaster>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
