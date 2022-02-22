using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotCatchedZone : MonoBehaviour
{

    public void AdjustPositionToPlayerY(float playerY)
    {
        transform.position = new Vector2(transform.position.x, playerY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FallingObject fallingObject = collision.gameObject.GetComponent<FallingObject>();
        if (fallingObject != null && fallingObject.IsRealObject)
        {
            Game.GetInstance().RealItemMissed();
        }
    }
}
