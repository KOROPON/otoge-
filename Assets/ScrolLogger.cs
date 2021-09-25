using UnityEngine;
using UnityEngine.UI;


public class ScrolLogger : MonoBehaviour
{
    
    public void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -200, transform.localPosition.z);
    }
}
