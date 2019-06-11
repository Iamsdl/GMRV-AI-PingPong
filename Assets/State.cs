using System;
using UnityEngine;

public struct State
{
    public State(float ballX, Vector2 ballVelocity)
    {
        X = -1;
        BallAngle = 0;
        Speed = 0;
        Time = 0;
        PadAngle = 0;
        Bounciness = -1;
        if (ballX < 0)
            return;
        SetX(ballX);
        SetAngle(ballVelocity);
        SetSpeed(ballVelocity);
    }

    public const float gridSize = 0.5f;
    public const int timeStep = 1;
    public const float ballAngleStep = 30;

    private int X;
    private int BallAngle;
    private int Speed;
    private int Time;
    private float PadAngle;
    private float Bounciness;

    public int GetX()
    {
        return X;
    }

    public void SetX(float value)
    {
        X = Convert.ToInt32(Math.Floor(value / gridSize));
    }

    public int GetAngle()
    {
        return BallAngle;
    }

    public void SetAngle(Vector2 value)
    {
        BallAngle = (int)(Math.Floor(Math.Asin(value.normalized.y) * 180 / Math.PI / ballAngleStep));
    }

    public int GetSpeed()
    {
        return Speed;
    }

    public void SetSpeed(Vector2 value)
    {
        Speed = (int)(Math.Round(value.magnitude));
    }

    public int GetTime()
    {
        return Time;
    }

    public void SetTime(float value)
    {
        Time = Convert.ToInt32(Math.Floor(value * 30 / timeStep));
    }

    public State Apply(Action a)
    {
        State state = this;
        if (a.Bounciness < 0)
        {
            if (state.X >= 0)
            {
                state.Time++;
            }
            return state;
        }
        state.Bounciness = a.Bounciness;
        state.PadAngle = a.Angle;
        return state;
    }

    public override string ToString()
    {
        return $"{this.X},{this.BallAngle},{this.Speed},{this.Time},{this.PadAngle},{this.Bounciness}";
    }

    public void FromString(string stateString)
    {
        string[] text = stateString.Split(',');

        X = Convert.ToInt32(text[0]);
        BallAngle = Convert.ToInt32(text[1]);
        Speed = Convert.ToInt32(text[3]);
        Time = Convert.ToInt32(text[4]);
        PadAngle = Convert.ToSingle(text[5]);
        Bounciness = Convert.ToSingle(text[6]);
    }
}
