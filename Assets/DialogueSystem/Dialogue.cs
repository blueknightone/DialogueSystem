using System.Collections.Generic;
using UnityEngine;

namespace lastmilegames.DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue System/Dialogue Node", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField, TextArea(5, 10)] private string speakerText;
        [SerializeField] private Texture2D speakerImage;
        [SerializeField] private Texture2D[] backgroundSpeakerImages;

        #region Constructors

        /// <summary>
        /// Creates a new instance of a dialogue.
        /// </summary>
        /// <param name="speakerText">(Required) The spoken dialogue.</param>
        /// <param name="speakerImage">(Optional) The active speaker's sprite.</param>
        /// <param name="backgroundSpeakerImages">(Optional) The background speaker's sprites.</param>
        public Dialogue(string speakerText, Texture2D speakerImage = null, Texture2D[] backgroundSpeakerImages = null)
        {
            this.speakerText = speakerText;
            this.speakerImage = speakerImage;
            this.backgroundSpeakerImages = backgroundSpeakerImages;
        }

        #endregion

    }
}