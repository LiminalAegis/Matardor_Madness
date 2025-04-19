using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class MapSwap : NetworkComponent
{
   public int MapID; //should be numbers in the spawn prefab aray


    public GameObject[] MapPrefabs;
    public int MapNum = 0; //num in array



    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if(flag == "MAPSWAP")
            {
                MapPrefabs[0].SetActive(false);
                //do visual effects for pickup
                //disable floating object effect
                //this.GetComponent<MeshRenderer>().enabled = false;
                int num = int.Parse(value);
                if (MapPrefabs[num] != null)
                {
                    MapPrefabs[num].SetActive(true);
                }
                else
                {
                    MapPrefabs[0].SetActive(true);
                }
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

    public void RandMap()
    {
        MapID = Random.Range(0, 2);
        MapID += 18;
        Debug.Log("MapID: " + MapID);

    }

    public void SpawnMap()
    {
        if (IsServer)
        {
            GameObject oldmap = GameObject.FindGameObjectWithTag("Arena0");

            MyCore.NetDestroyObject(oldmap.gameObject.GetComponent<NetworkID>().NetId);

            GameObject map = MyCore.NetCreateObject(
                MapID,
                this.Owner,
                Vector3.zero,
                Quaternion.Euler(0, 90, 0)
                );
            /*
            MapPrefabs[0].SetActive(false);
            if(MapPrefabs[MapNum] != null)
            {
                MapPrefabs[MapNum].SetActive(true);
                Debug.Log("enabling MapID: " + MapID);
                SendUpdate("MAPSWAP", MapNum.ToString());
            }
            else
            {
                MapPrefabs[0].SetActive(true);
                Debug.Log("forced MapID: " + MapID);
                SendUpdate("MAPSWAP", "0");
            }*/


            //move power up spawn spots
            GameObject[] powerUpSpots = GameObject.FindGameObjectsWithTag("PowerUpSpot");
            //see which map we are using

            if(MapID == 0)//make sure its spawn prefab value, default map
            {
                //place each power up spot
                for (int i = 0; i < powerUpSpots.Length; i++)
                {
                    Vector3 pos = Vector3.zero;

                    switch (i)
                    {
                        case 0:
                            pos = new Vector3(0, .5f, 20);
                            break;
                        case 1:
                            pos = new Vector3(0, .5f, 0);
                            break;
                        case 2:
                            pos = new Vector3(0, .5f, -20);
                            break;
                        case 3:
                            pos = new Vector3(-20, .5f, 10);
                            break;
                        case 4:
                            pos = new Vector3(20, 0.5f, 10);
                            break;
                        case 5:
                            pos = new Vector3(-20, 0.5f, -10);
                            break;
                        case 6:
                            pos = new Vector3(20, 0.5f, 10);
                            break;

                    }
                    //move
                    powerUpSpots[i].transform.position = pos;
                }

            }//do for for each map kind
            else if(MapID == 1)
            {
                for (int i = 0; i < powerUpSpots.Length; i++)
                {
                    Vector3 pos = Vector3.zero;

                    switch (i)
                    {
                        case 0:
                            pos = new Vector3(0, .5f, 20);
                            break;
                        case 1:
                            pos = new Vector3(0, .5f, -20);
                            break;
                        case 2:
                            pos = new Vector3(0, .5f, 11);
                            break;
                        case 3:
                            pos = new Vector3(-10, .5f, 0);
                            break;
                        case 4:
                            pos = new Vector3(-20, 0.5f, 0);
                            break;
                        case 5:
                            pos = new Vector3(10, 0.5f, 0);
                            break;
                        case 6:
                            pos = new Vector3(20, .5f, 0);
                            break;

                    }
                    //move
                    powerUpSpots[i].transform.position = pos;
                }
            }

        }
    }

    
}
