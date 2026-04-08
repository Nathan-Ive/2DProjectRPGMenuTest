using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueBox dialogueBox;

    [TextArea(3, 5)]
    public List<string> lines;

    public void TriggerDialogue()
    {
        dialogueBox.StartDialogue(lines);
    }
}