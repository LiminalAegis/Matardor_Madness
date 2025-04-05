using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NETWORK_ENGINE;

public class PlayerMovement : NetworkComponent
{
    public Rigidbody rb;
    //need the animation here too
    
    float speed = 5;
    public float directionUD, directionLR;
    public bool isMoving = false, isStealing = false, isPowerUp = false;
    public bool cooldown = false, itemCooldown = false;

    public AudioClip stealSFX, powerUpGetSFX;
    public AudioSource SFX;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "MOVE")
        {
            string[] keysDown = value.Trim('(').Trim(')').Split(',');
            float upDown = float.Parse(keysDown[1]);
            float leftRight = float.Parse(keysDown[0]);
            if (IsServer)
            {
                isMoving = true;
                directionLR = leftRight;
                directionUD = upDown;
            }
        }
        if (flag == "STOP")
        {
            if (IsServer)
            {
                isMoving = false;
            }
        }
        if (flag == "STEAL")
        {
            if (IsServer)
            {
                cooldown = true;
                //StartCoroutine(Cooldown());
                SendUpdate("STEAL", cooldown.ToString());
            }
            if (IsClient)
            {
                cooldown = bool.Parse(value);
                if (cooldown)
                {
                    //play the animation here
                }
            }
        }
        if (flag == "POWERUP_USE")
        {
            if (IsServer)
            {
                itemCooldown = true;
                //StartCoroutine(ItemCooldown());
                SendUpdate("POWERUP_USE", itemCooldown.ToString());
            }
            if (IsClient)
            {
                itemCooldown = bool.Parse(value);
                if (itemCooldown)
                {
                    //play the animation here
                }
            }
        }

    }

    public override void NetworkedStart()
    {
        //set player materials here
        if (IsServer)
        {
            //set player spawn points here
            rb.useGravity = true;
        }
        if(IsLocalPlayer)
        {
            //nothing for now, but camera manip might need to be here
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                if (isMoving)
                {
                    rb.velocity = (rb.transform.forward * directionUD * speed) + (rb.transform.right * directionLR * speed) + new Vector3(0, rb.velocity.y, 0);
                }
                else
                {
                    rb.velocity = Vector3.zero + new Vector3(0, rb.velocity.y, 0);
                }
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    public void OnDirectionChanged(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started || context.action.phase == InputActionPhase.Performed)
        {
            SendCommand("MOVE", context.action.ReadValue<Vector2>().ToString());
        }
        if (context.action.phase == InputActionPhase.Canceled)
        {   
            SendCommand("STOP", "doot doot");
        }
    }

    public void OnSteal(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            if (!cooldown && IsLocalPlayer)
            {
                //GET OTHER PLAYER DETAILS AND PUT THEM HERE INSTEAD OF BEEP
                //OR MAKE SERVER FIND OUT WHO IS NEAR ENOUGH TO STEAL FROM
                SendCommand("STEAL", "BEEP");
                MakeSound(0);
            }
        }
    }

    public void OnPUUse(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            if (!itemCooldown && IsLocalPlayer)
            {
                //SEND ITEM DEETS, NOT BEEP
                SendCommand("POWERUP_USE", "BEEP");
                MakeSound(1);
            }
        }
    }


    public void MakeSound(int sound)
    {
        switch (sound)
        {
            case 0: //stealing
                if (stealSFX != null) SFX.PlayOneShot(stealSFX);
                break;
            case 1: //get powerup
                if (powerUpGetSFX != null) SFX.PlayOneShot(powerUpGetSFX);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<NetworkRigidbody>().rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            //animation prompts go here
        }
    }
}
