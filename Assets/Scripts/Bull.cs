using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NETWORK_ENGINE;
using UnityEditor;

public class Bull : NetworkComponent
{
    //animator vars 
    public Animator myAnime;

    //nav mesh vars
    public NavMeshAgent agent;
    public float distance, speed = 10, acceleration = 10;
    public float rushSpeed = 0, rushAccel = 20, restSpeed = 5, restAccel = 5;
    public float rushTimer = 1, restTimer = 5;
    public GameObject lastTagged;

    //bull vars
    public float strength = 5;


    //sfx vars
    public AudioClip walking, rush, stunned;

    //animation vars
    public bool isRoaming = false, isResting = false, isRushing = false;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "STUNNED")
        {
            if (IsServer)
            {
                StartCoroutine(Stunned());
            }
        }
        if (IsClient)
        {
            
            if (flag == "MOVE")
            {
                string[] split = value.Split(',');

                Vector3 pos = new Vector3(
                    float.Parse(split[0]),
                    float.Parse(split[1]),
                    float.Parse(split[2])
                );

                Vector3 rot = new Vector3(
                    float.Parse(split[3]),
                    float.Parse(split[4]),
                    float.Parse(split[5])
                );

                transform.position = pos;
                transform.rotation = Quaternion.Euler(rot);
            }

            
        }
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
                Vector3 pos = transform.position;
                Vector3 rot = transform.rotation.eulerAngles;

                string message = $"{pos.x},{pos.y},{pos.z},{rot.x},{rot.y},{rot.z}";
                SendUpdate("MOVE", message);
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
        if (!isResting && !isRushing)
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
        if(other.CompareTag("Player")) //if bull hits an actual player
        {
            if (other.gameObject.GetComponent<PlayerCharacter>() != null)
            {
                //THIS CHECK ISNT RIGHT
                //placeholder for bull mask stun check
                StartCoroutine(Stunned());
            }
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
        isRushing = true;
        float radius = player.transform.GetComponentInChildren<SphereCollider>().radius;
        if (radius > 5)
        {
            rushTimer = 1f + (radius * 0.5f);
        }
        else
        {
            rushTimer = 1;
        }
        agent.speed = rushSpeed;
        agent.acceleration = rushAccel;
        Debug.Log("Rushing at "+player.name+" for "+rushTimer+" seconds.");
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
        if (!tempRushing)
        {
            Debug.Log("Rush timed out, resting.");
            isRushing = false;
            StartCoroutine(Rest());
        }
    }

    public IEnumerator Rest()
    {
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

    public IEnumerator Stunned()
    {
        Debug.Log("Stunned");
        isResting = false;
        isRoaming = false;
        isRushing = false;
        agent.isStopped = true;
        yield return new WaitForSeconds(2);
        isRoaming = true;
        agent.isStopped = false;
    }

    // Start is called before the first frame update
    void Start()
    {

        myAnime = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.acceleration = acceleration;
        rushSpeed = speed * 1.5f;
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
            //animation calls
            myAnime.SetBool("IsRoaming", isRoaming);
            myAnime.SetBool("IsRushing", isRushing);

        }
    }
}
