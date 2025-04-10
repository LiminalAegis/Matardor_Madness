using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class FlagThrowScript : NetworkComponent
{
    public bool PickedUp = false;
    public string Team; //Team1 or Team2

    public Rigidbody rb;


    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {

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
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {

            if (other.gameObject.CompareTag("Floor"))
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
}
