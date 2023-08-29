using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchResultsData", menuName = "ScriptableObjects/MatchResultsDataScriptableObject", order = 1)]
public class MatchResultsDataScriptableObject : ScriptableObject
{
    public MatchResultsData MatchResultsData;
    public (string Name, int Value) RatingRewardRowsData;
}
