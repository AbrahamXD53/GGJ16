using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

    public AudioClip[] effect;
    public AudioClip[] music;
    private AudioSource audioSource;
    public static SoundManager Instance;

    int nextSong = 1;
    float prevTime;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = music[0];
        audioSource.Play();
    }
    
    IEnumerator WaitForSync()
    {
        print(audioSource.timeSamples);
        yield return new WaitForSeconds(audioSource.time%.60f);
        print("reproducir");
        audioSource.clip = music[nextSong];
        audioSource.Play();
    }
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.K))
        {
            nextSong=Random.Range(1,3);
            audioSource.clip = music[nextSong];
            audioSource.Play((ulong)(audioSource.time % .96));
            //StartCoroutine(WaitForSync());
        }*/
    }
    private int lastSong=2;
    public void ChangeSong(int prog)
    {
        var posible = 1;
        if(prog<30)
        {
            posible = 1;
        }
        else
        {
            if(prog<70)
            {
                posible = 2;
            }
            else
            {
                posible = 3;
            }
        }
        if(lastSong!=posible)
        {
            lastSong = posible;
            audioSource.clip = music[lastSong];
            audioSource.Play((ulong)(audioSource.time % .96));
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
