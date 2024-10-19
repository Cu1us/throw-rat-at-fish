using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]

public class Door : InteractableTile
{
    bool IsOpen = false;
    [SerializeField] AudioClip OpenFailSound;
    [SerializeField] AudioClip OpenSuccessSound;
    [SerializeField] Material OpenedMaterial;
    public override bool OnPlayerMoveOnto(PlayerMovement player)
    {
        if (IsOpen) return true;
        if (player.Keys > 0)
        {
            player.Keys -= 1;
            IsOpen = true;
            GetComponent<AudioSource>().PlayOneShot(OpenSuccessSound);
            GetComponent<MeshRenderer>().material = OpenedMaterial;
            if (TryGetComponent(out Collider collider)) collider.enabled = false;
        }
        else GetComponent<AudioSource>().PlayOneShot(OpenFailSound);
        return false;
    }
}
