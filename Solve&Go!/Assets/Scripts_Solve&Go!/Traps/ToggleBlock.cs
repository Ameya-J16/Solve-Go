using UnityEngine;

// A block that appears out of nowhere or vanishes under your feet.
//
//   Appear: the block starts invisible and not solid. Put it above a gap
//           with the trigger zone under it. When you jump, it appears and
//           you bonk your head and fall in.
//   Vanish: normal-looking floor (on the "Ground" layer) that disappears the
//           moment you step on it. Put the trigger zone on top of it.
public class ToggleBlock : TrollTrap
{
    public enum Mode { Appear, Vanish }

    [Header("Toggle")]
    [SerializeField] Mode mode = Mode.Vanish;

    protected override void Awake()
    {
        base.Awake();
        if (mode == Mode.Appear)
        {
            SetSolid(false);
            SetVisible(false);
        }
    }

    protected override void Fire()
    {
        bool show = mode == Mode.Appear;
        SetSolid(show);
        SetVisible(show);
    }

    void Reset()
    {
        zoneOffset = new Vector2(0f, 0.75f);
        zoneSize = new Vector2(1f, 0.5f);
    }
}
