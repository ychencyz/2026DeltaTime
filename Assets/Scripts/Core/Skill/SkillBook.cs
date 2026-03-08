using UnityEngine;

public class SkillBook : UIWindow
{
    [SerializeField]
    private GameObject go_skillsContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}

    public void ShowCategory(GameObject go)
    {
        foreach (Transform child in go_skillsContainer.transform)
        {
            child.gameObject.SetActive(false);
        }
        go.SetActive(true);
    }
}
