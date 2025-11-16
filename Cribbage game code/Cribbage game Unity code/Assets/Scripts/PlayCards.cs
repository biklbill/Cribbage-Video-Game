using System;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayCards : MonoBehaviour
{
    public GameLoop gameLoop;
    public SendToCrib sendToCrib;
    public CardManager cardManager;
    public ScoreInPlay scoreInPlay;

    public TMP_Text totalCardValueTxt;
    public TMP_Text ownScoreTxt;

    public GameObject playCardBtn;
    public GameObject playArea;
    public GameObject cardToPlay;

    public int totalCardValue;

    private int cardValue;
    private int checkCardValue;

    public void PlayCard()
    {
        if (DataManager.isHost)
        {
            //Get the card selected and extract the number value
            cardToPlay = cardManager.player1Hand.Where(obj => obj.GetComponent<CardInfo>().selected == true).SingleOrDefault();
            cardValue = Convert.ToInt16(cardToPlay.name.Substring(1, 2));

            //In Cribbage Jack, Queen, and King all have a value of 10
            if (cardValue > 10) cardValue = 10;

            //If the value after the card is played exceeds max then the play is disallowed
            if (totalCardValue + cardValue > 31) return;

            //Add played cards to lists for scoring
            cardManager.playedCards.Add(cardToPlay);
            cardManager.playedCardsValue.Add(Convert.ToInt16(cardToPlay.name.Substring(1, 2)));

            //Add the card's value to the total and prepare for opponent's turn
            totalCardValue += cardValue;
            totalCardValueTxt.text = Convert.ToString(totalCardValue);
            cardToPlay.transform.SetParent(playArea.transform, false);
            gameLoop.numCardsSelected = 0;
            cardManager.player1Hand.Remove(cardToPlay);

            //Add any points made by the play
            gameLoop.player1Score += scoreInPlay.ScoreAll(true);
            ownScoreTxt.text = Convert.ToString(gameLoop.player1Score);

            //Change turn
            gameLoop.playerTurn = 2;
            gameLoop.playerTurnTxt.text = DataManager.player2Name;
        }
        else
        {
            //Same logic as above but opposite player
            cardToPlay = cardManager.player2Hand.Where(obj => obj.GetComponent<CardInfo>().selected == true).SingleOrDefault();
            cardValue = Convert.ToInt16(cardToPlay.name.Substring(1, 2));

            if (cardValue > 10) cardValue = 10;

            if (totalCardValue + cardValue > 31) return;

            cardManager.playedCards.Add(cardToPlay);
            cardManager.playedCardsValue.Add(Convert.ToInt16(cardToPlay.name.Substring(1, 2)));

            totalCardValue += cardValue;
            totalCardValueTxt.text = Convert.ToString(totalCardValue);
            cardToPlay.transform.SetParent(playArea.transform, false);
            gameLoop.numCardsSelected = 0;
            cardManager.player2Hand.Remove(cardToPlay);

            gameLoop.player2Score += scoreInPlay.ScoreAll(true);
            ownScoreTxt.text = Convert.ToString(gameLoop.player2Score);

            gameLoop.playerTurn = 1;
            gameLoop.playerTurnTxt.text = DataManager.player1Name;
        }
    }

    public bool CheckOver31(List<GameObject> hand, int currentCardValue)
    {
        //Search every card in hand 
        foreach (GameObject card in hand)
        {
            checkCardValue = Convert.ToInt16(card.name.Substring(1, 2));

            if (checkCardValue > 10) checkCardValue = 10;

            //If there is a card that doesn't exceed max after being added return false
            if (checkCardValue + currentCardValue <= 31) return false;
        }

        //If every card exceeds max after being added return true
        return true;
    }
}