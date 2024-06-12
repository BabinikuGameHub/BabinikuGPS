using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScorePopupScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void PopupEvent(string text)
    {
        this.gameObject.SetActive(true);

        _scoreText.text = text;

        StartCoroutine(PopupCoroutine());
    }

    public void PopupScoreEvent(int score)
    {
        this.gameObject.SetActive(true);

        _scoreText.text = $"{score}점\n 획득!!";

        StartCoroutine(PopupCoroutine());

    }

    IEnumerator PopupCoroutine()
    {
        yield return new WaitForSeconds(5);

        this.gameObject.SetActive(false);

    }
}
