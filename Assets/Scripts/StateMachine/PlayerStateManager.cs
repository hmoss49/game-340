using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public PlayerState currentState;
    private IPlayerState stateBehavior;

    [Header("Components")]
    public FighterController controller;
    public CheckGrounded checkGrounded;

    void Start()
    {
        // Start in Idle state
        ChangeState(new IdleState());
    }

    void Update()
    {
        stateBehavior?.Execute();
    }

    public void ChangeState(IPlayerState newState)
    {
        stateBehavior?.Exit();
        stateBehavior = newState;
        stateBehavior.Enter(this);
        currentState = GetStateType(newState);
    }

    private PlayerState GetStateType(IPlayerState state)
    {
        if (state is IdleState) return PlayerState.Idle;
        if (state is WalkingState) return PlayerState.Walking;
        if (state is JumpingState) return PlayerState.Jumping;
        if (state is FallingState) return PlayerState.Falling;
        return PlayerState.Idle;
    }
}