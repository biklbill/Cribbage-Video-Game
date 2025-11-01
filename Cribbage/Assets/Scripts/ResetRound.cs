using System.Collections.Generic;
using UnityEngine;

public class ResetRound : MonoBehaviour
{
    public CardManager cardManager;
    public SendToCrib sendToCrib;
    public GameLoop gameLoop;
    public DestroyChildObjects destroyChildObjects;
    public ScoreHand scoreHand;
    public ScoreCrib scoreCrib;

    public void Reset()
    {
        cardManager.cardPile = new List<GameObject>(cardManager.allCards);
        cardManager.cardsDealt.Clear();

        cardManager.player1Hand.Clear();
        cardManager.originalPlayer1Hand.Clear();
        cardManager.scorePlayer1Hand.Clear();
        cardManager.scorePlayer1HandValue.Clear();
        cardManager.player1Combinations.Clear();

        cardManager.player2Hand.Clear();
        cardManager.originalPlayer2Hand.Clear();
        cardManager.scorePlayer2Hand.Clear();
        cardManager.scorePlayer2HandValue.Clear();
        cardManager.player2Combinations.Clear();

        cardManager.scoreCrib.Clear();
        cardManager.scoreCribValue.Clear();
        cardManager.cribCombinations.Clear();

        cardManager.originalCrib.Clear();
        sendToCrib.crib.Clear();
        sendToCrib.player1Crib.Clear();
        sendToCrib.player2Crib.Clear();
        
        gameLoop.player1Over31 = false;
        gameLoop.player1Rdy = false;

        gameLoop.player2Over31 = false;
        gameLoop.player2Rdy = false;

        gameLoop.fillCribRunned = false;
        gameLoop.pickStarCardRunned = false;
        gameLoop.playCardRunned = false;
        gameLoop.displayHandCribRunned = false;
        gameLoop.decideWinnerRunned = false;
        gameLoop.gameOverRunned = false;

        sendToCrib.ownCribFilled = false;

        gameLoop.startCardMessageObj.SetActive(false);
        gameLoop.startCardArea.SetActive(false);

        gameLoop.ownHandScoreObj.SetActive(false);
        gameLoop.ownHandScoreMessageObj.SetActive(false);
        gameLoop.ownHandScoreAreaObj.SetActive(false);
        gameLoop.ownHandScoreMessage.SetActive(false);
        
        gameLoop.opponentHandScoreObj.SetActive(false);
        gameLoop.opponentHandScoreMessageObj.SetActive(false);
        gameLoop.opponentHandScoreAreaObj.SetActive(false);
        gameLoop.opponentHandScoreMessage.SetActive(false);

        gameLoop.cribScoreObj.SetActive(false);
        gameLoop.cribScoreMessageObj.SetActive(false);
        gameLoop.cribScoreAreaObj.SetActive(false);
        gameLoop.cribScoreMessage.SetActive(false);

        gameLoop.promptObj.SetActive(false);

        scoreHand.ownHandScoreTxt.text = "";
        scoreHand.opponentHandScoreTxt.text = "";
        scoreCrib.cribScoreTxt.text = "";
        gameLoop.promptTxt.text = "";

        destroyChildObjects.DestroyChild(gameLoop.ownHandScoreAreaObj);
        destroyChildObjects.DestroyChild(gameLoop.opponentHandScoreAreaObj);
        destroyChildObjects.DestroyChild(gameLoop.cribScoreAreaObj);
        destroyChildObjects.DestroyChild(gameLoop.startCardArea);

        if (gameLoop.dealer == 1)
        {
            gameLoop.dealer = 2;
            gameLoop.playerTurn = 2;
            gameLoop.playerTurnTxt.text = DataManager.player2Name;
        }
        else
        {
            gameLoop.dealer = 1;
            gameLoop.playerTurn = 1;
            gameLoop.playerTurnTxt.text = DataManager.player1Name;
        }

        gameLoop.gamePhase = "deal cards";
    }
}