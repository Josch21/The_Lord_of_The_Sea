using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleTransporte : MonoBehaviour
{
    public Transform Target;
    public GameObject ThePlayer;
    public AudioClip clip;
    public AudioSource audioplayer;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            audioplayer.clip = clip;
            audioplayer.Play();
            ThePlayer.transform.position = Target.transform.position; }
    }
}
