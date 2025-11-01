using UnityEngine;
using Unity.Netcode;
using System.Linq;
using TMPro;
using System;
using UnityEngine.UI;

public class Player1Network : NetworkBehaviour
{
    public GameLoop gameLoop;
    public DecideDealer decideDealer;
    public PlayCards playCards;
    public SendToCrib sendToCrib;
    public CardManager cardManager;
    public ScoreInPlay scoreInPlay;
    public UpdateCribBoard updateCribBoard;

    public TMP_Text totalCardValueTxt;
    public TMP_Text opponentScoreTxt;

    public GameObject startCardArea;
    public GameObject ownHand;
    public GameObject opponentHand;
    public GameObject playArea;
    public GameObject cribArea;

    private GameObject card;
    private GameObject instCard;

    private int count;
    private int cardValue;

    public void AddOpponentPickedCard(string cardName)
    {
        //Only applies for player 1 so stop immediately if player is player 2
        if (!DataManager.isHost) return;

        //Call AddPickedCardClientRpc with passed parameter
        AddPickedCardClientRpc(cardName);
    }

    public void AddOpponentDealtCards()
    {
        if (!DataManager.isHost) return;

        //Call DealtCardsClientRpc for each card in hand
        foreach (GameObject card in cardManager.player2Hand.Concat(cardManager.player1Hand))
        {
            DealtCardsClientRpc(card.name, count);
            count++;
        }
        count = 0;
    }

    public void FillOpponentCrib()
    {
        if (!DataManager.isHost) return;

        //Call FillCribClientRpc for both cards sent to Crib
        foreach (GameObject card in sendToCrib.player1Crib)
        {
            FillCribClientRpc(card.name);
        }
    }

    public void AddStartingCard(string cardName)
    {
        if (!DataManager.isHost) return;

        AddStartingCardClientRpc(cardName);
    }

    public void PlayOpponentCard()
    {
        if (!DataManager.isHost) return;

        PlayCardsClientRpc(playCards.cardToPlay.name);
    }

    public void ChangeTurn()
    {
        if (!DataManager.isHost) return;

        ChangeTurnClientRpc();
    }

    public void GetReady()
    {
        if (!DataManager.isHost) return;

        GetReadyClientRpc();
    }

    //Logic are all the same compared to the same local algorithms
    [ClientRpc]
    private void AddPickedCardClientRpc(string cardName)
    {
        if (DataManager.isHost) return;

        card = cardManager.allCards.Where(obj => obj.name == cardName).SingleOrDefault();
        Instantiate(card, opponentHand.transform);
        decideDealer.player1Picked = true;
        decideDealer.player1PickedCard = cardName;
    }

    [ClientRpc]
    private void DealtCardsClientRpc(string cardName, int index)
    {
        if (DataManager.isHost) return;

        if (index < 6)
        {
            card = cardManager.allCards.Where(obj => obj.name == cardName.Substring(0, 3)).SingleOrDefault();
            instCard = Instantiate(card, ownHand.transform);
            instCard.GetComponent<CardInfo>().ownHand = true;
            cardManager.player2Hand.Add(instCard);
            cardManager.originalPlayer2Hand.Add(instCard.name.Substring(0, 3));
            cardManager.scorePlayer2Hand.Add(instCard.name.Substring(0, 3));
            cardManager.scorePlayer2HandValue.Add(Convert.ToInt16(instCard.name.Substring(1, 2)));
        }
        else
        {
            card = cardManager.allCards.Where(obj => obj.name == cardName.Substring(0, 3)).SingleOrDefault();
            instCard = Instantiate(card, opponentHand.transform);
            instCard.GetComponent<CardInfo>().ownHand = false;
            instCard.GetComponent<Image>().sprite = instCard.GetComponent<CardInfo>().cardBack;
            cardManager.player1Hand.Add(instCard);
            cardManager.originalPlayer1Hand.Add(instCard.name.Substring(0, 3));
            cardManager.scorePlayer1Hand.Add(instCard.name.Substring(0, 3));
            cardManager.scorePlayer1HandValue.Add(Convert.ToInt16(instCard.name.Substring(1, 2)));
        }

        cardManager.cardsDealt.Add(card);
        cardManager.cardPile.Remove(card);
    }

