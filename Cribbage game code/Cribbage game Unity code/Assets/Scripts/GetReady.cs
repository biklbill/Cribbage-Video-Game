using UnityEngine;

public class GetReady : MonoBehaviour
{
    public GameLoop gameLoop;

    public void Ready()
    {
        if (DataManager.isHost)
        {
            gameLoop.player1Rdy = true;
        }
        else
        {
            gameLoop.player2Rdy = true;
        }

        gameLoop.readyBtn.SetActive(false);
    }
}