using System.Linq;
using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;
using UnityEngine.UI;

public class Player2Network : NetworkBehaviour
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
        //Only applies for player 2 so stop immediately if player is player 1
        if (DataManager.isHost) return;

        //Call AddPickedCardServerRpc with passed parameter
        AddPickedCardServerRpc(cardName);
    }

    public void AddOpponentDealtCards()
    {
        if (DataManager.isHost) return;

        //Call DealtCardsServerRpc for each card in hand
        foreach (GameObject card in cardManager.player1Hand.Concat(cardManager.player2Hand))
        {
            DealtCardsServerRpc(card.name, count);
            count++;
        }
        count = 0;
    }

    public void FillOpponentCrib()
    {
        if (DataManager.isHost) return;

        //Call FillCribServerRpc for both cards sent to Crib
        foreach (GameObject card in sendToCrib.player2Crib)
        {
            FillCribServerRpc(card.name);
        }
    }

    public void AddStartingCard(string cardName)
    {
        if (DataManager.isHost) return;

        AddStartingCardServerRpc(cardName);
    }

    public void PlayOpponentCard()
    {
        if (DataManager.isHost) return;

        PlayCardsServerRpc(playCards.cardToPlay.name);
    }

    public void ChangeTurn()
    {
        if (DataManager.isHost) return;

        ChangeTurnServerRpc();
    }

    public void GetReady()
    {
        if (DataManager.isHost) return;

        GetReadyServerRpc();
    }

    //Logic are all the same compared to the same local algorithms
    [ServerRpc(RequireOwnership = false)]
    private void AddPickedCardServerRpc(string cardName)
    {
        if (!DataManager.isHost) return;

        card = cardManager.allCards.Where(obj => obj.name == cardName).SingleOrDefault();
        Instantiate(card, opponentHand.transform);
        decideDealer.player2Picked = true;
        decideDealer.player2PickedCard = cardName;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DealtCardsServerRpc(string cardName, int index)
    {
        if (!DataManager.isHost) return;

        if (index < 6)
        {
            card = cardManager.allCards.Where(obj => obj.name == cardName.Substring(0, 3)).SingleOrDefault();
            instCard = Instantiate(card, ownHand.transform);
            instCard.GetComponent<CardInfo>().ownHand = true;
            cardManager.player1Hand.Add(instCard);
            cardManager.originalPlayer1Hand.Add(instCard.name.Substring(0, 3));
            cardManager.scorePlayer1Hand.Add(instCard.name.Substring(0, 3));
            cardManager.scorePlayer1HandValue.Add(Convert.ToInt16(instCard.name.Substring(1, 2)));
        }
        else
        {
            card = cardManager.allCards.Where(obj => obj.name == cardName.Substring(0, 3)).SingleOrDefault();
            instCard = Instantiate(card, opponentHand.transform);
            instCard.GetComponent<CardInfo>().ownHand = false;
            instCard.GetComponent<Image>().sprite = instCard.GetComponent<CardInfo>().cardBack;
            cardManager.player2Hand.Add(instCard);
            cardManager.originalPlayer2Hand.Add(instCard.name.Substring(0, 3));
            cardManager.scorePlayer2Hand.Add(instCard.name.Substring(0, 3));
            cardManager.scorePlayer2HandValue.Add(Convert.ToInt16(instCard.name.Substring(1, 2)));
        }

        cardManager.cardsDealt.Add(card);
        cardManager.cardPile.Remove(card);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FillCribServerRpc(string cardName)
    {
        if (!DataManager.isHost) return;

        card = cardManager.player2Hand.Where(obj => obj.name == cardName).SingleOrDefault();
        card.transform.SetParent(cribArea.transform, false);
        card.GetComponent<Image>().sprite = card.GetComponent<CardInfo>().cardBack;
        cardManager.player2Hand.Remove(card);
        cardManager.originalPlayer2Hand.Remove(card.name.Substring(0, 3));
        cardManager.scorePlayer2Hand.Remove(card.name.Substring(0, 3));
        cardManager.scorePlayer2HandValue.Remove(Convert.ToInt16(card.name.Substring(1, 2)));
        cardManager.originalCrib.Add(cardName.Substring(0, 3));
        cardManager.scoreCrib.Add(cardName.Substring(0, 3));
        cardManager.scoreCribValue.Add(Convert.ToInt16(cardName.Substring(1, 2)));
        sendToCrib.player2Crib.Add(card);
        sendToCrib.crib.Add(card); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddStartingCardServerRpc(string cardName)
    {
        if (!DataManager.isHost) return;

        card = cardManager.cardPile.Where(obj => obj.name == cardName).SingleOrDefault();
        instCard = Instantiate(card, startCardArea.transform);
        cardManager.cardPile.Remove(card);
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
            gameLoop.player2Score += 2;
            gameLoop.opponentScoreTxt.text = gameLoop.player2Score.ToString();
            updateCribBoard.UpdateScore(2, false);
            gameLoop.scoringTxt.color = Color.red;
            gameLoop.scoringTxt.text += "Two for his heels (+2)<br>";
        }

        gameLoop.gamePhase = "play card";
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayCardsServerRpc(string cardName)
    {
        if (!DataManager.isHost) return;

        card = cardManager.player2Hand.Where(obj => obj.name == cardName).SingleOrDefault();
        cardValue = Convert.ToInt16(cardName.Substring(1, 2));

        if (cardValue > 10) cardValue = 10;

        if (playCards.totalCardValue + cardValue > 31) return;

        cardManager.playedCards.Add(card);
        cardManager.playedCardsValue.Add(Convert.ToInt16(card.name.Substring(1, 2)));
        playCards.totalCardValue += cardValue;
        totalCardValueTxt.text = Convert.ToString(playCards.totalCardValue);
        card.transform.SetParent(playArea.transform, false);
        card.GetComponent<Image>().sprite = card.GetComponent<CardInfo>().cardFront;
        cardManager.player2Hand.Remove(card);
        gameLoop.player2Score += scoreInPlay.ScoreAll(false);
        gameLoop.playerTurn = 1;
        opponentScoreTxt.text = Convert.ToString(gameLoop.player2Score);
        gameLoop.playerTurnTxt.text = DataManager.player1Name;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeTurnServerRpc()
    {
        if (!DataManager.isHost) return;

        gameLoop.playerTurn = 1;
        gameLoop.playerTurnTxt.text = DataManager.player1Name;
        gameLoop.player2Over31 = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetReadyServerRpc()
    {
        if (!DataManager.isHost) return;

        gameLoop.player2Rdy = true;
    }
}