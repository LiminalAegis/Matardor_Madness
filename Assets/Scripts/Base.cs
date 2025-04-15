using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class Base : NetworkComponent
{
    public Material pink, green, blue, orange;
    public MeshRenderer baseFloor;
    public int team;


    public override void HandleMessage(string flag, string value)
    {
        if (flag == "COLOR")
        {
            ColorSet(int.Parse(value));
        }
        if (flag == "TEAM")
        {
            if (IsServer)
            {
                team = int.Parse(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        yield return null;
    }

    public void ColorSet(int color)
    {
        switch (color)
        {
            case 0:
                baseFloor.material = pink;
                break;
            case 1:
                baseFloor.material = green;
                break;
            case 2:
                baseFloor.material = blue;
                break;
            case 3:
                baseFloor.material = orange;
                break;

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
}
