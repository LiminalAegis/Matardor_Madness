using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class PowerUpSpawner : NetworkComponent
{
    public int numOfPowers = 5;
    public GameObject[] PowerUps;
    public Transform[] SpawnPoints;
    public Transform[] usedSpawnPoints;
    public float LastSpawnTime = 0;
    public float SpawnRate = 30;
    public bool Started = false;
    public int PowerUpsAtOnce = 4;


    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "")
            {
                
            }
            
        }

        if (IsServer)
        {
            
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
                if(Started)
                {

                    //check if power ups have been picked up
                    for(int i = 0; i < PowerUps.Length; i++)
                    {
                        if(PowerUps[i] != null)
                        {
                            if(PowerUps[i].GetComponent<MaskScript>() != null)
                            {
                                //is mask
                                if(PowerUps[i].GetComponent<MaskScript>().PickedUp)
                                {
                                    PowerUps[i] = null;
                                    usedSpawnPoints[i] = null;
                                }
                            }
                            else if(PowerUps[i].GetComponent<FlareScript>() != null)
                            {
                                //is flare
                                if (PowerUps[i].GetComponent<FlareScript>().PickedUp)
                                {
                                    PowerUps[i] = null;
                                    usedSpawnPoints[i] = null;
                                }

                            }
                            else if(PowerUps[i].GetComponent<FoodScript>() != null)
                            {
                                //is food
                                if (PowerUps[i].GetComponent<FoodScript>().PickedUp)
                                {
                                    PowerUps[i] = null;
                                    usedSpawnPoints[i] = null;
                                }

                            }
                            else if(PowerUps[i].GetComponent<LauncherScript>() != null)
                            {
                                //is launcher
                                if (PowerUps[i].GetComponent<LauncherScript>().PickedUp)
                                {
                                    PowerUps[i] = null;
                                    usedSpawnPoints[i] = null;
                                }

                            }
                            //food doesnt need to be checked, it will die when picked up
                        }
                    }

                    //check if spot available
                    int availableSpot = -1;
                    for (int i = 0; i < PowerUps.Length; i++)
                    {
                        if (PowerUps[i] == null)
                        {
                            availableSpot = i;
                            //Debug.Log("available spot: " + i);
                            break;
                            
                        }
                        if (i == PowerUps.Length-1)
                        {
                            //no spots
                            //Debug.Log("No available spots for powerups " + i);
                            availableSpot = -1;
                        }
                    }

                    //if there IS an available spot do the rest
                    if(availableSpot != -1)
                    {

                        //check spawn timer
                        if (LastSpawnTime + SpawnRate <= Time.time)
                        {
                            LastSpawnTime = Time.time;

                            //random power up
                            int powerupIndex = Random.Range(0, numOfPowers - 1);

                            //find available spawn point
                            /*
                            Transform tempSpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length - 1)];
                            Debug.Log("chose first spot: " + tempSpawnPoint.position.ToString());
                            for (int i = 0; i < usedSpawnPoints.Length; i++)
                            {
                                if (usedSpawnPoints[i] == tempSpawnPoint)
                                {
                                    Debug.Log("hit used spawn point: " + i);
                                    //this spawn point is already used
                                    tempSpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length - 1)];
                                    Debug.Log("chose new spot: " + tempSpawnPoint.position.ToString());
                                    i = 0;
                                }
                            }//might need optimized code if the spawn points isnt a lot
                            */

                            // list good
                            List<Transform> availableSpawnPoints = new List<Transform>();
                            foreach (Transform sp in SpawnPoints)
                            {
                                bool used = false;
                                foreach (Transform usp in usedSpawnPoints)
                                {
                                    if (usp == sp)
                                    {
                                        used = true;
                                        break;
                                    }
                                }
                                if (!used)
                                {
                                    availableSpawnPoints.Add(sp);
                                }
                            }

                            //no available spawn points
                            if (availableSpawnPoints.Count == 0)
                            {
                                //Debug.Log("no unused spawn points");
                                yield return new WaitForSeconds(.1f);
                                continue;
                            }

                            //pick spanw point
                            Transform tempSpawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];

                            //mark spawn point as used
                            usedSpawnPoints[availableSpot] = tempSpawnPoint;


                            //Debug.Log("spawning at " + tempSpawnPoint.position.ToString());
                            //spawn power up
                            GameObject powerup = MyCore.NetCreateObject(
                                1 + powerupIndex, //starting num in prefab array + powerupindex
                                this.Owner, //server owned?
                                tempSpawnPoint.position,
                                Quaternion.identity
                            );
                            PowerUps[availableSpot] = powerup;
                        }
                    }

                
                    
                
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        usedSpawnPoints = new Transform[PowerUpsAtOnce];
        PowerUps = new GameObject[PowerUpsAtOnce];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
