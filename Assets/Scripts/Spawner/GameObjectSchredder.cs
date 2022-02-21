using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSchredder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FallingObject fallingObject = collision.gameObject.GetComponent<FallingObject>();
        if (fallingObject.IsRealObject)
        {
            // TODO: most likely this need to be moved to a different collider to check
            Game.GetInstance().RealItemMissed();
        }
        collision.gameObject.GetComponent<FallingObject>().DestroySelf();

    }
}
