using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FighterMovement))]
public class FighterController : MonoBehaviour
{
    [Header("References")]
    public FighterMovement movement;
    public CheckGrounded checkGrounded;
    public Animator anim;

    [Header("Debug")]
    public bool printCompactSummary = false; // prints "236LP" to Console periodically
    public bool printVerboseLog = false;    // per-frame detailed table to Console
    public float debugPrintInterval = 0.25f;

    private FighterInput input = new FighterInput();
    private FighterContext ctx = new FighterContext();
    private FighterPermissions permissions = new FighterPermissions();

    private HitstunSM hitstun;
    private BlockSM block;
    private AttackSM attack;
    private MovementSM moveSM;

    // Input history
    private InputHistory inputHistory = new InputHistory();
    private float debugTimer = 0f;

    void Awake()
    {
        if (!movement) movement = GetComponent<FighterMovement>();
        if (!checkGrounded) checkGrounded = GetComponent<CheckGrounded>();

        hitstun = new HitstunSM(ctx);
        block   = new BlockSM(ctx);
        attack  = new AttackSM(ctx)
        {
            OnHitboxEnable = () => { /* enable hitbox */ },
            OnHitboxDisable = () => { /* disable hitbox */ },
            OnAnimTrigger = (name) => { if (anim) anim.SetTrigger(name); }
        };
        moveSM  = new MovementSM(ctx)
        {
            OnMove = (x) => movement.Move(x),
            OnJump = () => movement.Jump(),
            OnCrouch = (c) => movement.SetCrouch(c),
            OnAnimState = (s) => { if (anim) anim.Play(s); }
        };
    }

    void Update()
    {
        // 1) Update grounded + verticalVel
        if (!checkGrounded.IsGrounded() && input.CrouchHeld)
        {
            input.OnGround = movement.ApplyGravity(2f);
        }
        else
        {
            input.OnGround = movement.ApplyGravity(1f);
        }
        input.VerticalVel = movement.GetVerticalVelocity();

        // 2) State machines by priority
        hitstun.Tick(Time.deltaTime);
        block.Tick(Time.deltaTime, input);
        attack.Tick(Time.deltaTime);

        // 3) Resolve permissions
        permissions.Resolve(ctx);

        // 4) Movement logic
        moveSM.Tick(Time.deltaTime, input);

        // 5) Handle attacks (priority: Heavy > Medium > Light per type)
        if (ctx.CanAcceptNewAttack)
        {
            var btn = ReadAttackButtonPressed();
            if (btn != AttackSM.AttackButton.None)
                attack.BeginAttack(btn);
        }

        // 6) Record this frame into history (for summaries/motions)
        inputHistory.Record(input, ctx);

        // 7) Debug printing
        debugTimer += Time.deltaTime;
        if (debugTimer >= debugPrintInterval)
        {
            debugTimer = 0f;
            if (printCompactSummary)
                Debug.Log($"Input Summary: {inputHistory.GetSummary()}");
            if (printVerboseLog)
                Debug.Log(inputHistory.GetDetailedLog());
        }

        // 8) Reset one-frame inputs
        input.JumpPressed = false;

        input.LightPunchPressed  = false;
        input.MediumPunchPressed = false;
        input.HeavyPunchPressed  = false;

        input.LightKickPressed   = false;
        input.MediumKickPressed  = false;
        input.HeavyKickPressed   = false;
    }

    // === Input System callbacks (Send Messages) ===
    void OnMove(InputValue value)
    {
        input.MoveX = value.Get<float>();
        input.BackHeld = input.MoveX < 0; // until facing system added
    }

    void OnJump(InputValue value)          { if (value.isPressed) input.JumpPressed = true; }
    void OnCrouch(InputValue value)        { input.CrouchHeld = value.isPressed; }

    void OnLightPunch (InputValue v) { if (v.isPressed) input.LightPunchPressed  = true; }
    void OnMediumPunch(InputValue v) { if (v.isPressed) input.MediumPunchPressed = true; }
    void OnHeavyPunch (InputValue v) { if (v.isPressed) input.HeavyPunchPressed  = true; }

    void OnLightKick  (InputValue v) { if (v.isPressed) input.LightKickPressed   = true; }
    void OnMediumKick (InputValue v) { if (v.isPressed) input.MediumKickPressed  = true; }
    void OnHeavyKick  (InputValue v) { if (v.isPressed) input.HeavyKickPressed   = true; }

    // Expose input summary for on-screen overlay
    public string GetCompactSummary() => inputHistory.GetSummary();

    private AttackSM.AttackButton ReadAttackButtonPressed()
    {
        // Simple priority: Heavy > Medium > Light (punches first, then kicks).
        if (input.HeavyPunchPressed)  return AttackSM.AttackButton.HP;
        if (input.MediumPunchPressed) return AttackSM.AttackButton.MP;
        if (input.LightPunchPressed)  return AttackSM.AttackButton.LP;

        if (input.HeavyKickPressed)   return AttackSM.AttackButton.HK;
        if (input.MediumKickPressed)  return AttackSM.AttackButton.MK;
        if (input.LightKickPressed)   return AttackSM.AttackButton.LK;

        return AttackSM.AttackButton.None;
    }
}
