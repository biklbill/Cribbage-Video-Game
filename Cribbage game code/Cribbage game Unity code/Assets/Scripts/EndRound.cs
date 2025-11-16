using UnityEngine;

public class EndRound : MonoBehaviour
{
    public GameLoop gameLoop;
    public ResetMax resetMax;

    public void ResetRound()
    {
        //Remove played cards
        resetMax.Reset();
        //Stop displaying total card value
        gameLoop.totalCardValueObj.SetActive(false);
        //Enter game phase calculate hand score
        gameLoop.gamePhase = "score hand and crib";
    }
}