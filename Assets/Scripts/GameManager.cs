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

    public GameObject ScoreBoard;



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
                ScoreBoard.SetActive(true);

            }

            //flags for populating scoreboard, pass in "value, playernum"
            if(flag == "SBNAME")
            {
                string[] nameInfo = value.Split(',');
                int num = int.Parse(nameInfo[1]);
                SBNames[num].GetComponent<TextMeshProUGUI>().text = nameInfo[0];
            }
            if(flag == "SBSCORE")
            {
                string[] scoreInfo = value.Split(',');
                int num = int.Parse(scoreInfo[1]);
                SBScore[num].GetComponent<TextMeshProUGUI>().text = "Score: " + scoreInfo[0];
            }
            if(flag == "SBPF")
            {
                string[] pfInfo = value.Split(',');
                int num = int.Parse(pfInfo[1]);
                SBPF[num].GetComponent<TextMeshProUGUI>().text = "PF: " + pfInfo[0];
            }
            if(flag == "SBCF")
            {
                string[] cfInfo = value.Split(',');
                int num = int.Parse(cfInfo[1]);
                SBCF[num].GetComponent<TextMeshProUGUI>().text = "CF: " + cfInfo[0];
            }
            //team scores: "score, int" int should be 0 if lost, 1 if won, 3 if tie
            if(flag == "TEAM1SCORE")
            {
                string[] t1sInfo = value.Split(",");
                int num = int.Parse(t1sInfo[1]);
                if (num == 1)
                {
                    SBTeamScore[0].GetComponent<TextMeshProUGUI>().text = "Team 1 Won with " + t1sInfo[0] + " Points";
                }
                else if (num == 0)
                {
                    SBTeamScore[0].GetComponent<TextMeshProUGUI>().text = "Team 1 Lost with " + t1sInfo[0] + " Points";
                }
                else if (num == 3)
                {
                    SBTeamScore[0].GetComponent<TextMeshProUGUI>().text = "Team 1 Tied with " + t1sInfo[0] + " Points";
                }

            }
            if(flag == "TEAM2SCORE")
            {
                string[] t1sInfo = value.Split(",");
                int num = int.Parse(t1sInfo[1]);
                if (num == 1)
                {
                    SBTeamScore[1].GetComponent<TextMeshProUGUI>().text = "Team 2 Won with " + t1sInfo[0] + " Points";
                }
                else if (num == 0)
                {
                    SBTeamScore[1].GetComponent<TextMeshProUGUI>().text = "Team 2 Lost with " + t1sInfo[0] + " Points";
                }
                else if (num == 3)
                {
                    SBTeamScore[1].GetComponent<TextMeshProUGUI>().text = "Team 2 Tied with " + t1sInfo[0] + " Points";
                }
            }
            if(flag == "")
            {
                string[] temp = value.Split(",");
                PlayerUI ui = FindObjectOfType<PlayerUI>();
                ui.ScoreUpdate(1, int.Parse(temp[0]));
                ui.ScoreUpdate(2, int.Parse(temp[1]));
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
                    ;
                }
                SendUpdate("SCOREDISPLAY", Team1Score.ToString() + "," + Team2Score.ToString());
            }
        }
        

    }

    public override void NetworkedStart()
    {
        
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
                allReady = players.Length >= 4; // at least 2 players
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


            int team1num = 0;
            int team2num = 0;
            foreach (NPM player in players)
            {

                GameObject character = MyCore.NetCreateObject(
                    0,
                    player.Owner,
                    SpawnPoints[player.Owner].position,
                    Quaternion.identity
                );
                

                PlayerCharacter pc = character.GetComponent<PlayerCharacter>();
                pc.PName = player.PName;
                pc.ColorSelected = player.ColorSelected;

                //assign team
                pc.PTeam = player.NPTeam;
                Debug.Log("Assigned Player Team: " + pc.PTeam);
                Debug.Log("Assigned Player Team: " + player.NPTeam);
                if (pc.PTeam == "Team1")
                {
                    if(team1num == 0)
                    {
                        pc.PlayerNum = 0;
                        pc.transform.position = SpawnPoints[pc.PlayerNum].position;
                        team1num++;
                        Debug.Log("NUMBER: " + pc.PlayerNum);
                    }
                    else if(team1num == 1)
                    {
                        pc.PlayerNum = 1;
                        pc.transform.position = SpawnPoints[pc.PlayerNum].position;
                        team1num++;
                        Debug.Log("NUMBER: " + pc.PlayerNum);
                    }
                    
                }
                else if (pc.PTeam == "Team2")
                {
                    if (team2num == 0)
                    {
                        pc.PlayerNum = 2;
                        pc.transform.position = SpawnPoints[pc.PlayerNum].position;
                        team2num++;
                        Debug.Log("NUMBER: " + pc.PlayerNum);
                    }
                    else if (team2num == 1)
                    {
                        pc.PlayerNum = 3;
                        pc.transform.position = SpawnPoints[pc.PlayerNum].position;
                        team2num++;
                        Debug.Log("NUMBER: " + pc.PlayerNum);
                    }
                }

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

                //TESTING
                /*
                int rand = Random.Range(0, 3);
                if(rand == 0)
                {
                    Team1Score = WinScore;
                }
                if(rand == 1)
                {
                    Team2Score = WinScore;
                }
                if (rand == 2)
                {
                    Team1Score = WinScore;
                    Team2Score = WinScore;
                }*/

                //win conditions

                //win by WinScore
                if (Team1Score >= WinScore || Team2Score >= WinScore)
                {
                    if(Team1Score >= WinScore && Team2Score >= WinScore)
                    {
                        WinTeam = 3;
                    }
                    else if(Team1Score >= WinScore)
                    {
                        WinTeam = 1;
                    }
                    else if(Team2Score >= WinScore)
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

            DisplayScoreboard();
            yield return new WaitForSeconds(10);
            SendUpdate("GAMEOVER", "1");
            StartCoroutine(MyCore.DisconnectServer());


            yield return new WaitForSeconds(.1f);
        }
    }

    public void DisplayScoreboard()
    {
        ScoreBoard.SetActive(true);
        SendUpdate("SB", "1");

        //set it up with GM as canvas owner
        PlayerCharacter[] PCs = FindObjectsOfType<PlayerCharacter>();

        foreach (PlayerCharacter PC in PCs)
        {
            //int num = PC.PlayerNum - 1;
            int num = PC.PlayerNum;
            Debug.Log("PlayerNum: " + num.ToString());
            //Name
            SBNames[num].GetComponent<TextMeshProUGUI>().text = PC.PName;
            SendUpdate("SBNAME", PC.PName + "," + num.ToString());
            //Score
            SBScore[num].GetComponent<TextMeshProUGUI>().text = PC.PlayerScoreTotal.ToString();
            SendUpdate("SBSCORE", PC.PlayerScoreTotal.ToString() + "," + num.ToString());
            //PF
            SBPF[num].GetComponent<TextMeshProUGUI>().text = PC.PlayerPF.ToString();
            SendUpdate("SBPF", PC.PlayerPF.ToString() + "," + num.ToString());
            //CF
            SBCF[num].GetComponent<TextMeshProUGUI>().text = PC.PlayerCF.ToString();
            SendUpdate("SBCF", PC.PlayerCF.ToString() + "," + num.ToString());

            //team scores
            //switch on teamwin
            switch(WinTeam)
            {
                case 1:
                    SBTeamScore[0].GetComponent<TextMeshProUGUI>().text = "Team 1 Won with " + Team1Score + " Points";
                    SendUpdate("TEAM1SCORE", Team1Score.ToString() + "," + "1");
                    SBTeamScore[1].GetComponent<TextMeshProUGUI>().text = "Team 2 Lost with " + Team2Score + " Points";
                    SendUpdate("TEAM2SCORE", Team2Score.ToString() + "," + "0");
                    break;
                case 2:
                    SBTeamScore[0].GetComponent<TextMeshProUGUI>().text = "Team 1 Lost with " + Team1Score + " Points";
                    SendUpdate("TEAM1SCORE", Team1Score.ToString() + "," + "0");
                    SBTeamScore[1].GetComponent<TextMeshProUGUI>().text = "Team 2 Won with " + Team2Score + " Points";
                    SendUpdate("TEAM2SCORE", Team2Score.ToString() + "," + "1");
                    break;
                case 3:
                    SBTeamScore[0].GetComponent<TextMeshProUGUI>().text = "Team 1 Tied with " + Team1Score + " Points";
                    SendUpdate("TEAM1SCORE", Team1Score.ToString() + "," + "3");
                    SBTeamScore[1].GetComponent<TextMeshProUGUI>().text = "Team 2 Tied with " + Team2Score + " Points";
                    SendUpdate("TEAM2SCORE", Team2Score.ToString() + "," + "3");
                    break;
            }
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
