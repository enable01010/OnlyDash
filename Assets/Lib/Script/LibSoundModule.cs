using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

internal class LibSoundModule : MonoBehaviour
{
    #region Fields

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject audioObjectPrefab;

    [SerializeField] private int trollCount = 10;
    private int trollNowCount = 0;

    private AudioClip[] audioClipBGM;
    private AudioClip[] audioClipSE;

    private AudioSource[] audioSourceBGM;
    private AudioSource[] audioSourceSolo;
    private AudioSource[] audioSourceTroll;

    #endregion

    #region CustomMethod

    public void Init()
    {
        audioClipBGM = Resources.LoadAll<AudioClip>("Sound/BGM");
        audioClipSE = Resources.LoadAll<AudioClip>("Sound/SE");

        GameObject parentSound = new GameObject("Sound");
        GameObject parentBGM = new GameObject("BGM");
        GameObject parentSE = new GameObject("SE");
        GameObject parentSolo = new GameObject("Solo");
        GameObject parentTroll = new GameObject("Troll");

        parentSound.transform.SetParent(this.transform);
        parentBGM.transform.SetParent(parentSound.transform);
        parentSE.transform.SetParent(parentSound.transform);
        parentSolo.transform.SetParent(parentSE.transform);
        parentTroll.transform.SetParent(parentSE.transform);

        // BGMóp
        audioSourceBGM = new AudioSource[audioClipBGM.Length];
        for (int i = 0; i < audioClipBGM.Length; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentBGM.transform);
            obj.name = audioClipBGM[i].name;

            audioSourceBGM[i] = obj.GetComponent<AudioSource>();
            audioSourceBGM[i].clip = audioClipBGM[i];
            audioSourceBGM[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[1];
            audioSourceBGM[i].loop = true;
        }

        // ì∆è•SEóp
        audioSourceSolo = new AudioSource[audioClipSE.Length];
        for (int i = 0; i < audioClipSE.Length; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentSolo.transform);
            obj.name = audioClipSE[i].name;

            audioSourceSolo[i] = obj.GetComponent<AudioSource>();
            audioSourceSolo[i].clip = audioClipSE[i];
            audioSourceSolo[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];
        }

        // ó÷è•SEóp
        audioSourceTroll = new AudioSource[trollCount];
        for (int i = 0; i < trollCount; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentTroll.transform);
            obj.name = "Troll_" + i;

            audioSourceTroll[i] = obj.GetComponent<AudioSource>();
            audioSourceTroll[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];
        }
    }

    public void SetAudioMixerMaster(float value)
    {
        // dbÇ…ïœä∑
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("Master", volume);
    }

    public void SetAudioMixerBGM(float value)
    {
        // dbÇ…ïœä∑
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("BGM", volume);
    }

    public void SetAudioMixerSE(float value)
    {
        // dbÇ…ïœä∑
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("SE", volume);
    }

    public void PlayBGM(string name)
    {
        bool isMissing = true;
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            if (audioSourceBGM[i].clip.name == name)
            {
                audioSourceBGM[i].Play();
                isMissing = false;
            }
            else
            {
                audioSourceBGM[i].Stop();
            }
        }

#if UNITY_EDITOR
        if (isMissing == true)
        {
            Debug.Log(name + "ÇÕÇ†ÇËÇ‹ÇπÇÒ");
        }
#endif
    }

    public void PlaySoloSE(string name, float time)
    {
        for (int i = 0; i < audioSourceSolo.Length; i++)
        {
            if (audioSourceSolo[i].clip.name == name)
            {
                AudioSource audio = audioSourceSolo[i];
                audio.time = time;
                audio.Play();

                return;
            }
        }

#if UNITY_EDITOR
        Debug.Log(name + "ÇÕÇ†ÇËÇ‹ÇπÇÒ");
#endif
    }

    public void PlayTrollSE(string name, float time)
    {
        for (int i = 0; i < audioClipSE.Length; i++)
        {
            if (audioClipSE[i].name == name)
            {
                AudioSource audio = audioSourceTroll[trollNowCount];
                audio.clip = audioClipSE[i];
                audio.time = time;
                audio.Play();

                trollNowCount = (trollNowCount + 1) % trollCount;

                return;
            }
        }

#if UNITY_EDITOR
        Debug.Log(name + "ÇÕÇ†ÇËÇ‹ÇπÇÒ");
#endif
    }

    public void StopAll()
    {
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            audioSourceBGM[i].Stop();
        }

        for (int i = 0; i < audioSourceSolo.Length; i++)
        {
            audioSourceSolo[i].Stop();
        }

        for (int i = 0; i < audioSourceTroll.Length; i++)
        {
            audioSourceTroll[i].Stop();
        }
    }

    public float AudioMixerGetFloat(string name)
    {
        audioMixer.GetFloat(name, out float sliderMasterValue);

        return sliderMasterValue;
    }

    public void SliderValueChange(Slider sliderMaster, Slider sliderBGM, Slider sliderSE)
    {
        // èâä˙âπó ê›íË
        sliderMaster.value = Mathf.Pow(10f, LibSound.AudioMixerGetFloat("Master") / 20f);
        sliderBGM.value = Mathf.Pow(10f, LibSound.AudioMixerGetFloat("BGM") / 20f);
        sliderSE.value = Mathf.Pow(10f, LibSound.AudioMixerGetFloat("SE") / 20f);
    }

    #endregion
}

public class LibSound
{
    private static LibSound instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new LibSound();
            }
            return m_Instance; 
        }
    }

    private static LibSound m_Instance;

    private LibSoundModule module;

    private LibSound() 
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/SoundModule");

        GameObject tempObject = Object.Instantiate(prefab);

        module = tempObject.GetComponent<LibSoundModule>();

        module.Init();

        Object.DontDestroyOnLoad(tempObject);
    }

    #region CustomMethod

    public static void SetAudioMixerMaster(float value)
    {
        instance.module.SetAudioMixerMaster(value);
    }

    public static void SetAudioMixerBGM(float value)
    {
        instance.module.SetAudioMixerBGM(value);
    }

    public static void SetAudioMixerSE(float value)
    {
        instance.module.SetAudioMixerSE(value);
    }

    public static void PlayBGM(BGMName sound)
    {
        string name = sound.ToString();
        instance.module.PlayBGM(name);
    }

    public static void PlaySoloSE(SoundFxName sound, float startTime = 0)
    {
        string name = sound.ToString();
        instance.module.PlaySoloSE(name, startTime);
    }

    public static void PlayTrollSE(SoundFxName sound, float startTime = 0)
    {
        string name = sound.ToString();
        instance.module.PlayTrollSE(name, startTime);
    }

    public static void StopAll()
    {
        instance.module.StopAll();
    }

    public static float AudioMixerGetFloat(string name)
    {
        return instance.module.AudioMixerGetFloat(name);
    }

    public static void SliderValueChange(Slider sliderMaster, Slider sliderBGM, Slider sliderSE)
    {
        instance.module.SliderValueChange(sliderMaster, sliderBGM, sliderSE);
    }

    #endregion
}

public enum SoundFxName
{
    CharacterReadyJump = 0,
    CharacterJump,
    CharacterLanding,
    CharacterCollision,
    CharacterReadyWallKick,
    CharacterWallKick,
    TutorialOpen,
    TutorialClose,
    SettingOpen,
    SettingClose,
    GoalOpen,
    SceneLoad,
    Star,
    ResultTime,
    ResultTimeStop,
    Cheer,
    Window,
    ClearImage,
    Goal,
}

public enum BGMName
{
    BGM1 = 0,
}
