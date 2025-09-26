using UnityEngine;

public class PlayerAnimatorHelper : MonoBehaviour
{

    PlayerStateManager player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = transform.parent.GetComponent<PlayerStateManager>();
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
}
