using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject pad1;
    public GameObject pad2;
    public GameObject table;
    public GameObject edge;
    public AI ai;


    private bool hitTable;
    private bool hitPad;
    private int player;


    // Start is called before the first frame update
    void Start()
    {
        hitPad = false;
        player = 0;
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.GetComponent<Rigidbody2D>().simulated = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject == pad1)
        {
            if (player == 1 && !hitPad)
            {
                hitPad = true;
                ai.ChangeBallState(BallState.Relevant);
            }
            else
            {
                ai.SetReward(100);
                Reset();
            }
        }
        else if (collision.collider.gameObject == pad2)
        {
            if (player == 2 && !hitPad)
            {
                hitPad = true;
                ai.SetReward(1);
                ai.ChangeBallState(BallState.Irrelevant);
            }
            else
            {
                ai.SetReward(-100);
                Reset();
            }
        }
        else if (collision.collider.gameObject == table)
        {
            if (gameObject.transform.position.x < 0)
            {
                if (player == 1)
                {
                    ai.SetReward(100);
                    Reset();
                }
                else
                {
                    player = 1;
                    hitPad = false;
                    ai.SetReward(10);
                }
            }
            else
            {
                if (player == 2)
                {
                    ai.SetReward(-100);
                    Reset();
                }
                else
                {
                    player = 2;
                    hitPad = false;
                    ai.ChangeBallState(BallState.Urgent);
                }
            }
        }
        else if (collision.collider.gameObject == edge)
        {
            if (player == 1)
            {
                ai.SetReward(100);
                Reset();
            }
            else
            {
                ai.SetReward(-100);
                Reset();
            }
        }
    }

    private void Reset()
    {
        this.transform.position = new Vector3(-4, 3, 0);
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        this.GetComponent<Rigidbody2D>().simulated = false;

    }
}
