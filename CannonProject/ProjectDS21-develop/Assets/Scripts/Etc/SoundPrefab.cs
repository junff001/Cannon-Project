using System.Collections.Generic;
using UnityEngine;

public class SoundPrefab : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;
    private bool isSoundPlaying;

    private void OnEnable()  //1
    {
        isSoundPlaying = false;
    }

    public void PlaySound(AudioClip ac, float volume)  //2
    {
        _audio.clip = ac;
        _audio.volume = volume;
        _audio.Play();

        isSoundPlaying = true;
    }

    private void Update()  //3
    {
        if (isSoundPlaying && !_audio.isPlaying) gameObject.SetActive(false);
    }
}
