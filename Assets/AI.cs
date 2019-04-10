using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StareMinge
{
    Irelevant = 2,
    Relevant = 1,
    Urgent = 0
}

public class State
{
    public int minX;
    public int minY;
    public int minDir;

    public StareMinge stareMinge;

    public int palX;
    public int palY;
    public int palRot;

    public State GetNextState(Action action)
    {
        throw new NotImplementedException();
    }
}

public class Action
{
    public int h;
    public int v;
    public int r;
    public int b;

    public Action()
    {
        h = UnityEngine.Random.Range(-1, 1);
        v = UnityEngine.Random.Range(-1, 1);
        r = UnityEngine.Random.Range(-1, 1);
        b = UnityEngine.Random.Range(-1, 1);
    }

    public Action Next()
    {
        h = UnityEngine.Random.Range(-1, 1);
        v = UnityEngine.Random.Range(-1, 1);
        r = UnityEngine.Random.Range(-1, 1);
        b = UnityEngine.Random.Range(-1, 1);
        return this;
    }
}

public class AI : MonoBehaviour
{
    private State currentState;
    private State nextState;

    private Action action;
    private float qProb;


    // Start is called before the first frame update
    void Start()
    {
        qProb = 0.1f;
        action = new Action();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) < qProb)
        {
            action = action.Next();
            nextState = currentState.GetNextState(action);
        }
        else
        {

        }
    }
}
