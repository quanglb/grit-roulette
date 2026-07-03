using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("AudioManager");
                instance = go.AddComponent<AudioManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private AudioSource bgmSource;
    private List<AudioSource> activeSfxSources = new List<AudioSource>();
    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup BGM Source
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
    }

    private AudioClip GetAudioClip(string clipName)
    {
        if (string.IsNullOrEmpty(clipName)) return null;

        // Bỏ phần mở rộng nếu có (.wav, .mp3, .ogg)
        if (clipName.Contains("."))
        {
            clipName = System.IO.Path.GetFileNameWithoutExtension(clipName);
        }

        if (clipCache.TryGetValue(clipName, out AudioClip clip))
        {
            return clip;
        }

        clip = Resources.Load<AudioClip>("Audio/" + clipName);
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] AudioClip not found in Resources/Audio/: {clipName}");
            return null;
        }

        clipCache[clipName] = clip;
        return clip;
    }

    public void PlayBGM(string clipName)
    {
        AudioClip clip = GetAudioClip(clipName);
        if (clip == null) return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            return; // Đang chạy đúng bài rồi
        }

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public AudioSource PlaySFX(string clipName, bool loop = false)
    {
        AudioClip clip = GetAudioClip(clipName);
        if (clip == null) return null;

        // Dọn dẹp các AudioSource đã bị hủy hoặc không dùng
        activeSfxSources.RemoveAll(source => source == null);

        // Tìm một AudioSource nhàn rỗi
        AudioSource sfxSource = null;
        foreach (var source in activeSfxSources)
        {
            if (!source.isPlaying && !source.loop)
            {
                sfxSource = source;
                break;
            }
        }

        // Nếu không có sẵn, tạo mới
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            activeSfxSources.Add(sfxSource);
        }

        sfxSource.clip = clip;
        sfxSource.loop = loop;
        sfxSource.Play();
        return sfxSource;
    }

    public void StopSFX(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
        }
    }
}
