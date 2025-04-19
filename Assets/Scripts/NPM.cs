﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;


public class NPM : NetworkComponent
{
    public string PName;
    public string NPTeam;
    public bool IsReady;
    public int ColorSelected = -1;
    public int prevColorSelected = -1;
    public int CharSelected;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "READY")
        {
            IsReady = bool.Parse(value);
            if(IsServer)
            {
                SendUpdate("READY", value);
            }
        }

        if(flag == "NAME")
        {
            PName = value;
            if(IsServer)
            {
                SendUpdate("NAME", value);
            }
        }

        if(flag =="COLOR")
        {
            ColorSelected = int.Parse(value);
            if(IsServer)
            {
                SendUpdate("COLOR", value);
            }
        }

        if(flag == "CHAR")
        {
            CharSelected = int.Parse(value);
            if(IsServer)
            {
                SendUpdate("CHAR", value);
            }
        }
    }
    public void UI_Ready(bool r)
    {
        if(IsLocalPlayer)
        {
            SendCommand("READY", r.ToString());
            
        }
    }
    public override void NetworkedStart()
    {
       if(!IsLocalPlayer)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void UI_NameInput(string s)
    {
        if(IsLocalPlayer)
        {
            SendCommand("NAME", s);
        }

    }
    public void UI_ColorInput(int c)
    {
        if(IsLocalPlayer)
        {
            SendCommand("COLOR", c.ToString());
            //Debug.Log(c.ToString());
        }
    }

    public void UI_CharInput(int c)
    {
        if(IsLocalPlayer)
        {
            SendCommand("CHAR", c.ToString());
        }
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
                    SendUpdate("CHAR", CharSelected.ToString());

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
