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
    

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "GAMESTART" && IsClient)
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
        
        int tempPLayerLoop = 0;
        foreach (GameObject i in PlayerScores)
        {
            tempPLayerLoop++;
            int rand = Random.Range(0, 100);
            PlayerScores[tempPLayerLoop].GetComponent<TextMeshProUGUI>().text = "Score: " + rand;
        }
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
                    Mathf.Max(0, 0),
                    player.Owner,
                    SpawnPoints[player.Owner].position,
                    Quaternion.identity
                );

                PlayerCharacter pc = character.GetComponent<PlayerCharacter>();
                pc.PName = player.PName;
                pc.ColorSelected = player.ColorSelected;
                pc.PlayerNum = tempLoopNum;

                pc.SendUpdate("SET_NAME", pc.PName);
                pc.SendUpdate("SET_COLOR", pc.ColorSelected.ToString());


            }

            SendUpdate("GAMESTART", "1");
            MyCore.NotifyGameStart();

            while (!GameOver)
            {
                // Game logic here
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
