using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class CardHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardsManager cardsManager;

    [Header("Card Holder Logic")]
    public bool available;
    public bool hasToHaveSameNumberOrColor;
    public int maxAmount;
    public int amountToComplete;

    public HolderType holderType;

    [Header("UI")]
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI discardCounterText;
    public TextMeshProUGUI timerText;
    public GameObject endGameText; // UI текст для победы/поражения

    [Header("Log UI")]
    public TextMeshProUGUI comparisonLogUI;

    [Header("Timer Settings")]
    public float maxTime = 60f; // максимум времени

    [Header("Timer Blink Settings")]
    public float blinkThreshold = 10f; // время, с которого таймер начинает мигать
    public Color normalColor = Color.white;
    public Color blinkColor = Color.red;
    private bool blinkState = false;
    private float blinkInterval = 0.5f;
    private float blinkTimer = 0f;

    [Header("End Game Audio")]
    public AudioClip victoryClip;
    public AudioClip defeatClip;
    public float endGameDelay = 5f;

    private AudioSource audioSource;
    private float timer;
    private int discardedCards = 0;
    private bool endGameTriggered = false; // событие срабатывает один раз

    // Для предотвращения спама лога
    private bool lastComparisonMatch = false;
    private bool hasCompared = false;

    public enum HolderType
    {
        Play, Discard, CardTrader, MainHolder
    }

    private void Start()
    {
        timer = 0f;
        if (endGameText != null)
            endGameText.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Обнуляем discard при старте
        if (holderType == HolderType.Discard)
            discardedCards = 0;
    }

    private void Update()
    {
        if (!endGameTriggered)
        {
            timer += Time.deltaTime;

            // Проверяем количество карт для победы
            int currentAmount = holderType == HolderType.Play ? transform.childCount - 3 : transform.childCount;
            bool victory = currentAmount >= amountToComplete;

            if (victory)
            {
                StartCoroutine(EndGameRoutine(true)); // победа
            }
            else if (timer >= maxTime)
            {
                StartCoroutine(EndGameRoutine(false)); // поражение
            }

            UpdateUI();
            HandleCardHolderFunctionality();
            BlockChildrenCards();
        }
    }

    private void BlockChildrenCards()
    {
        foreach (Transform child in transform)
        {
            if (cardsManager.Cards.Contains(child.gameObject))
                cardsManager.Cards.Remove(child.gameObject);

            Card c = child.GetComponent<Card>();
            if (c != null)
            {
                c.CanDrag = false;
                c._CardState = Card.CardState.Played;
            }
        }
    }

    private void UpdateUI()
    {
        int currentAmount = holderType == HolderType.Play ? transform.childCount - 3 : transform.childCount;

        if (counterText != null)
        {
            counterText.text = currentAmount >= amountToComplete ? "Completed!" : $"{currentAmount} / {amountToComplete}";
        }

        if (discardCounterText != null)
        {
            discardCounterText.text = $"{discardedCards}";
        }

        if (timerText != null)
        {
            float displayTime = Mathf.Min(timer, maxTime);
            timerText.text = $"Time: {displayTime:F1}";

            if (maxTime - timer <= blinkThreshold)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= blinkInterval)
                {
                    blinkState = !blinkState;
                    timerText.color = blinkState ? blinkColor : normalColor;
                    blinkTimer = 0f;
                }
            }
            else
            {
                timerText.color = normalColor;
            }
        }
    }

    private Coroutine logCoroutine;
    private float logDuration = 1.5f;

    private void AddLog(string message)
    {
        Debug.Log(message);

        string uiMessage = " ";
        if (message.StartsWith("Match:"))
            uiMessage = "Match";
        else if (message.StartsWith("Don't Match"))
            uiMessage = "Don't Match";

        if (!string.IsNullOrEmpty(uiMessage) && comparisonLogUI != null)
        {
            if (logCoroutine != null)
                StopCoroutine(logCoroutine);

            comparisonLogUI.text = uiMessage;
            logCoroutine = StartCoroutine(ClearLogAfterDelay());
        }
    }

    private IEnumerator ClearLogAfterDelay()
    {
        yield return new WaitForSeconds(logDuration);
        if (comparisonLogUI != null)
            comparisonLogUI.text = " ";
        logCoroutine = null;
    }

    public void RegisterDiscard()
    {
        discardedCards++;
        UpdateUI();
    }

    public void HandleCardHolderFunctionality()
    {
        switch (holderType)
        {
            case HolderType.Play:
                HandlePlayHolder();
                break;
            case HolderType.Discard:
                available = true;
                discardedCards = 0;
                foreach (Transform child in transform)
                {
                    if (child.GetComponent<Card>() != null)
                        discardedCards++;
                }
                break;
            case HolderType.CardTrader:
                available = true;
                break;
            case HolderType.MainHolder:
                available = true;
                break;
        }
    }

    private void HandlePlayHolder()
    {
        if (hasToHaveSameNumberOrColor)
        {
            if (cardsManager.SelectedCard != null && transform.childCount > 3)
            {
                Card newCard = cardsManager.SelectedCard.GetComponent<Card>();
                Card lastCard = transform.GetChild(transform.childCount - 1).GetComponent<Card>();

                bool match =
                    newCard.cardNumber == lastCard.cardNumber ||
                    newCard.cardType.CardIcon == lastCard.cardType.CardIcon;

                if (!hasCompared || lastComparisonMatch != match)
                {
                    string newCardFull = $"{newCard.cardType.name} {newCard.cardNumber}";
                    string lastCardFull = $"{lastCard.cardType.name} {lastCard.cardNumber}";
                    Debug.Log($"Сравнение карт: {newCardFull}  VS  {lastCardFull}");

                    AddLog(match ? $"Match: {newCardFull}" : "Don't Match");

                    lastComparisonMatch = match;
                    hasCompared = true;
                }

                available = match ? transform.childCount - 3 < maxAmount : false;
            }
            else
            {
                available = true;
                hasCompared = false;
            }
        }
        else
        {
            available = transform.childCount - 3 < maxAmount;
        }
    }

    private IEnumerator EndGameRoutine(bool victory)
    {
        endGameTriggered = true;

        if (endGameText != null)
        {
            endGameText.SetActive(true);
            endGameText.GetComponent<TextMeshProUGUI>().text = victory ? "Victory!" : "Defeat!";
        }

        if (audioSource != null)
        {
            AudioClip clipToPlay = victory ? victoryClip : defeatClip;
            if (clipToPlay != null)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();
            }
        }

        yield return new WaitForSeconds(endGameDelay);

        string sceneName = victory ? "Menu" : "Menu";
        SceneManager.LoadScene(sceneName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (available)
            cardsManager.HoveringMenu = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (available)
            cardsManager.HoveringMenu = null;
    }
}
