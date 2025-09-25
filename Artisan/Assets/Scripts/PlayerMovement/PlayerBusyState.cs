using UnityEngine;

public class PlayerBusyState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player) 
    {
        player.characterAnimator.SetFloat("Speed", 0);
    }

    public override void UpdateState(PlayerStateManager player) 
    {

    }
}
