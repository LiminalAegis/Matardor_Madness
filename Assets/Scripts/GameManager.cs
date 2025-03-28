using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class GameMaster : NetworkComponent
{
    public bool GameStarted = false;
    public bool GameOver = false;

    public Transform[] SpawnPoints;

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



            foreach (NPM player in players)
            {

                GameObject character = MyCore.NetCreateObject(
                    Mathf.Max(0, 0),
                    player.Owner,
                    SpawnPoints[player.Owner].position,
                    Quaternion.identity
                );

                PlayerCharacter pc = character.GetComponent<PlayerCharacter>();
                pc.PName = player.PName;
                pc.ColorSelected = player.ColorSelected;

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
