using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ScoreHand : MonoBehaviour
{
    public CardManager cardManager;
    public UpdateCribBoard updateCribBoard;

    public TMP_Text ownHandScoreTxt;
    public TMP_Text opponentHandScoreTxt;

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

    public int Score15(int player, bool own)
    {
        numOf15 = 0;

        //Score player 1
        if (player == 1)
        {
            //i is a 2d array with combinations all of same length
            foreach (List<int[]> i in cardManager.player1Combinations)
            {
                //j is one combination
                foreach (int[] j in i)
                {
                    sum = 0;

                    //Add all values in the combination
                    for (int k = 0; k < j.Length; k++)
                    {
                        //Jack, Queen, King have a value of 10
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
        }
        //Same logic as player 1 but for player 2
        else
        {
            foreach (List<int[]> i in cardManager.player2Combinations)
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
        }

        //Display appropriate scoring text for both players
        if (own)
        {
            if (numOf15 > 0) ownHandScoreTxt.text += $"{numOf15} 15 (+{numOf15 * 2})<br>";
        }
        else
        {
            if (numOf15 > 0) opponentHandScoreTxt.text += $"{numOf15} 15 (+{numOf15 * 2})<br>";
        }

        return numOf15 * 2;
    }

    //Score cards with the same value, such as 5, 5, 5, 5
    public int ScoreSameValue(int player, bool own)
    {
        same2 = 0;
        same3 = 0;
        same4 = 0;

        //Score same value for player 1
        if (player == 1)
        {
            //Create a copy of the 3d array player1Combinations
            tempSameValueList = new List<List<int[]>>(cardManager.player1Combinations);

            //2d array that contains all combinations with length of 4
            foreach (int[] i in tempSameValueList[2])
            {
                //Check if every element is the same in sublist i
                if (i.Distinct().Count() == 1)
                {
                    same4++;

                    //Remove combinations with less length but same card value to avoid excess scoring, as only the longest length scores
                    foreach (int[] j in tempSameValueList[1])
                    {
                        if (j == i[..3]) cardManager.player1Combinations[1].Remove(j);
                    }

                    foreach (int[] j in tempSameValueList[0])
                    {
                        if (j == i[..2]) cardManager.player1Combinations[0].Remove(j);
                    }
                }
            }

            //Recreate the copy
            tempSameValueList = new List<List<int[]>>(cardManager.player1Combinations);

            //Same as scoring length 4 but length 3
            foreach (int[] i in tempSameValueList[1])
            {
                if (i.Distinct().Count() == 1)
                {
                    same3++;

                    foreach (int[] j in tempSameValueList[0])
                    {
                        if (j == i[..3]) cardManager.player1Combinations[0].Remove(j);
                    }
                }
            }

            //Same as scoring length 3 but length 2
            foreach (int[] i in cardManager.player1Combinations[0])
            {
                if (i.Distinct().Count() == 1) same2++;
            }
        }
        //Same as player 1 but player 2
        else
        {
            tempSameValueList = new List<List<int[]>>(cardManager.player2Combinations);

            foreach (int[] i in tempSameValueList[2])
            {
                if (i.Distinct().Count() == 1)
                {
                    same4++;

                    foreach (int[] j in tempSameValueList[1])
                    {
                        if (j == i[..3]) cardManager.player2Combinations[1].Remove(j);
                    }

                    foreach (int[] j in tempSameValueList[0])
                    {
                        if (j == i[..2]) cardManager.player2Combinations[0].Remove(j);
                    }
                }
            }

            tempSameValueList = new List<List<int[]>>(cardManager.player2Combinations);

            foreach (int[] i in tempSameValueList[1])
            {
                if (i.Distinct().Count() == 1)
                {
                    same3++;

                    foreach (int[] j in tempSameValueList[0])
                    {
                        if (j == i[..3]) cardManager.player2Combinations[0].Remove(j);
                    }
                }
            }

            foreach (int[] i in cardManager.player2Combinations[0])
            {
                if (i.Distinct().Count() == 1) same2++;
            }
        }

        //Display appropriate scoring text for both players
        if (own)
        {
            if (same4 > 0) ownHandScoreTxt.text += $"{same4} Double pair royale (+{same4 * 12})<br>";

            if (same3 > 0) ownHandScoreTxt.text += $"{same3} Pair royale (+{same3 * 6})<br>";

            if (same2 > 0) ownHandScoreTxt.text += $"{same2} Pair (+{same2 * 2})<br>";
        }
        else
        {
            if (same4 > 0) opponentHandScoreTxt.text += $"{same4} Double pair royale (+{same4 * 12})<br>";

            if (same3 > 0) opponentHandScoreTxt.text += $"{same3} Pair royale (+{same3 * 6})<br>";

            if (same2 > 0) opponentHandScoreTxt.text += $"{same2} Pair (+{same2 * 2})<br>";
        }

        //Return the scores
        return same4 * 12 + same3 * 6 + same2 * 2;
    }

    public int ScoreRun(int player, bool own)
    {
        runList = new List<int[]>();
        runOf3 = 0;
        runOf4 = 0;
        runOf5 = 0;
        runScore = 0;

        //Score run for player 1
        if (player == 1)
        {
            //Search runs from long to short
            for (int i = 3; i > 0; i--)
            {
                //j is a possible combination
                foreach (int[] j in cardManager.player1Combinations[i])
                {
                    //Assume a run is found at first
                    runFound = true;
                    //Make the combination in order
                    Array.Sort(j);

                    //Loops through the combination
                    for (int k = 0; k < j.Length - 1; k++)
                    {
                        //Check if the next value is the current value + 1
                        if (j[k] + 1 != j[k + 1])
                        {
                            //If not the combination isn't a run and breaks
                            runFound = false;
                            break;
                        }
                    }

                    //If loop ended without break then run is found and added to runList
                    if (runFound) runList.Add(j);
                }
            }
        }
        //Same as player 1 but player 2
        else
        {
            for (int i = 3; i > 0; i--)
            {
                foreach (int[] j in cardManager.player2Combinations[i])
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
        }

        //Create a copy of the run list
        tempRunList = new List<int[]>(runList);

        //Use 2 pointers in a for loop and nested for loop to compare every element
        foreach (int[] i in tempRunList)
        {
            foreach (int[] j in tempRunList)
            {
                //Remove the run if it is a subrun of another run
                if ((!j.Except(i).Any()) && (!j.SequenceEqual(i))) runList.Remove(j);
            }
        }

        //Score the remaining runs by length after the subruns are removed
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

        //Display appropriate scoring text for both players
        if (own)
        {
            if (runOf5 > 0) ownHandScoreTxt.text += $"{runOf5} Run of 5 (+{runOf5 * 5})<br>";

            if (runOf4 > 0) ownHandScoreTxt.text += $"{runOf4} Run of 4 (+{runOf4 * 4})<br>";

            if (runOf3 > 0) ownHandScoreTxt.text += $"{runOf3} Run of 3 (+{runOf3 * 3})<br>";
        }
        else
        {
            if (runOf5 > 0) opponentHandScoreTxt.text += $"{runOf5} Run of 5 (+{runOf5 * 5})<br>";

            if (runOf4 > 0) opponentHandScoreTxt.text += $"{runOf4} Run of 4 (+{runOf4 * 4})<br>";

            if (runOf3 > 0) opponentHandScoreTxt.text += $"{runOf3} Run of 3 (+{runOf3 * 3})<br>";
        }

        return runScore;
    }

    public int ScoreFlush(int player, bool own)
    {
        //Score flush for player 1
        if (player == 1)
        {
            //Compare every card in hand plus starting card
            if (cardManager.scorePlayer1Hand[0][0] == cardManager.scorePlayer1Hand[1][0] && cardManager.scorePlayer1Hand[1][0] == cardManager.scorePlayer1Hand[2][0] 
                && cardManager.scorePlayer1Hand[2][0] == cardManager.scorePlayer1Hand[3][0] && cardManager.scorePlayer1Hand[3][0] == cardManager.scorePlayer1Hand[4][0])
            {
                if (own)
                {
                    ownHandScoreTxt.text += $"Five card flush (+5)<br>";
                }
                else
                {
                    opponentHandScoreTxt.text += $"Five card flush (+5)<br>";
                }

                return 5;
            }
            //Compare every card in hand only
            else if (cardManager.scorePlayer1Hand[0][0] == cardManager.scorePlayer1Hand[1][0] && cardManager.scorePlayer1Hand[1][0] == cardManager.scorePlayer1Hand[2][0] 
                && cardManager.scorePlayer1Hand[2][0] == cardManager.scorePlayer1Hand[3][0])
            {
                if (own)
                {
                    ownHandScoreTxt.text += $"Four card flush (+4)<br>";
                }
                else
                {
                    opponentHandScoreTxt.text += $"Four card flush (+4)<br>";
                }

                return 4;
            }
        }
        //Same as scoring for player 1 but player 2
        else
        {
            if (cardManager.scorePlayer2Hand[0][0] == cardManager.scorePlayer2Hand[1][0] && cardManager.scorePlayer2Hand[1][0] == cardManager.scorePlayer2Hand[2][0] 
                && cardManager.scorePlayer2Hand[2][0] == cardManager.scorePlayer2Hand[3][0] && cardManager.scorePlayer2Hand[3][0] == cardManager.scorePlayer2Hand[4][0])
            {
                if (own)
                {
                    ownHandScoreTxt.text += $"Five card flush (+5)<br>";
                }
                else
                {
                    opponentHandScoreTxt.text += $"Five card flush (+5)<br>";
                }

                return 5;
            }
            else if (cardManager.scorePlayer2Hand[0][0] == cardManager.scorePlayer2Hand[1][0] && cardManager.scorePlayer2Hand[1][0] == cardManager.scorePlayer2Hand[2][0] 
                && cardManager.scorePlayer2Hand[2][0] == cardManager.scorePlayer2Hand[3][0])
            {
                if (own)
                {
                    ownHandScoreTxt.text += $"Four card flush (+4)<br>";
                }
                else
                {
                    opponentHandScoreTxt.text += $"Four card flush (+4)<br>";
                }

                return 4;
            }
        }

        //Return 0 if no flushes are found
        return 0;
    }

    //Score nobs
    public int ScoreJack(int player, bool own)
    {
        jack = 0;

        //Score player 1
        if (player == 1)
        {
            for (int i = 0; i < cardManager.scorePlayer1Hand.Count - 1; i++)
            {
                //Score 1 point if a card is 11 and has the same suit as starting card
                if (cardManager.scorePlayer1Hand[i].Substring(1, 2) == "11" && cardManager.scorePlayer1Hand[i][0] == cardManager.startingCard[0]) jack++;
            }
        }
        //Same as player 1 but player 2
        else
        {
            for (int i = 0; i < cardManager.scorePlayer2Hand.Count - 1; i++)
            {
                if (cardManager.scorePlayer2Hand[i].Substring(1, 2) == "11" && cardManager.scorePlayer2Hand[i][0] == cardManager.startingCard[0]) jack++;
            }
        }

        //Display appropriate scoring text for both players
        if (own)
        {
            if (jack > 0) ownHandScoreTxt.text += $"{jack} One for his nob (+{jack})<br>";
        }
        else
        {
            if (jack > 0) opponentHandScoreTxt.text += $"{jack} One for his nob (+{jack})<br>";
        }

        return jack;
    }

    public int ScoreAll(int player, bool own)
    {
        //Reset score
        score = 0;
        //Add scores from all methods together
        score = Score15(player, own) + ScoreSameValue(player, own) + ScoreRun(player, own) + ScoreFlush(player, own) + ScoreJack(player, own);

        //Display score on Cribbage board
        updateCribBoard.UpdateScore(score, own);

        return score;
    }
}