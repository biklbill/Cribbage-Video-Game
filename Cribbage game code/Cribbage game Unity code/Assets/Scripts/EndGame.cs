using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class EndGame : MonoBehaviour
{
    public RemoveRooms removeRooms;

    public TMP_Text gameOvertxt;

    public GameObject canvas;
    public GameObject exitBtn;
    public GameObject gameOverObj;

    private WWWForm form;
    private UnityWebRequest www;

    private int ownPoints;
    private int opponentPoints;

    private void ClearCanvas()
    {
        //Make everything inactive except the background
        foreach (Transform i in canvas.transform)
        {
            if (!(i.name == "Background" || i.name == "Cribbage Board" || i.name == "Own Score" || i.name == "Opponent Score")) i.gameObject.SetActive(false);
        }
    }

    public void DisplayResult(int ownScore, int opponentScore, bool win, string resultText)
    {
        //Kill connection
        NetworkManager.Singleton.Shutdown();

        //Make changes to objects to display result
        ClearCanvas();
        gameOverObj.SetActive(true);
        exitBtn.SetActive(true);
        gameOvertxt.text = resultText;

        //Winner adds both players' points
        if (win)
        {
            gameOvertxt.color = Color.cyan;

            if (DataManager.isHost)
            {
                CalculateScore(ownScore, opponentScore, DataManager.player2ID);
                Debug.Log(DataManager.player2ID);
            }
            else
            {
                CalculateScore(ownScore, opponentScore, DataManager.player1ID);
                Debug.Log(DataManager.player1ID);
            }

            StartCoroutine(removeRooms.RemoveRoom(DataManager.roomID));
        }
        else
        {
            gameOvertxt.color = Color.red;
        }
    }

    //Algorithm to determine how many points should a player be awarded
    //More points if opponent performs worse
    private void CalculateScore(int ownScore, int opponentScore, int opponentID)
    {
        ownPoints = (121 + ownScore - opponentScore) * 2;
        opponentPoints = opponentScore;

        StartCoroutine(UpdateScore(opponentID));
    }

    //Add points scored to the database
    private IEnumerator UpdateScore(int opponentID)
    {
        //Create a new form for passing values to sql
        form = new WWWForm();
        //Add leaderboard score to the form
        form.AddField("ownID", DataManager.userID);
        form.AddField("ownPoints", ownPoints);
        form.AddField("opponentID", opponentID);
        form.AddField("opponentPoints", opponentPoints);

        //Pass the form with the values to sql
        www = UnityWebRequest.Post("http://localhost/SQLconnect/UpdateScore.php", form);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text[0] == '0')
        {
            Debug.Log("Score uploaded successfully");
        }
        else
        {
            Debug.Log("Score upload failed. Error #" + www.downloadHandler.text);
        }

        www.Dispose();
    }
}