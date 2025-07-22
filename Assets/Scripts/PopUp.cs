using UnityEngine;

public class PopUp : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve popupCurve;

    [SerializeField]
    private float popupDuration;

    private float popupTimer = 1f;

    private void Update()
    {
        if (popupTimer < popupDuration)
        {
            float t = popupTimer / popupDuration;
            float scale = popupCurve.Evaluate(t);
            transform.localScale = new Vector3(scale, scale, scale);
            popupTimer += Time.deltaTime;
        }
    }

    public void Popup()
    {
        popupTimer = 0f;
    }
}
