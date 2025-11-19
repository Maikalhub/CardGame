using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //public setting changes to our card
    [Header("Settings")]
    public CardState _CardState;

    public CardsManager cardsManager;
    public CardType cardType;
    public int cardNumber;

    //All states our card can be in
    public enum CardState
    {
        Idle, IsDragging, Played
    }

    //Public varibles we dont want touched in the editor
    [HideInInspector] public bool CanDrag;
    [HideInInspector] public bool Hovering;
    [HideInInspector] public Canvas canvas;

    private void Start()
    {
        //Find and set starting Varibles
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        cardsManager = GameObject.Find("CardsManager").GetComponent<CardsManager>();
        cardsManager.Cards.Add(gameObject);
        CanDrag = true;

        //set random number
        cardNumber = cardType.setAmount == 0 ? Random.Range(0, cardType.MaxCardNumber) : cardType.setAmount;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Stop from dragging if conditions dont meet
        if (!CanDrag)
            return;

        //Set our Card state
        _CardState = CardState.IsDragging;

        //Configure the card
        cardsManager.SelectedCard = gameObject;
        cardsManager.GetComponent<AudioSource>().Play();
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Stop from dragging if conditions dont meet
        if (!CanDrag)
            return;

        //Dragging the card
        Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out position);
            transform.position = canvas.transform.TransformPoint(position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Set Card State
        _CardState = CardState.Idle;

        //Configure the card
        cardsManager.SelectedCard = null;
        if (cardsManager.HoveringMenu != null)
        {
            if(cardsManager.HoveringMenu.GetComponent<CardHolder>().holderType == CardHolder.HolderType.CardTrader)
            {
                cardsManager.AddCard(cardNumber);
            }

            Transform target = transform.parent;
            transform.position = cardsManager.HoveringMenu.transform.position;
            transform.SetParent(cardsManager.HoveringMenu.transform);
            Destroy(target.gameObject);
        }
        else
        {
            transform.transform.localPosition = Vector2.zero;
        }
        cardsManager.GetComponent<AudioSource>().Play();
        GetComponent<Image>().raycastTarget = true;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Hovering = true || _CardState == CardState.IsDragging;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cardsManager.SelectedCard = gameObject;
    }
}
