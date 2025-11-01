using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RemoveRooms : MonoBehaviour
{
    private WWWForm form;
    private UnityWebRequest www;

    public IEnumerator RemoveRoom(int roomID)
    {
        //Create a new form for passing values to sql
        form = new WWWForm();
        //Add roomID to the form
        form.AddField("roomID", roomID);

        //Pass the form with the values to sql
        www = UnityWebRequest.Post("http://localhost/SQLconnect/RemoveRooms.php", form);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text[0] != '0')
        {
            Debug.Log("Remove room failed. Error #" + www.downloadHandler.text);
        }

        www.Dispose();
    }
}
