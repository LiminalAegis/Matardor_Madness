using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using TMPro;

public class GameMaster : NetworkComponent
{
    public bool GameStarted = false;
    public bool GameOver = false;

    public Transform[] SpawnPoints;
    public GameObject[] PlayerNames;
    public GameObject[] PlayerScores;

    //gameloop variables
    public int Team1Score = 0;
    public int Team2Score = 0;
    public int WinScore = 20;
    public int WinTeam = 0; //1 = team1, 2 = team2, 3 = tie
    public float RoundTimer = 180; //3 minutes
    public float GameStartTime;



    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "GAMESTART")
            {
                GameStarted = true;

                NPM[] npm = Object.FindObjectsOfType<NPM>();
                foreach (NPM player in npm)
                {
                    //player.enabled = false;
                    //this can cause problems, so what's even better is to just disable the canvas [usually GetChild(0) or GetComponent<Renderer>()]

                    player.transform.GetChild(0).gameObject.SetActive(false);

                }
                
                StartCoroutine(DisplayScoreboard());
                StartCoroutine(AutoDisconnect());
                 // for testing

            }
            if (flag == "GAMEOVER")
            {
                //DisplayScoreboard(); //make sure to remove the coroutine, make it a normal function for real game

            }
        }

        if(IsServer)
        {
            //run flag for every point earned
            if (flag == "SCORE")
            {
                if (value == "Team1")
                {
                    Team1Score++;
                }
                if (value == "Team2")
                {
                    Team2Score++;
                }
            }
        }
        

    }

    public override void NetworkedStart()
    {
        
    }
    public IEnumerator AutoDisconnect()
    {
        yield return new WaitForSeconds(10);
        NetworkCore nc = GameObject.FindObjectOfType<NetworkCore>();
        nc.UI_Quit();
        
    }
    public IEnumerator DisplayScoreboard()
    {
        yield return new WaitForSeconds(5);
        NPM[] npm = Object.FindObjectsOfType<NPM>();

        
        MyCore.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);

        int rand = Random.Range(0, 100);
        MyCore.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Score: " + rand;
        rand = Random.Range(0, 100);
        MyCore.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Score: " + rand;
        rand = Random.Range(0, 100);
        MyCore.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Score: " + rand;
        rand = Random.Range(0, 100);
        MyCore.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Score: " + rand;

        /*
        int tempPLayerLoop = 0;
        foreach (GameObject i in PlayerScores)
        {
            
            int rand = Random.Range(0, 100);
            PlayerScores[tempPLayerLoop].GetComponent<TextMeshProUGUI>().text = "Score: " + rand;
            tempPLayerLoop++;
        }*/
        //idk why i couldnt get this to work
    }

    public override IEnumerator SlowUpdate()
    {
        if (IsServer)
        {
            NPM[] players;
            bool allReady = false;

            do
            {
                players = FindObjectsOfType<NPM>();
                allReady = players.Length >= 2; // at least 2 players
                //for matador, probably should make 4 players

                foreach (NPM player in players)
                {
                    if (!player.IsReady)
                    {
                        allReady = false;
                    }
                }

                yield return new WaitForSeconds(1);
            } while (!allReady);


            int tempLoopNum = 0;
            foreach (NPM player in players)
            {
                tempLoopNum++;

                GameObject character = MyCore.NetCreateObject(
                    0,
                    player.Owner,
                    SpawnPoints[player.Owner].position,
                    Quaternion.identity
                );

                PlayerCharacter pc = character.GetComponent<PlayerCharacter>();
                pc.PName = player.PName;
                pc.ColorSelected = player.ColorSelected;
                pc.PlayerNum = tempLoopNum;

                pc.SendUpdate("NUM", pc.PlayerNum.ToString());
                pc.SendUpdate("NAME", pc.PName);
                


            }

            //set start time
            GameStartTime = Time.time;

            SendUpdate("GAMESTART", "1");
            MyCore.NotifyGameStart();

            while (!GameOver)
            {
                // Game logic here
                


                //win conditions

                //win by WinScore
                if(Team1Score == WinScore || Team2Score == WinScore)
                {
                    if(Team1Score == WinScore && Team2Score == WinScore)
                    {
                        WinTeam = 3;
                    }
                    else if(Team1Score == WinScore)
                    {
                        WinTeam = 1;
                    }
                    else if(Team2Score == WinScore)
                    {
                        WinTeam = 2;
                    }
                   
                    GameOver = true;
                }

                //Win by Time

                if(GameStartTime + RoundTimer >= Time.time)
                {
                    if (Team1Score > Team2Score)
                    {
                        WinTeam = 1;
                    }
                    else if (Team2Score > Team1Score)
                    {
                        WinTeam = 2;
                    }
                    else
                    {
                        WinTeam = 3;
                    }
                    GameOver = true;
                }

                yield return new WaitForSeconds(0.1f);
            }

            SendUpdate("GAMEOVER", "1");
            StartCoroutine(MyCore.DisconnectServer());


            yield return new WaitForSeconds(.1f);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
