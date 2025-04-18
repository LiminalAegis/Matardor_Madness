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
            /*NPM[] npm = Object.FindObjectsOfType<NPM>();
            foreach (NPM player in npm)
            {
                //get player color from index 
                //
                Debug.Log(player.ColorSelected.ToString());

            }*/
            if (IsServer)
            {
               
            }
          
        }
        yield return new WaitForSeconds(.1f);
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
