using System.Collections.Generic;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue System/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        public List<DialogueResponse> responses;
        public DialogueCondition enterCondition;
        
        [SerializeField, TextArea(5, 10)] private string speakerText;
        [SerializeField] private Texture2D speakerImage;
        [SerializeField] private List<Texture2D> backgroundSpeakerImages;

        #region Constructors

        public Dialogue()
        {
            speakerText = "";
            speakerImage = null;
            backgroundSpeakerImages = null;
        }

        #endregion

    }
}