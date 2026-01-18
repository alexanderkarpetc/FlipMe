using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _flipSound;
    [SerializeField] private AudioClip _matchSound;
    [SerializeField] private AudioClip _unMatchSound;
    [SerializeField] private AudioClip _winSound;

    public void PlayFlipSound()
    {
        _source.PlayOneShot(_flipSound);
    }

    public void PlayMatchSound()
    {
        _source.PlayOneShot(_matchSound);
    }

    public void PlayUnMatchSound()
    {
        _source.PlayOneShot(_unMatchSound);
    }
    
    public void PlayWinSound()
    {
        _source.PlayOneShot(_winSound);
    }
}
