using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Key : InteractableTile
{
    [SerializeField] float RotationSpeed;
    [SerializeField] Transform RotationObject;
    bool pickedUp = false;
    void Update()
    {
        if (!pickedUp) RotationObject.Rotate(new Vector3(0, RotationSpeed * Time.deltaTime, 0));
    }
    public override GridMovement.TryWalkOnTileResult OnPlayerMoveOnto(PlayerMovement player)
    {
        if (pickedUp) return GridMovement.TryWalkOnTileResult.PassThrough;
        player.Keys++;
        pickedUp = true;
        GetComponent<AudioSource>().Play();
        RotationObject.gameObject.SetActive(false);
        return GridMovement.TryWalkOnTileResult.StopButCanPassNextTime;
    }
}