    [ClientRpc]
    private void FillCribClientRpc(string cardName)
    {
        if (DataManager.isHost) return;

        card = cardManager.player1Hand.Where(obj => obj.name == cardName).SingleOrDefault();
        card.transform.SetParent(cribArea.transform, false);
        card.GetComponent<Image>().sprite = card.GetComponent<CardInfo>().cardBack;
        cardManager.player1Hand.Remove(card);
        cardManager.originalPlayer1Hand.Remove(card.name.Substring(0, 3));
        cardManager.scorePlayer1Hand.Remove(card.name.Substring(0, 3));
        cardManager.scorePlayer1HandValue.Remove(Convert.ToInt16(card.name.Substring(1, 2)));
        cardManager.originalCrib.Add(cardName.Substring(0, 3));
        cardManager.scoreCrib.Add(cardName.Substring(0, 3));
        cardManager.scoreCribValue.Add(Convert.ToInt16(cardName.Substring(1, 2)));
        sendToCrib.player1Crib.Add(card);
        sendToCrib.crib.Add(card);
    }

    [ClientRpc]
    private void AddStartingCardClientRpc(string cardName)
    {
        if (DataManager.isHost) return;

        card = cardManager.cardPile.Where(obj => obj.name == cardName).SingleOrDefault();
        instCard = Instantiate(card, startCardArea.transform);
        cardManager.cardPile.Remove(instCard);
        cardManager.startingCard = cardName;
        cardManager.startingCardValue = Convert.ToInt16(cardName.Substring(1, 2));
        cardManager.scorePlayer1Hand.Add(instCard.name.Substring(0, 3));
        cardManager.scorePlayer1HandValue.Add(Convert.ToInt16(instCard.name.Substring(1, 2)));
        cardManager.scorePlayer2Hand.Add(instCard.name.Substring(0, 3));
        cardManager.scorePlayer2HandValue.Add(Convert.ToInt16(instCard.name.Substring(1, 2)));
        cardManager.scoreCrib.Add(cardName.Substring(0, 3));
        cardManager.scoreCribValue.Add(Convert.ToInt16(cardName.Substring(1, 2)));

        if (cardName.Substring(1, 2) == "11")
        {
            gameLoop.player1Score += 2;
            gameLoop.opponentScoreTxt.text = gameLoop.player1Score.ToString();
            updateCribBoard.UpdateScore(2, false);
            gameLoop.scoringTxt.color = Color.red;
            gameLoop.scoringTxt.text += "Two for his heels (+2)<br>";
        }

        gameLoop.gamePhase = "play card";
    }

    [ClientRpc]
    private void PlayCardsClientRpc(string cardName)
    {
        if (DataManager.isHost) return;

        card = cardManager.player1Hand.Where(obj => obj.name == cardName).SingleOrDefault();
        cardValue = Convert.ToInt16(cardName.Substring(1, 2));

        if (cardValue > 10) cardValue = 10;

        if (playCards.totalCardValue + cardValue > 31) return;

        cardManager.playedCards.Add(card);
        cardManager.playedCardsValue.Add(Convert.ToInt16(card.name.Substring(1, 2)));
        playCards.totalCardValue += cardValue;
        totalCardValueTxt.text = Convert.ToString(playCards.totalCardValue);
        card.transform.SetParent(playArea.transform, false);
        card.GetComponent<Image>().sprite = card.GetComponent<CardInfo>().cardFront;
        cardManager.player1Hand.Remove(card);
        gameLoop.player1Score += scoreInPlay.ScoreAll(false);
        gameLoop.playerTurn = 2;
        opponentScoreTxt.text = Convert.ToString(gameLoop.player1Score);
        gameLoop.playerTurnTxt.text = DataManager.player2Name;
    }

    [ClientRpc]
    private void ChangeTurnClientRpc()
    {
        if (DataManager.isHost) return;

        gameLoop.playerTurn = 2;
        gameLoop.playerTurnTxt.text = DataManager.player2Name;
        gameLoop.player1Over31 = true;
    }

    [ClientRpc]
    private void GetReadyClientRpc()
    {
        if (DataManager.isHost) return;

        gameLoop.player1Rdy = true;
    }
}