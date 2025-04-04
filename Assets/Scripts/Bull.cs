using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NETWORK_ENGINE;

public class Bull : NetworkComponent
{
    //nav mesh vars
    public NavMeshAgent agent;
    public float distance, speed = 10, acceleration = 10;
    public float rushSpeed = 20, rushAccel = 20, restSpeed = 5, restAccel = 5;
    public int rushTimer = 2, restTimer = 5;
    public GameObject lastTagged;

    //bull vars
    public float strength = 5;


    //sfx vars
    public AudioClip walking, rush, stunned;

    //animation vars
    public bool isRoaming = false, isResting = false, isRushing = false;

    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        
        if (IsClient)
        {
            agent.enabled = false;
        }
        else
        {
            Roam();
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                if (agent.remainingDistance <= 0.1 || agent.isPathStale)
                {
                    Roam();
                }
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    public Vector3 Roam()
    {
        isRoaming = true;
        isRushing = false;
        if (walking != null)
        {
            //trigger walk sound as an update to clients
        }
        if (!isResting)
        {
            agent.speed = speed;
            agent.acceleration = acceleration;
        }
        
        Vector3 pickDir = UnityEngine.Random.insideUnitSphere * distance; //get position in circle
        pickDir = new Vector3(pickDir.x, 0, pickDir.z) + this.transform.position;

        agent.destination = pickDir;
        return pickDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Contact with " + other.name);
        if (other.CompareTag("AGGRO")) //if the bull hits an aggro collider...
        {
            GameObject aggroOwner = other.transform.parent.gameObject;
            if (!isResting || aggroOwner != lastTagged)
            { //only rush at players, not bulls
                lastTagged = aggroOwner;
                Rush(other.gameObject.transform.parent.gameObject);
            }
        }
        if(other.CompareTag("Player"))
        {
            StopCoroutine("Rushing");
            isResting = true;
            if(isRushing) isRushing = false;
            StartCoroutine(Rest());
        }
    }


    public void Rush(GameObject player)
    {
        isRoaming = false;
        isResting = false;
        agent.speed = rushSpeed;
        agent.acceleration = rushAccel;
        Debug.Log("Rushing at "+player.name);
        StartCoroutine(Rushing(player));
    }

    public IEnumerator Rushing(GameObject target)
    {
        bool tempRushing = true;
        float tempTimer = 0;
        while (tempRushing)
        {
            tempTimer += 0.1f;
            if (tempTimer >= rushTimer) tempRushing = false;

            agent.destination = target.transform.position;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator Rest()
    {
        Debug.Log("Start rest.");
        isResting = true;
        isRoaming = true;
        isRushing = false;
        float delta = 0;
        agent.speed = restSpeed;
        agent.acceleration = restAccel;
        while (delta < restTimer)
        {
            delta += 0.5f;
            Roam();
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Stopped rest.");
        isResting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance <= 0.1 || agent.isPathStale) //REMOVE AFTER TESTING
        {
            Roam();
        }
        if (IsClient)
        {
            //put animations/animation functions here
        }
    }
}
