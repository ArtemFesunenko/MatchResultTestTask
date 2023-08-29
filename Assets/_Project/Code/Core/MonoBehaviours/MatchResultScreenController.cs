using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MatchResultScreenController : MonoBehaviour
{
    [SerializeField] private MatchResultsDataScriptableObject matchResultsDataScriptableObject;
    [Header("References")]
    [Header("Instances")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject ratingBlockRoot;
    [SerializeField] private Transform ratingRowsContainer;
    [SerializeField] private ProgressBar ratingProgressBar;
    [SerializeField] private GameObject experienceBlockRoot;
    [SerializeField] private Transform experienceRowsContainer;
    [SerializeField] private ProgressBar experienceProgressBar;
    [Header("Prefabs")]
    [SerializeField] private GameObject rewardRowPrefab;

    private const string victoryString = "VICTORY";
    private const string defeatString = "DEFEAT";

    void Start()
    {
        Cleanup();

        ShowMatchResults(matchResultsDataScriptableObject.MatchResultsData);
    }

    private void Cleanup()
    {
        ratingProgressBar.value = 0;
        experienceProgressBar.value = 0;
        for (int i = 0; i < ratingRowsContainer.childCount; i++)
        {
            Destroy(ratingRowsContainer.GetChild(i).gameObject);
        }
        for (int i = 0; i < experienceRowsContainer.childCount; i++)
        {
            Destroy(experienceRowsContainer.GetChild(i).gameObject);
        }
        ratingBlockRoot.SetActive(false);
        experienceBlockRoot.SetActive(false);
    }

    public void ShowMatchResults(MatchResultsData matchResultsData)
    {
        resultText.SetText(matchResultsData.IsWon ? victoryString : defeatString);
        ratingProgressBar.value = matchResultsData.RatingPreviousCurrentMaxValues.x;
        ratingProgressBar.maxValue = matchResultsData.RatingPreviousCurrentMaxValues.y;
        InitializeRatingBlock(matchResultsData.RatingRewardRowsData);
        experienceProgressBar.value = matchResultsData.ExperiencePreviousCurrentMaxValues.x;
        experienceProgressBar.maxValue = matchResultsData.ExperiencePreviousCurrentMaxValues.y;
        InitializeExperienceBlock(matchResultsData.ExperienceRewardRowsData);
    }

    private void InitializeRatingBlock(MatchResultsData.RewardRowData[] matchRewardsData)
    {
        if (matchRewardsData == null) return;
        
        if (matchRewardsData.Length > 0)
            ratingBlockRoot.SetActive(true);

        for (int i = 0; i < matchRewardsData.Length;i++)
        {
            AddRow(ratingRowsContainer, matchRewardsData[i].Name, matchRewardsData[i].Value);
            ratingProgressBar.value += matchRewardsData[i].Value;
        }
    }

    private void InitializeExperienceBlock(MatchResultsData.RewardRowData[] matchRewardsData)
    {
        if (matchRewardsData == null) return;

        if (matchRewardsData.Length > 0) 
            experienceBlockRoot.SetActive(true);

        for (int i = 0; i < matchRewardsData.Length; i++)
        {
            AddRow(experienceRowsContainer, matchRewardsData[i].Name, matchRewardsData[i].Value);
            experienceProgressBar.value += matchRewardsData[i].Value;
        }
    }

    private void AddRow(Transform container, string name, int value)
    {
        var rowGO = Instantiate(rewardRowPrefab, container);
        var textMeshes = rowGO.GetComponentsInChildren<TextMeshProUGUI>();
        textMeshes[0].text = name;
        textMeshes[1].text = value.ToString();
    }
}
