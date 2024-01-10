using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSlider : MonoBehaviour
{
    public float MinValue = 0, MaxValue = 100;

    public float CurrentValue;

    public Transform Fill;

    public bool ChangeScale;

    public Vector3 StartPos, EndPos;

    // Update is called once per frame
    void Update()
    {
        CurrentValue = Mathf.Clamp(CurrentValue, MinValue, MaxValue);

        if(ChangeScale)
        {
            Fill.localScale = new Vector3(CurrentValue / MaxValue, 1, 1);
        }

        Fill.localPosition = Vector3.Lerp(EndPos, StartPos, CurrentValue / MaxValue);
    }
}
