using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class jumplinkBtn : MonoBehaviour
{
    public Button button;
    public string linkUrl;
    // Start is called before the first frame update
    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button == null) this.enabled = false;
        else button.onClick.AddListener(() =>
            Application.OpenURL(linkUrl)
        );
    }

}
