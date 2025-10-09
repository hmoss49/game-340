using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraTargetSetter : MonoBehaviour
{
    private CinemachineCamera vcam;
    private CinemachinePositionComposer composer;

    [Header("Framing Settings")]
    [SerializeField] private Vector2 leftSide = new Vector2(-0.3f, -0.05f);
    [SerializeField] private Vector2 rightSide = new Vector2(0.3f, -0.05f);
    [SerializeField] private float flipSpeed = 3f;

    private bool facingRight = false;   // starts facing left
    private float currentX;
    private float targetX;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        composer = vcam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
    }

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            vcam.Follow = player.transform;

        // start camera on LEFT side
        currentX = leftSide.x;
        targetX = currentX;

        if (composer != null)
        {
            var comp = composer.Composition;
            comp.ScreenPosition = leftSide;
            composer.Composition = comp;
        }
    }

    void Update()
    {
        if (composer == null) return;

        // smooth transition between sides
        currentX = Mathf.Lerp(currentX, targetX, Time.deltaTime * flipSpeed);

        var comp = composer.Composition;
        comp.ScreenPosition = new Vector2(currentX, leftSide.y);
        composer.Composition = comp;
    }

    // Called when entering or exiting a trigger
    public void FlipScreenX()
    {
        facingRight = !facingRight;
        targetX = facingRight ? rightSide.x : leftSide.x;
    }
}