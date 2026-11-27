using UnityEngine;
using UnityEngine.Tilemaps;

// Patrols left and right, turning around at walls and at ledges.
//
// When the level starts, an enemy that was placed in mid-air drops onto the
// ground below it, so a misplaced enemy can't float. Enemies are kinematic
// (no gravity, no collisions with walls), so all of this is done with short
// raycasts against the "Ground" layer.
public class enemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;

    [Tooltip("Drop onto the ground below when the level starts.")]
    [SerializeField] bool snapToGround = true;

    [Tooltip("Distance from the ground to this object's pivot when standing.")]
    [SerializeField] float standHeight = 0.4f;

    [Tooltip("How far below to look for ground when snapping.")]
    [SerializeField] float maxSnapDistance = 40f;

    Rigidbody2D myRigidBody;
    int groundMask;
    float turnCooldown;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        groundMask = LayerMask.GetMask("Ground");
        if (snapToGround) { SnapToGround(); }
        FaceMoveDirection();
    }

    void Update()
    {
        myRigidBody.velocity = new Vector2(moveSpeed, 0f);

        turnCooldown -= Time.deltaTime;
        if (turnCooldown <= 0f && (WallAhead() || LedgeAhead()))
        {
            Turn();
        }
    }

    void SnapToGround()
    {
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, maxSnapDistance, groundMask);
        if (hit.collider == null) { return; }

        // The level's colliders are outlines, so a ray that starts inside a
        // wall still "hits" the wall's underside. Leave buried enemies alone.
        Tilemap tiles = hit.collider.GetComponent<Tilemap>();
        if (tiles != null && tiles.HasTile(tiles.WorldToCell(origin))) { return; }

        float gap = origin.y - hit.point.y;
        if (gap <= standHeight + 0.3f) { return; }   // already standing on it

        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, hit.point.y + standHeight, p.z);
        myRigidBody.position = transform.position;
    }

    bool WallAhead()
    {
        float dir = Mathf.Sign(moveSpeed);
        return Physics2D.Raycast(transform.position, new Vector2(dir, 0f), 0.6f, groundMask).collider != null;
    }

    bool LedgeAhead()
    {
        float dir = Mathf.Sign(moveSpeed);
        Vector2 ahead = (Vector2)transform.position + new Vector2(dir * 0.5f, 0f);
        return Physics2D.Raycast(ahead, Vector2.down, standHeight + 0.5f, groundMask).collider == null;
    }

    void Turn()
    {
        moveSpeed = -moveSpeed;
        FaceMoveDirection();
        turnCooldown = 0.15f;
    }

    // A negative moveSpeed starts the enemy walking left; flip the sprite to match.
    void FaceMoveDirection()
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * Mathf.Sign(moveSpeed);
        transform.localScale = s;
    }
}
