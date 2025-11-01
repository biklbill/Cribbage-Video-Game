using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendToCrib : MonoBehaviour
{
    public GameLoop gameLoop;
    public CardManager cardManager;

    public List<GameObject> crib;
    public List<GameObject> player1Crib;
    public List<GameObject> player2Crib;

    public GameObject sendToCribBtn;
    public GameObject cribArea;

    public bool ownCribFilled;

    public void MoveCards()
    {
        if (DataManager.isHost)
        {
            foreach (GameObject i in cardManager.player1Hand)
            {
                if (i.GetComponent<CardInfo>().selected)
                {
                    //Set the selected card's parent to the crib area to move it
                    i.transform.SetParent(cribArea.transform, false);
                    i.GetComponent<Image>().sprite = i.GetComponent<CardInfo>().cardBack;
                    //Record the card
                    crib.Add(i);
                    cardManager.originalCrib.Add(i.name.Substring(0, 3));
                    player1Crib.Add(i);
                    cardManager.scoreCrib.Add(i.name.Substring(0, 3));
                    cardManager.scoreCribValue.Add(Convert.ToInt16(i.name.Substring(1, 2)));
                }
            }

            foreach (GameObject i in player1Crib)
            {
                //Remove the card from hand
                cardManager.player1Hand.Remove(i);
                cardManager.originalPlayer1Hand.Remove(i.name.Substring(0, 3));
                cardManager.scorePlayer1Hand.Remove(i.name.Substring(0, 3));
                cardManager.scorePlayer1HandValue.Remove(Convert.ToInt16(i.name.Substring(1, 2)));
            }
        }
        //Same logic has player 1
        else
        {
            foreach (GameObject i in cardManager.player2Hand)
            {
                if (i.GetComponent<CardInfo>().selected)
                {
                    i.transform.SetParent(cribArea.transform, false);
                    i.GetComponent<Image>().sprite = i.GetComponent<CardInfo>().cardBack;
                    crib.Add(i);
                    cardManager.originalCrib.Add(i.name.Substring(0, 3));
                    player2Crib.Add(i);
                    cardManager.scoreCrib.Add(i.name.Substring(0, 3));
                    cardManager.scoreCribValue.Add(Convert.ToInt16(i.name.Substring(1, 2)));
                }
            }

            foreach (GameObject i in player2Crib)
            {
                cardManager.player2Hand.Remove(i);
                cardManager.originalPlayer2Hand.Remove(i.name.Substring(0, 3));
                cardManager.scorePlayer2Hand.Remove(i.name.Substring(0, 3));
                cardManager.scorePlayer2HandValue.Remove(Convert.ToInt16(i.name.Substring(1, 2)));
            }
        }

        gameLoop.numCardsSelected = 0;
        ownCribFilled = true;
    }
}