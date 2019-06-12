using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AI : MonoBehaviour
{
    public const int AiLoseReward = -10;
    public const int AiHitOpponentTableReward = 100;

    public GameObject ball;
    public Ball ballScript;
    private Rigidbody2D ballRB;
    private GameObject paddle;
    private Rigidbody2D padRB;

    private Vector2 initialPosition;

    private State currentState;
    private State nextState;

    private Action action;

    public static Dictionary<State, float> r_table;
    public static Dictionary<State, Dictionary<Action, float>> q_table;
    private const float alpha = 0.5f;
    private const float gamma = 0.5f;
    private const float qProb = 0.1f;

    private static List<Action> possibleActions;


    // Start is called before the first frame update
    void Start()
    {
        ballRB = ball.GetComponent<Rigidbody2D>();
        padRB = this.GetComponent<Rigidbody2D>();
        initialPosition = padRB.position;

        action = new Action();

        paddle = this.gameObject;

        possibleActions = Action.AllPossibleActions;

        q_table = new Dictionary<State, Dictionary<Action, float>>();
        currentState = State.waitingState;
        AddStateToQTable(currentState);

        padRB.isKinematic = true;

        r_table = new Dictionary<State, float>();
        SetReward(currentState);
    }

    public static void LoadFromFile()
    {
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
                State state = new State();
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

    void FixedUpdate()
    {
        if (AlreadyHitMyTable() && !AlreadyHitMyPad())
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < qProb)
            {
                action = action.NextRandom();
            }
            else
            {
                action = action.NextBest(q_table[currentState]);
            }

            nextState = currentState.Apply(action); //TODO see if current state and next state are different
            AddStateToQTable(nextState);
            SetReward(nextState);
            float reward = r_table[currentState];

            float old_value = q_table[currentState][action];
            float next_max = FindMaxValue(q_table[nextState]);

            float new_value = (1 - alpha) * old_value + alpha * (reward + gamma * next_max);
            q_table[currentState][action] = new_value;

            currentState = nextState;

            if (action.Bounciness >= 0)
            {
                //ballRB.simulated = false;
                float sign = Mathf.Sign(ballRB.position.x);
                float padAngle = sign * Mathf.Asin(ballRB.velocity.normalized.y) * Mathf.Rad2Deg;
                while (padAngle > 90)
                    padAngle -= 90;
                while (padAngle < -90)
                    padAngle += 90;
                padAngle = (padAngle + 90) / 2 - action.Angle;
                padRB.MoveRotation(padAngle);
                padRB.sharedMaterial.bounciness = action.Bounciness;
                //float DeltaAlfa = Mathf.Abs(action.Angle - ballAngle) * Mathf.Deg2Rad;
                Vector2 padNormal = Quaternion.AngleAxis(padAngle, Vector3.forward) * (Vector2.left * sign);
                //Vector2 padUp = Quaternion.AngleAxis(action.Angle, Vector3.forward) * Vector2.up;
                //padRB.MovePosition(ballRB.position + ballRB.velocity * Time.fixedDeltaTime + sign * 0.1f * ballRB.velocity.normalized + ballRB.velocity.normalized * (sign * Mathf.Cos(DeltaAlfa) + 0.1f * Mathf.Sin(DeltaAlfa)));
                //padRB.MovePosition(ballRB.position + ballRB.velocity * Time.fixedDeltaTime + Vector2.Dot(padNormal, ballRB.velocity.normalized) * 0.2f * padNormal + Vector2.Dot(padUp, ballRB.velocity.normalized) * padUp);
                //ballRB.simulated = true;

                //fara palete
                //ballScript.Reflect(padNormal, action.Bounciness);
                ballRB.velocity = Vector2.Reflect(ballRB.velocity, padNormal) * ballRB.sharedMaterial.bounciness * action.Bounciness;
                ballScript.HitPad(sign < 0);
            }
        }
    }

    private bool AlreadyHitMyTable()
    {
        return currentState.GetX() >= 0;
    }

    private bool AlreadyHitMyPad()
    {
        return currentState.GetBounciness() >= 0;
    }

    public bool HitMyTable()
    {
        if (AlreadyHitMyPad() || AlreadyHitMyTable())
        {
            SetReward(currentState, AiLoseReward);
            return true;
        }
        currentState = new State(Mathf.Abs(ballRB.transform.position.x), new Vector2(Mathf.Abs(ballRB.velocity.x), Mathf.Abs(ballRB.velocity.y)));
        AddStateToQTable(currentState);
        SetReward(currentState);
        return false;
    }

    public void HitOtherTable()
    {
        if (AlreadyHitMyPad())
            SetReward(currentState, AiHitOpponentTableReward);
        currentState = State.waitingState;
    }
    public void HitMyPad()
    {
        //StartCoroutine(ResetPosition());
        padRB.MovePosition(initialPosition);
    }

    private IEnumerator<WaitForSeconds> ResetPosition()
    {
        yield return new WaitForSeconds(1);
        padRB.MovePosition(initialPosition);
    }

    public void HitOtherPad()
    {
        currentState = State.waitingState;
    }
    public void HitEdge()
    {
        if (AlreadyHitMyPad())
        {
            SetReward(currentState, AiLoseReward);
        }
    }
    public void Reset()
    {
        currentState = State.waitingState;
        padRB.MovePosition(initialPosition);
        padRB.MoveRotation(0);

    }

    private static void SetReward(State state, float reward = 0)
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

    private static void AddStateToQTable(State state)
    {
        if (!q_table.ContainsKey(state))
        {
            q_table[state] = new Dictionary<Action, float>();
            foreach (var a in possibleActions)
            {
                q_table[state].Add(a, -1);
            }

            q_table[state][new Action() { Bounciness = -1, Angle = 0 }] = 0;
        }
    }

    private static void AddStateToQTable(State state, List<Action> actions, List<float> rewards)
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
        //File.WriteAllLines
        //(
        //    "test.txt",
        //    q_table.Select
        //    (
        //        kvp => $"{kvp.Key.ToString()};{kvp.Value.Select(x => x.Key)}"
        //    )
        //);

    }

    public static void SaveToFile()
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
    }
}
