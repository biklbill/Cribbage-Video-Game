using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class DecideDealer : MonoBehaviour
{
    public GameLoop gameLoop;
    public Player1Network player1Network;
    public Player2Network player2Network;
    public DealCards dealCards;
    public UpdateCribBoard updateCribBoard;

    public TMP_Text ownName;
    public TMP_Text opponentName;

    public GameObject cardSpread;
    public GameObject ownHand;
    public GameObject opponentHand;

    public Image player1Peg;
    public Image player2Peg;

    public bool player1Picked;
    public bool player2Picked;

    public string player1PickedCard;
    public string player2PickedCard;

    private int player1PickedCardValue;
    private int player2PickedCardValue;

    private void Start()
    {
        //Enter game phase decide dealer to decide the dealer at the start of the game
        gameLoop.gamePhase = "decide dealer";
        //Create a pile of cards in a fan shape for players to click to pick cards
        Instantiate(cardSpread, new Vector3(0, -1.5f, 0), Quaternion.Euler(90, 0, 0));
        //Promp players
        gameLoop.promptTxt.text = "Pick a card and compare with opponent's, player with the bigger value becomes dealer";

        //Assign score pegs, cyan is own, red is opponent
        if (DataManager.isHost)
        {
            player1Peg.color = Color.cyan;
            player2Peg.color = Color.red;
            ownName.text = DataManager.player1Name;
            opponentName.text = DataManager.player2Name;
        }
        else
        {
            player1Peg.color = Color.red;
            player2Peg.color = Color.cyan;
            ownName.text = DataManager.player2Name;
            opponentName.text = DataManager.player1Name;
        }

        gameLoop.promptTxt.text = "Pick a card, the player with the larger card value becomes the dealer first";
    }   

    private void Update()
    {
        //If both players have picked a card and still deciding dealer then call the coroutine CompareCards
        if (player1Picked && player2Picked && gameLoop.gamePhase == "decide dealer")
        {
            //Call CompareCards
            StartCoroutine("CompareCards");
        }
        else
        {
            //Stop CompareCards immediately
            StopCoroutine("CompareCards");
        }
    }

    private IEnumerator CompareCards()
    {
        //Get the value of the cards picked
        player1PickedCardValue = Convert.ToInt16(player1PickedCard.Substring(1, 2));
        player2PickedCardValue = Convert.ToInt16(player2PickedCard.Substring(1, 2));

        //If player 1's card is higher then player 1 becomes the dealer and plays first
        if (player1PickedCardValue > player2PickedCardValue)
        {
            gameLoop.promptTxt.text = $"{DataManager.player1Name} is the dealer";

            //Pause so players can see the result
            yield return new WaitForSeconds(3);

            //Assign variables with player 1 as dealer
            gameLoop.playerTurn = 1; 
            gameLoop.dealer = 1;
            gameLoop.playerTurnTxt.text = DataManager.player1Name;
            gameLoop.gamePhase = "deal cards";
        }
        //If player 2's card is higher then player 2 becomes the dealer and plays first
        else if (player1PickedCardValue < player2PickedCardValue)
        {
            gameLoop.promptTxt.text = $"{DataManager.player2Name} is the dealer";

            //Pause so players can see the result
            yield return new WaitForSeconds(3);

            //Assign variables with player 2 as dealer
            gameLoop.playerTurn = 2;
            gameLoop.dealer = 2;
            gameLoop.playerTurnTxt.text = DataManager.player2Name;
            gameLoop.gamePhase = "deal cards";
        }
        //If both cards have the same value then let both pick again
        else
        {
            gameLoop.promptTxt.text = "Same value, repick cards";

            //Pause so players can see the result
            yield return new WaitForSeconds(3);

            //Create card pile again
            Instantiate(cardSpread, new Vector3(0, -1.5f, 0), Quaternion.Euler(90, 0, 0));
        }

        //Clear previous picked cards
        Destroy(ownHand.transform.GetChild(0).gameObject);
        Destroy(opponentHand.transform.GetChild(0).gameObject);
        player1Picked = false;
        player2Picked = false;
    }
}