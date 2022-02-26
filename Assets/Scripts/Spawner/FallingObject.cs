using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : RealViewAffectedObject
{
    [Header("Falling Object Settings")]
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;

    [Header("RealView Fuel settings")]
    [SerializeField] float fuelGainOnRealCatch;
    [SerializeField] float fuelLostOnNotRealCatched;

    private float movementSpeed;

    public static FallingObject Create(Transform prefabTransform, Vector3 worldPosition, float speed, bool isReal)
    {
        Transform fallingObjTransform = Instantiate(prefabTransform, worldPosition, Quaternion.identity);

        FallingObject fallingObject = fallingObjTransform.GetComponent<FallingObject>();

        fallingObject.movementSpeed = speed;
        fallingObject.SetIsReal(isReal);

        fallingObject.ShowRealSprite(Game.GetInstance().IsRealViewEnabled);

        return fallingObject;
    }

    public float GetFuelGain() => fuelGainOnRealCatch;
    public float GetFuelLost() => fuelLostOnNotRealCatched;

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

    public override void DestroySelf()
    {
        Spawner.GetInstance().RemoveFallingObject(this);
        Spawner.GetInstance().OnProgressMultiplierChange -= FallingObject_OnProgressMultiplierChange;
        base.DestroySelf();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Spawner.GetInstance().OnProgressMultiplierChange += FallingObject_OnProgressMultiplierChange;
    }

    private void FallingObject_OnProgressMultiplierChange(float newSpeed)
    {
        movementSpeed = newSpeed;
    }
}
