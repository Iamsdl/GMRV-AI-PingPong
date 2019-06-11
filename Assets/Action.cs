﻿using System;
using System.Collections.Generic;

public struct Action
{
    private static List<Action> allPossibleActions;
    public static List<Action> AllPossibleActions
    {
        get
        {
            if (allPossibleActions == null)
            {
                allPossibleActions = new List<Action>();
                for (float angle = minPadAngle; angle <= maxPadAngle; angle += padAngleStep)
                {
                    for (float bounciness = minPadBounciness; bounciness <= maxPadBounciness; bounciness += bouncinessStep)
                    {
                        allPossibleActions.Add(new Action() { Angle = angle, Bounciness = bounciness });
                    }
                }
                allPossibleActions.Add(new Action() { Angle = 0, Bounciness = -1 });
            }
            return allPossibleActions;
        }
    }

    public const float minPadAngle = -90;
    public const float maxPadAngle = 90;
    public const float minPadBounciness = 0;
    public const float maxPadBounciness = 4;

    public const float padAngleStep = 30;
    public const float bouncinessStep = 0.5f;

    public float Angle;
    public float Bounciness;

    public Action NextRandom()
    {
        Angle = (int)Math.Round(UnityEngine.Random.Range(minPadAngle / padAngleStep, maxPadAngle / padAngleStep + 1)) * padAngleStep;
        Bounciness = (int)Math.Round(UnityEngine.Random.Range(minPadBounciness / bouncinessStep, maxPadBounciness / bouncinessStep + 1)) * bouncinessStep;
        return this;
    }

    internal Action NextBest(Dictionary<Action, float> a_table)
    {
        Action action = new Action();
        float max = float.MinValue;
        foreach (var item in a_table)
        {
            if (item.Value >= max)
            {
                action = item.Key;
                max = item.Value;
            }
        }
        return action;
    }

    public override string ToString()
    {
        return "Action=[" + string.Join(",", Angle, Bounciness) + "]";
    }
}
