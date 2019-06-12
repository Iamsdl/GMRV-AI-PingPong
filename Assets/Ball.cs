using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float timeScale = 1;

    public AI LeftAI;
    public AI RightAI;

    private Rigidbody2D ballRB;

    private bool autoStart = false;
    private bool mustReflect = false;
    private Vector2 reflectionNormal;
    private float reflectionBounciness;

    // Start is called before the first frame update
    void Start()
    {
        ballRB = this.GetComponent<Rigidbody2D>();
        this.transform.position = new Vector3(-4, 3, 0);
        ballRB.velocity = new Vector2(0, 0);
        ballRB.angularVelocity = 0;
        ballRB.simulated = false;
        AI.LoadFromFile();
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

    void FixedUpdate()
    {
        if (mustReflect)
        {
            ballRB.velocity = Vector2.Reflect(ballRB.velocity, reflectionNormal) * ballRB.sharedMaterial.bounciness * reflectionBounciness;
            mustReflect = false;
        }
    }

    public void Reflect(Vector2 N, float b)
    {
        reflectionNormal = N;
        reflectionBounciness = b;
        mustReflect = true;
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

    public void HitPad(bool left)
    {
        if (left)
            LeftAI.HitMyPad();
        else
            RightAI.HitMyPad();
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
