using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    [SerializeField]
    int _totalRoundTime;
    [SerializeField]
    float _currentRoundTime;

    [SerializeField]
    Dictionary<float, bool> _bigWaveSpawningThresholds = new Dictionary<float, bool>
    {
        {.33f, false },
        {.66f, false }
    };

    float _currentRoundTimePercentage => _currentRoundTime / _totalRoundTime;
    float _previousRoundTimePercentage;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    public void Initialize()
    {
        _currentRoundTime = 0;
    }

    void Update()
    {
        if (GameManager.CurrentState == GameState.Defending)
            CountUpTime();
    }

    void CountUpTime()
    {
        if (_currentRoundTime <= _totalRoundTime)
        {
            _currentRoundTime += Time.deltaTime;

            var nextThreshold = _bigWaveSpawningThresholds.Where(t => !t.Value).OrderBy(t => t.Key)
                .FirstOrDefault(t => _previousRoundTimePercentage < t.Key && _currentRoundTimePercentage >= t.Key);
            
            if (nextThreshold.Key != 0)
            {
                Debug.LogWarning($"Called in setting {nextThreshold.Key} to true");
                _bigWaveSpawningThresholds[nextThreshold.Key] = true;
            }
        }
        else
            RoundTimeOut();
    }

    public void RoundTimeOut()
    {
        Debug.LogWarning($"Round Ran out of time");
    }
}
