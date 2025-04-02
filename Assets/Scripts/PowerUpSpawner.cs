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


    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "")
            {
                
            }
            
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
                if (IsDirty)
                {
                    if(LastSpawnTime + SpawnRate >= Time.time)
                    {
                        LastSpawnTime = Time.time;
                        //spawn a powerup
                        int powerupIndex = Random.Range(0, PowerUps.Length);
                        GameObject powerup = MyCore.NetCreateObject(
                            powerupIndex,
                            this.Owner,
                            SpawnPoints[Random.Range(0, SpawnPoints.Length)].position,
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
