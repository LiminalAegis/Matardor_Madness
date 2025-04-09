using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
    public int PlayerScore = 0;
    public GameObject PowerUp;

    //movement/action commands
    public PlayerInput MyInput;
    public InputActionAsset MyMap;

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
            if(flag == "HIT" && IsLocalPlayer)
            {
                PlayerHp = int.Parse(value);
            }
            if(flag == "FLAG" && IsLocalPlayer)
            {
                PlayerScore = int.Parse(value);
            }
        }
       
    }

    public override void NetworkedStart()
    {
        MyInput = GetComponent<PlayerInput>();
        MyMap = MyInput.actions;
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
        //Base Camera movement
        if (IsLocalPlayer)
        {
            float cameraSpeed = 5.0f;
            Vector3 offsetVector = new Vector3(0, 40, -25);
            Vector3 targetCameraPosition = this.gameObject.transform.position + offsetVector;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPosition, cameraSpeed * Time.deltaTime);
            //orient
            Camera.main.transform.LookAt(this.gameObject.transform.position);
        }
    }

    //stun disables controls for a coroutine, triggers upon being hit. 
    public IEnumerator stunPlayer()
    {
        Debug.Log("stun");
        MyInput.DeactivateInput();
        //MyInput.PassivateInput();
        //disable input
        yield return new WaitForSeconds(2f);
        Debug.Log("Move again");
        MyInput.ActivateInput();
    }

    public IEnumerator playerDeath()
    {
        //wait until we have the actual map and camera set up for this
        //fix camera in place (or switch to a different camera?)
        //teleport player under the map
        yield return new WaitForSeconds(5f);
    }

    public void OnTriggerEnter(Collider other)
    {

        //think we should handle collision on server - Dominic
        if (other.gameObject.tag == "ENEMY" && IsLocalPlayer)
        {
            //if bull mask is powerup
            

            PlayerHp -= 1;
            SendCommand("HIT", PlayerHp.ToString());
            StartCoroutine(stunPlayer());
            //call stun coroutine
        }
        //NOT stealing, direct ground pickup 
        if(other.gameObject.tag == "FLAG" && IsLocalPlayer)
        {
            //send score command 
            PlayerScore += 1;
            SendCommand("FLAG", PlayerScore.ToString()); 
            Destroy(other.gameObject);
            //Increase cape glow
            //note that score should also update aggro calculation that should be handled elsewhere, however. 

        }

        //server version for mask
        /*
        if (PowerUp.GetComponent<MaskScript>() != null)
        {
            other.gameObject.GetComponent<Bull>().StartCoroutine(Stunned());
            MyCore.NetDestroyObject(PowerUp.GetComponent<NetworkID>().NetId);
            return;
        }*/

    }

}
