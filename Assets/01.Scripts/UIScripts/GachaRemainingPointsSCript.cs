using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GachaRemainingPointsSCript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _pointText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.ScoreUpdate.AddListener(UpdatePoints);
    }

    void OnEnable()
    {
        UpdatePoints();
        GameManager.Instance.ScoreUpdate.AddListener(UpdatePoints);
    }

    private void OnDisable()
    {
        GameManager.Instance.ScoreUpdate.RemoveAllListeners();
    }

    // Update is called once per frame
    void UpdatePoints()
    {
        int points = GameManager.Instance.GetScore();
        _pointText.text = points.ToString();
    }
}
