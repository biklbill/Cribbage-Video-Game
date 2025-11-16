using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System;

public class LeaderBoard : MonoBehaviour
{
    public MergeSort mergeSort;
    public DestroyChildObjects destroyChildObjects;

    public GameObject contentObj;
    public GameObject displayObj;

    public TMP_Text displayTxt;

    public Transform content;

    private UnityWebRequest www;

    private object[,] contentArray;

    private List<string> mappedArray;

    private string[] sqlResults;

    //Executes once when the scene loads
    private void Awake()
    {
        StartCoroutine(FillLeaderBoard());
    }

    public void CallFillLeaderBoard()
    {
        //Reset everything
        destroyChildObjects.DestroyChild(contentObj);

        StartCoroutine(FillLeaderBoard());
    }

    private void Fill()
    {
        mappedArray = new List<string>();

        //Change the data from 2d array to 1d array of string for easier manipulation
        for (int i = 0; i < contentArray.GetLength(0); i++)
        {
            mappedArray.Add(contentArray[i, 0] + " " + contentArray[i, 1].ToString());
        }

        //Perform merge sort to sort the data by score from high to low
        mergeSort.SortArray(mappedArray, 0, mappedArray.Count - 1);

        //Display content
        for (int i = 0; i < mappedArray.Count; i++)
        {
            //Spawn texts under a parent for one player's data
            displayTxt.text = (i + 1).ToString();
            Instantiate(displayObj, content);
            displayTxt.text = mappedArray[i][..(mappedArray[i].LastIndexOf(' ') + 1)];
            Instantiate(displayObj, content);
            displayTxt.text = mappedArray[i][(mappedArray[i].LastIndexOf(' ') + 1)..];
            Instantiate(displayObj, content);
        }
    }

    private IEnumerator FillLeaderBoard()
    {
        //Connect to the server
        www = UnityWebRequest.Get("http://localhost/SQLconnect/LeaderBoard.php");
        yield return www.SendWebRequest();

        //Output texts based on the debug messages received
        if (www.downloadHandler.text[0] == '0')
        {
            sqlResults = www.downloadHandler.text.Substring(1).Split("\t");
            sqlResults = sqlResults[..^1];

            contentArray = new object[sqlResults.Length / 2, 2];

            //Add formated output into an array
            for (int i = 0; i < sqlResults.Length; i = i + 2)
            {
                contentArray[i / 2, 0] = sqlResults[i].ToString();
                contentArray[i / 2, 1] = Convert.ToInt32(sqlResults[i + 1]);
            }

            //Call Fill to fill leader board after all data are added to contentArray
            Fill();
        }
        else
        {
            Debug.Log("Display leader board failed. Error #" + www.downloadHandler.text);
        }

        www.Dispose();
    }
}