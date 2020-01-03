# Dialogue System

A ScriptableObject based dialogue system with speaker images and flavorable dialogue.

## Requirements

* Unity 2019.3 or newer
* TextMeshPro

## Installation

1. Add the `TextMeshPro` package from the Unity Package Manager, if you haven't already.
2. Import the `.unitypackage` into your project


## Usage

1. Add the `UI > ui_Dialogue` prefab must be added to your scene.
2. Create a dialogue node using `Assets > Create > Dialogue > New Dialogue Node`
3. Create story choices using `Assest > Create > Dialogue > New Story Choice`
4. Call `DialogueUI.StartConversation()` from the object that you want to start the conversation and pass it your starting conversation.

### What do these fields mean?

Check the tooltips and sample project to see how it all works together.

#### `DialogueUI : MonoBehaviour`

This is the main script driving the dialogue system UI. The prefab `ui_Dialogue` includes a canvas, but also needs an `EventSystem` to work properly.

* `speakerNameObject`: The TextMeshPro UI Text component that will display the name of the speaker (`DialogueNode.speakerName`).
* `dialogueTextObject`: The TextMeshPro UI Text component that will display the speaker's lines (`DialogueNode.defaultSpeakerDialogue` or `DialogueNode.speakerDialogueWhenTrue`)
* `speakerImageObject`: (Required if any `DialogueNode.speakerImage` is set.) The Image component that will display the speaker's sprite.
* `responsePanel`: The UI Panel that will contain the response buttons. The prefab includes one that uses a `VerticalGroupLayout` component.
* `responseButton`: The prefab TextMeshPro UI Button that will display a single choice. These are the buttons that will advance a started conversation. (`DialogueNode.responses`)
* `endConversationButton`: the prefab TextMeshPro UI Button that will end the conversation. Will be automatically displayed on if a dialogue node has no responses. (`DialogueNode.responses`)

#### `DialogueNode : ScriptableObject`

Represents a single node in a conversation. `DialogueNode.Read` will reset to false when the object is loaded.

* `speakerImage`: (Optional) The `Sprite` to display on the `DialogueUI.speakerImageObject` Image component.
* `speakerName`: The `string` name to display in the `DialogueUI.speakerNameObject` TextMeshPro Text UI component
* `ignoreOnceRead`: Checking this box will cause `DialogueUI` to ignore this dialogue node once it has been read once.
* `responseLabel`: Text rendered by the response button for the previous dialogue node.
* `responses`: The next dialogue nodes in the conversation (branching dialogue).
* `choiceToActivate`: The `StoryChoice` node that will be set to true when this dialogue node is read.
* `storyChoice`: (Optional) The `StoryChoice` node that will determine which text to be displayed by `DialogueUI.dialogueTextObject`
* `defaultSpeakerDialogue`: The text that will be displayed by `DialogueUI.dialogueTextObject` if no `StoryChoice` has been selected or `StoryChoice` is inactive.
* `speakerDialogueWhenTrue`: The text that will be displayed by `DialogueUI.dialogueTextObject` when `StoryChoice` has been activated.


#### `StoryChoice : ScriptableObject`

Represents a single state for story keeping purposes. Can be reference throughout a project to reflect narrative choices by the player.

* `initialValue`: The default value (initially `false` or 'inactive') of the story choice
* `Value`: The runtime value of the story choice. Is set to `initialValue` when object is first loaded.
* `ApplyState()`: Saves the runtime value to the asset, allowing the updated state to persist between plays.
* `ActivateStoryChoice`: Sets the runtime `Value` of the choice to `true` (or 'active')

#### `ResponseButton : MonoBehaviour`

Updates the current conversation node (`DialogueUI.conversationNode`) with the `nextConversationNode`. Typically auto-instantiated by `DialougeUI`

* `nextConversationNode`: The next `DialogueNode` to load when the button is clicked.
* `dialogueUI`: The GameObject containing the `DialogueUI` component.
* `OnClick`: The function called by the button's On Click event listener.

#### `EndConversationButton : MonoBehaviour`

Ends the current conversation by calling `Dialogue.EndConversation()` when clicked. Typically auto-instantiated by `DialogueUI` when the current `DialogueNode.responses` is empty.

* `dialogueUI`: The GameObject containing the `DialogueUI` component.
* `OnClick`: The function called by the button's On Click event listener. 

 ## FAQ
 
 * **I get the error `ArgumentException: Object of type 'UnityEngine.Object' cannot be converted to type 'lastmilegames.DialogueSystem.DialogueNode'`**
    * Make sure that you are passing a starting `DialogueNode` to `DialogueUI.StartConversation()` 