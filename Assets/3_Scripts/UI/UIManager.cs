using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [Header("Hosting Panel")]
    [SerializeField]
    private GameObject hostPanel;
    [SerializeField]
    private Button nameBtn;    
    [SerializeField]
    private Button exitBtn;

    [Header("NameInp Panel")]
    [SerializeField]
    private GameObject namePanel;
    [SerializeField]
    private Button backBtn;

    private void Start()
    {
        ActivatePanel(hostPanel.name);

        nameBtn.onClick.AddListener(()=>ActivatePanel(namePanel.name));
        backBtn.onClick.AddListener(()=>ActivatePanel(hostPanel.name));

        exitBtn.onClick.AddListener(()=>Application.Quit());
    }

    private void ActivatePanel(string name)
    {
        hostPanel.gameObject.SetActive(name == hostPanel.gameObject.name);
        namePanel.gameObject.SetActive(name == namePanel.gameObject.name);
    }
}
