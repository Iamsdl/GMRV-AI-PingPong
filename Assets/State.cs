using System;
using UnityEngine;

public struct State
{
    //TODO adauga angle si bounciness in state
    //TODO action modifica state

    public State(bool mustHit = false, float ballX = 0, float ballY = 0, Vector2 ballVelocity = new Vector2())
    {
        MustHit = mustHit;
        BallX = 0;
        BallY = 0;
        BallDir = 0;
        BallSpeed = 0;

        SetBallX(ballX);
        SetBallY(ballY);
        SetBallDir(ballVelocity);
        SetBallSpeed(ballVelocity);
    }
    
    public const float gridSize = 0.5f;
    public const float ballAngleStep = 30;

    private bool MustHit;
    private int BallX;
    private int BallY;
    private int BallDir;
    private int BallSpeed;

    public int GetBallX()
    {
        return BallX;
    }

    public void SetBallX(float value)
    {
        BallX = Convert.ToInt32(Math.Floor(value / gridSize));
    }

    public int GetBallY()
    {
        return BallY;
    }

    public void SetBallY(float value)
    {
        int test = Convert.ToInt32(Math.Floor(value / gridSize));
        BallY = test;
    }

    public int GetBallDir()
    {
        return BallDir;
    }

    public void SetBallDir(Vector2 value)
    {
        BallDir = MustHit ? (int)(Math.Floor(Math.Asin(value.normalized.y) * 180 / Math.PI / ballAngleStep)) : 0;
    }

    public int GetBallSpeed()
    {
        return BallSpeed;
    }

    public void SetBallSpeed(Vector2 value)
    {
        BallSpeed = MustHit ? (int)(Math.Round(value.magnitude)) : 0;
    }

    //public State Apply(Action action)
    //{
    //    return new State();
    //}

    public override string ToString()
    {
        return $"{this.MustHit},{this.BallX},{this.BallY},{this.BallDir}, {this.BallSpeed}";
    }

    public void FromString(string stateString)
    {
        string[] text = stateString.Split(',');

        MustHit = Convert.ToBoolean(text[0]);
        BallX = Convert.ToInt32(text[1]);
        BallY = Convert.ToInt32(text[2]);
        BallDir = Convert.ToInt32(text[3]);
        BallSpeed = Convert.ToInt32(text[4]);
    }
}
