using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallState
{
    Irelevant = 2,
    Relevant = 1,
    Urgent = 0
}

public class State
{
    public State(GameObject c_ball, GameObject c_paddle, float c_gridSize, float c_angleStep, float c_bouncinessStep)
    {
        ball = c_ball;
        paddle = c_paddle;
        gridSize = c_gridSize;
        angleStep = c_angleStep;
        bouncinessStep = c_bouncinessStep;
    }

    private static float gridSize;
    private static float angleStep;
    private static float bouncinessStep;
    private static GameObject ball;
    private static GameObject paddle;

    public int BallX
    {
        get
        {
            return (int)Math.Floor(ball.transform.position.x / gridSize);
        }
    }
    public int BallY
    {
        get
        {
            return (int)Math.Floor(ball.transform.position.y / gridSize);
        }
    }
    public int BallDir
    {
        get
        {
            return (int)(Math.Floor(Math.Asin(ball.GetComponent<Rigidbody2D>().velocity.normalized.y) * 180 / Math.PI / angleStep));
        }
    }

    public BallState ballState;

    public int PadX
    {
        get
        {
            return (int)Math.Ceiling(paddle.transform.position.x / gridSize);
        }
        set
        {
            if (0 < (PadX + value) * gridSize && (PadX + value) * gridSize < 9)
            {
                paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(value * gridSize, 0));
            }
        }
    }
    public int PadY
    {
        get
        {
            return (int)Math.Ceiling(paddle.transform.position.y / gridSize);
        }
        set
        {
            if (-4 < (PadY + value) * gridSize && (PadY + value) * gridSize < 5)
            {
                paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(0, value * gridSize));
            }
        }
    }
    public int PadRot
    {
        get
        {
            return (int)(Math.Floor(paddle.GetComponent<Rigidbody2D>().transform.rotation.z / angleStep));
        }
        set
        {
            if (-90 < (PadRot + value) * angleStep && (PadRot + value) * angleStep < 90)
            {
                paddle.GetComponent<Rigidbody2D>().MoveRotation(value * angleStep);
            }
        }
    }
    public int PadBounciness
    {
        get
        {
            return (int)(Math.Floor(paddle.GetComponent<Rigidbody2D>().sharedMaterial.bounciness / bouncinessStep));
        }
        set
        {
            if (0 < (PadBounciness + value) * bouncinessStep && (PadBounciness + value) * bouncinessStep < 2)
            {
                paddle.GetComponent<Rigidbody2D>().sharedMaterial.bounciness += value * bouncinessStep;
            }
        }
    }


    //TODO
    public State Apply(Action action, out float reward, out bool done)
    {
        PadX = action.h;
        PadY = action.v;
        PadRot = action.r;
        PadBounciness = action.b;

        reward =;
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

    public Action NextRandom()
    {
        h = UnityEngine.Random.Range(-1, 1);
        v = UnityEngine.Random.Range(-1, 1);
        r = UnityEngine.Random.Range(-1, 1);
        b = UnityEngine.Random.Range(-1, 1);
        return this;
    }

    internal Action NextBest(Dictionary<Action, float> a_table)
    {
        Action action = null;
        float max = float.MinValue;
        foreach (var item in a_table)
        {
            if (item.Value >= max)
            {
                action = item.Key;
                max = item.Value;
            }
        }
        return action;
    }
}

public class AI : MonoBehaviour
{
    private GameObject paddle;

    private State currentState;
    private State nextState;

    private Action action;
    private float qProb;

    private Dictionary<State, Dictionary<Action, float>> q_table;
    private float alpha;
    private float gamma;

    private List<Action> possibleActions;


    // Start is called before the first frame update
    void Start()
    {
        paddle = this.gameObject;

        qProb = 0.1f;
        alpha = 0.5f;
        gamma = 0.5f;
        action = new Action();
        possibleActions = new List<Action>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    for (int l = -1; l <= 1; l++)
                    {
                        possibleActions.Add(new Action() { h = i, v = j, r = k, b = l });
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) < qProb)
        {
            action = action.NextRandom();
        }
        else
        {
            if (q_table[currentState] == null)
            {
                q_table[currentState] = new Dictionary<Action, float>();
                foreach (var a in possibleActions)
                {
                    q_table[currentState].Add(a, 0);
                }
            }

            action = action.NextBest(q_table[currentState]);
        }

        nextState = currentState.Apply(action, out float reward, out bool done);
        float old_value = q_table[currentState][action];
        float next_max = FindMaxValue(q_table[nextState]);

        float new_value = (1 - alpha) * old_value + alpha * (reward + gamma * next_max);
        q_table[currentState][action] = new_value;

        currentState = nextState;
    }

    private float FindMaxValue(Dictionary<Action, float> a_table)
    {
        float max = float.MinValue;
        foreach (var item in a_table)
        {
            if (item.Value >= max)
            {
                max = item.Value;
            }
        }
        return max;
    }
}
