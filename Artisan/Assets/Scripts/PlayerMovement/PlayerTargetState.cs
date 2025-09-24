using UnityEngine;

public class PlayerTargetState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        
    }

    public override void UpdateState(PlayerStateManager player) 
    {
        //What are doing during this state?
        //player.MovePlayer(player.default_speed / 4);
        player.CheckTarget();

        //On what conditions do we leave the state?
        if (player.isTargeting == false && player.movement.magnitude < 0.1)
        {
            player.SwitchState(player.idleState);
        }
        else if (player.isTargeting == false)
        {
            player.SwitchState(player.walkState);
        }
    }
}
