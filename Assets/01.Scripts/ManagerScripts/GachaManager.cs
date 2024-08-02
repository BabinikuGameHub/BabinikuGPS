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

    private CharacterSO RollSingleGatcha()
    {
        List<CharacterSO> lv1List = AllCharacterList.Where(x => x.Level == 1).ToList();
        List<CharacterSO> lv2List = AllCharacterList.Where(x => x.Level == 2).ToList();
        List<CharacterSO> lv3List = AllCharacterList.Where(x => x.Level == 3).ToList();

        int RandomNum = Random.Range(0, 101);

        List<CharacterSO> RolledList;

        if(RandomNum == 100)
        {
            RolledList = lv3List;
        }else if(RandomNum > 90 &&  RandomNum < 100)
        {
            RolledList = lv2List;
        }
        else
        {
            RolledList = lv1List;
        }


        int num = GameManager.Instance.Rand.Next(RolledList.Count);

        return RolledList[num];

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
            //int x = GameManager.Instance.Rand.Next(AllCharacterList.Count);

            //returnList.Add(AllCharacterList[x]);

            returnList.Add(RollSingleGatcha());
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
