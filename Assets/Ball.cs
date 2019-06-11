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

    private int EdgeLayer;
    private int LeftTableLayer;
    private int RightTableLayer;
    private int LeftPadLayer;
    private int RightPadLayer;



    // Start is called before the first frame update
    void Start()
    {
        EdgeLayer = LayerMask.GetMask("Edge");
        LeftTableLayer = LayerMask.GetMask("LeftTable");
        RightTableLayer = LayerMask.GetMask("RightTable");
        LeftPadLayer = LayerMask.GetMask("LeftPad");
        RightPadLayer = LayerMask.GetMask("RightPad");

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
        if (collision.collider.IsTouchingLayers(LeftTableLayer))
        {
            LeftAI.HitMyTable();
            RightAI.HitOtherTable();
        }
        if (collision.collider.IsTouchingLayers(RightTableLayer))
        {
            LeftAI.HitOtherTable();
            RightAI.HitMyTable();
        }
        if (collision.collider.IsTouchingLayers(LeftPadLayer))
        {
            LeftAI.HitMyPad();
            RightAI.HitOtherPad();
        }
        if (collision.collider.IsTouchingLayers(RightPadLayer))
        {
            LeftAI.HitOtherPad();
            RightAI.HitMyPad();
        }
        if (collision.collider.IsTouchingLayers(EdgeLayer))
        {
            LeftAI.HitEdge();
            RightAI.HitEdge();
            Reset();
        }
    }

    private void Reset()
    {
        this.transform.position = new Vector3(-4, 3, 0);
        ballRB.velocity = new Vector2(0, 0);
        ballRB.angularVelocity = 0;
        ballRB.simulated = false;
        //LeftAI.Reset()
        //RightAI.Reset()
        if (autoStart)
        {
            ballRB.simulated = true;
            ballRB.velocity = new Vector2(7, 0);
        }
    }
}
