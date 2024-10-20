using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetTile : InteractableTile
{
    [SerializeField] float FloatingSpeed;
    [SerializeField] float FloatingDistance;

    [SerializeField] Transform RotationObject;
    [SerializeField] Transform Rat;

    Vector3 defaultPosition;
    bool pickedUp = false;
    void Start()
    {
        defaultPosition = RotationObject.position;
    }
    void Update()
    {
        if (!pickedUp) RotationObject.transform.position = defaultPosition + Vector3.up * Mathf.Sin(Time.time * FloatingSpeed) * FloatingDistance;
    }
    public override GridMovement.TryWalkOnTileResult OnPlayerMoveOnto(PlayerMovement player)
    {
        if (pickedUp) return GridMovement.TryWalkOnTileResult.PassThrough;
        pickedUp = true;
        //GetComponent<AudioSource>().Play();
        RotationObject.gameObject.SetActive(false);
        Rat.gameObject.SetActive(false);

        FindFirstObjectByType<GameManager>().PickupHelmet();
        return GridMovement.TryWalkOnTileResult.StopButCanPassNextTime;
    }
}
