using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class RodPair      { public RodSfx key;      public AudioCue cue; }
[Serializable] public class BobberPair   { public BobberSfx key;   public AudioCue cue; }
[Serializable] public class CreaturePair { public CreatureSfx key; public AudioCue cue; }
[Serializable] public class BgmPair      { public BgmKey key;      public AudioClip clip; }

[CreateAssetMenu(menuName="Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [Header("Rod")]      public List<RodPair> rod = new();
    [Header("Bobber")]   public List<BobberPair> bobber = new();
    [Header("Creature")] public List<CreaturePair> creature = new();
    [Header("BGM")]      public List<BgmPair> bgm = new();

    public AudioCue  Get(RodSfx k)      => rod.Find(p=>p.key==k)?.cue;
    public AudioCue  Get(BobberSfx k)   => bobber.Find(p=>p.key==k)?.cue;
    public AudioCue  Get(CreatureSfx k) => creature.Find(p=>p.key==k)?.cue;
    public AudioClip Get(BgmKey k)      => bgm.Find(p=>p.key==k)?.clip;
}