using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

internal class LibSoundModule : MonoBehaviour
{
    #region Fields

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject audioObjectPrefab;

    private int bgmLength;
    private int soundLength;

    private AudioClip[] audioClipBGM;
    private AudioClip[] audioClipSE;

    private AudioSource[] audioSourceBGM;
    private AudioSource[] audioSourcePlay;
    private AudioSource audioSourcePlayOneShot;

    private Transform[] transformBGM;
    private Transform[] transformPlay;
    private Transform transformPlayOneShot;

    #endregion

    #region CustomMethod

    public void Init()
    {
        // オブジェクト生成
        GameObject parentSound = new GameObject("Sound");
        GameObject parentBGM = new GameObject("BGM");
        GameObject parentSE = new GameObject("SE");
        GameObject parentPlay = new GameObject("Play");
        GameObject parentPlayOneShot = new GameObject("PlayOneShot");

        parentSound.transform.SetParent(this.transform);
        parentBGM.transform.SetParent(parentSound.transform);
        parentSE.transform.SetParent(parentSound.transform);
        parentPlay.transform.SetParent(parentSE.transform);
        parentPlayOneShot.transform.SetParent(parentSE.transform);


        // AudioClipをenum順にソートして入れる
        bgmLength = Enum.GetValues(typeof(BGMName)).Length;
        soundLength = Enum.GetValues(typeof(SoundFxName)).Length;

        audioClipBGM = new AudioClip[bgmLength];
        audioClipSE = new AudioClip[soundLength];

        audioClipBGM = Resources.LoadAll<AudioClip>("Sound/BGM");
        audioClipSE = Resources.LoadAll<AudioClip>("Sound/SE");


        Array.Resize(ref audioClipBGM, bgmLength);
        Array.Resize(ref audioClipSE, soundLength);

        for (int i = 0; i < bgmLength; i++)// BGM
        {
            BGMName bGMName = (BGMName)Enum.ToObject(typeof(BGMName), i);

            for (int j = 0; j < bgmLength; j++)
            {
                if (audioClipBGM[j] != null && audioClipBGM[j].name == bGMName.ToString())
                {
                    AudioClip temp = audioClipBGM[i];
                    audioClipBGM[i] = audioClipBGM[j];
                    audioClipBGM[j] = temp;
                    break;
                }
            }
        }

        for (int i = 0; i < soundLength; i++)// Sound
        {
            SoundFxName soundFxName = (SoundFxName)Enum.ToObject(typeof(SoundFxName), i);

            for (int j = 0; j < soundLength; j++)
            {
                if (audioClipSE[j] != null && audioClipSE[j].name == soundFxName.ToString())
                {
                    AudioClip temp = audioClipSE[i];
                    audioClipSE[i] = audioClipSE[j];
                    audioClipSE[j] = temp;
                    break;
                }
            }
        }


        // BGM用
        audioSourceBGM = new AudioSource[bgmLength];
        for (int i = 0; i < bgmLength; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentBGM.transform);
            obj.name = ((BGMName)Enum.ToObject(typeof(BGMName), i)).ToString();

            audioSourceBGM[i] = obj.GetComponent<AudioSource>();
            audioSourceBGM[i].clip = audioClipBGM[i];
            audioSourceBGM[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[1];
            audioSourceBGM[i].loop = true;
        }


        // SE Play用(音が重ならず、上書きされる)
        audioSourcePlay = new AudioSource[soundLength];
        for (int i = 0; i < soundLength; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentPlay.transform);
            obj.name = ((SoundFxName)Enum.ToObject(typeof(SoundFxName), i)).ToString();

            audioSourcePlay[i] = obj.GetComponent<AudioSource>();
            if (audioClipSE[i] != null)
            {
                audioSourcePlay[i].clip = audioClipSE[i];
            }
            audioSourcePlay[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];
        }


        // SE PlayOneShot用(音が重なる)
        GameObject objShot = Instantiate(audioObjectPrefab);
        objShot.transform.SetParent(parentPlayOneShot.transform);
        objShot.name = "PlayOneShot";

