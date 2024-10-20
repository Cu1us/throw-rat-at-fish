using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]

public class Door : InteractableTile
{
    public bool IsOpen = false;
    [SerializeField] AudioClip OpenFailSound;
    [SerializeField] AudioClip OpenSuccessSound;
    [SerializeField] Material OpenedMaterial;
    public override GridMovement.TryWalkOnTileResult OnPlayerMoveOnto(PlayerMovement player)
    {
        if (IsOpen) return GridMovement.TryWalkOnTileResult.PassThrough;
        if (player.Keys > 0)
        {
            player.Keys -= 1;
            IsOpen = true;
            GetComponent<AudioSource>().PlayOneShot(OpenSuccessSound);
            GetComponent<MeshRenderer>().material = OpenedMaterial;
            player.failedToMoveOntoCell = new(-10000, 0, 0);
            if (TryGetComponent(out Collider collider)) collider.enabled = false;
            return GridMovement.TryWalkOnTileResult.StopButCanPassNextTime;
        }
        else GetComponent<AudioSource>().PlayOneShot(OpenFailSound);
        return GridMovement.TryWalkOnTileResult.Stop;
    }
}
