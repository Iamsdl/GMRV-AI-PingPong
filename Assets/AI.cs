using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StareMinge
{
    Irelevant=2,
    Relevant=1,
    Urgent=0
}

public class State
{
    public int minX;
    public int minY;
    public int minVel;

    public StareMinge stareMinge;

    public int palX;
    public int palY;
}

public enum Actiuni
{
    E,
    NE,
    N,
    NV,
    V,
    SV,
    S,
    SE,
    LNNV,
    LNV,
    LV,
    LSV,
    LSSV
}

public class AI : MonoBehaviour
{
    private int currentState;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
