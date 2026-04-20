using NaughtyAttributes;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    //NOTE: Local AudioSource used for playing non-location based sounds.
    //      Sounds that should be heard directionally will use PlayClipAtPoint
    private AudioSource audioSource;

    [SerializeField] private SoundEffect PlayerAttackSound;
    [SerializeField] private SoundEffect PlayerGotHitSound;
    [SerializeField] private SoundEffect PlayerBlockedHitSound;
    [SerializeField] private SoundEffect EnemyGotHitSound;
    [SerializeField] private SoundEffect EnemyDiedSound;
    [SerializeField] private SoundEffect BoomerangThrownSound;
    [SerializeField] private SoundEffect BoomerangFlyLoopSound;
    [SerializeField] private SoundEffect BoomerangCaughtSound;

    private void Start()
    {
        //Get AudioSource component
        audioSource = GetComponent<AudioSource>();

        //Add Event Listeners
        AudioEvents.PlayerDidAttack .AddListener(Handle_PlayerDidAttack );
        AudioEvents.PlayerGotHit    .AddListener(Handle_PlayerGotHit    );
        AudioEvents.PlayerBlockedHit.AddListener(Handle_PlayerBlockedHit);
        AudioEvents.EnemyGotHit     .AddListener(Handle_EnemyGotHit     );
        AudioEvents.EnemyDied       .AddListener(Handle_EnemyDied       );
        AudioEvents.BoomerangThrown .AddListener(Handle_BoomerangThrown );
        AudioEvents.BoomerangCaught .AddListener(Handle_BoomerangCaught );
    }

    private void Handle_PlayerDidAttack()
    {
        audioSource.PlayOneShot(PlayerAttackSound.Clip, PlayerAttackSound.VolumeScale);
    }
    private void Handle_PlayerGotHit()
    {
        Debug.Log("PLAYER GOT HIT SOUND PLAYING");
        audioSource.PlayOneShot(PlayerGotHitSound.Clip, PlayerGotHitSound.VolumeScale);
    }
    private void Handle_PlayerBlockedHit()
    {
        audioSource.PlayOneShot(PlayerBlockedHitSound.Clip, PlayerBlockedHitSound.VolumeScale);
    }
    private void Handle_EnemyGotHit(GameObject enemy)
    {
        AudioSource.PlayClipAtPoint(EnemyGotHitSound.Clip, enemy.transform.position, EnemyGotHitSound.VolumeScale);
    }
    private void Handle_EnemyDied(GameObject enemy)
    {
        AudioSource.PlayClipAtPoint(EnemyDiedSound.Clip, enemy.transform.position, EnemyDiedSound.VolumeScale);
    }
    private void Handle_BoomerangThrown(GameObject boomerang)
    {
        //play thrown sound locally
        audioSource.PlayOneShot(BoomerangThrownSound.Clip, BoomerangThrownSound.VolumeScale);

        //begin looped fly sound on boomerang audio source
        boomerang.GetComponent<AudioSource>().clip = BoomerangFlyLoopSound.Clip;
        boomerang.GetComponent<AudioSource>().volume = BoomerangFlyLoopSound.VolumeScale;
        boomerang.GetComponent<AudioSource>().Play();
    }
    private void Handle_BoomerangCaught()
    {
        //play caught sound locally
        audioSource.PlayOneShot(BoomerangCaughtSound.Clip, BoomerangCaughtSound.VolumeScale);
    }
}

[Serializable]
struct SoundEffect
{
    public AudioClip Clip;

    [Range(0.0f,1.0f)]
    public float VolumeScale;

    public SoundEffect(AudioClip clip, float volumeScale = 1.0f)
    {
        Clip = clip;
        VolumeScale = volumeScale;
    }
}