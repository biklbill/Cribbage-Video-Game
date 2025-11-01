using System.Linq;
using UnityEngine;

public class DisplayScoreCards : MonoBehaviour
{
    public CardManager cardManager;
    public SendToCrib sendToCrib;

    public void Display()
    {
        foreach (string i in cardManager.originalPlayer1Hand)
        {
            //Display player 1's hand at the start
            if (DataManager.isHost)
            {
                Instantiate(cardManager.allCards.Where(obj => obj.name == i.Substring(0, 3)).SingleOrDefault(), cardManager.ownHandScoreArea.transform);
            }
            else
            {
                Instantiate(cardManager.allCards.Where(obj => obj.name == i.Substring(0, 3)).SingleOrDefault(), cardManager.opponentHandScoreArea.transform);
            }
        }

        foreach (string i in cardManager.originalPlayer2Hand)
        {
            //Display player 2's hand at the start
            if (!DataManager.isHost)
            {
                Instantiate(cardManager.allCards.Where(obj => obj.name == i.Substring(0, 3)).SingleOrDefault(), cardManager.ownHandScoreArea.transform);
            }
            else
            {
                Instantiate(cardManager.allCards.Where(obj => obj.name == i.Substring(0, 3)).SingleOrDefault(), cardManager.opponentHandScoreArea.transform);
            }
        }

        foreach (string i in cardManager.originalCrib)
        {
            //Display Crib's cards
            Instantiate(cardManager.allCards.Where(obj => obj.name == i.Substring(0, 3)).SingleOrDefault(), cardManager.cribScoreArea.transform);
        }
    }
}