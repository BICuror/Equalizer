using UnityEngine;
using UnityEngine.Events;

public sealed class SpectrumGroupsManager : MonoBehaviour
{
    [Header("SpectrumGroupsSettins")]

    [SerializeField] private Group[] _spectrumGroups; 
    
    [System.Serializable] 
    private struct Group
    {
        public UnityEvent<float> UpdateSpectrumGroup;
        
        public float HighBorder;

        [HideInInspector] public float GroupValue;
    }

    [Header("FilteringSettings")]

    [SerializeField] private bool _useFiltering;

    [Range(0f, 1f)] [SerializeField] private float _a = 0.8f;

    [Range(0f, 1f)] [SerializeField] private float _h = 1f;

    [Range(0f, 1f)] [SerializeField] private float _filtrationStrength = 0.5f;

    [Range(0f, 1f)] [SerializeField] private float _maxPossibleNoiseValue = 0.3f;

    private KalmanFilter[] _filters;

    [Header("Links")]

    [SerializeField] private SpectrumDataManager _spectrumDataManager;

    private void Awake() => CreateKalmanFilters();

    private void OnValidate() => CreateKalmanFilters();

    public void CreateKalmanFilters()
    {
        _filters = new KalmanFilter[_spectrumGroups.Length];

        for (int i = 0; i < _spectrumGroups.Length; i++)
        {
            _filters[i] = new KalmanFilter(_a, _h, _filtrationStrength, _maxPossibleNoiseValue);
        }
    }

    public void UpdateSpectrumGroups()
    {
        CreateFrequencyGroups();

        for (int i = 0; i < _spectrumGroups.Length; i++)
        {
            _spectrumGroups[i].UpdateSpectrumGroup.Invoke(_spectrumGroups[i].GroupValue);
        }
    }

    private void CreateFrequencyGroups()
    {
        float[] samples = _spectrumDataManager.GetSamples();

        int sampleRange = _spectrumDataManager.GetSampleRange();

        float frequencySetp = AudioSettings.outputSampleRate / sampleRange; 

        float currentFrequency = 0f;

        int sampleIndex = 0;

        for (int x = 0; x < _spectrumGroups.Length; x++)
        {
            float groupAverage = 0f;
            int sampleCount = 0;

            while (currentFrequency < _spectrumGroups[x].HighBorder && sampleIndex < samples.Length)
            {
                groupAverage += samples[sampleIndex++] * (sampleIndex + 1);

                currentFrequency += frequencySetp;

                sampleCount++;
            }

            groupAverage /= sampleCount;

            if (_useFiltering == true) groupAverage = _filters[x].Filter(groupAverage);
            
            _spectrumGroups[x].GroupValue = groupAverage;
        }
    }
}

