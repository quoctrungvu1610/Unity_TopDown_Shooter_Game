using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    PlayerConversant playerConversant;
    [SerializeField] TextMeshProUGUI AIText;
    [SerializeField] Button nextButton;
    [SerializeField] GameObject AIResponse;
    [SerializeField] Transform choiceRoot;
    [SerializeField] GameObject choicePrefab;
    [SerializeField] Button quitButton;
    [SerializeField] TextMeshProUGUI conversantName;

    void Start()
    {
        playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        playerConversant.onConversationUpdated += UpdateUI;
        nextButton.onClick.AddListener(() => playerConversant.Next());
        quitButton.onClick.AddListener(() => playerConversant.Quit());
        UpdateUI();
    }

    private void Next() 
    {
        playerConversant.Next();
    }

    private void UpdateUI()
    {
        gameObject.SetActive(playerConversant.IsActive());

        if (!playerConversant.IsActive()) 
        {
            return;
        }

        AIResponse.SetActive(!playerConversant.IsChoosing());
        choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

        conversantName.text = playerConversant.GetCurrentConversantName();

        if (playerConversant.IsChoosing())
        {
            BuildChoiceList();
        }
        else 
        {
            AIText.text = playerConversant.GetText();
            nextButton.gameObject.SetActive(playerConversant.HasNext());
        }
    }

    private void BuildChoiceList()
    {
        foreach (Transform item in choiceRoot)
        {
            Destroy(item.gameObject);
        }

        foreach (DialogueNode choice in playerConversant.GetChoices())
        {
            GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
            TextMeshProUGUI textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
            textComp.text = choice.GetText();

            Button button = choiceInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                playerConversant.SelectChoice(choice);
            });
        }
    }
}
