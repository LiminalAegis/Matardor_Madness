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
    public Transform CapeMat;
    public Material[] MColor;
    public int ColorSelected = -1;
    public string PName = "<Default>";
    public string PTeam; //Team1 or Team2
    public int PlayerNum; //1, 2 for team1 | 3,4 for team2
    public int PlayerHp = 3;
    public int PlayerScore = 0;
    public int PlayerScoreTotal = 0;
    public int PlayerPF = 0;
    public int PlayerCF = 1; //1 is own flag.  should always have 1 flag when playing?
    public int TotalPlayerCF = 0;
    public int TotalPlayerPF = 0;
    public GameObject PowerUp;
    public GameObject LaunchPoint;
    public bool IsDead = false;
    public bool RecentlyHit = false;

    //movement/action commands
    public PlayerInput MyInput;
    public InputActionAsset MyMap;

    //animator related vars 
    public Animator MyAnime;
    public Rigidbody MyRig;

    //audio and UI vars
    public AudioClip death, stun, steal;
    public MatchAudio matchAudio;
    public PlayerUI ui;

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
                //cape colors. 
                CapeMat = this.gameObject.transform.GetChild(9);
                CapeMat.GetComponent<Renderer>().material = MColor[(ColorSelected+4)];
                //we need to change base color here too, or send a signal

            }
            if(flag == "NUM")
            {
                PlayerNum = int.Parse(value);
            }
            if(flag == "HIT" && IsLocalPlayer)
            {
                PlayerHp = int.Parse(value);
                if (matchAudio != null)
                {
                    matchAudio.SFX(4);
                }
                ui.HealthChange(PlayerHp - 1);
                StartCoroutine(stunPlayer());
                //do hit visual effects
                
            }
            if(flag == "DEAD" && IsLocalPlayer)
            {
                IsDead = true;
                if (matchAudio != null)
                {
                    matchAudio.SFX(2);
                }
                //start showing a respawn timer?
            }
            if(flag == "ALIVE" && IsLocalPlayer)
            {
                IsDead = false;
                ui.HealthChange(PlayerHp);
            }
            if(flag == "HEAL" && IsLocalPlayer)
            {
                PlayerHp += int.Parse(value);
                ui.HealthChange(PlayerHp);
            }

            //removed local player since all players should see the glow increase
            if (flag == "FLAG")
            {
                //Increase cape glow
                PlayerScore = int.Parse(value);
            }
            if(flag == "CLEARPU" && IsLocalPlayer)
            {
                if (ui != null)
                {
                    ui.PowerUpVisual(0);
                }
            }
            if (flag == "UI" && IsLocalPlayer)
            {
                if (int.Parse(value) != 1)
                {
                    ui.PowerUpVisual(int.Parse(value));
                }
            }
        }
       
    }

    public override void NetworkedStart()
    {
        MyInput = GetComponent<PlayerInput>();
        MyMap = MyInput.actions;
        matchAudio = FindObjectOfType<MatchAudio>();
        if (matchAudio == null)
        {
            Debug.Log("No Match audio bitch!");
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
        MyRig = GetComponent<Rigidbody>();
        MyRig.velocity = Vector3.zero;
        MyAnime = GetComponent<Animator>();
        ui = FindObjectOfType<PlayerUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //Base Camera movement
        if (IsLocalPlayer)
        {
            if(IsDead)
            {
                Camera.main.transform.position = new Vector3(0, 10, -25);
                Camera.main.transform.rotation = Quaternion.Euler(25, 0, 0);
            }
            else 
            {
                //works but movement would need to be turned 90 degrees depending on team
                /*
                if(PTeam == "Team1")
                {
                    float cameraSpeed = 5.0f;
                    Vector3 offsetVector = new Vector3(-25, 40, 0);
                    Vector3 targetCameraPosition = this.gameObject.transform.position + offsetVector;
                    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPosition, cameraSpeed * Time.deltaTime);
                    //orient
                    Camera.main.transform.LookAt(this.gameObject.transform.position);
                }
                else if(PTeam == "Team2")
                {
                    float cameraSpeed = 5.0f;
                    Vector3 offsetVector = new Vector3(25, 40, 0);
                    Vector3 targetCameraPosition = this.gameObject.transform.position + offsetVector;
                    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPosition, cameraSpeed * Time.deltaTime);
                    //orient
                    Camera.main.transform.LookAt(this.gameObject.transform.position);
                } else
                {*/
                    float cameraSpeed = 5.0f;
                    Vector3 offsetVector = new Vector3(0, 40, -25);
                    Vector3 targetCameraPosition = this.gameObject.transform.position + offsetVector;
                    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPosition, cameraSpeed * Time.deltaTime);
                    //orient
                    Camera.main.transform.LookAt(this.gameObject.transform.position);
                //}
            }
                
        }
        if (IsClient)
        {
            if (MyRig.velocity.magnitude > .5f)
            {
                MyAnime.SetBool("IsMoving", true);
            }
            else
            {
                MyAnime.SetBool("IsMoving", false);
            }
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
        GameObject playerDrop = MyCore.NetCreateObject(
                                11, //make sure to keep right index
                                this.Owner,
                                this.transform.position,
                                this.transform.rotation
                            );
        playerDrop.GetComponent<FlagDrop>().CF = PlayerCF;
        playerDrop.GetComponent<FlagDrop>().PF = PlayerPF;
        PlayerScore = 0;
        PlayerPF = 0;
        PlayerCF = 0; //set to 1 after respawn?

        //wait until we have the actual map and camera set up for this
        //fix camera in place (or switch to a different camera?)
        SendUpdate("DEAD", "1");
        //teleport player under the map
        this.transform.position = new Vector3(0, -100, 0);

        yield return new WaitForSeconds(5f);
        //respawn
        //need to place player at same spawn from game start.
        GameMaster gameManager = FindObjectOfType<GameMaster>();
        if (gameManager != null)
        {
            this.transform.position = gameManager.SpawnPoints[PlayerNum].transform.position;
            this.transform.rotation = gameManager.SpawnPoints[PlayerNum].transform.rotation;
        }
        PlayerCF = 1;
        PlayerHp = 3;
        SendUpdate("ALIVE", "1");
        SendUpdate("HEAL", "3");
        //maybe either save spawn point as variable or use player num
    }

    //trigger will be our aggro collider, use actual collision for getting hit
    public void OnCollisionEnter(Collision other)
    {
        if(IsServer)
        {
            if (other.gameObject.tag == "ENEMY")
            {
                if(RecentlyHit)
                {
                    return;
                }
                RecentlyHit = true;
                StartCoroutine(RecentHitTimer());
                //if bull mask is powerup
                if (PowerUp != null && PowerUp.GetComponent<MaskScript>() != null)
                {
                    //this syntax sucks ass
                    other.gameObject.GetComponent<Bull>().StartCoroutine(other.gameObject.GetComponent<Bull>().Stunned());
                    MyCore.NetDestroyObject(PowerUp.GetComponent<NetworkID>().NetId);
                    SendUpdate("CLEARPU", "");
                    return;
                }

                other.gameObject.GetComponent<Bull>().HitPlayer = true;

                PlayerHp -= 1;
                if(PlayerHp <= 0 )
                {
                    PlayerHp = 0;
                    StartCoroutine(playerDeath());
                }
                SendUpdate("HIT", PlayerHp.ToString());
                //StartCoroutine(stunPlayer()); //stun in hit on local?
                //call stun coroutine

            }
            //NOT stealing, direct ground pickup 
            if (other.gameObject.tag == "FLAG")
            {
                Debug.Log("Player touched flag");
                //check for type of flag script
                if (other.gameObject.GetComponent<FlagThrowScript>() != null)
                {
                    Debug.Log("Player touched thrown flag");
                    FlagThrowScript temp = other.gameObject.GetComponent<FlagThrowScript>();

                    //make sure we send our own team flag back to spawn
                    if (other.gameObject.GetComponent<FlagThrowScript>().Team == PTeam)
                    {
                        Debug.Log("Player touched own team thrown flag");
                        //destroy flag and send command to spawn a new one at spawn point
                        if (temp.PickedUp)
                        {
                            return;
                        }
                        temp.PickedUp = true;
                        PlayerCF += other.gameObject.GetComponent<FlagThrowScript>().CF;

                        //make sure a PF was thrown
                        if(other.gameObject.GetComponent<FlagThrowScript>().PF >= 1)
                        {
                            //send command to spawn new flag at spawn point
                            BaseFlag[] allFlags = FindObjectsOfType<BaseFlag>();

                            foreach (BaseFlag flag in allFlags)
                            {
                                if (flag.Team == PTeam)
                                {
                                    flag.Respawn();
                                }
                            }
                        }

                        
                        MyCore.NetDestroyObject(other.gameObject.GetComponent<NetworkID>().NetId);

                        return;
                    }
                    else
                    {
                        Debug.Log("Player touched other team thrown flag");

                        if (temp.PickedUp)
                        {
                            return;
                        }

                        temp.PickedUp = true;
                        //enemy flag, capture
                        PlayerPF += other.gameObject.GetComponent<FlagThrowScript>().PF;
                        PlayerScore += other.gameObject.GetComponent<FlagThrowScript>().PF *3;
                        PlayerCF += other.gameObject.GetComponent<FlagThrowScript>().CF;
                        PlayerScore += other.gameObject.GetComponent<FlagThrowScript>().CF;
                        MyCore.NetDestroyObject(other.gameObject.GetComponent<NetworkID>().NetId);
                        SendUpdate("FLAG", PlayerScore.ToString());
                        Debug.Log("Player Pciked up thrown flag");
                    }
                }
                if(other.gameObject.GetComponent<BaseFlag>() != null)
                {
                    BaseFlag temp = other.gameObject.GetComponent<BaseFlag>();

                    //make sure its not our flag
                    if (temp.Team == PTeam)
                    {
                        return;
                    }
                    else
                    {
                        //enemy flag, capture
                        if (temp.PickedUp)
                        {
                            return;
                        }
                        temp.PickedUp = true;
                        
                        temp.Pickup();
                        PlayerScore += 3;
                        PlayerPF += 1;
                        SendUpdate("FLAG", PlayerScore.ToString());
                        Debug.Log("Player Stole Flag");
                    }
                }

            }
        }

    }
    //need to check trigger colission but also only when close enough
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("FLAG"))
        {
            //make sure its base flag
            if (other.gameObject.GetComponent<BaseFlag>() != null)
            {
                //Debug.Log("Player checking Scoring");
                //make sure its our flag for banking score
                if (other.gameObject.GetComponent<BaseFlag>().Team != PTeam)
                {
                    return;
                    //Debug.Log("Player not Scoring");
                }

                Vector3 myCenter = transform.position;
                Vector3 otherCenter = other.transform.position;

                float distance = Vector3.Distance(myCenter, otherCenter);
                //Debug.Log("Player checking Scoring distance: " + distance);
                //within 1m of the flag
                if (distance <= 2f)
                {
                    //Debug.Log("Player Scoring");
                    
                    //bank score
                    GameMaster GM = FindObjectOfType<GameMaster>();
                    if (PTeam == "Team1")
                    {
                        //maybe add tracking for PF/CF?
                        GM.Team1Score += PlayerScore;
                        PlayerScoreTotal += PlayerScore;
                        ui.ScoreUpdate(1, GM.Team1Score);
                        TotalPlayerCF += PlayerCF - 1;
                        TotalPlayerPF += PlayerPF;
                        if(PlayerPF > 0)
                        {
                            //respawn base flag
                            BaseFlag[] baseFlags = FindObjectsOfType<BaseFlag>();
                            foreach (BaseFlag flag in baseFlags)
                            {
                                if (flag.Team != PTeam)
                                {
                                    flag.Respawn();
                                }
                            }

                        }
                        PlayerCF = 1;
                        PlayerPF = 0;
                        PlayerScore = 0;
                    }
                    if (PTeam == "Team2")
                    {
                        GM.Team2Score += PlayerScore;
                        PlayerScoreTotal += PlayerScore;
                        ui.ScoreUpdate(2, GM.Team2Score);
                        TotalPlayerCF += PlayerCF;
                        TotalPlayerPF += PlayerPF;
                        if (PlayerPF > 0)
                        {
                            //respawn base flag
                            BaseFlag[] baseFlags = FindObjectsOfType<BaseFlag>();
                            foreach (BaseFlag flag in baseFlags)
                            {
                                if (flag.Team != PTeam)
                                {
                                    flag.Respawn();
                                }
                            }

                        }
                        PlayerCF = 0;
                        PlayerPF = 0;
                        PlayerScore = 0;
                    }
                    if (matchAudio != null)
                    {
                        matchAudio.SFX(3);
                    }
                }
            }
            //just pick up the score?
            if (other.gameObject.GetComponent<FlagDrop>() != null)
            {
                //check distance
                Vector3 myCenter = transform.position;
                Vector3 otherCenter = other.transform.position;

                float distance = Vector3.Distance(myCenter, otherCenter);
                //within 2m of the flag
                if (distance >= 2f)
                {
                    return;
                }


                PlayerScore += other.gameObject.GetComponent<FlagDrop>().CF + other.gameObject.GetComponent<FlagDrop>().PF * 3;
                PlayerCF += other.gameObject.GetComponent<FlagDrop>().CF;
                PlayerPF += other.gameObject.GetComponent<FlagDrop>().PF;
                SendUpdate("FLAG", PlayerScore.ToString());
                MyCore.NetDestroyObject(other.gameObject.GetComponent<NetworkID>().NetId);
            }
        }
        //for banking
        /*
        if (other.CompareTag("TEAM1BASE"))
        {
            //not our base
            if (PTeam == "Team2")
            {
                return;
            }

            Vector3 myCenter = transform.position;
            Vector3 otherCenter = other.transform.position;

            float distance = Vector3.Distance(myCenter, otherCenter);

            if (distance <= .1f)
            {
                //in our base
                //bank
                GameMaster GM = FindObjectOfType<GameMaster>();

            }
            if (other.CompareTag("TEAM2BASE"))
            {
                //not our base
                if (PTeam == "Team1")
                {
                    return;
                }

                Vector3 myCenter = transform.position;
                Vector3 otherCenter = other.transform.position;

                float distance = Vector3.Distance(myCenter, otherCenter);

                if (distance <= .1f)
                {

                }

            }
        }*/
    }
    public IEnumerator RecentHitTimer()
    {
        yield return new WaitForSeconds(.5f);
        RecentlyHit = false;
    }
}
