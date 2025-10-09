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
        if (coinAudioSource == null)
        {
            coinAudioSource = gameObject.AddComponent<AudioSource>();
            if (audioMixer != null)
            {
                AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("SFX");
                if (groups.Length > 0)
                {
                    coinAudioSource.outputAudioMixerGroup = groups[0];
                }
            }
        }
        useAudioMixer = audioMixer != null && audioMixer.GetFloat(pitchParameterName, out _);
        ResetCoinCount();
    }

    public void PlayCoinSound()
    {
        coinCount++;
        float newPitch = Mathf.Min(basePitch + (coinCount * pitchIncrement), maxPitch);
        if (useAudioMixer && audioMixer != null)
        {
            audioMixer.SetFloat(pitchParameterName, newPitch);
        }
        else if (coinAudioSource != null)
        {
            coinAudioSource.pitch = newPitch;
        }
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