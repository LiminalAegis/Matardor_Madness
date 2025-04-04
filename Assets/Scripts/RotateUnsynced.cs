using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;


public class RotateUnsynced : NetworkComponent
{



    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentAngle = this.transform.rotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(currentAngle.y, currentAngle.y + 10, Time.deltaTime), 0);
        
    }

}
