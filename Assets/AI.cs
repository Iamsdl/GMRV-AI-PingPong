using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallState
{
    Irrelevant = 2,
    Relevant = 1,
    Urgent = 0
}

public struct State
{
    public State(GameObject c_ball, GameObject c_paddle)
    {
        ball = c_ball;
        paddle = c_paddle;
        ballState = BallState.Relevant;
    }

    private const float minPadX = 0;
    private const float maxPadX = 9;
    private const float minPadY = -4;
    private const float maxPadY = 5;
    private const float minPadAngle = -90;
    private const float maxPadAngle = 90;
    private const float minPadBounciness = 0;
    private const float maxPadBounciness = 2;

    private const float gridSize = 0.5f;
    private const float angleStep = 30;
    private const float bouncinessStep = 0.5f;

    private static GameObject ball;
    private static GameObject paddle;

    public BallState ballState;

    public int BallX
    {
        get
        {
            return ballState == BallState.Irrelevant ? 0 : (int)Math.Floor(ball.transform.position.x / gridSize);
        }
    }
    public int BallY
    {
        get
        {
            return ballState == BallState.Irrelevant ? 0 : (int)Math.Floor(ball.transform.position.y / gridSize);
        }
    }
    public int BallDir
    {
        get
        {
            return ballState == BallState.Irrelevant ? 0 : (int)(Math.Floor(Math.Asin(ball.GetComponent<Rigidbody2D>().velocity.normalized.y) * 180 / Math.PI / angleStep));
        }
    }

    public int PadX
    {
        get
        {
            return ballState == BallState.Irrelevant ? 0 : (int)Math.Ceiling(paddle.transform.position.x / gridSize);
        }
        set
        {
            if ((minPadX < value * gridSize) && (value * gridSize < maxPadX))
            {
                paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(value * gridSize, PadY * gridSize));
            }
        }
    }
    public int PadY
    {
        get
        {
            return ballState == BallState.Irrelevant ? 0 : (int)Math.Ceiling(paddle.transform.position.y / gridSize);
        }
        set
        {
            if ((minPadY < value * gridSize) && (value * gridSize < maxPadY))
            {
                paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(PadX * gridSize, value * gridSize));
            }
        }
    }
    public int PadRot
    {
        get
        {
            return ballState == BallState.Irrelevant ? 0 : (int)(Math.Floor(paddle.GetComponent<Rigidbody2D>().transform.rotation.z / angleStep));
        }
        set
        {
            if ((minPadAngle < value * angleStep) && (value * angleStep < maxPadAngle))
            {
                paddle.GetComponent<Rigidbody2D>().MoveRotation(value * angleStep);
            }
        }
    }
    public int PadBounciness
    {
        get
        {
            return ballState == BallState.Irrelevant ? 0 : (int)(Math.Floor(paddle.GetComponent<Rigidbody2D>().sharedMaterial.bounciness / bouncinessStep));
        }
        set
        {
            if ((minPadBounciness < value * bouncinessStep) && (value * bouncinessStep < maxPadBounciness))
            {
                paddle.GetComponent<Rigidbody2D>().sharedMaterial.bounciness = value * bouncinessStep;
            }
        }
    }


    //TODO
    public State Apply(Action action, out float reward)
    {
        State state = this;

        state.PadX += action.h;
        state.PadY += action.v;
        state.PadRot += action.r;
        state.PadBounciness += action.b;

        reward = AI.r_table[state];
        return state;
    }
}

public struct Action
{
    public int h;
    public int v;
    public int r;
    public int b;

    //public Action()
    //{
    //    h = UnityEngine.Random.Range(-1, 1);
    //    v = UnityEngine.Random.Range(-1, 1);
    //    r = UnityEngine.Random.Range(-1, 1);
    //    b = UnityEngine.Random.Range(-1, 1);
    //}

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
        Action action = new Action();
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
    public GameObject ball;
    private GameObject paddle;

    private State currentState;
    private State nextState;

    private Action action;

    public static Dictionary<State, float> r_table;
    public static Dictionary<State, Dictionary<Action, float>> q_table;
    private const float alpha = 0.5f;
    private const float gamma = 0.5f;
    private const float qProb = 0.1f;

    private List<Action> possibleActions;


    // Start is called before the first frame update
    void Start()
    {
        action = new Action();

        paddle = this.gameObject;
        //State test1 = new State(ball, paddle);
        //test1.PadX = 1;

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

        q_table = new Dictionary<State, Dictionary<Action, float>>();
        currentState = new State(ball, paddle);
        q_table[currentState] = new Dictionary<Action, float>();
        foreach (var a in possibleActions)
        {
            q_table[currentState].Add(a, 0);
        }

        r_table = new Dictionary<State, float>();
        r_table.Add(currentState, -0.5f);
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
            action = action.NextBest(q_table[currentState]);
        }

        nextState = currentState.Apply(action, out float reward);
        if (!q_table.ContainsKey(nextState))
        {
            q_table[nextState] = new Dictionary<Action, float>();
            foreach (var a in possibleActions)
            {
                q_table[nextState].Add(a, 0);
            }
        }
        if (!r_table.ContainsKey(nextState))
        {
            r_table.Add(nextState, -0.5f);
        }

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

    public void SetReward(float value)
    {
        if (!r_table.ContainsKey(currentState))
        {
            r_table.Add(currentState, value);
        }
        else
        {
            r_table[currentState] = value;
        }
    }

    public void ChangeBallState(BallState ballState)
    {
        currentState.ballState = ballState;
    }
}
