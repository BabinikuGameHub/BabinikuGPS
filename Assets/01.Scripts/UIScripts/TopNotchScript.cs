using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopNotchScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentTimeText;
    [SerializeField] private TextMeshProUGUI _currentDateText;


    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        UpdateDate();
    }

    void UpdateTime()
    {
        _currentTimeText.text = $"{System.DateTime.UtcNow.ToString("t")}";
    }

    void UpdateDate()
    {
        _currentDateText.text = $"{System.DateTime.UtcNow.ToString("d")}";
    }
}
