using UnityEngine;


public interface IInteractable
{

    string PromptText { get; }
    bool CanInteract { get; }
    void Interact();
    Transform PromptTransform { get; }
}
