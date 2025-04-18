using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TeamManager : NetworkComponent
{
    public Button blue;
    public Button green;
    public Button orange;
    public Button pink;

    public bool Team1Assigned;
    public int Team1Color;


    public override void HandleMessage(string flag, string value)
    {
        if(IsClient)
        {
            if(flag == "TEAMSELECT")
            {
                Debug.Log("Is this flag even getting called");

                int teamColor = int.Parse(value); 

                //spaghetti but it works
                //set the given team as uninterractable.
                if(teamColor == 0)
                {
                    blue.interactable = false;
                }
                else if (teamColor == 1)
                {
                    green.interactable = false;
                }
                else if (teamColor == 2)
                {
                    orange.interactable = false;
                }
                else if (teamColor == 3)
                {
                    pink.interactable = false;
                }

                if(Team1Assigned == false)
                {
                    Team1Color = int.Parse(value);
                    Team1Assigned = true;
                }
            }
        }
    }

    public override void NetworkedStart()
    {
       
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                int blueTotal = 0;
                int greenTotal = 0;
                int orangeTotal = 0;
                int pinkTotal = 0;

                NPM[] npm = Object.FindObjectsOfType<NPM>();
                foreach (NPM player in npm)
                {
                    //get player color from index 
                    //
                    if (Team1Assigned && player.ColorSelected == Team1Color)
                    {
                        player.NPTeam = "Team1";
                    }
                    else if (Team1Assigned && player.ColorSelected != Team1Color)
                    {
                        player.NPTeam = "Team2";
                    }

                    if (player.ColorSelected == 0)
                    {
                        blueTotal++;
                        checker(blueTotal, 0);
                    }
                    else if (player.ColorSelected == 1)
                    {
                        greenTotal++;
                        checker(greenTotal, 1);
                    }
                    else if (player.ColorSelected == 2)
                    {
                        orangeTotal++;
                        checker(orangeTotal, 2);
                    }
                    else if (player.ColorSelected == 3)
                    {
                        pinkTotal++;
                        checker(pinkTotal, 3);
                    }
                    //Debug.Log(player.ColorSelected.ToString());
                    //Debug.Log(player.Owner);

                }
               
            }
            yield return new WaitForSeconds(.1f);
        }
        
    }
    public void checker(int amount, int type)
    {
        //amount is 1 for debugging. Set it to 2 in the actual game. 
        if (amount >= 1)
        {
            //set the corresponding button to be un-interactable
            SendUpdate("TEAMSELECT", type.ToString());
            Debug.Log(type);
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
