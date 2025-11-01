using Unity.Netcode;
using UnityEngine;

public class CheckConnection : NetworkBehaviour
{
    public string failReason;

    private float serverTimer;
    private float clientTimer;

    private string serverKey;
    private string clientKey;

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

    void Update()
    {
        if (failReason == "")
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

                if (clientTimer > 5)
                {
                    if (clientKey != "0")
                    {
                        failReason = "client fail";
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

                if (serverTimer > 5)
                {
                    if (serverKey != "0")
                    {
                        failReason = "host fail";
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
