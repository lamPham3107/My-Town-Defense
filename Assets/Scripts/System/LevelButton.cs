using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;

    [SerializeField] private GameObject _imgLock;
    [SerializeField] private GameObject _imgUnlock;
    [SerializeField] private GameObject _imgPlayed;    
    [SerializeField] private TextMeshProUGUI _txtNumber;
    
    [SerializeField] private GameObject _star1;        
    [SerializeField] private GameObject _star2;         
    [SerializeField] private GameObject _star3;         

    [SerializeField] private Button _button;

    public void Setup()
    {
        bool unlocked = GameMaster.IsLevelUnlocked(levelIndex);
        int stars = GameMaster.GetLevelStars(levelIndex);
        bool played = stars > 0;

        if (!unlocked)
        {
            // chua mo
            _imgLock.SetActive(true);
            _imgUnlock.SetActive(false);
            _imgPlayed.SetActive(false);
            _txtNumber.gameObject.SetActive(false);
            _star1.SetActive(false);
            _star2.SetActive(false);
            _star3.SetActive(false);
            _button.interactable = false;
        }
        else if (!played)
        {
            // mo nhung chua choi
            _imgLock.SetActive(false);
            _imgUnlock.SetActive(true); 
            _imgPlayed.SetActive(false);
            _txtNumber.gameObject.SetActive(true);
            _txtNumber.text = levelIndex.ToString();
            _button.interactable = true;
            _star1.SetActive(false);
            _star2.SetActive(false);
            _star3.SetActive(false);
        }
        else
        {
            // da thang
            _imgLock.SetActive(false);
            _imgUnlock.SetActive(false);
            _imgPlayed.SetActive(true);
            _txtNumber.gameObject.SetActive(true);
            _txtNumber.text = levelIndex.ToString();
            _button.interactable = true;
            _star1.SetActive(stars >= 1);
            _star2.SetActive(stars >= 2);
            _star3.SetActive(stars >= 3);
        }
    }
}