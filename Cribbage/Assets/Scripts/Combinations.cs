using System.Collections.Generic;
using UnityEngine;

public class Combinations: MonoBehaviour
{
    public void GetCombinations(ref List<int[]> list, List<int> t, int n, int m, int[] b, int M)
    {
        for (int i = n; i >= m; i--)
        {
            b[m - 1] = i - 1;

            if (m > 1)
            {
                GetCombinations(ref list, t, i - 1, m - 1, b, M);
            }
            else
            {
                if (list == null) list = new List<int[]>();

                int[] temp = new int[M];

                for (int j = 0; j < b.Length; j++)
                {
                    temp[j] = t[b[j]];
                }

                list.Add(temp);
            }
        }
    }

    public List<int[]> GetCombination(List<int> t, int n)
    {
        if (t.Count < n) return null;

        int[] temp = new int[n];
        List<int[]> list = new List<int[]>();
        GetCombinations(ref list, t, t.Count, n, temp, n);

        return list;
    }
}