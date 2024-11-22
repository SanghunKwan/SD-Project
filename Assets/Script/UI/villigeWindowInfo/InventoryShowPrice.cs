using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class PriceNode
{
    public GameObject gameObject { get; set; }
    Text text;
    public PriceNode(Transform transform)
    {
        gameObject = transform.gameObject;
        text = transform.GetChild(1).GetComponent<Text>();
    }
    public void ChangePrice(int newPrice)
    {
        text.text = newPrice.ToString("N0");
    }
}
public class InventoryShowPrice : InitObject
{
    InventoryComponent inventoryComponent;
    PriceNode priceNode;
    public override void Init()
    {
        inventoryComponent = transform.parent.parent.GetComponent<InventoryComponent>();
        priceNode = new PriceNode(transform.GetChild(1));
    }
    public void SetShow(bool onoff)
    {
        priceNode.gameObject.SetActive(onoff);
    }
    public void ChangePrice(int newPrice)
    {
        priceNode.ChangePrice(newPrice);
    }

}
