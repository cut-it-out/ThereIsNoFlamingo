using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : RealViewAffectedObject
{
    [Header("Falling Object Settings")]    
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;    

    private float movementSpeed;

    public static FallingObject Create(Transform prefabTransform, Vector3 worldPosition, float speed, bool isReal, bool showRealSprite)
    {
        Transform fallingObjTransform = Instantiate(prefabTransform, worldPosition, Quaternion.identity);

        FallingObject fallingObject = fallingObjTransform.GetComponent<FallingObject>();

        fallingObject.movementSpeed = speed;
        fallingObject.SetIsReal(isReal);

        fallingObject.ShowRealSprite(showRealSprite);

        return fallingObject;
    }

    // TODO: handle collision with player / schedder


    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 delta = Vector2.down * movementSpeed * Time.deltaTime;
        transform.position += delta;
    }

    public void GetPadding(out float paddingLeft, out float paddingRight)
    {
        paddingLeft = this.paddingLeft;
        paddingRight = this.paddingRight;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}
