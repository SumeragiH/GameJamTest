using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacableSelectPanel : BasePanel
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private List<PlacableView> placablePrefabList = new();
    private VisualElement placablePanelRoot;

    private ScrollView placableList;
    private Button closeButton;

    private static readonly PlacableCategoryEnum[] CategoryOrder =
    {
        PlacableCategoryEnum.Building,
        PlacableCategoryEnum.TreasureBox,
        PlacableCategoryEnum.MonsterSpawner,
        PlacableCategoryEnum.ResourcePoint,
        PlacableCategoryEnum.MajorEvent,
    };

    protected override void Awake()
    {
        base.Awake();
        if (uiDocument == null || uiDocument.rootVisualElement == null)
        {
            return;
        }

        placableList = uiDocument.rootVisualElement.Q<ScrollView>("PlacableList");
        closeButton = uiDocument.rootVisualElement.Q<Button>("CloseButton");
        placablePanelRoot = uiDocument.rootVisualElement.Q<VisualElement>("PlacementPanel");
        if (closeButton != null)
        {
            closeButton.clicked += OnCloseButtonClicked;
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.clicked -= OnCloseButtonClicked;
        }
    }

    public override void Show(UnityEngine.Events.UnityAction show = null)
    {
        RefreshList();
        base.Show(show);
    }

    public void SetPosition(Vector2 position)
    {
        if (placablePanelRoot == null)
        {
            return;
        }

        placablePanelRoot.style.right = position.x;
        placablePanelRoot.style.bottom = position.y;
    }

    private void RefreshList()
    {
        if (placableList == null)
        {
            return;
        }

        placableList.Clear();

        Dictionary<PlacableCategoryEnum, List<PlacableView>> grouped = new();
        foreach (PlacableCategoryEnum category in CategoryOrder)
        {
            grouped[category] = new List<PlacableView>();
        }

        foreach (PlacableView prefab in placablePrefabList)
        {
            if (prefab == null)
            {
                continue;
            }

            if (!grouped.ContainsKey(prefab.category))
            {
                grouped[prefab.category] = new List<PlacableView>();
            }

            grouped[prefab.category].Add(prefab);
        }

        foreach (PlacableCategoryEnum category in CategoryOrder)
        {
            grouped.TryGetValue(category, out List<PlacableView> listInCategory);
            listInCategory ??= new List<PlacableView>();

            Label header = new Label(category.GetDescription());
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginTop = 6;
            header.style.marginBottom = 2;
            placableList.Add(header);

            if (listInCategory.Count == 0)
            {
                Label emptyLabel = new Label("暂无可放置项");
                emptyLabel.style.marginBottom = 4;
                placableList.Add(emptyLabel);
                continue;
            }

            foreach (PlacableView prefab in listInCategory)
            {
                Button button = new Button(() => OnPlacableClicked(prefab));
                button.text = $"{prefab.viewName} | 策划点：{prefab.designPointCost}";
                button.style.unityTextAlign = TextAnchor.MiddleLeft;
                button.style.marginBottom = 4;
                placableList.Add(button);
            }
        }
    }

    private void OnCloseButtonClicked()
    {
        UIMgr.Instance.HidePanel<PlacableSelectPanel>();
    }

    private void OnPlacableClicked(PlacableView placablePrefab)
    {
        if (placablePrefab == null)
        {
            return;
        }

        EventCenter.Instance.EventTrigger<PlacableView>("地块放置开始", placablePrefab);
        UIMgr.Instance.HidePanel<PlacableSelectPanel>();
    }
}
