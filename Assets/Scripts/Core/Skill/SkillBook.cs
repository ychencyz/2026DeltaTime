using UnityEngine;

public class SkillBook : UIWindow
{
    [SerializeField]
    private GameObject go_skillsContainer;

    public void ShowCategory(GameObject go)
    {
        foreach (Transform child in go_skillsContainer.transform)
        {
            child.gameObject.SetActive(false);
        }
        go.SetActive(true);
    }
}
