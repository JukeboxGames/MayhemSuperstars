using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThreeDotText : MonoBehaviour
{
    private TMP_Text threeDotText;
    [SerializeField] private string baseString;
    [SerializeField] private float intervalTime;
    int numberOfDots = 0;

    private void OnEnable()
    {
        threeDotText = GetComponent<TMP_Text>();
        numberOfDots = 0;
        StartCoroutine(StartStringDots());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator StartStringDots()
    {
        string displayString;
        displayString = baseString;
        for (int i = 0; i < numberOfDots; i++)
        {
            displayString += ".";
        }
        threeDotText.text = displayString;
        numberOfDots++;
        if (numberOfDots > 3) numberOfDots = 0;
        yield return new WaitForSeconds(intervalTime);
        StartCoroutine(StartStringDots());
    }
}
