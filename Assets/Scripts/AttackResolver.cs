using System.Collections.Generic;

public class AttackResolver
{
    private readonly List<AttackData> moveList;

    public AttackResolver(List<AttackData> moves)
    {
        moveList = moves ?? new List<AttackData>();
    }

    /// <summary>
    /// Choose the best matching move from recent inputs.
    /// Prefers exact button match, then motion suffix match with longest motion winning.
    /// Returns null if nothing matches this frame.
    /// </summary>
    public AttackData Resolve(InputHistory history, float motionLookbackSeconds = 0.6f)
    {
        if (history == null || moveList.Count == 0) return null;

        string button = history.GetLatestButton();
        if (string.IsNullOrEmpty(button)) return null;

        string motion = history.GetRecentMotionString(motionLookbackSeconds); // e.g., "236", "66", etc.

        AttackData best = null;
        int bestScore = -1;

        for (int i = 0; i < moveList.Count; i++)
        {
            var move = moveList[i];
            if (string.IsNullOrEmpty(move.buttonName)) continue;

            // button must match (case-insensitive)
            if (!string.Equals(move.buttonName, button, System.StringComparison.OrdinalIgnoreCase))
                continue;

            // no motion required: score = 0
            if (string.IsNullOrEmpty(move.motionInput))
            {
                if (best == null || bestScore < 0)
                {
                    best = move;
                    bestScore = 0;
                }
                continue;
            }

            // motion required: prefer longest suffix match
            if (!string.IsNullOrEmpty(motion) && motion.EndsWith(move.motionInput))
            {
                int score = move.motionInput.Length;
                if (score > bestScore)
                {
                    best = move;
                    bestScore = score;
                }
            }
        }

        return best;
    }
}