using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class JoinRoom : MonoBehaviour
{
    public TMP_Text message;

    public string password;

    private WWWForm form;

    private UnityWebRequest www;

    //Join room when a displayed room is clicked
    public void Join()
    {
        //Prompts the user if clicked room is full and doesn't join
        if (transform.GetChild(3).GetComponent<TMP_Text>().text == "Full")
        {
            message.text = "Room full";
            return;
        }

        //If password is active, private room
        if (transform.GetChild(4).gameObject.activeSelf)
        {
            //If password is incorrect prompts the user and doesn't join
            if (transform.GetChild(4).GetComponent<TMP_InputField>().text != password)
            {
                message.text = "Incorrect password";
                return;
            }
        }

        StartCoroutine(JoinRooms(gameObject.name));
    }

    private IEnumerator JoinRooms(string roomID)
    {
        //Create a new form for passing values to sql
        form = new WWWForm();
        //Add values from the user input to the form
        form.AddField("roomID", roomID);
        form.AddField("playerID", DataManager.userID.ToString());

        //Pass the form with the values to sql
        www = UnityWebRequest.Post("http://localhost/SQLconnect/JoinRooms.php", form);
        yield return www.SendWebRequest();

        //Output starts with 0 if sql execution is successful
        if (www.downloadHandler.text[0] == '0')
        {
            //Assign static room data
            DataManager.roomID = Convert.ToInt16(gameObject.name);
            DataManager.isHost = false;
            DataManager.roomName = transform.GetChild(0).GetComponent<TMP_Text>().text;
            DataManager.player2ID = DataManager.userID;
            DataManager.player1Name = transform.GetChild(1).GetComponent<TMP_Text>().text;
            DataManager.player2Name = DataManager.username;

            //Load Waiting Room scene
            SceneManager.LoadScene(7);
        }
        else
        {
            Debug.Log("Join room failed. Error #" + www.downloadHandler.text);

            //Output starts with 1 means database errors
            if (www.downloadHandler.text[0] == '1')
            {
                message.text = "Join room failed: database errors";
            }
            //Output startw with 2 means user errors
            else
            {
                message.text = "Join room failed: " + www.downloadHandler.text.Substring(5);
            }
        }

        //Close connection
        www.Dispose();
    }
}