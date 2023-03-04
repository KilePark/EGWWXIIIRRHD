using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public sealed class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance { get; private set; }

    private static Dictionary<string, int> _score = new Dictionary<string, int>();

    // updates score for appropiate element and updates text
    public void add_score(string element, int value)
    {
        _score[element] += value;
        string msg = ""; 
        foreach (string key in _score.Keys)
        {
            msg += $"{key} score: {_score[key]}\n";
        }
        scoreText.SetText(msg);
    }

    [SerializeField] private TextMeshProUGUI scoreText;
    private void Awake() => Instance = this;
    
    // On Start adds the all the elements to a Dictionary 
    // with the value being the score for said element 
    void Start()
    {
        foreach (string x in ItemDatabase.elements)
        {
            _score.Add(x, 0);
        }
    }
}
