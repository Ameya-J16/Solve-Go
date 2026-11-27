using UnityEngine;

// A hidden hazard that shoots out when the player gets close.
//
// Examples:
//   - spikes hidden in the floor that pop up as you walk over them
//   - a block that slams in from the side of a corridor
//
// Setup: place the object where it should END UP (fully visible), put it on
// the "Hazards" layer so touching it kills the player, and give it a
// non-trigger collider. "Hidden Offset" is where it hides before it pops
// out. (0, -1) means one unit below, inside the floor.
public class PopOutTrap : TrollTrap
{
    [Header("Pop out")]
    [Tooltip("Where the trap hides, relative to where you placed it. (0,-1) = one unit down.")]
    [SerializeField] Vector2 hiddenOffset = new Vector2(0f, -1f);

    [Tooltip("How fast it pops out, in units per second.")]
    [SerializeField] float popSpeed = 30f;

    [Tooltip("Hide the sprite until the trap fires. Turn off if it hides behind other art anyway.")]
    [SerializeField] bool invisibleUntilFired = true;

    protected override void Awake()
    {
        base.Awake();
        transform.position = StartPosition + (Vector3)hiddenOffset;
        SetSolid(false);
        if (invisibleUntilFired) { SetVisible(false); }
    }

    protected override void Fire()
    {
        SetSolid(true);
        SetVisible(true);
        StartCoroutine(MoveTo(StartPosition, popSpeed));
    }

    void Reset()
    {
        zoneOffset = new Vector2(0f, 1f);
        zoneSize = new Vector2(1.5f, 2f);
    }
}
