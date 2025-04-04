using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class PowerUpSpawner : NetworkComponent
{
    public GameObject[] PowerUps;
    public Transform[] SpawnPoints;
    public float LastSpawnTime = 0;
    public float SpawnRate = 30;
    public bool Started = false;


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
                    if (LastSpawnTime + SpawnRate <= Time.time)
                    {
                        LastSpawnTime = Time.time;

                        //spawn a powerup
                        int powerupIndex = Random.Range(0, PowerUps.Length-1);
                        GameObject powerup = MyCore.NetCreateObject(
                            1+ powerupIndex, //starting num in prefab array + powerupindex
                            this.Owner, //server owned?
                            SpawnPoints[Random.Range(0, SpawnPoints.Length - 1)].position,
                            Quaternion.identity
                        );
                    }
                
                    
                
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
