using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class PlayerCharacter : NetworkComponent
{
    public TextMeshProUGUI PlayerName;
    public Transform PlayerMat;
    public Material[] MColor;
    public int ColorSelected = -1;
    public string PName = "<Default>";
    public string PTeam; //Team1 or Team2
    public int PlayerNum;
    public int PlayerHp = 3;

    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {

            if (flag == "NAME")
            {
                /*
                GameObject gameObject = GameObject.Find("PlayerName" + PlayerNum);
                PlayerName = gameObject.GetComponent<TextMeshProUGUI>();
                PName = value;
                PlayerName.text = PName;*/ //no idea why it didnt work

                PName = value;
                switch (PlayerNum)
                {
                    case 1:
                        MyCore.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0)
                            .GetComponent<TextMeshProUGUI>().text = PName;
                        break;

                    case 2:
                        MyCore.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(0)
                            .GetComponent<TextMeshProUGUI>().text = PName;
                        break;

                    case 3:
                        MyCore.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0)
                            .GetComponent<TextMeshProUGUI>().text = PName;
                        break;

                    case 4:
                        MyCore.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(0)
                            .GetComponent<TextMeshProUGUI>().text = PName;
                        break;
                }
            }
            if (flag == "COLOR")
            {

                ColorSelected = int.Parse(value);

                //the "Chest" component which stores the teamcolor/Mat we are changing should always be the 0th child 
                PlayerMat = this.gameObject.transform.GetChild(0);
                PlayerMat.GetComponent<Renderer>().material = MColor[ColorSelected];
                GetComponent<SpriteRenderer>().color = MColor[ColorSelected].color;


            }
            if(flag == "NUM")
            {
                PlayerNum = int.Parse(value);
            }
            if(flag == "HIT")
            {
                PlayerHp -= int.Parse(value);
                Debug.Log(PlayerHp);
            }
        }
       
    }

    public override void NetworkedStart()
    {
    
    }

    public override IEnumerator SlowUpdate()
    {
      while(IsConnected)
        {

            if(IsServer)
            {
                if(IsDirty)
                {
                    SendUpdate("NAME", PName);
                    SendUpdate("COLOR", ColorSelected.ToString());
                    SendUpdate("HIT", PlayerHp.ToString());
                    IsDirty = false;
                }
            }
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

    //stun disables controls for a coroutine, triggers upon being hit. 

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ENEMY" && IsLocalPlayer)
        {
            //1 for default, replace with a damage modifier if we have bull variations later on 
            SendCommand("HIT", "1");
            //call stun coroutine
        }
    }
}
