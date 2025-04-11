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
    public int PF;
    public int CF;


    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if(flag == "FREEZE")
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            if (flag == "ROT")
            {
                string[] rot = value.Trim('(').Trim(')').Split(',');
                float x = float.Parse(rot[0]);
                float y = float.Parse(rot[1]);
                float z = float.Parse(rot[2]);
                Quaternion targetRot = Quaternion.Euler(x, y, z);
                transform.rotation = targetRot;
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
        StartCoroutine(ThrowImmunity());

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

    public IEnumerator RotateUpright()
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            SendUpdate("ROT", transform.rotation.eulerAngles.ToString());
            yield return null;
        }

        transform.rotation = targetRot;

        rb.constraints = RigidbodyConstraints.FreezeAll;
        SendUpdate("FREEZE", "1");
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
                StartCoroutine(RotateUpright());
                solidCollider.enabled = true;
            }
        }
    }
}
