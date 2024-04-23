using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioClip[] songs;
    private int currentSongIndex = 0;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextSong();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextSong();
        }
    }

    void PlayNextSong()
    {
        if (currentSongIndex < songs.Length)
        {
            audioSource.clip = songs[currentSongIndex];
            audioSource.Play();
            currentSongIndex++;
        }
        else
        {
            currentSongIndex = 0;
        }
    }
}
