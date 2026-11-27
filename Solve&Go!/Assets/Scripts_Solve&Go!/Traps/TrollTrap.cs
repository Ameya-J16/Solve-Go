using System.Collections;
using UnityEngine;

// Base class for every troll trap.
//
// Each trap watches a box-shaped "trigger zone" (the red box you see in the
// Scene view) and springs the moment the player's body touches that zone.
// Traps reset on their own when the player dies, because dying reloads the
// level.
//
// Don't put traps under the "Screen Persist" object. Anything under it
// survives level reloads, so a trap there would stay sprung after you die.
public abstract class TrollTrap : MonoBehaviour
{
    [Header("Trigger zone (relative to where you placed this object)")]
    [Tooltip("Centre of the zone, relative to this object's position.")]
    [SerializeField] protected Vector2 zoneOffset = new Vector2(0f, 1f);

    [Tooltip("Width and height of the zone, in world units.")]
    [SerializeField] protected Vector2 zoneSize = new Vector2(2f, 2f);

    [Tooltip("Seconds to wait after the player enters the zone. 0 = instant (the unfair way).")]
    [SerializeField] protected float delay = 0f;

    protected Vector3 StartPosition { get; private set; }
    protected bool Triggered { get; private set; }

    Transform player;
    Collider2D playerBody;

    protected virtual void Awake()
    {
        StartPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (Triggered) { return; }

        if (player == null)
        {
            playerMovement found = FindObjectOfType<playerMovement>();
            if (found == null) { return; }
            player = found.transform;
            playerBody = found.GetComponent<CapsuleCollider2D>();
        }

        if (PlayerInZone())
        {
            Activate();
        }
    }

    // Public so you can also fire a trap from somewhere else, e.g. from a
    // UnityEvent on another object.
    public void Activate()
    {
        if (Triggered) { return; }
        Triggered = true;
        StartCoroutine(FireAfterDelay());
    }

    protected abstract void Fire();

    IEnumerator FireAfterDelay()
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }
        Fire();
    }

    bool PlayerInZone()
    {
        Vector2 zoneCentre = (Vector2)StartPosition + zoneOffset;
        Vector2 zoneHalf = zoneSize * 0.5f;

        Vector2 playerCentre = player.position;
        Vector2 playerHalf = Vector2.zero;
        if (playerBody != null)
        {
            playerCentre = playerBody.bounds.center;
            playerHalf = playerBody.bounds.extents;
        }

        return Mathf.Abs(playerCentre.x - zoneCentre.x) <= zoneHalf.x + playerHalf.x
            && Mathf.Abs(playerCentre.y - zoneCentre.y) <= zoneHalf.y + playerHalf.y;
    }

    // ---- Helpers shared by the traps ----

    protected IEnumerator MoveTo(Vector3 target, float speed)
    {
        while ((transform.position - target).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    protected void SetSolid(bool solid)
    {
        foreach (Collider2D c in GetComponentsInChildren<Collider2D>(true))
        {
            c.enabled = solid;
        }
    }

    protected void SetVisible(bool visible)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
        {
            r.enabled = visible;
        }
    }

    // Draws the trigger zone in the Scene view so you can see and tweak it.
    protected virtual void OnDrawGizmos()
    {
        Vector3 origin = Application.isPlaying ? StartPosition : transform.position;
        Gizmos.color = Triggered ? new Color(1f, 1f, 1f, 0.25f) : new Color(1f, 0.15f, 0.15f, 0.9f);
        Gizmos.DrawWireCube(origin + (Vector3)zoneOffset, zoneSize);
    }
}
