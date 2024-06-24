using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaManager : MonoBehaviour
{

    public static GachaManager Instance;

    [SerializeField] private List<CharacterSO> AllCharacterList;


    private void Awake()
    {
        Instance = this;

    }

    public CharacterSO RollSingleCharacter()
    {
        int num = GameManager.Instance.Rand.Next(AllCharacterList.Count);

        return AllCharacterList[num];
    }

    public List<CharacterSO> RollMultiCharacter(int num)
    {

        List<CharacterSO> returnList = new();

        for (int i = 0; i < num; i++)
        {
            int x = GameManager.Instance.Rand.Next(AllCharacterList.Count);

            returnList.Add(AllCharacterList[x]);
        }

        return returnList;
    }

}
