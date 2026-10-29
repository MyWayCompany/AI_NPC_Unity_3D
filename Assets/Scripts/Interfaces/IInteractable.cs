using UnityEngine;

public interface IInteractable
{
    string GetPromptText();
    void Interact(Vector3 interactorPosition); 
}
