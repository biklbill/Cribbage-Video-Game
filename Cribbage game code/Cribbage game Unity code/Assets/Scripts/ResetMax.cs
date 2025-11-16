using System;
using UnityEngine;

public class ResetMax : MonoBehaviour
{
    public GameLoop gameLoop;
    public PlayCards playCards;
    public CardManager cardManager;
    public DestroyChildObjects destroyChildObjects;

    public GameObject playArea;

    public void Reset()
    {
        //Reset total card value to 0
        playCards.totalCardValue = 0;
        playCards.totalCardValueTxt.text = Convert.ToString(playCards.totalCardValue);
        //Reset so that both players can play cards without exceeding max
        gameLoop.player1Over31 = false;
        gameLoop.player2Over31 = false;

        //Clear child objects
        destroyChildObjects.DestroyChild(playArea);

        //Reset played cards
        cardManager.playedCards.Clear();
        cardManager.playedCardsValue.Clear();
    }
}