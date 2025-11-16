using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class DisplayRooms : MonoBehaviour
{
    public DestroyChildObjects destroyChildObjects;

    public Transform content;

    public GameObject parent;

    public TMP_InputField searchIpf;

    public TMP_Text messageTxt;

    public string roomType = "All";
    public string status = "All";

    private UnityWebRequest www;

    private GameObject instParent;

    //{room id, host id, room name, host name, password, status}
    private object[,] roomInfoArray;

    private string[] sqlResults;

    private int childCount;

    private bool roomTypeMet;
    private bool statusMet;
    private bool roomSearchMet;
    private bool hostSearchMet;

    private void Awake()
    {
        //Call RetrieveRooms
        StartCoroutine(RetrieveRooms());

        //Destroy left over network objects after leaving a room
        Cleanup();
    }

    private void Cleanup()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }

    public void CallRetrieveRooms()
    {
        //Reset error message
        messageTxt.text = "";

        //Reset everything
        destroyChildObjects.DestroyChild(content.gameObject);

        //Call RetrieveRooms
        StartCoroutine(RetrieveRooms());
    }

    //Filter which room type is displayed
    public void FilterRoomType(int value)
    {
        //All room types
        if (value == 0)
        {
            roomType = "All";
            childCount = content.childCount;

            for (int i = 0; i < childCount; i++)
            {
                if (!content.GetChild(i).gameObject.activeSelf)
                {
                    //Assign boolean variables based on room data
                    statusMet = content.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text == status || status == "All";
                    roomSearchMet = content.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);
                    hostSearchMet = content.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);

                    //Set room active if requirements are met
                    if (statusMet && (roomSearchMet || hostSearchMet)) content.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        //Public rooms only
        else if (value == 1)
        {
            roomType = "Public";
            childCount = content.childCount;

            for (int i = 0; i < childCount; i++)
            {
                //Set all private rooms to inactive
                if (content.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text == "Private")
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
                //Set public rooms to active if they meet the other requirements
                else
                {
                    statusMet = content.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text == status || status == "All";
                    roomSearchMet = content.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);
                    hostSearchMet = content.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);

                    if (statusMet && (roomSearchMet || hostSearchMet)) content.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        //Private rooms only, same logic as public rooms
        else
        {
            roomType = "Private";
            childCount = content.childCount;

            for (int i = 0; i < childCount; i++)
            {
                if (content.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text == "Public")
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    statusMet = content.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text == status || status == "All";
                    roomSearchMet = content.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);
                    hostSearchMet = content.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);

                    if (statusMet && (roomSearchMet || hostSearchMet)) content.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    //Fiter which room status is displayed, same logic as FilterRoomType
    public void FilterStatus(int value)
    {
        if (value == 0)
        {
            status = "All";
            childCount = content.childCount;

            for (int i = 0; i < childCount; i++)
            {
                roomTypeMet = content.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text == roomType || roomType == "All";
                roomSearchMet = content.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);
                hostSearchMet = content.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);

                if (roomTypeMet && (roomSearchMet || hostSearchMet)) content.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (value == 1)
        {
            status = "Available";
            childCount = content.childCount;

            for (int i = 0; i < childCount; i++)
            {
                if (content.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text == "Full")
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    roomTypeMet = content.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text == roomType || roomType == "All";
                    roomSearchMet = content.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);
                    hostSearchMet = content.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);

                    if (roomTypeMet && (roomSearchMet || hostSearchMet)) content.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        else
        {
            status = "Full";
            childCount = content.childCount;

            for (int i = 0; i < childCount; i++)
            {
                if (content.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text == "Available")
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    roomTypeMet = content.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text == roomType || roomType == "All";
                    roomSearchMet = content.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);
                    hostSearchMet = content.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);

                    if (roomTypeMet && (roomSearchMet || hostSearchMet)) content.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    //Allow users to search a room by room name or host name
    public void SearchName()
    {
        childCount = content.childCount;

        for (int i = 0; i < childCount; i++)
        {
            //Assigns variable on whether the room or host name contains user input characters
            roomSearchMet = content.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);
            hostSearchMet = content.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text.Contains(searchIpf.text, StringComparison.OrdinalIgnoreCase);

            //No characters entered
            if (searchIpf.text == null)
            {
                //Check other conditions
                roomTypeMet = content.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text == roomType || roomType == "All";
                statusMet = content.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text == status || status == "All";

                //Display room if other conditions are met
                if (roomTypeMet && statusMet) content.GetChild(i).gameObject.SetActive(true);
            }
            //Characters entered and is part of room or host name
            else if (roomSearchMet || hostSearchMet)
            {
                //Check other conditions
                roomTypeMet = content.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text == roomType || roomType == "All";
                statusMet = content.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text == status || status == "All";

                //Display room if other conditions are met
                if (roomTypeMet && statusMet) content.GetChild(i).gameObject.SetActive(true);
            }
            //Characters entered but isn't part of room or host name
            else
            {
                //Set room to inactive
                content.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void FillRooms()
    {
        //Display content
        for (int i = 0; i < roomInfoArray.GetLength(0); i++)
        {
            //Assign variabes from room data
            instParent = Instantiate(parent, content);
            instParent.name = roomInfoArray[i, 0].ToString();
            instParent.transform.GetChild(0).GetComponent<TMP_Text>().text = roomInfoArray[i, 2].ToString();
            instParent.transform.GetChild(1).GetComponent<TMP_Text>().text = roomInfoArray[i, 3].ToString();

            //No password means public room
            if (roomInfoArray[i, 4].ToString() == "")
            {
                instParent.transform.GetChild(2).GetComponent<TMP_Text>().text = "Public";
            }
            //Password means private room
            else
            {
                instParent.transform.GetChild(2).GetComponent<TMP_Text>().text = "Private";
            }

            //Assign variables from room data
            instParent.transform.GetChild(3).GetComponent<TMP_Text>().text = roomInfoArray[i, 5].ToString();
            instParent.transform.GetChild(4).gameObject.SetActive(roomInfoArray[i, 4].ToString() != "" && roomInfoArray[i, 5].ToString() != "Full");
            instParent.GetComponent<JoinRoom>().password = roomInfoArray[i, 4].ToString();
        }
    }

    private IEnumerator RetrieveRooms()
    {
        //Connect to the server
        www = UnityWebRequest.Get("http://localhost/SQLconnect/RetrieveRooms.php");
        yield return www.SendWebRequest();

        //Output texts based on the debug messages received
        if (www.downloadHandler.text[0] == '0')
        {
            //Reformat texts received into a list for easier manipulation
            sqlResults = www.downloadHandler.text.Substring(1).Split("\t");
            sqlResults = sqlResults[..^1];

            roomInfoArray = new object[sqlResults.Length / 6, 6];

            for (int i = 0; i < sqlResults.Length; i = i + 6)
            {
                //Assign variables with data retrieved from the database
                roomInfoArray[i / 6, 0] = sqlResults[i];
                roomInfoArray[i / 6, 1] = sqlResults[i + 1];
                roomInfoArray[i / 6, 2] = sqlResults[i + 2].ToString();
                roomInfoArray[i / 6, 3] = sqlResults[i + 3].ToString();

                //No password
                if (sqlResults[i + 4] == null)
                {
                    roomInfoArray[i / 6, 4] = "";
                }
                else
                {
                    roomInfoArray[i / 6, 4] = sqlResults[i + 4].ToString();
                }

                //0 means able to join, 1 means full
                if (Convert.ToInt16(sqlResults[i + 5]) == 0)
                {
                    roomInfoArray[i / 6, 5] = "Available";
                }
                else
                {
                    roomInfoArray[i / 6, 5] = "Full";
                }
            }

            //Call FillRooms to display the sql results
            FillRooms();
        }
        else
        {
            //Display error message
            messageTxt.gameObject.SetActive(true);

            //If output start with 1 it is databse error
            if (www.downloadHandler.text[0] == '1')
            {
                //Output error
                Debug.Log("Get rooms failed. Error #" + www.downloadHandler.text);

                messageTxt.text = "Get rooms failed: database errors";
            }
            //If output start with 2 then there are no rooms
            else
            {
                messageTxt.text = "No rooms currently";
            }
        }

        www.Dispose();
    }
}