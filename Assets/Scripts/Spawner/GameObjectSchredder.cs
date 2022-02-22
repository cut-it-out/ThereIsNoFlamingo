using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSchredder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FallingObject fallingObject = collision.gameObject.GetComponent<FallingObject>();
        fallingObject.DestroySelf();
    }
}
