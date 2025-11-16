using UnityEngine;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Collections;

public class UpdateLobby : MonoBehaviour
{
    public TMP_Text usernametxt;
    public TMP_Text scoretxt;
    public TMP_Text gamesPlayedtxt;
    public TMP_Text gamesWontext;

    private WWWForm form;
    private UnityWebRequest www;

    private string[] sqlResults;

    private void Awake()
    {
        //Retrieve user data
        StartCoroutine(UpdateData());
    }

    private void DisplayData()
    {
        //Display user information
        usernametxt.text = DataManager.username;
        scoretxt.text = DataManager.score.ToString();
        gamesPlayedtxt.text = DataManager.gamesPlayed.ToString();
        gamesWontext.text = DataManager.gamesWon.ToString();
    }

    private IEnumerator UpdateData()
    {
        //Create a new form for passing values to sql
        form = new WWWForm();
        //Add userID to the form
        form.AddField("userID", DataManager.userID);

        //Pass the form with the values to sql
        www = UnityWebRequest.Post("http://localhost/SQLconnect/UpdateUserData.php", form);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text[0] == '0')
        {
            sqlResults = www.downloadHandler.text.Split("\t");

            //Assign static variables with user data
            DataManager.username = sqlResults[1];
            DataManager.emailAddress = sqlResults[2];
            DataManager.password = sqlResults[3];
            DataManager.score = Convert.ToInt32(sqlResults[4]);
            DataManager.gamesPlayed = Convert.ToInt16(sqlResults[5]);
            DataManager.gamesWon = Convert.ToInt16(sqlResults[6]);

            DisplayData();
        }

        www.Dispose();
    }
}