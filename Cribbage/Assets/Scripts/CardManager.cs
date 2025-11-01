using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<GameObject> allCards;
    public List<GameObject> cardPile;
    public List<GameObject> cardsDealt;
    public List<GameObject> player1Hand;
    public List<GameObject> player2Hand;
    public List<GameObject> playedCards;

    public List<List<int[]>> player1Combinations;
    public List<List<int[]>> player2Combinations;
    public List<List<int[]>> cribCombinations;

    public List<int> playedCardsValue;
    public List<int> scorePlayer1HandValue;
    public List<int> scorePlayer2HandValue;
    public List<int> scoreCribValue;

    public List<string> originalPlayer1Hand;
    public List<string> originalPlayer2Hand;
    public List<string> originalCrib;
    public List<string> scorePlayer1Hand;
    public List<string> scorePlayer2Hand;
    public List<string> scoreCrib;

    public GameObject ownHand;
    public GameObject opponentHand;
    public GameObject ownHandScoreArea;
    public GameObject opponentHandScoreArea;
    public GameObject cribScoreArea;

    public int startingCardValue;

    public string startingCard;
}