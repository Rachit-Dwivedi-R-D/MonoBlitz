using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SoundEffects
{
    public string name;
    public AudioClip audioClip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sound Effects")]
    [SerializeField] private SoundEffects[] soundEffects;
    [SerializeField] private AudioSource sfxSource; // Assigned manually in Inspector

    [Header("Background Music")]
    [SerializeField] private AudioSource musicSource; // Assigned manually in Inspector

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (sfxSource == null)
          

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
           
        }
    }

    // === Automatically stop music when game scene loads ===
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Multiplayer Game" || scene.name == "SinglePlayerGame")
        {
            StopMusic();
        }
    }

    // === Sound Effects ===
    public void PlaySoundEffect(string name, float volume = 1f)
    {
        if (sfxSource == null)
        {
          
            return;
        }

        foreach (var sfx in soundEffects)
        {
            if (sfx.name == name && sfx.audioClip != null)
            {
                sfxSource.PlayOneShot(sfx.audioClip, volume);
                return;
            }
        }

        
    }

    // === Music Control ===
 public void StopMusic()
{

    AudioSource[] sources = FindObjectsOfType<AudioSource>();

    foreach (var source in sources)
    {


        if (source.isPlaying)
        {
            source.Stop();
            
        }
    }
}

    public void PlayMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
