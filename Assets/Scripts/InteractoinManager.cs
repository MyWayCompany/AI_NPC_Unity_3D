using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public float interactionRange = 2f;
    public LayerMask interactableLayer;

    [Header("Мировая подсказка")]
    public GameObject promptPrefab;

    private WorldPrompt prompt;
    private IInteractable currentInteractable;
    private Collider currentCollider;

    void Start()
    {
        // Создаём одну подсказку на всю игру
        prompt = Instantiate(promptPrefab).GetComponent<WorldPrompt>();
        prompt.Hide();
    }

    void Update()
    {        
        if (ConversationController.Instance != null && ConversationController.Instance.IsBusy)
        {
            prompt.Hide();
            return;
        }

        FindInteractable();

        if (currentInteractable != null && currentCollider != null)
        {          
            Bounds b = currentCollider.bounds;
            Vector3 pos = b.center + Vector3.up * (b.extents.y + 1f);
          
            string text = $"<color=#FFD700>[E]</color> {currentInteractable.GetPromptText()}";
            prompt.Show(text, pos);
        }
        else
        {
            prompt.Hide();
        }

        // Нажатие E
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard[Key.E].wasPressedThisFrame)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact(transform.position);
            }
        }
    }

    void FindInteractable()
    {
        currentInteractable = null;
        currentCollider = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);

        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentInteractable = interactable;
                    currentCollider = hit;
                }
            }
        }
    }
}