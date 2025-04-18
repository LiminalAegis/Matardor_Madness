using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;

public class TeamManager : NetworkComponent
{
    public Button blue;
    public Button green;
    public Button orange;
    public Button pink;



    public override void HandleMessage(string flag, string value)
    {

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
                NPM[] npm = Object.FindObjectsOfType<NPM>();
                foreach (NPM player in npm)
                {
                    //get player color from index 
                    //
                    int blueTotal = 0;
                    int greenTotal = 0;
                    int orangeTotal = 0;
                    int pinkTotal = 0;

                    if (player.ColorSelected == 0)
                    {
                        blueTotal++;
                        checker(blueTotal, blue);
                    }
                    else if (player.ColorSelected == 1)
                    {
                        greenTotal++;
                        checker(greenTotal, green);
                    }
                    else if (player.ColorSelected == 2)
                    {
                        orangeTotal++;
                        checker(orangeTotal, orange);
                    }
                    else if (player.ColorSelected == 3)
                    {
                        pinkTotal++;
                        checker(pinkTotal, pink);
                    }
                    //Debug.Log(player.ColorSelected.ToString());
                    //Debug.Log(player.Owner);

                }

                if (IsDirty)
                {
                    IsDirty = false;
                }
               
            }
            yield return new WaitForSeconds(.1f);
        }
        
    }
    public void checker(int amount, Button type)
    {
        if (amount >= 2)
        {
          //set type.interactable to false. 
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
