using UnityEngine;

public class Credits : MonoBehaviour
{
    public GameObject credit;

    void Start()
    {
        credit = GameObject.Find("Credit");
        credit.SetActive(false);
    }

    public void ToCredit()
    {
        credit.SetActive(true);
    }

    public void BackCredit()
    {
        credit.SetActive(false);
    }
}
