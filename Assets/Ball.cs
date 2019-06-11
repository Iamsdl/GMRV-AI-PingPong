using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int timeScale = 1;

    public AI LeftAI;
    public AI RightAI;

    private Rigidbody2D ballRB;

    private bool autoStart = false;

    // Start is called before the first frame update
    void Start()
    {
        ballRB = this.GetComponent<Rigidbody2D>();
        AI.LoadFromFile();
        this.transform.position = new Vector3(-4, 3, 0);
        ballRB.velocity = new Vector2(0, 0);
        ballRB.angularVelocity = 0;
        ballRB.simulated = false;
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
            ballRB.velocity = new Vector2(7, 0);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            autoStart ^= true;
        }
        UnityEngine.Time.timeScale = timeScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("LeftTable"))
        {
            if (LeftAI.HitMyTable())
                Reset();
            RightAI.HitOtherTable();
        }
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("RightTable"))
        {
            LeftAI.HitOtherTable();
            if (RightAI.HitMyTable())
                Reset();
        }
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("LeftPad"))
        {
            LeftAI.HitMyPad();
        }
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("RightPad"))
        {
            RightAI.HitMyPad();
        }
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Edge"))
        {
            LeftAI.HitEdge();
            RightAI.HitEdge();
            Reset();
        }
    }

    private void Reset()
    {
        LeftAI.Reset();
        RightAI.Reset();

        this.transform.position = new Vector3(-4, 3, 0);
        ballRB.velocity = new Vector2(0, 0);
        ballRB.angularVelocity = 0;
        ballRB.simulated = false;
        if (autoStart)
        {
            ballRB.simulated = true;
            ballRB.velocity = new Vector2(7, 0);
        }
    }

    private void OnApplicationQuit()
    {
        AI.SaveToFile();
    }
}
