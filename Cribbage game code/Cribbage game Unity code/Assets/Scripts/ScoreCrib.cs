using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

//Same logic as scoring hand
public class ScoreCrib : MonoBehaviour
{
    public CardManager cardManager;
    public UpdateCribBoard updateCribBoard;

    public TMP_Text cribScoreTxt;

    public int numOf15;
    public int same2;
    public int same3;
    public int same4;
    public int runOf3;
    public int runOf4;
    public int runOf5;
    public int jack;

    private List<List<int[]>> tempSameValueList;

    private List<int[]> runList;
    private List<int[]> tempRunList;

    private int sum;
    private int runScore;
    private int score;

    private bool runFound;

    public int Score15()
    {
        numOf15 = 0;

        foreach (List<int[]> i in cardManager.cribCombinations)
        {
            foreach (int[] j in i)
            {
                sum = 0;

                for (int k = 0; k < j.Length; k++)
                {
                    if (j[k] > 10)
                    {
                        sum += 10;
                    }
                    else
                    {
                        sum += j[k];
                    }
                }

                if (sum == 15) numOf15++;
            }
        }

        if (numOf15 > 0) cribScoreTxt.text += $"{numOf15} 15 (+{numOf15 * 2})<br>";

        return numOf15 * 2;
    }

    public int ScoreSameValue()
    {
        same2 = 0;
        same3 = 0;
        same4 = 0;
        tempSameValueList = new List<List<int[]>>(cardManager.cribCombinations);

        foreach (int[] i in tempSameValueList[2])
        {
            if (i.Distinct().Count() == 1)
            {
                same4++;

                foreach (int[] j in tempSameValueList[1])
                {
                    if (j == i[..3]) cardManager.cribCombinations[1].Remove(j);
                }

                foreach (int[] j in tempSameValueList[0])
                {
                    if (j == i[..2]) cardManager.cribCombinations[0].Remove(j);
                }
            }
        }

        tempSameValueList = new List<List<int[]>>(cardManager.cribCombinations);

        foreach (int[] i in tempSameValueList[1])
        {
            if (i.Distinct().Count() == 1)
            {
                same3++;

                foreach (int[] j in cardManager.cribCombinations[0])
                {
                    if (j == i[..3]) cardManager.cribCombinations[0].Remove(j);
                }
            }
        }

        foreach (int[] i in cardManager.cribCombinations[0])
        {
            if (i.Distinct().Count() == 1) same2++;
        }

        if (same4 > 0) cribScoreTxt.text += $"{same4} Double pair royale (+{same4 * 12})<br>";

        if (same3 > 0) cribScoreTxt.text += $"{same3} Pair royale (+{same3 * 6})<br>";

        if (same2 > 0) cribScoreTxt.text += $"{same2} Pair (+{same2 * 2})<br>";

        return same4 * 12 + same3 * 6 + same2 * 2;
    }

    public int ScoreRun()
    {
        runList = new List<int[]>();
        runOf3 = 0;
        runOf4 = 0;
        runOf5 = 0;
        runScore = 0;

        for (int i = 3; i > 0; i--)
        {
            foreach (int[] j in cardManager.cribCombinations[i])
            {
                runFound = true;
                Array.Sort(j);

                for (int k = 0; k < j.Length - 1; k++)
                {
                    if (j[k] + 1 != j[k + 1])
                    {
                        runFound = false;
                        break;
                    }
                }

                if (runFound) runList.Add(j);
            }
        }

        tempRunList = new List<int[]>(runList);

        foreach (int[] i in tempRunList)
        {
            foreach (int[] j in tempRunList)
            {
                if ((!j.Except(i).Any()) && (!j.SequenceEqual(i))) runList.Remove(j);
            }
        }

        foreach (int[] i in runList)
        {
            if (i.Length == 3)
            {
                runOf3++;
                runScore += 3;
            }
            else if (i.Length == 4)
            {
                runOf4++;
                runScore += 4;
            }
            else
            {
                runOf5++;
                runScore += 5;
            }
        }

        if (runOf5 > 0) cribScoreTxt.text += $"{runOf5} Run of 5 (+{runOf5 * 5})<br>";

        if (runOf4 > 0) cribScoreTxt.text += $"{runOf4} Run of 4 (+{runOf4 * 4})<br>";

        if (runOf3 > 0) cribScoreTxt.text += $"{runOf3} Run of 3 (+{runOf3 * 3})<br>";

        return runScore;
    }

    public int ScoreFlush()
    {
        if (cardManager.scoreCrib[0][0] == cardManager.scoreCrib[1][0] && cardManager.scoreCrib[1][0] == cardManager.scoreCrib[2][0] 
            && cardManager.scoreCrib[2][0] == cardManager.scoreCrib[3][0] && cardManager.scoreCrib[3][0] == cardManager.scoreCrib[4][0])
        {
            cribScoreTxt.text += $"Five card flush (+5)<br>";

           return 5;
        }
        else if (cardManager.scoreCrib[0][0] == cardManager.scoreCrib[1][0] && cardManager.scoreCrib[1][0] == cardManager.scoreCrib[2][0] 
            && cardManager.scoreCrib[2][0] == cardManager.scoreCrib[3][0])
        {
            cribScoreTxt.text += $"Four card flush (+4)<br>";

            return 4;
        }

        return 0;
    }

    public int ScoreJack()
    {
        jack = 0;

        for (int i = 0; i < cardManager.scoreCrib.Count - 1; i++)
        {
            if (cardManager.scoreCrib[i].Substring(1, 2) == "11" && cardManager.scoreCrib[i][0] == cardManager.startingCard[0]) jack++;
        }

        if (jack > 0) cribScoreTxt.text += $"{jack} One for his nob (+{jack})<br>";

        return jack;
    }

    public int ScoreAll(bool own)
    {
        score = 0;
        score = Score15() + ScoreSameValue() + ScoreRun() + ScoreFlush() + ScoreJack();

        updateCribBoard.UpdateScore(score, own);

        if (own)
        {
            cribScoreTxt.color = Color.cyan;
        }
        else
        {
            cribScoreTxt.color = Color.red;
        }

        return score;
    }
}