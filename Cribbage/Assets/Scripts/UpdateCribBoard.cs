using System;
using UnityEngine;

public class UpdateCribBoard : MonoBehaviour
{
    public Transform player1Peg;
    public Transform player2Peg;

    public void UpdateScore(int score, bool own)
    {
        //Update own score
        if (own)
        {
            if (DataManager.isHost)
            {
                //Move the peg by assigning them under new parents that represents the score
                player1Peg.SetParent(GameObject.Find("/Canvas/Cribbage Board/Player 1/" + (Convert.ToInt16(player1Peg.transform.parent.name) + score)).transform, false);
            }
            else
            {
                player2Peg.SetParent(GameObject.Find("/Canvas/Cribbage Board/Player 2/" + (Convert.ToInt16(player2Peg.transform.parent.name) + score)).transform, false);
            }
        }
        //Same as updating own score but opponent score
        else
        {
            if (DataManager.isHost)
            {
                player2Peg.SetParent(GameObject.Find("/Canvas/Cribbage Board/Player 2/" + (Convert.ToInt16(player2Peg.transform.parent.name) + score)).transform, false);
            }
            else
            {
                player1Peg.SetParent(GameObject.Find("/Canvas/Cribbage Board/Player 1/" + (Convert.ToInt16(player1Peg.transform.parent.name) + score)).transform, false);
            }
        }
    }
}