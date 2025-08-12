using UnityEngine;

[CreateAssetMenu(menuName="Audio/Audio Cue")]
public class AudioCue : ScriptableObject
{
    public AudioClip[] clips;
    [Range(0f, 1f)] public float baseVolume = 1f;
    [Range(-12f, 12f)] public float volumeRandomDb = 0f;
    [Range(0.5f, 2f)] public float basePitch = 1f;
    [Range(0f, 0.5f)] public float pitchRandom = 0.05f;
    public bool loop;

    public AudioClip Pick()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    public float RandomVolumeLinear()
    {
        float deltaDb = Random.Range(-volumeRandomDb, volumeRandomDb);
        return baseVolume * Mathf.Pow(10f, deltaDb / 20f);
    }

    public float RandomPitch() => basePitch + Random.Range(-pitchRandom, pitchRandom);
}