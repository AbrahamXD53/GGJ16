using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

    public AudioClip[] effect;
    public AudioClip[] music;
    private AudioSource audioSource;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    public void OnLevelWasLoaded(int level)
    {
        print("level loaded: "+level);
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            if (level == 0)
            {
                audioSource.clip = music[0];
                audioSource.Play();
            }
        }
    }

}
