using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_script : MonoBehaviour
{
    public static audio_script instance {get; private set; }
    public GameObject hit;
    public GameObject Wicket;
    private AudioSource hit_audio;
    private AudioSource Wicket_audio;

    void Awake()
    {
        instance=this;
    }
    void Start()
    {
        hit_audio = hit.GetComponent<AudioSource>();
        Wicket_audio = Wicket.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    public void play_hit_audio()
    {
        hit_audio.Play();
    }
    public void play_Wicket_audio()
    {
        Wicket_audio.Play();
    }
}
