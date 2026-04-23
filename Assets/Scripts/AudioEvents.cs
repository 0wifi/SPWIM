using UnityEngine;
using UnityEngine.Events;

public static class AudioEvents
{
    public static readonly UnityEvent PlayerDidAttack  = new();
    public static readonly UnityEvent PlayerGotHit     = new();
    public static readonly UnityEvent PlayerBlockedHit = new();

    //GameObject -> Enemy that got hit/died
    public static readonly UnityEvent<GameObject> EnemyGotHit = new();
    public static readonly UnityEvent<GameObject> EnemyDied   = new();

    //GameObject -> boomerang that was thrown
    public static readonly UnityEvent<GameObject> BoomerangThrown = new();
    public static readonly UnityEvent             BoomerangCaught = new();
}