        audioSourcePlayOneShot = objShot.GetComponent<AudioSource>();
        audioSourcePlayOneShot.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];
    }

    public void SetAudioMixerMaster(float value)
    {
        // dbに変換
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("Master", volume);
    }

    public void SetAudioMixerBGM(float value)
    {
        // dbに変換
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("BGM", volume);
    }

    public void SetAudioMixerSE(float value)
    {
        // dbに変換
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("SE", volume);
    }

    public void PlayBGM(int number, float startTime)
    {
        bool isMissing = true;
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            if (i == number)
            {
                if (audioClipBGM[number] != null)
                {
                    AudioSource audio = audioSourceBGM[number];
                    //audio.time = startTime;
                    audio.Play();
                    audio.time = startTime;
                    isMissing = false;
                }
            }
            else
            {
                audioSourceBGM[i].Stop();
            }
        }

#if UNITY_EDITOR
        if (isMissing == true)
        {
            string name = ((BGMName)Enum.ToObject(typeof(BGMName), number)).ToString();
            Debug.Log(name + "はありません");
        }
#endif
    }

    public void PlaySoloSE(int number, float startTime)
    {
        if(audioClipSE[number] != null)
        {
            AudioSource audio = audioSourcePlay[number];
            //audio.time = startTime;
            audio.Play();
            audio.time = startTime;

            return;
        }

#if UNITY_EDITOR
        string name = ((SoundFxName)Enum.ToObject(typeof(SoundFxName), number)).ToString();
        Debug.Log(name + "はありません");
#endif
    }

    public void PlayTrollSE(int number, float startTime)
    {
        if (audioClipSE[number] != null)
        {
            AudioSource audio = audioSourcePlayOneShot;
            //audio.time = startTime;
            audio.PlayOneShot(audioClipSE[number]);
            audio.time = startTime;

            return;
        }

#if UNITY_EDITOR
        string name = ((SoundFxName)Enum.ToObject(typeof(SoundFxName), number)).ToString();
        Debug.Log(name + "はありません");
#endif
    }

    public void StopAll()
    {
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            audioSourceBGM[i].Stop();
        }

        for (int i = 0; i < audioSourcePlay.Length; i++)
        {
            audioSourcePlay[i].Stop();
        }

        audioSourcePlayOneShot.Stop();
    }

    public float AudioMixerGetFloat(string name)
    {
        audioMixer.GetFloat(name, out float sliderMasterValue);

        return sliderMasterValue;
    }

    public void SliderValueChange(Slider sliderMaster, Slider sliderBGM, Slider sliderSE)
    {
        // 初期音量設定
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

        GameObject tempObject = UnityEngine.Object.Instantiate(prefab);//////////////////////////////////////////

        module = tempObject.GetComponent<LibSoundModule>();

        module.Init();

        UnityEngine.Object.DontDestroyOnLoad(tempObject);////////////////////////////////////////////////////////
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

    public static void PlayBGM(BGMName sound, float startTime = 0)
    {
        string name = sound.ToString();
        instance.module.PlayBGM((int)sound, startTime);
    }

    public static void PlaySoloSE(SoundFxName sound, float startTime = 0)
    {
        instance.module.PlaySoloSE((int)sound, startTime);
    }

    public static void PlayTrollSE(SoundFxName sound, float startTime = 0)
    {
        instance.module.PlayTrollSE((int)sound, startTime);
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

public enum BGMName
{
    BGM1 = 0,
    BGM2,
    BGM3,
}

public enum SoundFxName
{
    CharacterReadyJump = 0,
    CharacterJump,
    CharacterLanding,
    琴の滑奏,
    CharacterCollision,
    CharacterReadyWallKick,
    CharacterWallKick,
    ゲージ回復2,
    TutorialOpen,
    TutorialClose,
    SettingOpen,
    SettingClose,
    GoalOpen,
    きらーん2,
    SceneLoad,
    Star,
    ResultTime,
    ResultTimeStop,
    Cheer,
    Window,
    決定ボタンを押す51,
    ClearImage,
    Goal,
}


