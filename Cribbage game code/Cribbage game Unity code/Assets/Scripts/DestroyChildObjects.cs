using System.Collections.Generic;
using UnityEngine;

public class DestroyChildObjects : MonoBehaviour
{
    private List<GameObject> childList;

    private int childCount;

    public void DestroyChild(GameObject parent)
    {
        //Create new list to contain children of the parent game object
        childList = new List<GameObject>();
        childCount = parent.transform.childCount;

        //Add each child into the list
        for (int i = 0; i < childCount; i++)
        {
            childList.Add(parent.transform.GetChild(i).gameObject);
        }

        //Destroy each child
        for (int i = 0; i < childCount; i++)
        {
            childList[i].transform.SetParent(null);
            Destroy(childList[i]);
        }
    }
}