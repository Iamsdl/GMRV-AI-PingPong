using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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
        AddStateToQTable(currentState);

        r_table = new Dictionary<State, float>();
        AddStateToRTable(currentState);
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        AddStateToQTable(currentState);
        AddStateToRTable(currentState);
        if (currentState.ballState != BallState.Irrelevant)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < qProb)
            {
                action = action.NextRandom();
            }
            else
            {
                action = action.NextBest(q_table[currentState]);
            }
        }
        else
        {
            action.h = 0;
            action.b = 0;
            action.r = 0;
            action.v = 0;
        }
        nextState = currentState.Apply(action);
        AddStateToQTable(nextState);
        AddStateToRTable(nextState);
        float reward = r_table[nextState];

        float old_value = q_table[currentState][action];
        float next_max = FindMaxValue(q_table[nextState]);

        float new_value = (1 - alpha) * old_value + alpha * (reward + gamma * next_max);
        q_table[currentState][action] = new_value;

        currentState = nextState;
    }

    private void AddStateToRTable(State state)
    {
        if (!r_table.ContainsKey(state))
        {
            r_table.Add(state, -0.5f);
        }
    }

    private void AddStateToQTable(State state)
    {
        if (!q_table.ContainsKey(state))
        {
            q_table[state] = new Dictionary<Action, float>();
            foreach (var a in possibleActions)
            {
                q_table[state].Add(a, -1);
            }

            q_table[state][new Action() { b = 0, h = 0, r = 0, v = 0 }] = 0;
        }
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



    public void SetBallState(BallState ballState)
    {
        currentState.ballState = ballState;
    }
    public BallState GetBallState()
    {
        return currentState.ballState;
    }

    private void OnApplicationQuit()
    {
        //File.WriteAllLines
        //(
        //    "test.txt",
        //    q_table.Select
        //    (
        //        kvp => $"{kvp.Key.ToString()};{kvp.Value.Select(x=>x.Key)}"
        //    )
        //);

        string test = string.Join(";", q_table);
        File.WriteAllText("test.txt", test);


    }
}
