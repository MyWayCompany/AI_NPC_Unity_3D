using UnityEngine;

public class InteractiveDoor : MonoBehaviour, IInteractable
{
    public float openAngle = 90f;
    public float openSpeed = 3f;

    private bool isOpen = false;
    private bool isAnimating = false;
    private float animationProgress = 0f;
    private float currentAngle = 0f;
    private float targetAngle = 0f;
    private float openDirection = 1f; 
    private Quaternion baseRotation;

    void Start()
    {
        baseRotation = transform.localRotation;
    }

    void Update()
    {
        if (isAnimating)
        {
            animationProgress += Time.deltaTime * openSpeed;

            if (animationProgress >= 1f)
            {
                animationProgress = 1f;
                isAnimating = false;
                currentAngle = targetAngle;
            }

            float t = Mathf.SmoothStep(0f, 1f, animationProgress);
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, t);

            transform.localRotation = baseRotation * Quaternion.Euler(0, currentAngle, 0);
        }
    }

    public string GetPromptText()
    {
        return isOpen ? "Закрыть дверь" : "Открыть дверь";
    }

    public void Interact(Vector3 interactorPosition)
    {
        if (isAnimating) return;

        if (!isOpen)
        {
            // Дверь закрыта → открываем
            Vector3 toPlayer = interactorPosition - transform.position;
            toPlayer.y = 0;

            Vector3 doorForward = transform.forward;
            float dot = Vector3.Dot(doorForward, toPlayer.normalized);

            // Запоминаем направление открытия
            openDirection = dot > 0 ? -1f : 1f;
            targetAngle = openDirection * openAngle;
        }
        else
        {
            // Дверь открыта → закрываем (возвращаем к 0)
            targetAngle = 0f;
        }

        isOpen = !isOpen;
        isAnimating = true;
        animationProgress = 0f;
    }
}