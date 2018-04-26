//using UnityEngine;


///**
// * Game - UIの橋渡しを行う
// */
//public class GameUIManager : MonoBehaviour
//{
//    private static GameUIManager _instance;

//    public static GameUIManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = FindObjectOfType<GameUIManager>();

//                if (_instance == null)
//                {
//                    _instance = new GameObject("Managers/GameUIManager").AddComponent<GameUIManager>();
//                }
//            }
//            return _instance;
//        }
//    }

//    [SerializeField]
//    private PlayerController playerController;

//    [SerializeField]
//    private UIController uiController;

//    void Awake()
//    {
//        //playerController.OnEnemyDefeated += (enemy) =>
//        //{
//        //    uiController.UpdateScore(enemy.Score);
//        //};
//    }

//    void Start()
//    {

//    }

//    void Update()
//    {

//    }
//}
