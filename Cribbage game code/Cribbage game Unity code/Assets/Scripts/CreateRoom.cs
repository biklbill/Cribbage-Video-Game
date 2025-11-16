using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class CreateRoom : MonoBehaviour
{
    public TMP_InputField roomNameIpf;
    public TMP_InputField passwordIpf;

    public TMP_Text message;

    public GameObject password;

    public Image publicType;
    public Image privateType;

    private WWWForm form;

    public UnityWebRequest www;

    private string[] sqlResults;

    public void SetPublic()
    {
        publicType.color = Color.white;
        privateType.color = new Color32(106, 106, 106, 255);
        password.SetActive(false);
    }

    public void SetPrivate()
    {
        publicType.color = new Color32(106, 106, 106, 255);
        privateType.color = Color.white;
        password.SetActive(true);
    }

    public void Create()
    {
        if (roomNameIpf.text.Length == 0)
        {
            message.text = "Room name must not be blank";
            return;
        }

        if (!Regex.IsMatch(roomNameIpf.text, "^[a-zA-Z0-9]*$"))
        {
            message.text = "Room name must only have alphanumerical characters";
            return;
        }

        if (password.activeSelf && passwordIpf.text.Length == 0)
        {
            message.text = "Password must not be empty for a private room";
            return;
        }

        StartCoroutine(CreateRooms());
    }

    private IEnumerator CreateRooms()
    {
        //Create a new form for passing values to sql
        form = new WWWForm();
        //Add values entered by the user to the form
        form.AddField("roomName", roomNameIpf.text);
        form.AddField("password", passwordIpf.text);
        form.AddField("playerID", DataManager.userID.ToString());

        //Pass the form with the values to sql
        www = UnityWebRequest.Post("http://localhost/SQLconnect/CreateRooms.php", form);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text[0] == '0')
        {
            sqlResults = www.downloadHandler.text.Split("\t");

            DataManager.roomID = Convert.ToInt16(sqlResults[1]);
            DataManager.roomName = roomNameIpf.text;
            DataManager.player1Name = DataManager.username;
            DataManager.isHost = true;

            SceneManager.LoadScene(7);
        }
        else
        {
            Debug.Log("Room creation failed. Error #" + www.downloadHandler.text);

            if (www.downloadHandler.text[0] == '1')
            {
                message.text = "Room creation failed: database errors";
            }
            else
            {
                message.text = "Room creation failed: " + www.downloadHandler.text.Substring(5);
            }
        }

        www.Dispose();
    }
}