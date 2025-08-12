using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Header("Assign sliders here")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    void OnEnable()
    {
        // 初始化 UI 值（從 PlayerPrefs 取回）
        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("vol_master", 0.8f));
        musicSlider .SetValueWithoutNotify(PlayerPrefs.GetFloat("vol_music" , 0.8f));
        sfxSlider   .SetValueWithoutNotify(PlayerPrefs.GetFloat("vol_sfx"   , 0.8f));

        // 套用到 AudioHub（確保即時）
        ApplyMaster(masterSlider.value);
        ApplyMusic (musicSlider.value);
        ApplySFX   (sfxSlider.value);

        // 綁定事件
        masterSlider.onValueChanged.AddListener(ApplyMaster);
        musicSlider .onValueChanged.AddListener(ApplyMusic);
        sfxSlider   .onValueChanged.AddListener(ApplySFX);
    }

    void OnDisable()
    {
        masterSlider.onValueChanged.RemoveListener(ApplyMaster);
        musicSlider .onValueChanged.RemoveListener(ApplyMusic);
        sfxSlider   .onValueChanged.RemoveListener(ApplySFX);
    }

    void ApplyMaster(float v)
    {
        if (AudioHub.I == null) return;
        AudioHub.I.SetMaster(v);
    }

    void ApplyMusic(float v)
    {
        if (AudioHub.I == null) return;
        AudioHub.I.SetMusic(v);
    }

    void ApplySFX(float v)
    {
        if (AudioHub.I == null) return;
        AudioHub.I.SetSFX(v);
    }
}