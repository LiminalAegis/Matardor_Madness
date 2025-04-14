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
    //defunct?
    public GameObject[] PlayerNames;
    public GameObject[] PlayerScores;

    //gameloop variables
    public int Team1Score = 0;
    public int Team2Score = 0;
    public int WinScore = 20;
    public int WinTeam = 0; //1 = team1, 2 = team2, 3 = tie
    public float RoundTimer = 180; //3 minutes
    public float CurrentRoundTime;
    public float GameStartTime;

    public GameObject Timer;

    //Scoreboard variables
    //each of these should have 4 elements for 4 players
    public GameObject[] SBNames;
    public GameObject[] SBScore;
    public GameObject[] SBPF;
    public GameObject[] SBCF;
    public GameObject[] SBTeamScore; //should be 2



    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "LOBBYEND")
            {

                NPM[] npm = Object.FindObjectsOfType<NPM>();
                foreach (NPM player in npm)
                {
                    //player.enabled = false;
                    //this can cause problems, so what's even better is to just disable the canvas [usually GetChild(0) or GetComponent<Renderer>()]

                    player.transform.GetChild(0).gameObject.SetActive(false);

                }
                 // for testing
            }
            if(flag == "GAMESTART")
            {
                GameStarted = true;
            }
            if (flag == "TIMER")
            {
                //set the timer UI
                PlayerUI UI = FindObjectOfType<PlayerUI>();
                UI.StartCoroutine(UI.StartMatchUI(RoundTimer));
            }
            if (flag == "SB")
            {
                this.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

            }

            //flags for populating scoreboard, pass in "value, playernum"
            if(flag == "SBNAME")
            {
                string[] nameInfo = value.Split(',');
                int num = int.Parse(nameInfo[1]);
                SBNames[num].GetComponent<TextMeshPro>().text = nameInfo[0];
            }
            if(flag == "SBSCORE")
            {
                string[] scoreInfo = value.Split(',');
                int num = int.Parse(scoreInfo[1]);
                SBScore[num].GetComponent<TextMeshPro>().text = scoreInfo[0];
            }
            if(flag == "SBPF")
            {
                string[] pfInfo = value.Split(',');
                int num = int.Parse(pfInfo[1]);
                SBPF[num].GetComponent<TextMeshPro>().text = pfInfo[0];
            }
            if(flag == "SBCF")
            {
                string[] cfInfo = value.Split(',');
                int num = int.Parse(cfInfo[1]);
                SBCF[num].GetComponent<TextMeshPro>().text = cfInfo[0];
            }
            //team scores: "score, int" int should be 0 if lost, 1 if won, 3 if tie
            if(flag == "TEAM1SCORE")
            {
                string[] t1sInfo = value.Split(",");
                int num = int.Parse(t1sInfo[1]);
                if (num == 1)
                {
                    SBTeamScore[0].GetComponent<TextMeshPro>().text = "Team 1 Won with " + t1sInfo[0] + " Points";
                }
            }
            if(flag == "TEAM2SCORE")
            {

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
        yield return new WaitForSeconds(100);
        NetworkCore nc = GameObject.FindObjectOfType<NetworkCore>();
        nc.UI_Quit();
        
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
            CurrentRoundTime = Time.time;

            PowerUpSpawner PUSpawner = FindObjectOfType<PowerUpSpawner>();
            PUSpawner.Started = true;
            SendUpdate("TIMER", RoundTimer.ToString());
            SendUpdate("LOBBYEND", "1");

            yield return new WaitForSeconds(3);
            SendUpdate("GAMESTART", "1");
            
            MyCore.NotifyGameStart();

            while (!GameOver)
            {

                //handle time updates
                CurrentRoundTime = Time.time;

                // Game logic here



                //win conditions

                //win by WinScore
                if (Team1Score == WinScore || Team2Score == WinScore)
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

                if(GameStartTime + RoundTimer <= Time.time)
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

    public void DisplayScoreboard()
    {
        //set it up with GM as canvas owner
        PlayerCharacter[] PCs = FindObjectsOfType<PlayerCharacter>();

        foreach (PlayerCharacter PC in PCs)
        {
            int num = PC.PlayerNum - 1;
            //Name
            SBNames[num].GetComponent<TextMeshPro>().text = PC.PName;
            SendUpdate("SBNAME", PC.PName + "," + num.ToString());
            //Score
            SBScore[num].GetComponent<TextMeshPro>().text = PC.PlayerScoreTotal.ToString();
            SendUpdate("SBSCORE", PC.PlayerScoreTotal.ToString() + "," + num.ToString());
            //PF
            SBPF[num].GetComponent<TextMeshPro>().text = PC.PlayerPF.ToString();
            SendUpdate("SBPF", PC.PlayerPF.ToString() + "," + num.ToString());
            //CF
            SBCF[num].GetComponent<TextMeshPro>().text = PC.PlayerCF.ToString();
            SendUpdate("SBCF", PC.PlayerCF.ToString() + "," + num.ToString());

            //team scores
            //switch on teamwin
            switch(WinTeam)
            {
                case 1:
                    SBTeamScore[0].GetComponent<TextMeshPro>().text = "Team 1 Won with " + Team1Score + " Points";
                    SendUpdate("TEAM1SCORE", Team1Score.ToString() + "," + "1");
                    SBTeamScore[1].GetComponent<TextMeshPro>().text = "Team 2 Lost with " + Team2Score + " Points";
                    SendUpdate("TEAM2SCORE", Team1Score.ToString());
                    break;
                case 2:
                    SBTeamScore[0].GetComponent<TextMeshPro>().text = "Team 1 Lost with " + Team1Score + " Points";
                    SBTeamScore[1].GetComponent<TextMeshPro>().text = "Team 2 Won with " + Team2Score + " Points";
                    break;
                case 3:
                    SBTeamScore[0].GetComponent<TextMeshPro>().text = "Team 1 Tied with " + Team1Score + " Points";
                    SBTeamScore[1].GetComponent<TextMeshPro>().text = "Team 2 Tied with " + Team2Score + " Points";
                    break;
            }
        }
        this.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        SendUpdate("SB", "1");
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
