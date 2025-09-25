using UnityEngine;

public interface IPlayerState
{
    void Enter(PlayerStateManager player);
    void Execute();
    void Exit();
}

