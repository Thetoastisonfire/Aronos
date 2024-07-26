using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

#region variables
    public static SoundManager Instance {get; private set; }
    public AudioSource clipsSource; // For sound effects
    public AudioSource clipsSource2; // For sound effects so they don't overlap
    public AudioSource clipsSource3; // For dialogue
    public AudioSource clipsSource4; // For specifically walk sounds cause they mess with everything
    public AudioSource clipsSource5; // For specifically walk sounds cause they mess with everything so they don't overlap
    public AudioSource backgroundMusicSource; // For background music
    public AudioSource backgroundAmbientSource; // For ambient effects
     [Space(20)]
    [SerializeField] private AudioSource miscAudioSource1;
    [SerializeField] private AudioSource miscAudioSource2;
    [SerializeField] private AudioSource miscAudioSource3;
    [SerializeField] private AudioSource miscAudioSource4;
    [SerializeField] private AudioSource miscAudioSource5;
    [SerializeField] private AudioSource miscAudioSource6;  
    [SerializeField] private AudioSource miscAudioSource7;
    private AudioSource[] audioSources;
    private float[] originalVolumes;



    private Dictionary<string, AudioClip> dialogueClips = new Dictionary<string, AudioClip>(); //dialogue, organized like "1,1" and "48,3"
    private Dictionary<string, AudioClip> SEClips = new Dictionary<string, AudioClip>(); //sound effects
    private Dictionary<string, AudioClip> backgroundMusic = new Dictionary<string, AudioClip>(); //background music
    public bool initializationComplete = false;

#endregion


#region initializing 
    // Start is called before the first frame update
  
    void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure clipsSource, backgroundMusicSource, and clipsSource3 are initialized
        if (clipsSource == null || backgroundMusicSource == null || clipsSource3 == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length < 3)
            {
                clipsSource = gameObject.AddComponent<AudioSource>();
                backgroundMusicSource = gameObject.AddComponent<AudioSource>();
                clipsSource3 = gameObject.AddComponent<AudioSource>(); // Add clipsSource3 if not already present
            }
            else
            {
                clipsSource = sources[0];
                backgroundMusicSource = sources[1];
                clipsSource3 = sources[2]; // Assign clipsSource3 if already present
            }
        }

        StartCoroutine(InitializeAudioClips());
    }

    private IEnumerator InitializeAudioClips()
    {
        StartCoroutine(LoadAudioClips("audio/dialogueClips", dialogueClips));
        StartCoroutine(LoadAudioClips("audio/SEClips", SEClips));
        yield return StartCoroutine(LoadAudioClips("audio/background", backgroundMusic));
        initializationComplete = true;
    }

    private IEnumerator LoadAudioClips(string path, Dictionary<string, AudioClip> clipDictionary)
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(path);
        
        if (clips.Length <= 0) Debug.Log("no clips error");

        int batchSize = 4; // Adjust batch size as needed for performance
        for (int i = 0; i < clips.Length; i += batchSize)
        {
            for (int j = 0; j < batchSize && (i + j) < clips.Length; j++)
            {
                AudioClip clip = clips[i + j];
                clipDictionary[clip.name] = clip;
            }
            // Yield control back to the main thread to avoid freezing
            yield return null;
        }
    }

    private bool TryParseClipName(string clipName, out int num1, out int num2)
    {
        // Split the clip name by the comma
        string[] parts = clipName.Split(',');

        // Try to parse both parts into integers
        if (parts.Length == 2 && int.TryParse(parts[0], out num1) && int.TryParse(parts[1], out num2))
        {
            return true;
        }

        // If parsing fails, return false and default values
        num1 = 0;
        num2 = 0;
        return false;
    }

    // Method to set the volume
    public void SetGlobalVolume(float volume)
    {   
        volume = Mathf.Clamp(volume, 0f, 1f);
        
        for (int i = 0; i < audioSources.Length; i++ ) {
            if (audioSources[i] != null) audioSources[i].volume = originalVolumes[i] * volume;
        }
        
    }

    public void Start() {
        audioSources =
        new AudioSource[] {
            clipsSource, clipsSource2,
            clipsSource3, clipsSource4,
            clipsSource5,
            backgroundMusicSource, backgroundAmbientSource,
            miscAudioSource1, miscAudioSource2,
            miscAudioSource3, miscAudioSource4, 
            miscAudioSource5, miscAudioSource6, 
            miscAudioSource7
        };

        originalVolumes = new float[audioSources.Length];

        for (int i = 0; i < audioSources.Length; i++) {
            if (audioSources[i] != null) {
                originalVolumes[i] = audioSources[i].volume;
                Debug.Log("Assigned AudioSource at index " + i + ": " + audioSources[i].name);
            }
        }//14 audio sources, null sources are ignored

        SetGlobalVolume(0f);
    }

#endregion

