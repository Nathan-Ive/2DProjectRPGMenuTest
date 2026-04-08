using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public DialogueTrigger trigger;

    void Start()
    {
        trigger.TriggerDialogue();
    }
}