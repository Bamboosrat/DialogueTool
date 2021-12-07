using UnityEngine;
using UnityEngine.UI;
using DialogueTool;
using TMPro;

namespace Test.UI
{
    public class DialogueUI : MonoBehaviour
    {
        #region Fields / Variables
        private PlayerConversant playerConversant;

        [SerializeField] private TextMeshProUGUI AIText;
        [SerializeField] private TextMeshProUGUI conversantName;
        [SerializeField] private GameObject AIResponse;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button quitButton;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(() => playerConversant.Next());
            quitButton.onClick.AddListener(() => playerConversant.Quit());

            UpdateUI();
        }
        #endregion

        #region Update UI Method
        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive())
            {
                return;
            }
            conversantName.text = playerConversant.GetCurrentConversantName();
            AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());
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
        #endregion

        #region BuildChoiceList Method
        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }
            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }
        #endregion
    }
}
