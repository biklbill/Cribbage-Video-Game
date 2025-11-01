using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

public class WaitingRoom : NetworkBehaviour
{
    public RemoveRooms removeRooms;

    public TMP_Text roomName;
    public TMP_Text playerCount;
    public TMP_Text player1Name;
    public TMP_Text player2Name;
    public TMP_Text message;

    public Image player1ReadyImg;
    public Image player2ReadyImg;

    private WWWForm form;
    private UnityWebRequest www;

    private float serverTimer;
    private float clientTimer;

    private bool player1Ready;
    private bool player2Ready;
    private bool clientConnected;

    private string serverKey;
    private string clientKey;

    private void Start()
    {
        roomName.text = DataManager.roomName;

        //Start host connection if user is the room host
        if (DataManager.isHost)
        {
            NetworkManager.Singleton.StartHost();

            DataManager.player1ID = DataManager.userID;
            player1Name.text = DataManager.player1Name;
        }
        //Start client connection if user joined
        else
        {
            NetworkManager.Singleton.StartClient();

            playerCount.text = "2/2";
            player2Name.text = DataManager.player2Name;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            //Sync data to server once connection is established
            FillInfoServerRpc(DataManager.userID, DataManager.username);
        }
    }

    private void OnApplicationQuit()
    {
        //Leave game
        //Action from host
        if (DataManager.isHost)
        {
            //Remove room from database
            StartCoroutine(removeRooms.RemoveRoom(DataManager.roomID));
        }
        //Action from client
        else
        {
            //Update database
            StartCoroutine(LeaveRoom());
        }
    }

    //Assigned to Ready button, activated on click
    public void Ready()
    {
        //Ready button clicked by host
        if (DataManager.isHost)
        {
            //Only allow the host to get readyy if the room is full
            if (playerCount.text != "2/2") return;

            //Change visually and logically to reflect player 1 ready
            player1ReadyImg.color = Color.white;
            player1Ready = true;

            //Call ReadyClientRpc
            ReadyClientRpc();

            //Load Game scene if both players are ready, has to be called from the host
            if (player1Ready && player2Ready) NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        //Ready button clicked by client
        else
        {
            //Change visually and logically to reflect player 2 ready
            player2ReadyImg.color = Color.white;
            player2Ready = true;

            //Call ReadyServerRpc
            ReadyServerRpc();
        }
    }

    //Assigned to the return button, activated on click
    public void Leave()
    {
        //Action from host
        if (DataManager.isHost)
        {
            //Remove room from database
            StartCoroutine(removeRooms.RemoveRoom(DataManager.roomID));

            //Close connection between host and client
            NetworkManager.Singleton.Shutdown();

            //Return back to Rooms scene
            SceneManager.LoadScene(4);
        }
        //Action from client
        else
        {
            //Update database
            StartCoroutine(LeaveRoom());
        }
    }

    public void RemoveClient()
    {
        clientConnected = false;

        //Show client left and restore data to before the client joined
        playerCount.text = "1/2";
        player1Ready = false;
        player1ReadyImg.color = new Color32(106, 106, 106, 255);
        player2Name.text = "";
        player2Ready = false;
        player2ReadyImg.color = new Color32(106, 106, 106, 255);
        //Reset connection check variables
        clientConnected = false;
        serverTimer = 0;
        serverKey = "";
        clientTimer = 0;
        clientKey = "";
        //Tell the host the other user has left
        message.text = "The other player has left the room";
    }

    [ClientRpc]
    private void CheckConnectionClientRpc()
    {
        serverKey = "0";
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckConnectionServerRpc()
    {
        clientKey = "0";
    }

    //Called by host to communicate with client
    [ClientRpc]
    private void FillInfoClientRpc(int player1ID)
    {
        //Do nothing on host side
        if (DataManager.isHost) return;

        DataManager.player1ID = player1ID;
        //Display player 1 name on client side
        player1Name.text = DataManager.player1Name;
        clientConnected = true;
    }

    //Called by client to communicate with host
    [ServerRpc(RequireOwnership = false)]
    private void FillInfoServerRpc(int player2ID, string name)
    {
        //Do nothing on client side
        if (!DataManager.isHost) return;

        DataManager.player2ID = player2ID;
        //Assign and display player 2 name
        player2Name.text = name;
        DataManager.player2Name = name;
        //Show that a player has joined and the room is now full
        playerCount.text = "2/2";
        message.text = "";
        clientConnected = true;

        //Call host to sync data to client
        FillInfoClientRpc(DataManager.userID);
    }

    //Same logic as getting ready locally for both host and client
    [ClientRpc]
    private void ReadyClientRpc()
    {
        if (DataManager.isHost) return;

        player1ReadyImg.color = Color.white;
        player1Ready = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReadyServerRpc()
    {
        if (!DataManager.isHost) return;

        player2ReadyImg.color = Color.white;
        player2Ready = true;

        if (player1Ready && player2Ready) NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    private IEnumerator LeaveRoom()
    {
        //Create a new form for passing values to sql
        form = new WWWForm();
        //Add roomID to the form
        form.AddField("roomID", DataManager.roomID);
        form.AddField("playerID", DataManager.userID);

        //Pass the form with the values to sql
        www = UnityWebRequest.Post("http://localhost/SQLconnect/LeaveRooms.php", form);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text[0] == '0')
        {
            //Return back to Rooms scene
            SceneManager.LoadScene(4);

            //Close connection between host and client
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            Debug.Log("Leave room failed. Error #" + www.downloadHandler.text);
        }

        www.Dispose();
    }

    private void Update()
    {
        if (clientConnected)
        {
            clientTimer += Time.deltaTime;
            serverTimer += Time.deltaTime;

            if (DataManager.isHost)
            {
                if (serverTimer > 0.5)
                {
                    CheckConnectionClientRpc();
                    serverTimer = 0;
                }

                if (clientTimer > 3)
                {
                    if (clientKey != "0")
                    {
                        RemoveClient();
                    }
                    else
                    {
                        clientKey = "";
                    }

                    clientTimer = 0;
                }
            }
            else
            {
                if (clientTimer > 0.5)
                {
                    CheckConnectionServerRpc();
                    clientTimer = 0;
                }

                if (serverTimer > 3)
                {
                    if (serverKey != "0")
                    {
                        if (!player1Ready || !player2Ready) SceneManager.LoadScene(4);
                    }
                    else
                    {
                        serverKey = "";
                    }

                    serverTimer = 0;
                }
            }
        }
    }
}