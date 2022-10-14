using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI desriptionText;

    public void Inintialize(string description)
    {
        desriptionText.text = description;
    }
}
