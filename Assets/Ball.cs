using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const int AiWinReward = 1000;
    private const int AiLoseReward = -10;
    private const int AiHitOpponentTableReward = 100;
    private const int AiHitBallReward = 10;

    public int timeScale = 1;

    private State lastRelevantState;

    /// <summary>
    /// player pad
    /// </summary>
    public GameObject leftPlayerPad;
    /// <summary>
    /// ai pad
    /// </summary>
    public GameObject rightPlayerPad;
    public GameObject table;
    public GameObject edge;
    public AI ai;

    private LastHit lastHit;

    private bool p = false;

    private Rigidbody2D ballRB;

    // Start is called before the first frame update
    void Start()
    {
        ballRB = this.GetComponent<Rigidbody2D>();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ballRB.simulated = true;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ballRB.simulated = true;
            ai.SetBallState(BallState.Relevant);
            lastHit = LastHit.LeftPad;
            ballRB.velocity = new Vector2(7, 0);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            p = true;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            p = false;
        }
        UnityEngine.Time.timeScale = timeScale;
    }

    void FixedUpdate()
    {
        State currentState = ai.GetCurrentState();
        currentState.SetBallX(ballRB.transform.position.x);
        currentState.SetBallY(ballRB.transform.position.y);
        currentState.SetBallDir(ballRB.velocity);
    }

    enum LastHit
    {
        LeftTable,
        RightTable,
        LeftPad,
        RightPad
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject == leftPlayerPad)
        {
            if (lastHit == LastHit.LeftTable)
            {
                lastHit = LastHit.LeftPad;
                ai.SetBallState(BallState.Relevant);
            }
            else
            {
                ai.SetReward(lastRelevantState, AiWinReward);
                Reset();
            }
        }
        else if (collision.collider.gameObject == rightPlayerPad)
        {
            if (lastHit == LastHit.RightTable)
            {
                //TODO : lastRelevantState
                lastRelevantState = ai.GetCurrentState();
                lastHit = LastHit.RightPad;
                ai.SetReward(AiHitBallReward);
                ai.SetBallState(BallState.Irrelevant);
            }
            else
            {
                ai.SetReward(AiLoseReward);
                Reset();
            }
        }
        else if (collision.collider.gameObject == table)
        {
            if (gameObject.transform.position.x < 0)
            {
                if (lastHit == LastHit.RightPad)
                {
                    lastHit = LastHit.LeftTable;
                    ai.SetReward(lastRelevantState, AiHitOpponentTableReward);
                }
                else
                {
                    ai.SetReward(lastRelevantState, AiWinReward);
                    Reset();
                }
            }
            else
            {
                if (lastHit == LastHit.LeftPad)
                {
                    lastHit = LastHit.RightTable;
                    ai.SetBallState(BallState.Urgent);
                }
                else
                {
                    ai.SetReward(lastRelevantState, AiLoseReward);
                    Reset();
                }
            }
        }
        else if (collision.collider.gameObject == edge)
        {
            if (lastHit == LastHit.LeftPad || lastHit == LastHit.LeftTable)
            {
                ai.SetReward(lastRelevantState, AiWinReward);
                Reset();
            }
            else
            {
                ai.SetReward(lastRelevantState, AiLoseReward);
                Reset();
            }
        }
    }

    private void Reset()
    {
        lastHit = LastHit.RightPad;
        ai.SetBallState(BallState.Irrelevant);

        this.transform.position = new Vector3(-4, 3, 0);
        ballRB.velocity = new Vector2(0, 0);
        ballRB.angularVelocity = 0;
        ballRB.simulated = false;
        ai.SetBallState(BallState.Irrelevant);
        lastRelevantState = ai.GetCurrentState();
        if (p)
        {
            ballRB.simulated = true;
            ai.SetBallState(BallState.Relevant);
            lastHit = LastHit.LeftPad;
            ballRB.velocity = new Vector2(7, 0);
        }
    }
}
