using UnityEngine;

public class FighterInput
{
    public float MoveX;           // Horizontal input (-1..+1)
    public bool JumpPressed;      // One-frame
    public bool CrouchHeld;

    // Six SF buttons (one-frame)
    public bool LightPunchPressed;
    public bool MediumPunchPressed;
    public bool HeavyPunchPressed;

    public bool LightKickPressed;
    public bool MediumKickPressed;
    public bool HeavyKickPressed;

    public bool BackHeld;         // relative to facing
    public bool OnGround;
    public float VerticalVel;
}