﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManage : MonoBehaviour
    {
        
        [Header("音频设置")]
        public AudioClip musicAudioClip;
        public AudioMixerGroup musicOutput;
        public bool musicPlayOnAwake = true;
        [Range(0f, 1f)]
        public float musicVolume = 1f;
        [Header("背景音频设置")]
        public AudioClip ambientAudioClip;
        public AudioMixerGroup ambientOutput;
        public bool ambientPlayOnAwake = true;
        [Range(0f, 1f)]
        public float ambientVolume = 1f;

        protected AudioSource m_MusicAudioSource;
        protected AudioSource m_AmbientAudioSource;

        protected bool m_TransferMusicTime, m_TransferAmbientTime;
        protected AudioManage m_OldInstanceToDestroy = null;

        //every clip pushed on that stack throught "PushClip" function will play until completed, then pop
        //once that stack is empty, it revert to the musicAudioClip
        protected Stack<AudioClip> m_MusicStack = new Stack<AudioClip>();

        void Awake()
        {            
       // m_TransferAmbientTime = true;
       // m_TransferMusicTime = true;
            DontDestroyOnLoad(gameObject);

            m_MusicAudioSource = gameObject.AddComponent<AudioSource>();
            m_MusicAudioSource.clip = musicAudioClip;
            m_MusicAudioSource.outputAudioMixerGroup = musicOutput;
            m_MusicAudioSource.loop = true;
            m_MusicAudioSource.volume = musicVolume;

            if (musicPlayOnAwake)
            {
                m_MusicAudioSource.time = 0f;
                m_MusicAudioSource.Play();
            }

            m_AmbientAudioSource = gameObject.AddComponent<AudioSource>();
            m_AmbientAudioSource.clip = ambientAudioClip;
            m_AmbientAudioSource.outputAudioMixerGroup = ambientOutput;
            m_AmbientAudioSource.loop = true;
            m_AmbientAudioSource.volume = ambientVolume;

            if (ambientPlayOnAwake)
            {
                m_AmbientAudioSource.time = 0f;
                m_AmbientAudioSource.Play();
            }
        }

        private void Start()
        {
            //if delete & trasnfer time only in Start so we avoid the small gap that doing everything at the same time in Awake would create 
            if (m_OldInstanceToDestroy != null)
            {
                if (m_TransferAmbientTime) m_AmbientAudioSource.timeSamples = m_OldInstanceToDestroy.m_AmbientAudioSource.timeSamples;
                if (m_TransferMusicTime) m_MusicAudioSource.timeSamples = m_OldInstanceToDestroy.m_MusicAudioSource.timeSamples;
                m_OldInstanceToDestroy.Stop();
                Destroy(m_OldInstanceToDestroy.gameObject);
            }
        Messenger.AddListener<string>(EventCode.PLAY_AUDIO, Push);
        }


        private void Update()
        {
            if (m_MusicStack.Count > 0)
            {
                //isPlaying will be false once the current clip end up playing
                if (!m_MusicAudioSource.isPlaying)
                {
                    m_MusicStack.Pop();
                    if (m_MusicStack.Count > 0)
                    {
                        m_MusicAudioSource.clip = m_MusicStack.Peek();
                        m_MusicAudioSource.Play();
                    }
                    else
                    {//Back to looping music clip
                        m_MusicAudioSource.clip = musicAudioClip;
                        m_MusicAudioSource.loop = true;
                        m_MusicAudioSource.Play();
                    }
                }
            }
        }
    public void Push(string name) {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/"+name);
        PushClip(clip);
    }
    /// <summary>
    /// 添加一个新音频
    /// </summary>
    /// <param name="clip"></param>
        public void PushClip(AudioClip clip)
        {
            m_MusicStack.Push(clip);
            m_MusicAudioSource.Stop();
            m_MusicAudioSource.loop = false;
            m_MusicAudioSource.time = 0;
            m_MusicAudioSource.clip = clip;
            m_MusicAudioSource.Play();
        }
    /// <summary>
    /// 改变正在播放的音频
    /// </summary>
    /// <param name="clip"></param>
        public void ChangeMusic(AudioClip clip)
        {
            musicAudioClip = clip;
            m_MusicAudioSource.clip = clip;
        }
    /// <summary>
    /// 改变正在播放的背景音频
    /// </summary>
    /// <param name="clip"></param>
        public void ChangeAmbient(AudioClip clip)
        {
            ambientAudioClip = clip;
            m_AmbientAudioSource.clip = clip;
        }

    /// <summary>
    /// 开始播放全部音频
    /// </summary>
        public void Play()
        {
            PlayJustAmbient();
            PlayJustMusic();
        }
    /// <summary>
    /// 开始播放音频
    /// </summary>
    public void PlayJustMusic()
        {
            m_MusicAudioSource.time = 0f;
            m_MusicAudioSource.Play();
        }
    /// <summary>
    /// 开始播放背景音频
    /// </summary>
    public void PlayJustAmbient()
        {
            m_AmbientAudioSource.Play();
        }
    /// <summary>
    /// 停止播放全部音频
    /// </summary>
    public void Stop()
        {
            StopJustAmbient();
            StopJustMusic();
        }
    /// <summary>
    /// 停止播放音频
    /// </summary>
        public void StopJustMusic()
        {
            m_MusicAudioSource.Stop();
        }
    /// <summary>
    /// 停止播放背景音频
    /// </summary>
        public void StopJustAmbient()
        {
            m_AmbientAudioSource.Stop();
        }
    /// <summary>
    /// 全部音频静音
    /// </summary>
    public void Mute()
        {
            MuteJustAmbient();
            MuteJustMusic();
        }
    /// <summary>
    /// 音频音量静音
    /// </summary>
    public void MuteJustMusic()
        {
            m_MusicAudioSource.volume = 0f;
        }
    /// <summary>
    /// 背景音频静音
    /// </summary>
    public void MuteJustAmbient()
        {
            m_AmbientAudioSource.volume = 0f;
        }
    /// <summary>
    /// 全部音频取消静音
    /// </summary>
    public void Unmute()
        {
            UnmuteJustAmbient();
            UnmuteJustMustic();
        }
    /// <summary>
    /// 音频取消静音
    /// </summary>
    public void UnmuteJustMustic()
        {
            m_MusicAudioSource.volume = musicVolume;
        }
    /// <summary>
    /// 背景音频取消静音
    /// </summary>
    public void UnmuteJustAmbient()
        {
            m_AmbientAudioSource.volume = ambientVolume;
        }
    /// <summary>
    /// x秒后静音
    /// </summary>
    /// <param name="fadeTime"></param>
        public void Mute(float fadeTime)
        {
            MuteJustAmbient(fadeTime);
            MuteJustMusic(fadeTime);
        }
    /// <summary>
    /// x秒后静音
    /// </summary>
    /// <param name="fadeTime"></param>
    public void MuteJustMusic(float fadeTime)
        {
            StartCoroutine(VolumeFade(m_MusicAudioSource, 0f, fadeTime));
        }
    /// <summary>
    /// x秒后静音
    /// </summary>
    /// <param name="fadeTime"></param>
    public void MuteJustAmbient(float fadeTime)
        {
            StartCoroutine(VolumeFade(m_AmbientAudioSource, 0f, fadeTime));
        }
    /// <summary>
    /// x秒后取消静音
    /// </summary>
    /// <param name="fadeTime"></param>
    public void Unmute(float fadeTime)
        {
            UnmuteJustAmbient(fadeTime);
            UnmuteJustMusic(fadeTime);
        }
    /// <summary>
    /// x秒后取消静音
    /// </summary>
    /// <param name="fadeTime"></param>
    public void UnmuteJustMusic(float fadeTime)
        {
            StartCoroutine(VolumeFade(m_MusicAudioSource, musicVolume, fadeTime));
        }
    /// <summary>
    /// x秒后取消静音
    /// </summary>
    /// <param name="fadeTime"></param>
    public void UnmuteJustAmbient(float fadeTime)
        {
            StartCoroutine(VolumeFade(m_AmbientAudioSource, ambientVolume, fadeTime));
        }

        protected IEnumerator VolumeFade(AudioSource source, float finalVolume, float fadeTime)
        {
            float volumeDifference = Mathf.Abs(source.volume - finalVolume);
            float inverseFadeTime = 1f / fadeTime;

            while (!Mathf.Approximately(source.volume, finalVolume))
            {
                float delta = Time.deltaTime * volumeDifference * inverseFadeTime;
                source.volume = Mathf.MoveTowards(source.volume, finalVolume, delta);
                yield return null;
            }
            source.volume = finalVolume;
        }
    }

