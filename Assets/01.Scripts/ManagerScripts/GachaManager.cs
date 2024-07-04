using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public List<CharacterSO> GetCharacterList()
    {
        return AllCharacterList;
    }

    public CharacterSO GetSOByName(string name)
    {
        return AllCharacterList.FirstOrDefault(x => x.CharacterName.Equals(name));
    }

    public CharacterSO GetSObyID(string ID)
    {
        CharacterSO result = AllCharacterList?.FirstOrDefault(x => x.uniqueID.Equals(ID));

        return result;

    }
}
