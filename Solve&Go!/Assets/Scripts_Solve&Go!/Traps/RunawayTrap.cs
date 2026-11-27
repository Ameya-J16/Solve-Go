using UnityEngine;

// Moves away when the player gets close.
//
// Examples:
//   - Put it on the exit door: the door runs off right before you reach it.
//   - Put it on a platform: it slides out from under you mid-jump.
//   - Point "Move By" at the player to make a wall or block charge at you.
//     Put that one on the "Hazards" layer.
public class RunawayTrap : TrollTrap
{
    [Header("Run away")]
    [Tooltip("How far it moves, relative to where you placed it.")]
    [SerializeField] Vector2 moveBy = new Vector2(6f, 0f);

    [Tooltip("Speed in units per second.")]
    [SerializeField] float speed = 10f;

    public void SetMove(Vector2 by, float unitsPerSecond)
    {
        moveBy = by;
        speed = unitsPerSecond;
    }

    protected override void Fire()
    {
        StartCoroutine(MoveTo(StartPosition + (Vector3)moveBy, speed));
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Vector3 origin = Application.isPlaying ? StartPosition : transform.position;
        Gizmos.color = new Color(1f, 0.8f, 0.1f, 0.9f);
        Gizmos.DrawLine(origin, origin + (Vector3)moveBy);
    }

    void Reset()
    {
        zoneOffset = new Vector2(-2f, 0f);
        zoneSize = new Vector2(2f, 3f);
    }
}
