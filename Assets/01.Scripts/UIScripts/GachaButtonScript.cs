using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaButtonScript : MonoBehaviour
{
    [SerializeField] GameObject _popUpHolder;
    [SerializeField] GameObject _gachaResultPopupPrefab;
    Animation _gachaRollAnimation;

    private int _currentGachaCost = 10000;

    public void Awake()
    {
        _gachaRollAnimation = GetComponent<Animation>();
    }

    public void RollGacha()
    {
        if (_gachaRollAnimation.isPlaying)
            return;

        int currentPoint = GameManager.Instance.GetScore();

        if (currentPoint < _currentGachaCost)
        {
            GameManager.Instance.PopupMessage("포인트가 부족합니다!");
            return;
        }

        DialAnimation();


    }

    private void DialAnimation()
    {
        _gachaRollAnimation.Play("GachaDialRotation");

    }


    private void DialRewind()
    {
        _gachaRollAnimation.Play("GachaDialRewind");

    }

    private void ResolveGacha()
    {

        GameManager.Instance.SubtractScore(_currentGachaCost);

        List<CharacterSO> list = GachaManager.Instance.RollMultiCharacter(10); 

        GameObject gachaResultPopUpObject = Instantiate(_gachaResultPopupPrefab, _popUpHolder.transform);
        GachaResultPopupScript gachaResultPopupScript = gachaResultPopUpObject.GetComponent<GachaResultPopupScript>();

        gachaResultPopupScript.InitializeWithSO(list);

        GameManager.Instance.AddObtainedCharacters(list);

        GameManager.Instance.SaveProgress();

        DialRewind();
    }

}
