using UnityEngine;

// Something that falls when the player gets near.
//
// Examples:
//   - Ceiling crusher: put it on the "Hazards" layer and move the trigger
//     zone UNDER it (the default). It drops on your head.
//   - Collapsing floor: put it on the "Ground" layer so you can stand and
//     jump on it, and move the trigger zone just ON TOP of it. It falls
//     away with you on it.
//
// Needs a Rigidbody2D (added automatically) and a non-trigger collider.
[RequireComponent(typeof(Rigidbody2D))]
public class DropTrap : TrollTrap
{
    [Header("Drop")]
    [Tooltip("Gravity once it starts falling. Higher = faster and meaner.")]
    [SerializeField] float fallGravity = 5f;

    [Tooltip("Remove the object this many seconds after it starts falling. 0 = never.")]
    [SerializeField] float destroyAfter = 4f;

    Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
    }

    protected override void Fire()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
        rb.freezeRotation = true;
        if (destroyAfter > 0f) { Destroy(gameObject, destroyAfter); }
    }

    void Reset()
    {
        zoneOffset = new Vector2(0f, -2.5f);
        zoneSize = new Vector2(1.2f, 4f);
    }
}
