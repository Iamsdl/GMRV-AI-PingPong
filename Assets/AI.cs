using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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
        SetReward(currentState);
        string path = EditorUtility.OpenFilePanel("Load qtable", "", "txt");
        if (path.Length != 0)
        {
            string[] rows = File.ReadAllText(path).Split('\n');
            for (int i = 0; i < rows.Length-1; i++)
            {
                List<Action> actions = new List<Action>();
                List<float> rewards = new List<float>();

                string[] rowSplit = rows[i].Split(';');
                string stateString = rowSplit[0];
                State state = new State(ball, paddle);
                state.FromString(stateString);
                for (int j = 1; j < rowSplit.Length-1; j++)
                {
                    string actionString = rowSplit[j];
                    int index1 = actionString.IndexOf('[', 1);
                    int index2 = actionString.IndexOf(']');
                    string[] actionComponents = actionString.Substring(index1 + 1, index2 - index1 - 1).Split(',');
                    Action action = new Action()
                    {
                        h = Convert.ToInt32(actionComponents[0]),
                        v = Convert.ToInt32(actionComponents[1]),
                        r = Convert.ToInt32(actionComponents[2]),
                        b = Convert.ToInt32(actionComponents[3])
                    };
                    actions.Add(action);
                    float reward = Convert.ToSingle(actionString.Substring(index2 + 2).Trim(']'));
                    rewards.Add(reward);
                }
                AddStateToQTable(state, actions, rewards);
                SetReward(state, Convert.ToSingle(rowSplit[rowSplit.Length - 1]));
            }
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        AddStateToQTable(currentState);
        SetReward(currentState);
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
        SetReward(nextState);
        float reward = r_table[nextState];

        float old_value = q_table[currentState][action];
        float next_max = FindMaxValue(q_table[nextState]);

        float new_value = (1 - alpha) * old_value + alpha * (reward + gamma * next_max);
        q_table[currentState][action] = new_value;

        currentState = nextState;
    }

    //private void AddStateToRTable(State state)
    //{
    //    if (!r_table.ContainsKey(state))
    //    {
    //        r_table.Add(state, -0.5f);
    //    }
    //}

    public void SetReward(State state, float reward=-0.5f)
    {
        if (!r_table.ContainsKey(state))
        {
            r_table.Add(state, reward);
        }
        else
        {
            r_table[state] = reward;
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

    private void AddStateToQTable(State state, List<Action> actions, List<float> rewards)
    {
        if (!q_table.ContainsKey(state))
        {
            q_table[state] = new Dictionary<Action, float>();
            for (int i = 0; i < actions.Count; i++)
            {
                q_table[state].Add(actions[i], rewards[i]);
            }
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

    public State GetCurrentState()
    {
        return currentState;
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
        var path = EditorUtility.SaveFilePanel(
            "Save qtable",
            "",
            "test.txt",
            "txt");

        if (path.Length != 0)
        {
            string test = "";
            foreach (var item in q_table.Keys)
            {
                test += "State=[" + item + "];" + string.Join(";", q_table[item]) + ";" + r_table[item] + "\n";
            }
            File.WriteAllText(path, test);
        }

        //File.WriteAllLines
        //(
        //    "test.txt",
        //    q_table.Select
        //    (
        //        kvp => $"{kvp.Key.ToString()};{kvp.Value.Select(x => x.Key)}"
        //    )
        //);

    }
}
