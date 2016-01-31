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
        audioSource.clip = music[0];
        audioSource.Play();
    }
    int nextSong = 1;
    float prevTime;
    IEnumerator WaitForSync()
    {
        print(audioSource.timeSamples);
        yield return new WaitForSeconds(audioSource.time%.60f);
        print("reproducir");
        audioSource.clip = music[nextSong];
        audioSource.Play((ulong)(audioSource.time % .60f));
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            nextSong=Random.Range(1,3);
            audioSource.clip = music[nextSong];
            audioSource.Play((ulong)(audioSource.time % .96));
            //StartCoroutine(WaitForSync());
        }
    }

    public void OnLevelWasLoaded(int level)
    {
        print("level loaded: "+level);
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            switch (level)
            {
                case 0:
                    audioSource.clip = music[0];
                    break;
                case 1:
                    audioSource.clip = music[2];
                    break;
                default:
                    break;
            }
            audioSource.Play();
        }
    }

}
