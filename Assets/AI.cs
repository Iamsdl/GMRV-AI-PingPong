using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AI : MonoBehaviour
{
    private bool hitMyTable;
    private bool hitMyPad;


    private State lastRelevantState;

    public const int AiWinReward = 1000;
    public const int AiLoseReward = -10;
    public const int AiHitOpponentTableReward = 100;
    public const int AiHitBallReward = 10;

    public GameObject ball;
    private Rigidbody2D ballRB;
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
        hitMyTable = false;
        hitMyPad = false;



        ballRB = ball.GetComponent<Rigidbody2D>();
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
                        possibleActions.Add(new Action() { h = i, v = j, Angle = k, Bounciness = l });
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
            for (int i = 0; i < rows.Length - 1; i++)
            {
                List<float> rewards = new List<float>();

                //string[] rowSplit = rows[i].Split(';');
                string[] rowSplit = rows[i].Split('[', ']');
                string stateString = rowSplit[1];
                string[] rewardsString = rowSplit[3].Split(';');
                string RString = rowSplit[5];
                State state = new State(ball, paddle);
                state.FromString(stateString);
                int ri = 0;
                foreach (Action action in possibleActions)
                {
                    rewards.Add(Convert.ToSingle(rewardsString[ri]));
                    ri++;
                }
                AddStateToQTable(state, possibleActions, rewards);
                SetReward(state, Convert.ToSingle(RString));
            }
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        currentState.SetBallX(ballRB.transform.position.x);
        currentState.SetBallY(ballRB.transform.position.y);
        currentState.SetBallDir(ballRB.velocity);
        AddStateToQTable(currentState);
        SetReward(currentState);
        if (currentState.MustHit != BallState.Irrelevant)
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
            action.Bounciness = 0;
            action.Angle = 0;
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

    public void HitMyTable()
    {
        if (hitMyPad)
        {
            SetReward(lastRelevantState, AiLoseReward);
        }
        else
        {
            currentState = new State(true, ballRB.transform.position.x, ballRB.transform.position.y, ballRB.velocity);
        }
    }
    public void HitOtherTable()
    {

    }
    public void HitMyPad()
    {
        lastRelevantState = currentState;
        currentState = new State(false);
    }
    public void HitOtherPad()
    {

    }
    public void HitEdge()
    {

    }
    public void Reset()
    {

    }

    public void SetReward(State state, float reward = -0.5f)
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

            q_table[state][new Action() { Bounciness = 0, h = 0, Angle = 0, v = 0 }] = 0;
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
        else
        {
            for (int i = 0; i < actions.Count; i++)
            {
                q_table[state][actions[i]] = rewards[i];
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

    public State GetCurrentState()
    {
        return currentState;
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
            StringBuilder test = new StringBuilder();
            foreach (var item in q_table.Keys)
            {
                test.Append("State=[" + item + "];Rewards=[" + string.Join(";", q_table[item].Values) + "];R=[" + r_table[item] + "]\n");
            }
            File.WriteAllText(path, test.ToString());
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
