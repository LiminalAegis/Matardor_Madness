using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NETWORK_ENGINE;

public class Bull : NetworkComponent
{
    public NavMeshAgent agent;
    public float distance;
    

    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {



            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    public Vector3 Roam()
    {
        Debug.Log("Picking roam location.");
        Vector3 pickDir = UnityEngine.Random.insideUnitSphere * distance; //get position in circle
        pickDir = new Vector3(pickDir.x, 0, pickDir.z) + this.transform.position;

        agent.SetDestination(pickDir);
        return pickDir;
    }

    public void Rush()
    {

    }

    public void Rest()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (IsClient)
        {
            agent.enabled = false;
        }
        else
        {
            Roam();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance <= 0.01 || agent.isPathStale    )
        {
            Roam();
        }
    }
}
