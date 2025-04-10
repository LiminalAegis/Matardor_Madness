using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NETWORK_ENGINE;
using ProjectileCurveVisualizerSystem;

public class PlayerMovement : NetworkComponent
{
    public Rigidbody rb;
    //need the animation here too
    
    public float speed = 5;
    public float directionUD, directionLR;
    public bool isMoving = false, isStealing = false, isPowerUp = false;
    public bool cooldown = false, itemCooldown = false;

    public AudioClip stealSFX, powerUpGetSFX;
    public AudioSource SFX;

    //Aim Visualizer Variables
    public float launchSpeed = 20f;
    public GameObject launchPoint;
    public bool isAiming = false;
    public bool LauncherEquipped = false; //if the player has a launcher equipped or not, for local player
    //visualizer specific
    private Vector3 updatedProjectileStartPosition;
    private RaycastHit hitInfo;
    public ProjectileCurveVisualizer curve;
    


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
                GameObject powerUp = GetComponent<PlayerCharacter>().PowerUp;
                //StartCoroutine(ItemCooldown());

                //boop means aim cancelled
                if (value == "BOOP" && powerUp.GetComponent<LauncherScript>() != null)
                {
                    powerUp.GetComponent<LauncherScript>().UsePower();
                    SendUpdate("LAUNCHER", "false");

                }

                SendUpdate("POWERUP_USE", itemCooldown.ToString());
                
                //check for which powerup we have and call its use
                
                if (powerUp != null)
                {
                    //food powerup
                    if (powerUp.GetComponent<FoodScript>() != null)
                    {
                        powerUp.GetComponent<FoodScript>().UsePower();
                    }
                    //flare powerup
                    else if(powerUp.GetComponent<FlareScript>() != null)
                    {
                        powerUp.GetComponent<FlareScript>().UsePower();
                    }
                    else
                    {
                        //other powerup use
                    }

                }
                
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
        if(flag == "SPEEDCHANGE")
        {
            if(IsServer)
            {
                speed = float.Parse(value);
            }
        }
        //for aiming
        if (IsLocalPlayer)
        {
            if (flag == "LAUNCHER")
            {

                LauncherEquipped = bool.Parse(value);


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
                    rb.velocity = new Vector3(directionLR*speed, 0, directionUD*speed) + new Vector3(0, rb.velocity.y, 0);
                    rb.rotation = Quaternion.LookRotation(new Vector3(directionUD,0,-1*directionLR), rb.transform.up);
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
        //for aiming
        if (context.action.phase == InputActionPhase.Performed)
        {
            if (IsLocalPlayer)
            {
                isAiming = true;
            }
        }
        
        
        if(context.action.phase == InputActionPhase.Canceled)
        {
            if (IsLocalPlayer)
            {
                isAiming = false;
                LauncherEquipped = false; //maybe works?
                SendCommand("POWERUP_USE", "BOOP");
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
        if (curve == null)
        {
            curve = FindObjectOfType<ProjectileCurveVisualizer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            //animation prompts go here

        }
        if(IsLocalPlayer)
        {
            //aim visualizer
            if (isAiming && LauncherEquipped)
            {
                curve.gameObject.SetActive(true);
                if (launchPoint != null && curve != null)
                {
                    Vector3 velocity = launchPoint.transform.forward * launchSpeed;

                    curve.VisualizeProjectileCurve(
                        launchPoint.transform.position,
                        0f, //start point offset
                        velocity,
                        0.1f, // projectile radius
                        0.1f, // end point offset
                        true, // debug draw
                        out updatedProjectileStartPosition,
                        out hitInfo
                    );
                }

            }
            else
            {
                //curve.gameObject.SetActive(false);
                curve.HideProjectileCurve();
            }
        }
    }
}
