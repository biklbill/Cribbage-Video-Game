using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public TMP_InputField usernameOrEmailAddress;
    public TMP_InputField password;

    public TMP_Text message;

    private WWWForm form;
    private UnityWebRequest www;

    private string[] sqlResults;

    private void Start()
    {
        //Set limits for inputs
        usernameOrEmailAddress.characterLimit = 64;
        password.characterLimit = 16;
    }

    public void CallSignIn()
    {
        StartCoroutine(SignIn());
    }

    private IEnumerator SignIn()
    {
        //Create a new form for passing values to sql
        form = new WWWForm();
        //Add values entered by the user to the form
        form.AddField("usernameOrEmailAddress", usernameOrEmailAddress.text);
        form.AddField("password", password.text);

        //Pass the form with the values to sql
        www = UnityWebRequest.Post("http://localhost/SQLconnect/Login.php", form);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text[0] == '0')
        {
            sqlResults = www.downloadHandler.text.Split("\t");

            DataManager.userID = Convert.ToInt16(sqlResults[1]);

            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log("Login failed. Error #" + www.downloadHandler.text);

            if (www.downloadHandler.text[0] == '1')
            {
                message.text = "Login failed: database errors";
            }
            else
            {
                message.text = "Login failed: " + www.downloadHandler.text.Substring(5);
            }
        }

        www.Dispose();
    }
}