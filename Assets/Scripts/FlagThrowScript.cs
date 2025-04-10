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
    public Collider solidCollider;
    public bool JustThrown = true;


    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if(flag == "FREEZE")
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
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
        rb = GetComponent<Rigidbody>();
        solidCollider.enabled = false;

    }
    public IEnumerator ThrowImmunity()
    {
        yield return new WaitForSeconds(.1f);
        JustThrown = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if(JustThrown)
            {
                return;
            }

            if (other.gameObject.CompareTag("Floor"))
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                SendUpdate("FREEZE", "1");
                solidCollider.enabled = true;
            }
        }
    }
}
