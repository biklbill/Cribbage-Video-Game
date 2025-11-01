using UnityEngine;

public class PickStartingCard : MonoBehaviour
{
    public GameLoop gameLoop;
    public CardManager cardManager;

    public GameObject cardSpread;

    public void PickStartCard()
    {
        if (gameLoop.dealer == 1)
        {
            if (!DataManager.isHost) return;
        }
        else
        {
            if (DataManager.isHost) return;
        }

        if (!GameObject.Find("Card Spread(Clone)")) Instantiate(cardSpread, new Vector3(0, -1.5f, 0), Quaternion.Euler(90, 0, 0));
    }
}