using UnityEngine;

public class PlayerBusyState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player) 
    {
        if (player.currentAnimation != "")
            player.characterAnimator.SetTrigger(player.currentAnimation);
    }

    public override void UpdateState(PlayerStateManager player) 
    {

    }
}
