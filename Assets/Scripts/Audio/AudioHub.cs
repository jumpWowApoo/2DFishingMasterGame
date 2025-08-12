using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class AudioHub : MonoBehaviour
{
    public static AudioHub I { get; private set; }

    [Header("Database & Mixer")]
    [SerializeField] AudioDatabase database;
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioMixerGroup musicGroup;
    [SerializeField] AudioMixerGroup sfxGroup;
    [SerializeField] string masterParam = "MasterVol";
    [SerializeField] string musicParam  = "MusicVol";
    [SerializeField] string sfxParam    = "SfxVol";

    [Header("Music")]
    [SerializeField] float musicFadeTime = 1.0f;
    AudioSource musicA, musicB;
    bool usingA = true;

    [Header("SFX Pool")]
    [SerializeField] int poolSize = 16;
    readonly Queue<AudioSource> pool = new();
    
    [Header("Hotkey")]
    [SerializeField, Range(0f,1f)] float defaultVolume = 0.8f;
    public event System.Action<float,float,float> OnVolumeReset; 

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // 音樂雙聲道（交叉淡入）
        musicA = gameObject.AddComponent<AudioSource>();
        musicB = gameObject.AddComponent<AudioSource>();
        foreach (var s in new[]{musicA, musicB}) {
            s.playOnAwake = false;
            s.loop = true;
            s.outputAudioMixerGroup = musicGroup;
        }

        // SFX 物件池
        for (int i=0;i<poolSize;i++){
            var go = new GameObject("SFX_"+i);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = sfxGroup;
            pool.Enqueue(src);
        }

        // 套用上次音量
        SetMaster(PlayerPrefs.GetFloat("vol_master", 0.8f));
        SetMusic (PlayerPrefs.GetFloat("vol_music" , 0.8f));
        SetSFX   (PlayerPrefs.GetFloat("vol_sfx"   , 0.8f));
    }

    private void Start()
    {
        PlayBGM(BgmKey.LakeDay);
    }
    
    void Update()
    {
        // Ctrl + Space → 重置為預設值
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            && Input.GetKeyDown(KeyCode.Space))
        {
            ResetVolumesToDefault();
        }
    }

    // ===== Volume (0~1) =====
    public void SetMaster(float v){ SetDb(masterParam, v); PlayerPrefs.SetFloat("vol_master", v); }
    public void SetMusic (float v){ SetDb(musicParam , v); PlayerPrefs.SetFloat("vol_music" , v); }
    public void SetSFX   (float v){ SetDb(sfxParam   , v); PlayerPrefs.SetFloat("vol_sfx"   , v); }
    void SetDb(string param, float linear01)
    {
        float dB = Mathf.Log10(Mathf.Clamp(linear01, 0.0001f, 1f)) * 20f;
        if (mixer) mixer.SetFloat(param, dB);
    }
    

    // ===== Music =====
    public void PlayBGM(BgmKey key, float? fade=null)
    {
        if (!database) return;
        var clip = database.Get(key);
        if (!clip) return;

        var (from,to) = usingA ? (musicA, musicB) : (musicB, musicA);
        usingA = !usingA;

        to.clip = clip;
        to.volume = 0f;
        to.time = 0f;
        to.Play();

        StartCoroutine(Crossfade(from, to, fade ?? musicFadeTime));
    }

    IEnumerator Crossfade(AudioSource from, AudioSource to, float dur)
    {
        float t=0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t/dur);
            to.volume   = k;
            from.volume = 1f-k;
            yield return null;
        }
        from.Stop(); from.volume = 0f;
        to.volume = 1f;
    }

    // ===== SFX =====
    AudioSource GetSrc()
    {
        var s = pool.Dequeue();
        pool.Enqueue(s);
        return s;
    }

    void ApplyCue(AudioSource src, AudioCue cue)
    {
        var clip = cue.Pick();
        if (!clip) return;
        src.clip   = clip;
        src.loop   = cue.loop;
        src.pitch  = cue.RandomPitch();
        src.volume = cue.RandomVolumeLinear();
        src.Play();
        if (!cue.loop) StartCoroutine(StopAfter(src, clip.length + 0.05f));
    }

    IEnumerator StopAfter(AudioSource src, float t)
    {
        yield return new WaitForSeconds(t);
        if (src)
        {
            src.Stop();
            src.transform.SetParent(transform);
            src.spatialBlend = 0f;
        }
    }

    void BindPos(AudioSource s, Transform at)
    {
        if (at == null)
        {
            s.transform.SetParent(transform);
            s.transform.localPosition = Vector3.zero;
            s.spatialBlend = 0f; // 2D
        }
        else
        {
            s.transform.SetParent(at);
            s.transform.localPosition = Vector3.zero;
            s.spatialBlend = 1f;  // 3D
            s.minDistance  = 3f;
            s.maxDistance  = 30f;
        }
    }

    // ===== Public API（遊戲內只呼叫這些） =====
    public void PlayRod(RodSfx e, Transform at=null)
    {
        if (!database) return;
        var cue = database.Get(e);
        if (!cue) return;
        var s = GetSrc();
        BindPos(s, at);
        ApplyCue(s, cue);
    }

    public void PlayBobber(BobberSfx e, Transform at=null)
    {
        if (!database) return;
        var cue = database.Get(e);
        if (!cue) return;
        var s = GetSrc();
        BindPos(s, at);
        ApplyCue(s, cue);
    }

    public void PlayCreature(CreatureSfx e, Transform at=null)
    {
        if (!database) return;
        var cue = database.Get(e);
        if (!cue) return;
        var s = GetSrc();
        BindPos(s, at);
        ApplyCue(s, cue);
    }
    
    public void ResetVolumesToDefault()
    {
        // 套用到 Mixer + 存 PlayerPrefs
        SetMaster(defaultVolume);
        SetMusic (defaultVolume);
        SetSFX   (defaultVolume);
        PlayerPrefs.Save();

        Debug.Log($"[AudioHub] Volumes reset to {defaultVolume}");

        // 通知 UI 想同步滑桿者（可選）
        OnVolumeReset?.Invoke(
            PlayerPrefs.GetFloat("vol_master", defaultVolume),
            PlayerPrefs.GetFloat("vol_music" , defaultVolume),
            PlayerPrefs.GetFloat("vol_sfx"   , defaultVolume)
        );
    }
}
