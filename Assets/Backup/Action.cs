using System.Collections.Generic;

namespace Backup
{


    public struct Action
    {
        public int h;
        public int v;
        public int r;
        public int b;

        //public Action()
        //{
        //    h = UnityEngine.Random.Range(-1, 2);
        //    v = UnityEngine.Random.Range(-1, 2);
        //    r = UnityEngine.Random.Range(-1, 2);
        //    b = UnityEngine.Random.Range(-1, 2);
        //}

        public Action NextRandom()
        {
            h = UnityEngine.Random.Range(-1, 2);
            v = UnityEngine.Random.Range(-1, 2);
            r = UnityEngine.Random.Range(-1, 2);
            b = UnityEngine.Random.Range(-1, 2);
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
            return "Action=[" + string.Join(",", h, v, r, b) + "]";
        }
    }
}