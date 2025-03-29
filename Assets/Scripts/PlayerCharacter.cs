using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
public class PlayerCharacter : NetworkComponent
{
    public Text PlayerName;
    public Transform PlayerMat;
    public Material[] MColor;
    public int ColorSelected = -1;
    public string PName = "<Default>";


    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "NAME")
            {
                PName = value;
                PlayerName.text = PName;
            }
            if (flag == "COLOR")
            {
                ColorSelected = int.Parse(value);

                //the "Chest" component which stores the teamcolor/Mat we are changing should always be the 0th child 
                PlayerMat = this.gameObject.transform.GetChild(0);
                //PlayerMat.GetComponent<Renderer>().material = MColor[ColorSelected];
                GetComponent<SpriteRenderer>().color = MColor[ColorSelected].color;

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
}
