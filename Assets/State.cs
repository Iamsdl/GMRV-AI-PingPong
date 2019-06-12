using System;
using UnityEngine;

public struct State
{
    public static State waitingState = new State(-1, Vector2.zero);

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
        SetBallAngle(ballVelocity);
        SetSpeed(ballVelocity);
    }

    public const float gridSize = 0.1f;
    public const int timeStep = 1;
    public const float ballAngleStep = 30;

    private int X;
    private int BallAngle;
    private int Speed;
    private int Time;
    private float PadAngle;
    private float Bounciness;

    public float GetBounciness()
    {
        return Bounciness;
    }

    public float GetPadAngle()
    {
        return PadAngle;
    }

    public int GetX()
    {
        return X;
    }

    public void SetX(float value)
    {
        X = Convert.ToInt32(Mathf.Floor(value / gridSize));
    }

    public int GetAngle()
    {
        return BallAngle;
    }

    public void SetBallAngle(Vector2 value)
    {
        BallAngle = (int)(Mathf.Floor(Mathf.Asin(value.normalized.y) * 180 / Mathf.PI / ballAngleStep));
    }

    public int GetSpeed()
    {
        return Speed;
    }

    public void SetSpeed(Vector2 value)
    {
        Speed = (int)(Mathf.Floor(0.5f+value.magnitude));
    }

    public int GetTime()
    {
        return Time;
    }

    public void SetTime(float value)
    {
        Time = Convert.ToInt32(Mathf.Floor(value * 30 / timeStep));
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
        Speed = Convert.ToInt32(text[2]);
        Time = Convert.ToInt32(text[3]);
        PadAngle = Convert.ToSingle(text[4]);
        Bounciness = Convert.ToSingle(text[5]);
    }
}
