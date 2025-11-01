using System;
using UnityEngine;
using UnityEngine.UI;

public class DealCards : MonoBehaviour
{
    public CardManager cardManager;

    private GameObject cardChosen;

    public void Deal()
    {
        //Loop 12 times for 12 cards, 6 for each player
        for (int i = 0; i < 12; i++)
        {
            //Use random to simulate dealing cards
            cardChosen = cardManager.cardPile[UnityEngine.Random.Range(0, cardManager.cardPile.Count)];
            cardManager.cardsDealt.Add(cardChosen);

            //Remove cards picked from list so same card can't be picked twice
            cardManager.cardPile.Remove(cardChosen);

            //First 6 cards goes to dealer
            if (i < 6)
            {
                //Create the card on screen
                cardChosen = Instantiate(cardChosen, cardManager.ownHand.transform);
                cardChosen.GetComponent<CardInfo>().ownHand = true;

                //Add cards to own hand
                if (DataManager.isHost)
                {
                    cardManager.player1Hand.Add(cardChosen);
                    cardManager.originalPlayer1Hand.Add(cardChosen.name.Substring(0, 3));
                    cardManager.scorePlayer1Hand.Add(cardChosen.name.Substring(0, 3));
                    cardManager.scorePlayer1HandValue.Add(Convert.ToInt16(cardChosen.name.Substring(1, 2)));
                }
                else
                {
                    cardManager.player2Hand.Add(cardChosen);
                    cardManager.originalPlayer2Hand.Add(cardChosen.name.Substring(0, 3));
                    cardManager.scorePlayer2Hand.Add(cardChosen.name.Substring(0, 3));
                    cardManager.scorePlayer2HandValue.Add(Convert.ToInt16(cardChosen.name.Substring(1, 2)));
                }
            }
            //Last 6 cards goes to non dealer
            else
            {
                //Create the card on screen
                cardChosen = Instantiate(cardChosen, cardManager.opponentHand.transform);
                cardChosen.GetComponent<CardInfo>().ownHand = false;
                cardChosen.GetComponent<Image>().sprite = cardChosen.GetComponent<CardInfo>().cardBack;

                //Add cards to opponent hand
                if (DataManager.isHost)
                {
                    cardManager.player2Hand.Add(cardChosen);
                    cardManager.originalPlayer2Hand.Add(cardChosen.name.Substring(0, 3));
                    cardManager.scorePlayer2Hand.Add(cardChosen.name.Substring(0, 3));
                    cardManager.scorePlayer2HandValue.Add(Convert.ToInt16(cardChosen.name.Substring(1, 2)));
                }
                else
                {
                    cardManager.player1Hand.Add(cardChosen);
                    cardManager.originalPlayer1Hand.Add(cardChosen.name.Substring(0, 3));
                    cardManager.scorePlayer1Hand.Add(cardChosen.name.Substring(0, 3));
                    cardManager.scorePlayer1HandValue.Add(Convert.ToInt16(cardChosen.name.Substring(1, 2)));
                }
            }
        }
    }
}