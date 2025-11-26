using UnityEngine;

public class PlayerAnimatorHelper : MonoBehaviour
{
    [SerializeField]
    AudioClip footstep;
    [SerializeField]
    AudioClip hit;
    PlayerStateManager player;
    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = transform.parent.GetComponent<PlayerStateManager>();
        audioSource = GetComponent<AudioSource>();
    }

    void LockPlayer(int l)
    {
        if (l == 1)
        {
            player.SwitchState(player.busyState);
        }
        else
        {
            player.SwitchState(player.idleState);
        }
    }

    void Footstep()
    {
        audioSource.PlayOneShot(footstep);
    }

    void HitSound()
    {
        audioSource.PlayOneShot(hit);
    }
}
