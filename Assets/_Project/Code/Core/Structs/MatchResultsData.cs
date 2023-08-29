using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MatchResultsData
{
    [System.Serializable]
    public struct RewardRowData
    {
        public string Name;
        public int Value;
    }

    public bool IsWon;
    public Vector2Int RatingPreviousCurrentMaxValues;
    public RewardRowData[] RatingRewardRowsData;
    public Vector2Int ExperiencePreviousCurrentMaxValues;
    public RewardRowData[] ExperienceRewardRowsData;
}
