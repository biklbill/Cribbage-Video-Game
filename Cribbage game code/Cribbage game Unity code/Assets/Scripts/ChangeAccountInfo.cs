using System.Collections;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class ChangeAccountInfo : MonoBehaviour
{
    public GameObject changeNameObj;
    public GameObject newNameObj;
    public GameObject confirmNameObj;
    public GameObject changePasswordObj;
    public GameObject currentPasswordObj;
    public GameObject newPasswordObj;
    public GameObject confirmNewPasswordObj;
    public GameObject confirmPasswordObj;
    public GameObject changeEmailObj;
    public GameObject newEmailObj;
    public GameObject confirmEmailObj;

    public TMP_InputField newNameIpf;
    public TMP_InputField currentPasswordIpf;
    public TMP_InputField newPasswordIpf;
    public TMP_InputField confirmNewPasswordIpf;
    public TMP_InputField newEmailIpf;

    public TMP_Text messageTxt;

    private WWWForm form;

    private UnityWebRequest www;

    //Allow user to enter data after button is clicked
    public void ChangeNameClicked()
    {
        changeNameObj.transform.localPosition = new Vector3(-250, 75, 0);
        newNameObj.SetActive(true);
        confirmNameObj.SetActive(true);
    }

    //Allow user to enter data after button is clicked
    public void ChangePasswordClicked()
    {
        changePasswordObj.transform.localPosition = new Vector3(0, 125, 0);
        currentPasswordObj.SetActive(true);
        newPasswordObj.SetActive(true);
        confirmNewPasswordObj.SetActive(true);
        confirmPasswordObj.SetActive(true);
    }

    //Allow user to enter data after button is clicked
    public void ChangeEmailClicked()
    {
        changeEmailObj.transform.localPosition = new Vector3(250, 75, 0);
        newEmailObj.SetActive(true);
        confirmEmailObj.SetActive(true);
    }

    //Check if the new name meets all the requirements
    public void ChangeName()
    {
        if (newNameIpf.text.Length == 0)
        {
            messageTxt.text = "New name cannot be blank";
            return;
        }

        if (newNameIpf.text == DataManager.username)
        {
            messageTxt.text = "New name cannot be the same as the current name";
            return;
        }

        if (!Regex.IsMatch(newNameIpf.text, "^[a-zA-Z0-9]*$"))
        {
            messageTxt.text = "New name must only have alphanumerical characters";
            return;
        }

        StartCoroutine(ToChangeName());
    }

    //Check if the new password meets all the requirements
    public void ChangePassword()
    {   
        if (currentPasswordIpf.text != DataManager.password)
        {
            messageTxt.text = "Current password is incorrect";
            return;
        }

        if (newPasswordIpf.text == DataManager.password)
        {
            messageTxt.text = "New password cannot be the same as the current password";
            return;
        }

        if (newPasswordIpf.text != confirmNewPasswordIpf.text)
        {
            messageTxt.text = "New password and confirm new password must be the same";
            return;
        }

        if (newPasswordIpf.text.Length < 8)
        {
            messageTxt.text = "Password must have at least 8 characters";
            return;
        }

        StartCoroutine(ToChangePassword());
    }

    //Check if the new email meets all the requirements
    public void ChangeEmail()
    {
        if (newEmailIpf.text.Length == 0)
        {
            messageTxt.text = "Email address cannot be blank";
            return;
        }

        if (newEmailIpf.text == DataManager.emailAddress)
        {
            messageTxt.text = "New email cannot be the same as the current email";
            return;
        }

        StartCoroutine(ToChangeEmail());
    }

    private IEnumerator ToChangeName()
    {
        //Pass user input to the sql
        form = new WWWForm();
        form.AddField("userID", DataManager.userID);
        form.AddField("newName", newNameIpf.text);

        www = UnityWebRequest.Post("http://localhost/SQLconnect/ChangeName.php", form);
        yield return www.SendWebRequest();

        //Output texts based on the debug messages received
        if (www.downloadHandler.text == "0")
        {
            DataManager.username = newNameIpf.text;
            messageTxt.text = "Name changed successfully";
        }
        else
        {
            Debug.Log("Name change failed. Error #" + www.downloadHandler.text);

            //Database error
            if (www.downloadHandler.text[0] == '1')
            {
                messageTxt.text = "Name change failed: database errors";
            }
            //User error
            else
            {
                messageTxt.text = "Name change failed: " + www.downloadHandler.text.Substring(5);
            }
        }

        www.Dispose();
    }

    //Same as before
    private IEnumerator ToChangePassword()
    {
        form = new WWWForm();
        form.AddField("userID", DataManager.userID);
        form.AddField("newPassword", newPasswordIpf.text);

        www = UnityWebRequest.Post("http://localhost/SQLconnect/ChangePassword.php", form);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "0")
        {
            DataManager.password = newPasswordIpf.text;
            messageTxt.text = "Password changed successfully";
        }
        else
        {
            Debug.Log("Password change failed. Error #" + www.downloadHandler.text);

            if (www.downloadHandler.text[0] == '1')
            {
                messageTxt.text = "Password change failed: database errors";
            }
            else
            {
                messageTxt.text = "Password change failed: " + www.downloadHandler.text.Substring(5);
            }
        }

        www.Dispose();
    }

    //Same as before
    private IEnumerator ToChangeEmail()
    {
        form = new WWWForm();
        form.AddField("userID", DataManager.userID);
        form.AddField("newEmail", newEmailIpf.text);

        www = UnityWebRequest.Post("http://localhost/SQLconnect/ChangeEmail.php", form);
        yield return www.SendWebRequest();

        //Output texts based on the debug messages received
        if (www.downloadHandler.text == "0")
        {
            DataManager.password = newEmailIpf.text;
            messageTxt.text = "Email changed successfully";
        }
        else
        {
            Debug.Log("Email change failed. Error #" + www.downloadHandler.text);

            if (www.downloadHandler.text[0] == '1')
            {
                messageTxt.text = "Email change failed: database errors";
            }
            else
            {
                messageTxt.text = "Email change failed: " + www.downloadHandler.text.Substring(5);
            }
        }

        www.Dispose();
    }
}