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
        Vector3 pickDir = UnityEngine.Random.insideUnitSphere * distance; //get position in circle
        
        pickDir += this.transform.position; //add direction to our position

        NavMeshHit hit;

        NavMesh.SamplePosition(pickDir, out hit, distance, -1);

        return hit.position;
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
        Roam();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
