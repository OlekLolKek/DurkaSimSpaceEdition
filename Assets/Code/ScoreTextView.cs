using TMPro;
using UnityEngine;


namespace Code
{
    public sealed class ScoreTextView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        
        public void SetScore(int score)
        {
            _scoreText.text = $"Score: {score}";
        }
    }
}