using UnityEngine;
using UnityEngine.Audio;

public class CoinAudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string pitchParameterName = "CoinPitch";
    [SerializeField] private float basePitch = 1.0f;
    [SerializeField] private float pitchIncrement = 0.05f;
    [SerializeField] private float maxPitch = 2.0f;

    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioSource coinAudioSource;

    private int coinCount = 0;
    private bool useAudioMixer = false;

    void Start()
    {
        // Initialize audio source if not assigned
        if (coinAudioSource == null)
        {
            coinAudioSource = gameObject.AddComponent<AudioSource>();

            // Try to assign the output group if mixer exists
            if (audioMixer != null)
            {
                AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("SFX");
                if (groups.Length > 0)
                {
                    coinAudioSource.outputAudioMixerGroup = groups[0];
                }
            }
        }

        // Check if we can use audio mixer
        useAudioMixer = audioMixer != null && audioMixer.GetFloat(pitchParameterName, out _);

        // Initialize pitch
        ResetCoinCount();
    }

    public void PlayCoinSound()
    {
        // Increment coin count
        coinCount++;

        // Calculate new pitch value
        float newPitch = Mathf.Min(basePitch + (coinCount * pitchIncrement), maxPitch);

        // Update pitch either through mixer or directly on audio source
        if (useAudioMixer && audioMixer != null)
        {
            audioMixer.SetFloat(pitchParameterName, newPitch);
        }
        else if (coinAudioSource != null)
        {
            coinAudioSource.pitch = newPitch;
        }

        // Play the sound
        if (coinSound != null && coinAudioSource != null)
        {
            coinAudioSource.PlayOneShot(coinSound);
        }
    }

    public void ResetCoinCount()
    {
        // Reset coin count
        coinCount = 0;

        // Reset pitch either through mixer or directly
        if (useAudioMixer && audioMixer != null)
        {
            audioMixer.SetFloat(pitchParameterName, basePitch);
        }
        else if (coinAudioSource != null)
        {
            coinAudioSource.pitch = basePitch;
        }
    }
}