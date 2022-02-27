using UnityEngine;

public class RealViewAffectedObject : MonoBehaviour
{
    [Header("Not Real View Sprites")]
    [SerializeField] private Sprite notRealSprite;
    [SerializeField] private Sprite realSprite;
    [Header("Not Real View Mesh")]
    [SerializeField] private Transform objMeshWithSpriteRenderer;

    public bool IsRealObject { get; private set; } = false;

    protected virtual void OnEnable()
    {
        Game.GetInstance().OnRealViewToggle += RealViewObject_OnRealViewToggle;
    }

    //protected virtual void OnDisable()
    //{
    //    Game.GetInstance().OnRealViewToggle -= Player_OnRealViewToggle;
    //}

    private void RealViewObject_OnRealViewToggle(bool value)
    {
        ShowRealSprite(value);
    }

    public void ShowRealSprite(bool showReal = true)
    {
        objMeshWithSpriteRenderer.GetComponent<SpriteRenderer>().sprite = showReal ? 
            (IsRealObject == true ? realSprite : null ) : notRealSprite;
    }

    public void SetIsReal(bool value)
    {
        IsRealObject = value;
    }

    public virtual void DestroySelf(float timeToTween = 0.1f)
    {
        Game.GetInstance().OnRealViewToggle -= RealViewObject_OnRealViewToggle;
        Destroy(gameObject, timeToTween + 0.1f);
        // TODO: Add animation/effect 
    }
}
