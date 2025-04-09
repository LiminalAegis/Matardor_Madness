using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class FlareCollisionScript : NetworkComponent
{

    public bool active = false;
    public List<GameObject> HitBulls = new List<GameObject>();


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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {
        active = true;
        StartCoroutine(End());
    }
    public IEnumerator End()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (GameObject hitBull in HitBulls)
        {
            Bull bull = hitBull.GetComponent<Bull>();
            bull.StartCoroutine(bull.Stunned());
        }

        HitBulls.Clear();
        active = false;
    }

    public void OnTriggerStay(Collider other)
    {
        if (IsServer)
        {
            if (!active)
            {
                return;
            }
            //collect hit bulls
            if (other.gameObject.CompareTag("ENEMY"))
            {
                GameObject bull = other.gameObject;
                if (!HitBulls.Contains(bull))
                {
                    HitBulls.Add(bull);
                }
            }
        }
    }
}
