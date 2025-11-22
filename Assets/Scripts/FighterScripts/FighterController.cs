using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FighterMovement))]
public class FighterController : MonoBehaviour
{
    [Header("References")]
    public FighterMovement movement;
    public CheckGrounded checkGrounded;
    public Animator anim;
    public List<AttackData> attacks = new List<AttackData>();

    [Header("Debug")]
    public bool printCompactSummary;
    public bool printVerboseLog;
    public float debugPrintInterval = 0.25f;

    private FighterInput input = new FighterInput();
    private FighterContext ctx = new FighterContext();
    private StatePermissions permissions = new StatePermissions();

    private HitstunSM hitstun;
    private BlockSM block;
    private AttackSM attackSM;
    private MovementSM moveSM;

    private InputHistory inputHistory = new InputHistory();
    private AttackResolver attackResolver;

    private float debugTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Enemy enemy;

    void Awake()
    {
        if (!movement) movement = GetComponent<FighterMovement>();
        if (!checkGrounded) checkGrounded = GetComponent<CheckGrounded>();

        hitstun = new HitstunSM(ctx);
        block   = new BlockSM(ctx);

        attackSM = new AttackSM(ctx)
        {
            OnAnimTrigger = (name) => { if (!string.IsNullOrEmpty(name) && anim) anim.SetTrigger(name); },
            OnAttackEnd   = () => { /* optional hook */ }
        };

        moveSM = new MovementSM(ctx)
        {
            OnMove = (x) => movement.Move(x),
            OnJump = () => movement.Jump(),
            OnCrouch = (c) => movement.SetCrouch(c),
            OnAnimState = (s) => { if (anim) anim.Play(s); }
        };

        attackResolver = new AttackResolver(attacks);
        AddAttack(Resources.Load<AttackData>("Upgrades/Attacks/Defaults/LightPunch"));
        AddAttack(Resources.Load<AttackData>("Upgrades/Attacks/Defaults/MediumPunch"));
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Start()
    { 
        enemy = FindFirstObjectByType<Enemy>();
    }

    public void FlipFacingDirection()
    {
        ctx.FacingDirection *= -1;
        var camSetter = FindFirstObjectByType<CameraTargetSetter>();
        if (camSetter != null)
            camSetter.FlipScreenX();
    }
    void Update()
    {
        if (!checkGrounded.IsGrounded() && input.CrouchHeld)
            input.OnGround = movement.ApplyMotionPhysics(1.5f);
        else
            input.OnGround = movement.ApplyMotionPhysics(1f);

        input.Velocity = movement.GetVelocity();

        // 2) State machines
        hitstun.Tick(Time.deltaTime);
        block.Tick(Time.deltaTime, input);
        attackSM.Tick(Time.deltaTime);
        permissions.Resolve(ctx);
        moveSM.Tick(Time.deltaTime, input);

        // 3) Record inputs for buffer/motions
        inputHistory.Record(input, ctx);
        
        // 4) Attack Resolviing 
        bool attackPressedThisFrame =
            input.LightPunchPressed  ||
            input.MediumPunchPressed ||
            input.HeavyPunchPressed  ||
            input.LightKickPressed   ||
            input.MediumKickPressed  ||
            input.HeavyKickPressed;

        if (ctx.CanAcceptNewAttack && !attackSM.IsAttacking && attackPressedThisFrame)
        {
            var resolved = attackResolver.Resolve(inputHistory);
            if (resolved != null)
                attackSM.BeginAttack(resolved, transform);
        }


        // 5) Debug prints (unchanged)
        debugTimer += Time.deltaTime;
        if (debugTimer >= debugPrintInterval)
        {
            debugTimer = 0f;
            if (printCompactSummary) Debug.Log($"Input Summary: {inputHistory.GetSummary()}");
            if (printVerboseLog) Debug.Log(inputHistory.GetDetailedLog());
        }

        // 6) Reset one-frame inputs (unchanged)
        input.JumpPressed = false;
        input.LightPunchPressed  = false;
        input.MediumPunchPressed = false;
        input.HeavyPunchPressed  = false;
        input.LightKickPressed   = false;
        input.MediumKickPressed  = false;
        input.HeavyKickPressed   = false;
        
        // 7) Change Color Based On State
        switch (ctx.Attack)
        {
            case FighterContext.AttackState.Startup:
                spriteRenderer.color = new Color(originalColor.r * 1.5f, originalColor.g * 1.5f, originalColor.b * 1.5f, 1f);
                break;
            case FighterContext.AttackState.Active:
                spriteRenderer.color = Color.red;
                break;
            case FighterContext.AttackState.Recovery:
                spriteRenderer.color = Color.gray; //new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f, 1f);
                break;
            case FighterContext.AttackState.None:
                spriteRenderer.color = originalColor;
                break;
        }
        
        //8) Enemy
        if (enemy == null)
        {
            enemy = FindFirstObjectByType<Enemy>();
        }
        else
        {
            CheckFacingDirection();
        }
    }

    // === Input System callbacks (unchanged) ===
    void OnMove(InputValue value)   { input.MoveX = value.Get<float>(); input.BackHeld = input.MoveX < 0; }
    void OnJump(InputValue value)   { if (value.isPressed) input.JumpPressed = true; }
    void OnCrouch(InputValue value) { input.CrouchHeld = value.isPressed; }

    void OnLightPunch (InputValue v) { if (v.isPressed) input.LightPunchPressed  = true; }
    void OnMediumPunch(InputValue v) { if (v.isPressed) input.MediumPunchPressed = true; }
    void OnHeavyPunch (InputValue v) { if (v.isPressed) input.HeavyPunchPressed  = true; }

    void OnLightKick  (InputValue v) { if (v.isPressed) input.LightKickPressed   = true; }
    void OnMediumKick (InputValue v) { if (v.isPressed) input.MediumKickPressed  = true; }
    void OnHeavyKick  (InputValue v) { if (v.isPressed) input.HeavyKickPressed   = true; }

    public string GetCompactSummary() => inputHistory.GetSummary();

    public void AddAttack(AttackData data)
    {
        if (!attacks.Contains(data))
        {
            attacks.Add(data.Clone());
        }
    }

    private void CheckFacingDirection()
    {
        if (this.transform.position.x > enemy.transform.position.x & ctx.FacingDirection != -1)
        {
            FlipFacingDirection();
        } else if (this.transform.position.x < enemy.transform.position.x & ctx.FacingDirection == -1)
        {
            FlipFacingDirection();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Flip"))
        {
            FlipFacingDirection();
        }
    }
}
