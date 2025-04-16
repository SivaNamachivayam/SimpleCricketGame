using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject itemPrefab; // Assign a prefab for shop items
    public Transform batGrid, ballGrid, gemGrid; // Assign different section parents

    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public Sprite itemImage;
        public string itemType; // "Bat", "Ball", "Gem"
    }

    public List<ShopItem> shopItems = new List<ShopItem>();

    void Start()
    {
        PopulateShop();
    }

    void PopulateShop()
    {
        foreach (ShopItem item in shopItems)
        {
            Transform parentGrid = GetParentGrid(item.itemType);
            if (parentGrid != null)
            {
                GameObject newItem = Instantiate(itemPrefab, parentGrid);
                newItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
                newItem.transform.Find("Skin").GetComponent<Image>().sprite = item.itemImage;
                newItem.transform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(() => BuyItem(item));
            }
        }
    }

    Transform GetParentGrid(string itemType)
    {
        switch (itemType)
        {
            case "bat": return batGrid;
            case "Ball": return ballGrid;
            case "Gems": return gemGrid;
            default: return null;
        }
    }

    void BuyItem(ShopItem item)
    {
        Debug.Log("Bought: " + item.itemName);
        Debug.Log("Bought: " + item.itemType);

        // Add purchase logic here

        if (item.itemType == "Ball")
        {
           BallMaterial.SetTexture("_MainTex" +
               "", item.itemImage.texture);

        }
        else if (item.itemType == "bat")
        {
            BatMaterial.SetTexture("_MainTex", item.itemImage.texture);

        }
    }

    public Material BatMaterial;   
    public Material BallMaterial;     
    

}