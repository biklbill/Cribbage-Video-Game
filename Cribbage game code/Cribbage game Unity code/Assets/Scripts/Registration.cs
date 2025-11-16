using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class Registration : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField emailAddress;
    public TMP_InputField password;
    public TMP_InputField confirmPassword;

    public TMP_Text message;

    private WWWForm form;

    private UnityWebRequest www;

    private bool validInputs;

    private void Start()
    {
        //Set limits for inputs
        username.characterLimit = 16;
        emailAddress.characterLimit = 64;
        password.characterLimit = 16;
        confirmPassword.characterLimit = 16;
    }

    public void VerifyInputs()
    {
        //Initial verification for simple invalid inputs
        if (username.text.Length == 0)
        {
            message.text = "Username must not be blank";
            validInputs = false;
            return;
        }

        if (!Regex.IsMatch(username.text, "^[a-zA-Z0-9]*$"))
        {
            message.text = "Username must only have alphanumerical characters";
            validInputs = false;
            return;
        }

        if (emailAddress.text.Length == 0)
        {
            message.text = "Email address must not be blank";
            validInputs = false;
            return;
        }

        if (password.text != confirmPassword.text)
        {
            message.text = "Password and confirm password must be the same";
            validInputs = false;
            return;
        }

        if (password.text.Length < 8)
        {
            message.text = "Password must have at least 8 characters";
            validInputs = false;
            return;
        }

        validInputs = true;
    }

    public void CallRegister()
    {
        //Call Register if the inputs meet initial requirements
        if (validInputs)
        {
            StartCoroutine(Register());
        }
    }

    private IEnumerator Register()
    {
        //Create a new form for passing values to sql
        form = new WWWForm();
        //Add values entered by the user to the form
        form.AddField("username", username.text);
        form.AddField("emailAddress", emailAddress.text);
        form.AddField("password", password.text);

        //Pass the form with the values to sql
        www = UnityWebRequest.Post("http://localhost/SQLconnect/Register.php", form);
        yield return www.SendWebRequest();
        
        //Output texts based on the debug messages received
        if (www.downloadHandler.text == "0")
        {
            message.text = "Account created successfully";
        }
        else
        {
            Debug.Log("User creation failed. Error #" + www.downloadHandler.text);

            if (www.downloadHandler.text[0] == '1')
            {
                message.text = "Account creation failed: database errors";
            }
            else
            {
                message.text = "Account creation failed: " + www.downloadHandler.text.Substring(5);
            }
        }

        www.Dispose();
    }
}