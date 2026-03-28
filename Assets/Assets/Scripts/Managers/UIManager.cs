using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private List<UIPanel> allPanels;
    [SerializeField] private List<PanelName> defaultPanels;

    private Dictionary<PanelName, UIPanel> panelDictionary = new Dictionary<PanelName, UIPanel>();

    // Stack để lưu trữ thứ tự các Panel đã mở
    private Stack<PanelName> openPanelsStack = new Stack<PanelName>();

    private void Awake()
    {
        Instance = this;
        panelDictionary.Clear();
        foreach (var panel in allPanels)
        {
            if (panel != null && !panelDictionary.ContainsKey(panel.panelName))
            {
                panelDictionary.Add(panel.panelName, panel);
            }
        }
    }

    private void Start()
    {
        HideAll();
        foreach (var panelName in defaultPanels)
        {
            ShowPanel(panelName, false);
        }
    }

    private void Update()
    {
        // Kiểm tra phím ESC mỗi khung hình
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLastPanel();
        }
    }

    public void ShowPanel(PanelName name, bool hideOther)
    {
        Debug.Log("Show" + name);
        if (!panelDictionary.TryGetValue(name, out UIPanel targetPanel)) return;

        if (hideOther)
        {
            HideAll();
            openPanelsStack.Clear();
        }


        if (!targetPanel.gameObject.activeSelf)
        {
            targetPanel.Show();
            openPanelsStack.Push(name);
        }


        if (targetPanel.panelsActiveWith != null)
        {
            foreach (var otherName in targetPanel.panelsActiveWith)
            {
                if (panelDictionary.TryGetValue(otherName, out UIPanel otherPanel))
                    otherPanel.Show();
            }
        }
    }

    public void HidePanel(PanelName name)
    {
        Debug.Log("Hide" + name);
        if (panelDictionary.TryGetValue(name, out UIPanel panel))
        {
            panel.Hide();
            if (panel.panelsActiveWith != null)
            {
                foreach (var extra in panel.panelsActiveWith)
                {
                    if (panelDictionary.TryGetValue(extra, out UIPanel extraPanel))
                        extraPanel.Hide();
                }
            }
        }
    }

    public void CloseLastPanel()
    {
        if (openPanelsStack.Count > 0)
        {
            PanelName lastPanel = openPanelsStack.Pop();
            HidePanel(lastPanel);
        }
    }

    public void HideAll()
    {
        foreach (var panel in panelDictionary.Values)
        {
            if (panel != null) panel.Hide();
        }
        openPanelsStack.Clear();
    }

    public void TogglePanel(PanelName name, bool hideOther)
    {
        if (!panelDictionary.TryGetValue(name, out UIPanel panel)) return;

        if (panel.gameObject.activeSelf)
        {
            RemoveFromStack(name);
            HidePanel(name);
        }
        else
        {
            ShowPanel(name, hideOther);
        }
    }

    private void RemoveFromStack(PanelName name)
    {
        if (openPanelsStack.Contains(name))
        {
          
            List<PanelName> temp = new List<PanelName>(openPanelsStack);
            temp.Remove(name);
            temp.Reverse();
            openPanelsStack = new Stack<PanelName>(temp);
        }
    }
}
