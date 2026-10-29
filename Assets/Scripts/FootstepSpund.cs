using UnityEngine;

public class FootstepSpund : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public AudioSource audioSource;
    public PlayerController playerController;
    public float stepInterval = 0.4f;

    private float lastStepTime = 0f;
    private int clipIndex = 0;

    void Update()
    {
        if (playerController != null && playerController.IsMoving)
        {
            if (Time.time - lastStepTime > stepInterval)
            {
                PlayFootstep();
                lastStepTime = Time.time;
            }
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0 && audioSource != null)
        {
            audioSource.clip = footstepClips[clipIndex];
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
            clipIndex = (clipIndex + 1) % footstepClips.Length;
        }
    }
}