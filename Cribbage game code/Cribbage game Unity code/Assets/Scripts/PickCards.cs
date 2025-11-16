using System;
using System.Linq;
using UnityEngine;

public class PickCards : MonoBehaviour
{
    private GameObject logicManager;
    private GameObject cardPicked;

    private int randNum;

    //Triggers when clicked
    private void OnMouseDown()
    {
        //Find and assigns the game object named Logic Manager
        logicManager = GameObject.Find("Logic Manager");

        //Use random to pick a random card
        randNum = UnityEngine.Random.Range(0, logicManager.GetComponent<CardManager>().cardPile.Count);
        cardPicked = logicManager.GetComponent<CardManager>().cardPile[randNum];    

        //Visual update to show card has been picked and which card it is
        Destroy(GameObject.Find("Card Spread(Clone)"));

        if (logicManager.GetComponent<GameLoop>().gamePhase == "decide dealer")
        {
            Instantiate(cardPicked, GameObject.Find("Own Hand").transform);

            //Send card to the other player to update their end
            if (DataManager.isHost)
            {
                logicManager.GetComponent<Player1Network>().AddOpponentPickedCard(cardPicked.name);
                logicManager.GetComponent<DecideDealer>().player1PickedCard = cardPicked.name;
                logicManager.GetComponent<DecideDealer>().player1Picked = true;
            }
            else
            {
                logicManager.GetComponent<Player2Network>().AddOpponentPickedCard(cardPicked.name);
                logicManager.GetComponent<DecideDealer>().player2PickedCard = cardPicked.name;
                logicManager.GetComponent<DecideDealer>().player2Picked = true;
            }
        }
        else
        {
            Instantiate(cardPicked, GameObject.Find("Starting Card").transform);
            logicManager.GetComponent<CardManager>().cardPile.Remove(cardPicked);
            logicManager.GetComponent<CardManager>().startingCard = cardPicked.name;
            logicManager.GetComponent<CardManager>().startingCardValue = Convert.ToInt16(cardPicked.name.Substring(1, 2));
            logicManager.GetComponent<CardManager>().scorePlayer1Hand.Add(cardPicked.name);
            logicManager.GetComponent<CardManager>().scorePlayer2Hand.Add(cardPicked.name);
            logicManager.GetComponent<CardManager>().scoreCrib.Add(cardPicked.name);
            logicManager.GetComponent<CardManager>().scorePlayer1HandValue.Add(Convert.ToInt16(cardPicked.name.Substring(1, 2)));
            logicManager.GetComponent<CardManager>().scorePlayer2HandValue.Add(Convert.ToInt16(cardPicked.name.Substring(1, 2)));
            logicManager.GetComponent<CardManager>().scoreCribValue.Add(Convert.ToInt16(cardPicked.name.Substring(1, 2)));

            if (DataManager.isHost)
            {
                logicManager.GetComponent<Player1Network>().AddStartingCard(cardPicked.name);

                if (cardPicked.name.Substring(1, 2) == "11")
                {
                    logicManager.GetComponent<GameLoop>().player1Score += 2;
                    logicManager.GetComponent<GameLoop>().ownScoreTxt.text  = logicManager.GetComponent<GameLoop>().player1Score.ToString();
                    logicManager.GetComponent<UpdateCribBoard>().UpdateScore(2, true);
                    logicManager.GetComponent<GameLoop>().scoringTxt.color = Color.cyan;
                    logicManager.GetComponent<GameLoop>().scoringTxt.text += "Two for his heels (+2)<br>";
                }
            }
            else
            {
                logicManager.GetComponent<Player2Network>().AddStartingCard(cardPicked.name);

                if (cardPicked.name.Substring(1, 2) == "11")
                {
                    logicManager.GetComponent<GameLoop>().player2Score += 2;
                    logicManager.GetComponent<GameLoop>().ownScoreTxt.text = logicManager.GetComponent<GameLoop>().player2Score.ToString();
                    logicManager.GetComponent<UpdateCribBoard>().UpdateScore(2, true);
                    logicManager.GetComponent<GameLoop>().scoringTxt.color = Color.cyan;
                    logicManager.GetComponent<GameLoop>().scoringTxt.text += "Two for his heels (+2)<br>";
                }  
            }

            logicManager.GetComponent<GameLoop>().gamePhase = "play card";
        }
    }
}