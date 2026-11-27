using UnityEngine;

// Marks a hazard that kills the player even if only their feet touch it, so
// you can't land on or walk over it. Used for pop-up spikes and crushers.
// Spike and water tiles don't have it: for those only the body counts, as in
// the original game. See playerMovement.Die().
public class KillsOnTouch : MonoBehaviour { }
