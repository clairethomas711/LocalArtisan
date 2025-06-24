using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player) 
    {

    }

    public override void UpdateState(PlayerStateManager player) 
    {
        //What are doing during this state?
        player.CheckTarget();
        
        //On what conditions do we leave the state?
        if (player.isTargeting)
        {
            player.SwitchState(player.targetState);
        }
        else if (player.movement.magnitude > 0.1)
        {
            player.SwitchState(player.walkState);
        }
    }
}
