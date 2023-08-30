using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class MatchResultScreenController : MonoBehaviour
{
    [SerializeField] private MatchResultsDataScriptableObject[] matchResultsDataScriptableObjects;
    [Header("Instances")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject ratingBlockRoot;
    [SerializeField] private Transform ratingRowsContainer;
    [SerializeField] private ProgressBar ratingProgressBar;
    [Space]
    [SerializeField] private GameObject experienceBlockRoot;
    [SerializeField] private Transform experienceRowsContainer;
    [SerializeField] private ProgressBar experienceProgressBar;
    [SerializeField] private Button nextMatchButton;
    [Header("Prefabs")]
    [SerializeField] private GameObject rewardRowPrefab;
    [SerializeField] private Sprite ratingSprite;
    [SerializeField] private Sprite experienceSprite;
    [Header("Values")]
    [SerializeField] private float animationSegmentsDuration = 0.5f;

    private const string victoryString = "VICTORY";
    private const string defeatString = "DEFEAT";
    private Sequence ratingAnimationSequence;
    private Sequence experienceAnimationSequence;
    private LayoutGroup ratingRowsLayoutGroup;
    private LayoutGroup experienceRowsLayoutGroup;

    private void Awake()
    {
        ratingRowsLayoutGroup = ratingRowsContainer.GetComponent<LayoutGroup>();
        experienceRowsLayoutGroup = experienceRowsContainer.GetComponent<LayoutGroup>();
    }

    private void Start()
    {
        ShowRandomMatch();
    }

    private void OnEnable()
    {
        nextMatchButton.onClick.AddListener(ShowRandomMatch);
    }

    private void OnDisable()
    {
        nextMatchButton.onClick.RemoveListener(ShowRandomMatch);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ratingAnimationSequence.IsActive() == true)
            {
                ratingAnimationSequence.Complete(true);
                var finishSequence = DOTween.Sequence();
                finishSequence.Append(ratingBlockRoot.transform.DOScale(1.3f, animationSegmentsDuration * 0.1f));
                finishSequence.Append(ratingBlockRoot.transform.DOScale(1f, animationSegmentsDuration * 0.9f));
                experienceAnimationSequence.SetDelay(animationSegmentsDuration);
            }
            else if (experienceAnimationSequence.IsActive() == true)
            {
                experienceAnimationSequence.Complete(true);
                var finishSequence = DOTween.Sequence();
                finishSequence.Append(experienceBlockRoot.transform.DOScale(1.3f, animationSegmentsDuration * 0.1f));
                finishSequence.Append(experienceBlockRoot.transform.DOScale(1f, animationSegmentsDuration * 0.9f));
            }
        }
    }

    private void ShowRandomMatch()
    {
        ShowMatchResults(matchResultsDataScriptableObjects[Random.Range(0, matchResultsDataScriptableObjects.Length)].MatchResultsData);
    }

    private void Cleanup()
    {
        ratingAnimationSequence.Kill();
        experienceAnimationSequence.Kill();
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
        
        ratingRowsLayoutGroup.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)ratingRowsContainer);
        ratingRowsLayoutGroup.enabled = true;
        experienceRowsLayoutGroup.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)experienceRowsContainer);
        experienceRowsLayoutGroup.enabled = true;
        ratingBlockRoot.SetActive(false);
        experienceBlockRoot.SetActive(false);
    }

    public void ShowMatchResults(MatchResultsData matchResultsData)
    {
        Cleanup();

        resultText.SetText(matchResultsData.IsWon ? victoryString : defeatString);
        ratingProgressBar.value = matchResultsData.RatingPreviousCurrentMaxValues.x;
        ratingProgressBar.maxValue = matchResultsData.RatingPreviousCurrentMaxValues.y;
        InitializeRewardBlock(ratingBlockRoot, ref ratingAnimationSequence, matchResultsData.RatingPreviousCurrentMaxValues.x, 
            ratingRowsContainer, matchResultsData.RatingRewardRowsData, ratingProgressBar, ratingSprite);
        experienceProgressBar.value = matchResultsData.ExperiencePreviousCurrentMaxValues.x;
        experienceProgressBar.maxValue = matchResultsData.ExperiencePreviousCurrentMaxValues.y;
        InitializeRewardBlock(experienceBlockRoot, ref experienceAnimationSequence, matchResultsData.ExperiencePreviousCurrentMaxValues.x, 
            experienceRowsContainer, matchResultsData.ExperienceRewardRowsData, experienceProgressBar, experienceSprite);
        
        ratingAnimationSequence.AppendCallback(() => experienceAnimationSequence.Play());
        ratingAnimationSequence.Play();
    }

    private void InitializeRewardBlock(GameObject blockRoot, ref Sequence animationSequence, int startProgressValue, 
        Transform rowsContainer, MatchResultsData.RewardRowData[] matchRewardsData, ProgressBar progressBar, Sprite rewardSprite)
    {
        if (matchRewardsData == null) return;


        animationSequence = DOTween.Sequence();
        animationSequence.Pause();
        int currentProgressValue = startProgressValue;

        if (matchRewardsData.Length > 0)
        {
            animationSequence.AppendCallback(() => blockRoot.SetActive(true));
            animationSequence.Append(blockRoot.transform.DOScale(1.3f, animationSegmentsDuration * 0.1f));
            animationSequence.Append(blockRoot.transform.DOScale(1f, animationSegmentsDuration * 0.9f));
        }

        for (int i = 0; i < matchRewardsData.Length; i++)
        {
            var rowGO = AddRow(rowsContainer, matchRewardsData[i].Name, matchRewardsData[i].Value, rewardSprite);
            animationSequence.AppendCallback(() => rowGO.SetActive(true));
            animationSequence.AppendCallback(() => LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rowsContainer));
            animationSequence.Append(rowGO.transform.DOScale(1.3f, animationSegmentsDuration * 0.1f));
            animationSequence.Append(rowGO.transform.DOScale(1f, animationSegmentsDuration * 0.9f));
            animationSequence.Insert(i * animationSegmentsDuration + animationSegmentsDuration, 
                DOVirtual.Int(currentProgressValue, currentProgressValue + matchRewardsData[i].Value, animationSegmentsDuration, v => progressBar.value = v));
            currentProgressValue += matchRewardsData[i].Value;
        }
    }

    private GameObject AddRow(Transform container, string name, int value, Sprite sprite)
    {
        var rowGO = Instantiate(rewardRowPrefab, container);
        var textMeshes = rowGO.GetComponentsInChildren<TextMeshProUGUI>();
        textMeshes[0].text = name;
        textMeshes[1].text = value.ToString();
        rowGO.GetComponentInChildren<Image>().sprite = sprite;
        rowGO.SetActive(false);
        return rowGO;
    }
}