#region playing sounds

    //play audio clip methods start
    public IEnumerator PlayAudioClip(string clipName, bool isDialogue) {
        if (isDialogue)
        {
            if (dialogueClips.TryGetValue(clipName, out AudioClip clip))
            {
                yield return PlayClip(clip, true);
            }
            else
            {
                Debug.LogWarning($"Dialogue audio clip not found: {clipName}");
            }
        }
        else
        {
            if (SEClips.TryGetValue(clipName, out AudioClip clip))
            {
                yield return PlayClip(clip, false);
            }
            else
            {
                Debug.LogWarning($"Sound effect audio clip not found: {clipName}");
            }
        }
    }//end of playAudioClip

        private IEnumerator PlayClip(AudioClip clip, bool isDialogue) {
            AudioSource selectedSource = clipsSource.isPlaying ? clipsSource2 : clipsSource;
            if (isDialogue) selectedSource = clipsSource3; //dialogue only plays on clip source 3

            selectedSource.clip = clip;
            selectedSource.loop = false; // No looping for sound effects
            selectedSource.Play();

            // Wait until the clip finishes playing
            while (selectedSource.isPlaying)
            {
                yield return null;
            }

            StartCoroutine(FadeOutAndStopCoroutine(selectedSource, 0.1f));
        }
    //play audio clip methods end


    public IEnumerator PlayStuntedClip(string clipName)
    {
        Dictionary<string, AudioClip> clipDictionary = SEClips;

        if (clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            AudioSource selectedSource = clipsSource;

            selectedSource.clip = clip;
            selectedSource.loop = false; // No looping for sound effects
            selectedSource.Play();       

            // Wait until the clip finishes playing
            while (selectedSource.isPlaying)
            {
                yield return null;
            }
            StartCoroutine(FadeOutAndStopCoroutine(selectedSource, 0.1f));
        }
        else
        {
            Debug.LogWarning($"Audio clip not found: {clipName}");
        }
    }
    
    public IEnumerator PlayWalkingAudioClip(string clipName) {

            if (SEClips.TryGetValue(clipName, out AudioClip clip))
            {
                yield return PlayWalkingClip(clip);
            }
            else
            {
                Debug.LogWarning($"Sound effect audio clip not found: {clipName}");
            }
    }//end of PlayWalkingAudioClip

    private IEnumerator PlayWalkingClip(AudioClip clip) {
            AudioSource selectedSource = clipsSource5.isPlaying ? clipsSource4 : clipsSource5;

            selectedSource.clip = clip;
            selectedSource.loop = false; // No looping for sound effects
            selectedSource.Play();

            // Wait until the clip finishes playing
            while (selectedSource.isPlaying)
            {
                yield return null;
            }

            StartCoroutine(FadeOutAndStopCoroutine(selectedSource, 0.1f));
    }

    public IEnumerator PlayBackgroundMusic(string clipName)
    {
        if (backgroundMusic.TryGetValue(clipName, out AudioClip clip))
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.loop = true; // Background music typically loops
            backgroundMusicSource.Play();

            // Wait until the clip finishes playing (or looping is stopped externally)
            while (backgroundMusicSource.isPlaying)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning($"Background music clip not found: {clipName}");
        }
    }

    public IEnumerator PlayAmbient(string clipName)
    {
        if (backgroundMusic.TryGetValue(clipName, out AudioClip clip))
        {
            backgroundAmbientSource.clip = clip;
            backgroundAmbientSource.loop = true; // Background music typically loops
            backgroundAmbientSource.Play();

            // Wait until the clip finishes playing (or looping is stopped externally)
            while (backgroundAmbientSource.isPlaying)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning($"Background music clip not found: {clipName}");
        }
    }

#endregion

#region stopping sounds
    //stop sound
    public void StopDialogueClip(){
        if (clipsSource3.isPlaying)
        {
            clipsSource3.Stop();
        }
    }

    public void StopSEClip() {
        if (clipsSource.isPlaying)
        {
            clipsSource.Stop();
        }
        if (clipsSource2.isPlaying)
        {
            clipsSource2.Stop();
        }
    }

    public void StopWalkClip() {
          if (clipsSource4.isPlaying)
        {
            clipsSource4.Stop();
        }
        if (clipsSource5.isPlaying)
        {
            clipsSource5.Stop();
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource.isPlaying && backgroundMusicSource.loop)
        {
            backgroundMusicSource.Stop();
        }
    }

    //fade then stop
    public void FadeOutAndStopAudioClip(float fadeDuration, bool isSE = true) {
        AudioSource source = isSE ? clipsSource : backgroundMusicSource;
        StartCoroutine(FadeOutAndStopCoroutine(source, fadeDuration));
    }

    private IEnumerator FadeOutAndStopCoroutine(AudioSource source, float fadeDuration) {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; //reset volume for future use
    }



#endregion

}
