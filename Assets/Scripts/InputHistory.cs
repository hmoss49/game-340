using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class InputHistory
{
    public struct InputFrame
    {
        public float time;      // Time.time
        public float moveX;
        public int dir;         // numpad 1..9 (5 = neutral)
        public bool crouch;
        public bool jump;
        public bool grounded;
        public bool blocking;
        public bool hitstun;

        public bool LP, MP, HP, LK, MK, HK;
    }

    [Tooltip("How many seconds of inputs to keep in the rolling buffer.")]
    public float bufferSeconds = 2.0f;

    private readonly List<InputFrame> buffer = new List<InputFrame>();

    public void Record(FighterInput input, FighterContext ctx)
    {
        var frame = new InputFrame
        {
            time = Time.time,
            moveX = input.MoveX,
            dir = EncodeDirection(input.MoveX, input.CrouchHeld, input.VerticalVel, input.OnGround),
            crouch = input.CrouchHeld,
            jump = input.JumpPressed,
            grounded = input.OnGround,
            blocking = (ctx.Block == FighterContext.BlockState.Blocking),
            hitstun  = (ctx.Hitstun == FighterContext.HitstunState.Hit),

            LP = input.LightPunchPressed,
            MP = input.MediumPunchPressed,
            HP = input.HeavyPunchPressed,
            LK = input.LightKickPressed,
            MK = input.MediumKickPressed,
            HK = input.HeavyKickPressed
        };

        buffer.Add(frame);
        Cleanup();
    }

    public string GetSummary()
    {
        if (buffer.Count == 0) return "(empty)";
        var sb = new StringBuilder();
        int lastDir = -1;

        foreach (var f in buffer)
        {
            if (f.dir != lastDir)
            {
                sb.Append(f.dir);
                lastDir = f.dir;
            }
            // Append buttons on the frames they occur
            if (f.LP) sb.Append("LP");
            if (f.MP) sb.Append("MP");
            if (f.HP) sb.Append("HP");
            if (f.LK) sb.Append("LK");
            if (f.MK) sb.Append("MK");
            if (f.HK) sb.Append("HK");
        }
        return sb.ToString();
    }

    public string GetDetailedLog()
    {
        if (buffer.Count == 0) return "(empty)\n";

        var sb = new StringBuilder();
        float t0 = buffer[0].time;

        foreach (var f in buffer)
        {
            float dt = f.time - t0;
            sb.AppendFormat("t={0:F2} Dir={1} MoveX={2,4:0.00}  Crouch={3}  Grounded={4}  Block={5}  Hitstun={6}",
                dt, f.dir, f.moveX, f.crouch, f.grounded, f.blocking, f.hitstun);

            if (f.LP || f.MP || f.HP || f.LK || f.MK || f.HK)
            {
                sb.Append("  ");
                if (f.LP) sb.Append("LP ");
                if (f.MP) sb.Append("MP ");
                if (f.HP) sb.Append("HP ");
                if (f.LK) sb.Append("LK ");
                if (f.MK) sb.Append("MK ");
                if (f.HK) sb.Append("HK ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public IReadOnlyList<InputFrame> GetBuffer() => buffer.AsReadOnly();

    private void Cleanup()
    {
        float cutoff = Time.time - bufferSeconds;
        // remove old front items (keep order)
        int idx = 0;
        while (idx < buffer.Count && buffer[idx].time < cutoff) idx++;
        if (idx > 0) buffer.RemoveRange(0, idx);
    }

    private int EncodeDirection(float moveX, bool crouch, float vertVel, bool grounded)
    {
        int x = 5, y = 5;

        if (moveX < -0.3f) x = 4;
        else if (moveX > 0.3f) x = 6;

        if (crouch) y = 2;
        else if (!grounded && vertVel > 0.1f) y = 8;
        else y = 5;

        if (y == 2 && x == 4) return 1;
        if (y == 2 && x == 6) return 3;
        if (y == 8 && x == 4) return 7;
        if (y == 8 && x == 6) return 9;
        if (y == 2) return 2;
        if (y == 8) return 8;
        if (x == 4) return 4;
        if (x == 6) return 6;
        return 5;
    }
}
