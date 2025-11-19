using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardFace : MonoBehaviour
{
    //Refences we have to set in the inspector
    [Header("Refences")]
    public GameObject target;
    public GameObject Visual;
    public Image Icon;
    public Image IconShadow;
    public TMP_Text RightNumber;
    public TMP_Text LeftNumber;

    //public setting changes to our cards movement
    [Header("Settings")]
    public float rotationSpeed;
    public float movementSpeed;
    public float rotationAmount;
    public Vector3 offset;

    //private setting changes to our cards movement
    private Vector3 rotation;
    private Vector3 movement;
    private float randomRot;
    private bool Hovering;

    private void Start()
    {
        randomRot = Random.Range(-rotationAmount, rotationAmount);
    }

    void Update()
    {
        //Make sure our refences are set
        if (target == null)
            return;

        //Set our hovering boolean to the targets hovering
        Hovering = target.GetComponent<Card>().Hovering && target.GetComponent<Card>()._CardState != Card.CardState.Played;

        //Set Cards visual to the target cards position
        transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * movementSpeed);
        Visual.transform.position = Vector3.Lerp(Visual.transform.position, (Hovering || target.GetComponent<Card>().cardsManager.SelectedCard == target) ? target.transform.position + offset : target.transform.position, Time.deltaTime * movementSpeed);

        //If Card is not played
        if (target.GetComponent<Card>()._CardState != Card.CardState.Played)
        {
            // Calculate position offset relative to the camera
            Vector3 localPos = Camera.main.transform.InverseTransformPoint(transform.position) - Camera.main.transform.InverseTransformPoint(target.transform.position);
            movement = Vector3.Lerp(movement, localPos, 10 * Time.deltaTime); // Adjust speed as needed

            // Sway effect
            Vector3 movementRotation = movement;
            rotation = Vector3.Lerp(rotation, movementRotation, rotationSpeed * Time.deltaTime);

            // Apply sway effect using Quaternion for smoother rotation
            float clampedRotation = Mathf.Clamp(movementRotation.x, -rotationAmount, rotationAmount);
            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, clampedRotation);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, randomRot);
        }

        UpdateCardInfo();
    }

    public void UpdateCardInfo()
    {
        CardType Info = target.GetComponent<Card>().cardType;

        Icon.sprite = Info.CardIcon;
        IconShadow.sprite = Info.CardIcon;
        RightNumber.text = target.GetComponent<Card>().cardNumber.ToString();
        LeftNumber.text = target.GetComponent<Card>().cardNumber.ToString();
    }
}
