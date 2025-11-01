using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public Player1Network player1Network;
    public Player2Network player2Network;
    public DealCards dealCards;
    public SendToCrib sendToCrib;
    public PlayCards playCards;
    public ResetMax resetMax;
    public EndRound endRound;
    public PickStartingCard pickStartingCard;
    public CardManager cardManager;
    public ScoreInPlay scoreInPlay;
    public ScoreHand scoreHand;
    public ScoreCrib scoreCrib;
    public ResetRound resetRound;
    public Combinations combinations;
    public DestroyChildObjects destroyChildObjects;
    public DisplayScoreCards displayScoreCards;
    public UpdateCribBoard updateCribBoard;
    public EndGame endGame;
    public CheckConnection checkConnection;

    public TMP_Text playerTurnTxt;
    public TMP_Text scoringTxt;
    public TMP_Text ownScoreTxt;
    public TMP_Text opponentScoreTxt;
    public TMP_Text ownHandScoreTxt;
    public TMP_Text opponentHandScoreTxt;
    public TMP_Text cribScoreTxt;
    public TMP_Text promptTxt;

    public GameObject sendToCribBtn;
    public GameObject playCardBtn;
    public GameObject readyBtn;

    public GameObject ownHandArea;
    public GameObject opponentHandArea;
    public GameObject cribArea;
    public GameObject playArea;
    public GameObject startCardArea;

    public GameObject startCardMessageObj;
    public GameObject ownScoreObj;
    public GameObject opponentScoreObj;
    public GameObject cribbageBoard;

    public GameObject playerTurnObj;
    public GameObject playerTurnMessageObj;

    public GameObject totalCardValueObj;
    public GameObject totalCardValueMessageObj;

    public GameObject scoreInPlayMessage;
    public GameObject ownHandScoreMessage;
    public GameObject opponentHandScoreMessage;
    public GameObject cribScoreMessage;
    
    public GameObject ownHandScoreObj;
    public GameObject opponentHandScoreObj;
    public GameObject cribScoreObj;

    public GameObject ownHandScoreAreaObj;
    public GameObject opponentHandScoreAreaObj;
    public GameObject cribScoreAreaObj;
    
    public GameObject ownHandScoreMessageObj;
    public GameObject opponentHandScoreMessageObj;
    public GameObject cribScoreMessageObj;

    public GameObject promptObj;

    public int playerTurn;
    public int dealer;
    public int numCardsSelected;
    public int player1Score;
    public int player2Score;
    public int ownHandScore;
    public int opponentHandScore;
    public int cribScore;
    public int winner;
    public int lastPlayedPlayer;

    public bool player1Over31;
    public bool player2Over31;
    public bool player1Rdy;
    public bool player2Rdy;

    public bool fillCribRunned;
    public bool pickStarCardRunned;
    public bool playCardRunned;
    public bool displayHandCribRunned;
    public bool decideWinnerRunned;
    public bool gameOverRunned;

    public string gamePhase;

    private void Update()
    {
        //CheckConnection connection
        if (checkConnection.failReason != "")
        {
            if (!gameOverRunned)
            {
                if (DataManager.isHost)
                {
                    endGame.DisplayResult(player1Score, player2Score, true, DataManager.player2Name + " disconnected. You Won!");
                }
                else
                {
                    endGame.DisplayResult(player2Score, player1Score, true, DataManager.player1Name + " disconnected. You Won!");
                }

                gameOverRunned = true;
            }

            return;
        }

        //If there is a winner
        if ((player1Score >= 121 || player2Score >= 121) && !decideWinnerRunned)
        {
            //Determines the winner
            if (player1Score >= 121)
            {
                winner = 1;
            }
            else
            {
                winner = 2;
            }

            decideWinnerRunned = true;
            gamePhase = "game over";
        }

        if (gamePhase == "deal cards")
        {
            //Set the hand areas active to display dealt cards
            ownHandArea.SetActive(true);
            opponentHandArea.SetActive(true);

            //Dealer deals cards
            if (playerTurn == 1)
            {
                if (DataManager.isHost)
                {
                    dealCards.Deal();
                    player1Network.AddOpponentDealtCards();
                }
            }
            else
            {
                if (!DataManager.isHost)
                {
                    dealCards.Deal();
                    player2Network.AddOpponentDealtCards();
                }
            }

            //Enter game phase fill crib after finishing dealing cards
            gamePhase = "fill crib";
        }

        if (gamePhase == "fill crib")
        {
            if (!fillCribRunned)
            {
                //Set necessary objects to active
                promptObj.SetActive(true);
                playerTurnMessageObj.SetActive(true);
                playerTurnObj.SetActive(true);
                cribArea.SetActive(true);

                promptTxt.text = "Pick two cards to send to the Crib";

                fillCribRunned = true;
            }

            //Allow button sendToCrib to be clicked if cards selected meet all conditions
            if (numCardsSelected == 2)
            {
                if (!sendToCrib.ownCribFilled)
                {
                    sendToCribBtn.SetActive(true);
                }
            }
            else
            {
                sendToCribBtn.SetActive(false);
            }
            
            if (sendToCrib.crib.Count == 4)
            {
                //Enter game phase play card after both players have sent cards to the Crib
                gamePhase = "pick starting card";
            }
        }

        if (gamePhase == "pick starting card" && !pickStarCardRunned)
        {
            //Display appropriate message for each player
            if ((DataManager.isHost && playerTurn == 1) || (!DataManager.isHost && playerTurn == 2))
            {
                promptTxt.text = "Pick a card as starting card";
            }
            else
            {
                promptTxt.text = "Waiting for opponent to pick the starting card";
            }

            //Set the necessary objects active
            startCardArea.SetActive(true);
            startCardMessageObj.SetActive(true);
            scoreInPlayMessage.SetActive(true);
            cribbageBoard.SetActive(true);
            ownScoreObj.SetActive(true);
            opponentScoreObj.SetActive(true);

            pickStartingCard.PickStartCard();

            //Stop the if statement from executed again
            pickStarCardRunned = true;
        }

        if (gamePhase == "play card")
        {
            if (!playCardRunned)
            {
                promptTxt.text = "Play a card without the total value exceeding 31 if possible";

                //Display necessary game objects
                playArea.SetActive(true);
                totalCardValueObj.SetActive(true);
                totalCardValueMessageObj.SetActive(true);

                playCardRunned = true;
            }

            //If neither player can play a card
            if ((player1Over31 && player2Over31) || (cardManager.player1Hand.Count == 0 && cardManager.player2Hand.Count == 0))
            {
                if (playCards.totalCardValue < 31)
                {
                    //Check who played the last card
                    lastPlayedPlayer = scoreInPlay.ScoreLastPlayed();

                    //Add score to the player who played the last card
                    if (DataManager.isHost)
                    {
                        if (lastPlayedPlayer == 1)
                        {
                            scoringTxt.color = Color.cyan;
                            player1Score++;
                            ownScoreTxt.text = player1Score.ToString();
                            updateCribBoard.UpdateScore(1, true);
                        }
                        else
                        {
                            scoringTxt.color = Color.red;
                            player2Score++;
                            opponentScoreTxt.text = player2Score.ToString();
                            updateCribBoard.UpdateScore(1, false);
                        }
                    }
                    else
                    {
                        if (lastPlayedPlayer == 2)
                        {
                            scoringTxt.color = Color.cyan;
                            player2Score++;
                            ownScoreTxt.text = player2Score.ToString();
                            updateCribBoard.UpdateScore(1, true);
                        }
                        else
                        {
                            scoringTxt.color = Color.red;
                            player1Score++;
                            opponentScoreTxt.text = player1Score.ToString();
                            updateCribBoard.UpdateScore(1, false);
                        }
                    }

                    //Reset variables after score is added
                    player1Over31 = false;
                    player2Over31 = false;
                }

                //Both have no cards left
                if (cardManager.player1Hand.Count == 0 && cardManager.player2Hand.Count == 0)
                {
                    endRound.ResetRound();
                }
                //Both have cards left but can't play as it would exceed 31
                else
                {
                    resetMax.Reset();
                }
            }

            //Allow player to play a card if all conditions are met
            if (DataManager.isHost)
            {
                if (playerTurn == 1)
                {
                    //Skip turn if player can't play a card without exceeding 31
                    if (playCards.CheckOver31(cardManager.player1Hand, playCards.totalCardValue))
                    {
                        playerTurn = 2;
                        playerTurnTxt.text = DataManager.player2Name;
                        player1Network.ChangeTurn();
                        player1Over31 = true;
                    }

                    //Allow player to play card if one card is selected
                    if (numCardsSelected == 1)
                    {
                        playCardBtn.SetActive(true);
                    }
                    else
                    {
                        playCardBtn.SetActive(false);
                    }
                }
                else
                {
                    playCardBtn.SetActive(false);
                }
            }
            //Same logic as host
            else
            {
                if (playerTurn == 2)
                {
                    //Skip turn if player can't play a card without exceeding 31
                    if (playCards.CheckOver31(cardManager.player2Hand, playCards.totalCardValue))
                    {
                        playerTurn = 1;
                        playerTurnTxt.text = DataManager.player1Name;
                        player2Network.ChangeTurn();
                        player2Over31 = true;
                    }

                    if (numCardsSelected == 1)
                    {
                        playCardBtn.SetActive(true);
                    }
                    else
                    {
                        playCardBtn.SetActive(false);
                    }
                }
                else
                {
                    playCardBtn.SetActive(false);
                }
            }
        }

        if (gamePhase == "score hand and crib")
        {
            //Set necessary objects active for the game phase
            ownHandScoreMessage.SetActive(true);
            opponentHandScoreMessage.SetActive(true);
            cribScoreMessage.SetActive(true);

            //Initialise lists
            cardManager.player1Combinations = new List<List<int[]>>();
            cardManager.player2Combinations = new List<List<int[]>>();
            cardManager.cribCombinations = new List<List<int[]>>();

            //Add every combination into a 3d list from length 2 to 5 using GetCombination function, used for scoring
            for (int i = 2; i < 6; i++)
            {
                cardManager.player1Combinations.Add(combinations.GetCombination(cardManager.scorePlayer1HandValue, i));
            }

            for (int i = 2; i < 6; i++)
            {
                cardManager.player2Combinations.Add(combinations.GetCombination(cardManager.scorePlayer2HandValue, i));
            }

            for (int i = 2; i < 6; i++)
            {
                cardManager.cribCombinations.Add(combinations.GetCombination(cardManager.scoreCribValue, i));
            }

            //Initialise score
            ownHandScore = 0;
            opponentHandScore = 0;
            cribScore = 0;

            //Stop displaying unnecessary objects
            totalCardValueMessageObj.SetActive(false);
            cribArea.SetActive(false);
            scoreInPlayMessage.SetActive(false);
            promptObj.SetActive(false);

            //Add each player's hand score, Crib score is added to dealer
            if (DataManager.isHost)
            {
                ownHandScore = scoreHand.ScoreAll(1, true);
                opponentHandScore = scoreHand.ScoreAll(2, false);
                player1Score += ownHandScore;
                player2Score += opponentHandScore;

                if (dealer == 1)
                {
                    cribScore += scoreCrib.ScoreAll(true);
                    player1Score += cribScore;
                }
                else
                {
                    cribScore += scoreCrib.ScoreAll(false);
                    player2Score += cribScore;
                }

                ownScoreTxt.text = player1Score.ToString();
                opponentScoreTxt.text = player2Score.ToString();
            }
            //Same logic as host
            else
            {
                ownHandScore = scoreHand.ScoreAll(2, true);
                opponentHandScore = scoreHand.ScoreAll(1, false);
                player2Score += ownHandScore;
                player1Score += opponentHandScore;

                if (dealer == 2)
                {
                    cribScore += scoreCrib.ScoreAll(true);
                    player2Score += cribScore;
                }
                else
                {
                    cribScore += scoreCrib.ScoreAll(false);
                    player1Score += cribScore;
                }

                ownScoreTxt.text = player2Score.ToString();
                opponentScoreTxt.text = player1Score.ToString();
            }

            //Clear Crib area
            destroyChildObjects.DestroyChild(cribArea);
            gamePhase = "display hand and crib score";
        }

        if (gamePhase == "display hand and crib score")
        {
            if (!displayHandCribRunned)
            {
                scoringTxt.text = "";

                //Set necessary objects to active
                ownHandScoreObj.SetActive(true);
                opponentHandScoreObj.SetActive(true);
                cribScoreObj.SetActive(true);
                ownHandScoreMessageObj.SetActive(true);
                opponentHandScoreMessageObj.SetActive(true);
                cribScoreMessageObj.SetActive(true);
                ownHandScoreAreaObj.SetActive(true);
                opponentHandScoreAreaObj.SetActive(true);
                cribScoreAreaObj.SetActive(true);
                readyBtn.SetActive(true);

                ownHandScoreTxt.text = ownHandScore.ToString();
                opponentHandScoreTxt.text = opponentHandScore.ToString();
                cribScoreTxt.text = cribScore.ToString();

                //Display both players' hands and the Crib
                displayScoreCards.Display();

                displayHandCribRunned = true;
            }

            //Reset to prepare for next round if both players are ready
            if (player1Rdy && player2Rdy) resetRound.Reset();
        }

        if (gamePhase == "game over" && !gameOverRunned)
        {
            //Display result and add score
            if (DataManager.isHost)
            {
                if (winner == 1)
                {
                    endGame.DisplayResult(player1Score, player2Score, true, "Game Over. You Won!");
                }
                else
                {
                    endGame.DisplayResult(player1Score, player2Score, false, "Game Over. You Lost!");
                }
            }
            else
            {
                if (winner == 2)
                {
                    endGame.DisplayResult(player2Score, player1Score, true, "Game Over. You Won!");
                }
                else
                {
                    endGame.DisplayResult(player2Score, player1Score, false, "Game Over. You Lost!");
                }
            }

            gameOverRunned = true;
        }
    }
}