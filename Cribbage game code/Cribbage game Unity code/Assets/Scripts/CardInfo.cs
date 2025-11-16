using UnityEngine;
using UnityEngine.EventSystems;

public class CardInfo : MonoBehaviour, IPointerClickHandler
{
    public bool selected;
    public bool ownHand;

    public Sprite cardFront;
    public Sprite cardBack;

    private GameObject logicManager;

    //Trigger when clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!ownHand) return;

        logicManager = GameObject.Find("Logic Manager");

        //Move up to show card is selected
        if (selected)
        {
            transform.position -= new Vector3(0, 0.5f, 0);
            logicManager.GetComponent<GameLoop>().numCardsSelected -= 1;
            selected = false;
        }
        //Move down to show card is deselected
        else
        {
            transform.position += new Vector3(0, 0.5f, 0);
            logicManager.GetComponent<GameLoop>().numCardsSelected += 1;
            selected = true;
        }
    }
}