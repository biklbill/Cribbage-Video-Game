using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreInPlay : MonoBehaviour
{
    public PlayCards playCards;
    public GameLoop gameLoop;
    public CardManager cardManager;
    public UpdateCribBoard updateCribBoard;

    private List<int> run;

    private int justPlayedValue;
    private int sameValue;
    private int length;
    private int score;

    private bool runFound;

    //Score 15 and 31
    public int ScoreReach()
    {
        //Check the total value
        if (playCards.totalCardValue == 15)
        {
            gameLoop.scoringTxt.text += "15 (+2)<br>";
            return 2;
        }
        else if (playCards.totalCardValue == 31)
        {
            gameLoop.scoringTxt.text += "31 (+2)<br>";
            return 2;
        }

        return 0;
    }

    public int ScoreSameValue()
    {
        sameValue = 0;
        justPlayedValue = cardManager.playedCardsValue[cardManager.playedCardsValue.Count - 1];

        for (int i = cardManager.playedCardsValue.Count - 2; i >= 0; i--)
        {
            if (cardManager.playedCardsValue[i] == justPlayedValue)
            {
                sameValue++;
            }
            else
            {
                //Break out of loop if the card before isn't the same
                break;
            }
        }

        //Score based on how many same value cards in a row
        if (sameValue >= 3)
        {
            gameLoop.scoringTxt.text += "Double pair royale (+12)<br>";
            return 12;
        }
        else if (sameValue == 2)
        {
            gameLoop.scoringTxt.text += "Pair royale (+6)<br>";
            return 6;
        }
        else if (sameValue == 1)
        {
            gameLoop.scoringTxt.text += "Pair (+2)<br>";
            return 2;
        }

        return 0;
    }

    public int ScoreRun()
    {
        length = cardManager.playedCardsValue.Count;

        if (length < 3) return 0;

        run = new List<int>();

        //i is how long the run is
        for (int i = length; i > 2; i--)
        {
            runFound = true;
            run = cardManager.playedCardsValue.GetRange(length - i, i);
            run.Sort();

            //j searches backward to see if the elements are consecutive
            for (int j = 0; j < i - 1; j++)
            {
                if (run[j] + 1 != run[j + 1])
                {
                    //Break if the card before isn't consecutive
                    runFound = false;
                    break;
                }
            }

            if (runFound) break;
        }

        if (runFound)
        {
            gameLoop.scoringTxt.text += $"Run of {run.Count} (+{run.Count})<br>";
            return run.Count;
        }
        
        return 0;
    }

    //Return the player who played last to score Go
    public int ScoreLastPlayed()
    {
        gameLoop.scoringTxt.text += "Go (+1)<br>";

        if (cardManager.originalPlayer1Hand.Contains(cardManager.playedCards.Last().name.Substring(0, 3))) return 1;

        return 2;
    }

    //Combine all function that scores
    public int ScoreAll(bool own)
    {
        score = 0;
        gameLoop.scoringTxt.text = "";

        score = ScoreReach() + ScoreSameValue() + ScoreRun();

        updateCribBoard.UpdateScore(score, own);

        //Change the scoring text's colour to reflect if it's own or opponent
        if (own)
        {
            gameLoop.scoringTxt.color = Color.cyan;
        }
        else
        {
            gameLoop.scoringTxt.color = Color.red;
        }

        return score;
    }
}