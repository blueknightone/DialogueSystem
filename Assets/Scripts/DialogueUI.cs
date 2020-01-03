using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lastmilegames.DialogueSystem
{
    public class DialogueUI : MonoBehaviour
    {
        // ReSharper disable RedundantDefaultMemberInitializer
        [SerializeField, Tooltip("The TextMeshPro UI GameObject that will display the speaker's name.")]
        private TextMeshProUGUI speakerNameObject = null;
        [SerializeField, Tooltip("The TextMeshPro UI GameObject that will display the dialogue for the conversation.")]
        private TextMeshProUGUI dialogueTextObject = null;
        [SerializeField, Tooltip("Optional. The Image component that will display the speaker's sprite.")]
        private Image speakerImageObject = null;
        [SerializeField,
         Tooltip("The GameObject that will contain the response buttons.")]
        private GameObject responsePanel = null;
        [SerializeField, Tooltip("The prefab for the response buttons.")]
        private GameObject responseButton = null;
        [SerializeField, Tooltip("The prefab for the button to end the conversation.")]
        private GameObject endConversationButton = null;
        // ReSharper restore RedundantDefaultMemberInitializer
        private bool _isDirty; // Set to true to refresh the dialogue.

        /// <summary>
        /// The dialogue node to display
        /// </summary>
        [HideInInspector] public DialogueNode conversationNode;

        private void Awake()
        {
            // Hide Dialogue UI when game starts
            transform.gameObject.SetActive(false);
            
            // Ensure button prefabs have the correct components attached.
            if (responseButton.GetComponent<ResponseButton>() == null)
                responseButton.AddComponent(typeof(ResponseButton));
            if (endConversationButton.GetComponent<EndConversationButton>() == null)
                endConversationButton.AddComponent(typeof(EndConversationButton));
        }

        private void Update()
        {
            if (!_isDirty) return; // If not dirty, skip everything else.
            conversationNode.ActivateStoryChoice();

            UpdateSpeaker();
            UpdateResponses();
        }

        /// <summary>
        /// Update the speaker
        /// </summary>
        private void UpdateSpeaker()
        {
            speakerNameObject.text = conversationNode.SpeakerName;
            dialogueTextObject.text = conversationNode.GetStoryDialogue();
            if (speakerImageObject && conversationNode.SpeakerImage)
            {
                speakerImageObject.sprite = conversationNode.SpeakerImage;
            }
        }

        /// <summary>
        /// Instantiates buttons for each response in the conversation node
        /// </summary>
        private void UpdateResponses()
        {
            ClearResponsePanel();
            if (conversationNode.Responses.Count > 0)
            {
                // If the conversation can continue, draw the available responses.
                foreach (DialogueNode response in conversationNode.Responses)
                {
                    // Skip the response if we read it and don't want to see it again.
                    if (response.IgnoreOnceRead && response.Read) continue;
                    GameObject button = Instantiate(responseButton, responsePanel.transform);

                    button.GetComponentInChildren<TextMeshProUGUI>().text = response.ResponseLabel;
                    button.GetComponent<ResponseButton>().nextConversationNode = response;
                    button.GetComponent<ResponseButton>().dialogueUI = this;
                }
            }
            else // allow the player to end the dialogue.
            {
                GameObject button = Instantiate(endConversationButton, responsePanel.transform);
                button.GetComponent<EndConversationButton>().dialogueUI = this;
            }

            _isDirty = false;
        }

        /// <summary>
        /// Clears all the buttons from the response panel in preparation for new responses.
        /// </summary>
        private void ClearResponsePanel()
        {
            foreach (Transform child in responsePanel.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Shows the conversation UI and sets the first node in the conversation.
        /// </summary>
        /// <param name="startingNode">The starting conversation node.</param>
        [UsedImplicitly]
        public void StartConversation(DialogueNode startingNode)
        {
            gameObject.SetActive(true);
            conversationNode = startingNode;
            _isDirty = true;
        }

        /// <summary>
        /// Hides the Dialogue UI.
        /// </summary>
        public void EndConversation()
        {
            transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// Triggers a Dialogue UI update.
        /// </summary>
        public void SetDirty()
        {
            _isDirty = true;
        }
    }
}