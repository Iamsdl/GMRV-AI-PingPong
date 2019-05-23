using System;
using UnityEngine;

public struct State
{
    public State(GameObject c_ball, GameObject c_paddle)
    {
        ball = c_ball;
        paddle = c_paddle;
        ballState = BallState.Irrelevant;
        BallX = 0;
        BallY = 0;
        BallDir = 0;
        PadX = 0;
        PadY = 0;
        PadRot = 0;
        PadBounciness = 0;
        GetBallX();
        GetBallY();
        GetBallDir();
        GetPadX();
        GetPadY();
        GetPadRot();
        GetPadBounciness();
    }

    private const bool isRightPaddle = true;
    private const int r1lm1 = isRightPaddle ? 1 : -1;
    private const float minPadX = 0;
    private const float maxPadX = 9;
    private const float tableX = 7;
    private const float tableY = -1;
    private const float minPadY = -4;
    private const float maxPadY = 5;
    private const float minPadAngle = -90;
    private const float maxPadAngle = 90;
    private const float minPadBounciness = 0;
    public const float maxPadBounciness = 4;

    public const float gridSize = 0.5f;
    public const float angleStep = 30;
    public const float bouncinessStep = 0.5f;

    private static GameObject ball;
    private static GameObject paddle;

    public BallState ballState;
    public int BallX;
    public int BallY;
    public int BallDir;
    public int PadX;
    public int PadY;
    public int PadRot;
    public int PadBounciness;


    public void FromString(string stateString)
    {
        string[] text = stateString.Substring(7).TrimEnd(']').Split(',');

        ballState = (BallState)Enum.Parse(typeof(BallState), text[0]);
        BallX = Convert.ToInt32(text[1]);
        BallY = Convert.ToInt32(text[1]);
        BallDir = Convert.ToInt32(text[1]);
        PadX = Convert.ToInt32(text[1]);
        PadY = Convert.ToInt32(text[1]);
        PadRot = Convert.ToInt32(text[1]);
        PadBounciness = Convert.ToInt32(text[1]);
    }

    public int GetBallX()
    {
        //BallX = ballState == BallState.Irrelevant ? 0 : (int)Math.Floor(ball.transform.position.x / gridSize);
        return BallX;
    }

    public void SetBallX(float value)
    {
        BallX = Convert.ToInt32(Math.Floor(value/gridSize));
    }

    public int GetBallY()
    {
        //BallY = ballState == BallState.Irrelevant ? 0 : (int)Math.Floor(ball.transform.position.y / gridSize);
        return BallY;
    }

    public void SetBallY(float value)
    {
        BallY = Convert.ToInt32(Math.Floor(value/gridSize));
    }
    
public int GetBallDir()
    {
        //BallDir = ballState == BallState.Irrelevant ? 0 : (int)(Math.Floor(Math.Asin(ball.GetComponent<Rigidbody2D>().velocity.normalized.y) * 180 / Math.PI / angleStep));
        return BallDir;
    }
    
    public void SetBallDir(Vector2 value)
    {
        BallDir = ballState == BallState.Irrelevant ? 0 : (int)(Math.Floor(Math.Asin(value.normalized.y) * 180 / Math.PI / angleStep));
    }
    
    public int GetPadX()
    {
        //PadX = ballState == BallState.Irrelevant ? 0 : (int)Math.Ceiling(paddle.transform.position.x / gridSize);
        return PadX;
    }
    public void SetPadX(int value)
    {
        if ((minPadX < value * gridSize) && (value * gridSize < maxPadX))
        {
            paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(value * gridSize, PadY * gridSize));
            PadX = value;
        }
        if (value * gridSize < minPadX)
        {
            paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(minPadX * gridSize, PadY * gridSize));
            PadX = Convert.ToInt32(minPadX / gridSize);
        }
        if (value * gridSize > maxPadX)
        {
            paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(maxPadX * gridSize, PadY * gridSize));
            PadX = Convert.ToInt32(maxPadX / gridSize);
        }
        if (PadY * gridSize < tableY && value * gridSize < tableX)
        {
            paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(tableX, PadY * gridSize));
            PadX = Convert.ToInt32(tableX / gridSize);
        }
    }
    public int GetPadY()
    {
        //PadY = ballState == BallState.Irrelevant ? 0 : (int)Math.Ceiling(paddle.transform.position.y / gridSize);
        return PadY;
    }
    public void SetPadY(int value)
    {
        if ((minPadY < value * gridSize) && (value * gridSize < maxPadY))
        {
            paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(PadX * gridSize, value * gridSize));
            PadY = value;
        }
        if (value * gridSize < minPadY)
        {
            paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(PadX * gridSize, minPadY * gridSize));
            PadY = Convert.ToInt32(minPadY / gridSize);
        }
        if (value * gridSize > maxPadY)
        {
            paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(PadX * gridSize, maxPadY * gridSize));
            PadY = Convert.ToInt32(maxPadY / gridSize);
        }
        if (PadX * gridSize < tableX && value * gridSize < tableY)
        {
            paddle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(PadX * gridSize, tableY));
            PadY = Convert.ToInt32(tableY / gridSize);
        }
    }
    public int GetPadRot()
    {
        //PadRot = ballState == BallState.Irrelevant ? 0 : (int)(Math.Floor(paddle.GetComponent<Rigidbody2D>().transform.rotation.z / angleStep));
        return PadRot;
    }
    public void SetPadRot(int value)
    {
        if ((minPadAngle < value * angleStep) && (value * angleStep < maxPadAngle))
        {
            PadRot = value;
            paddle.GetComponent<Rigidbody2D>().MoveRotation(value * angleStep);
        }
    }
    public int GetPadBounciness()
    {
        //PadBounciness = ballState == BallState.Irrelevant ? 0 : (int)(Math.Floor(paddle.GetComponent<Rigidbody2D>().sharedMaterial.bounciness / bouncinessStep));
        return PadBounciness;
    }
    public void SetPadBounciness(int value)
    {
        if ((minPadBounciness < value * bouncinessStep) && (value * bouncinessStep < maxPadBounciness))
        {
            PadBounciness = value;
            paddle.GetComponent<Rigidbody2D>().sharedMaterial.bounciness = PadBounciness * bouncinessStep;
        }
    }

    public State Apply(Action action)
    {
        State state = this;

        state.SetPadX(state.PadX + action.h);
        state.SetPadY(state.PadY + action.v);
        state.SetPadRot(state.PadRot + action.r);
        state.SetPadBounciness(state.PadBounciness + action.b);

        return state;
    }

    public override string ToString()
    {
        return $"{this.ballState},{this.BallX},{this.BallY},{this.BallDir},{this.PadX},{this.PadY},{this.PadRot},{this.PadBounciness}";
    }
}
