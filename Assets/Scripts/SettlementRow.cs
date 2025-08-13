using UnityEngine;
using UnityEngine.UI;

public class SettlementRow : MonoBehaviour
{
    [SerializeField] Text txtName;
    [SerializeField] Text txtPrice;
    [SerializeField] Text txtCount;
    [SerializeField] Text txtSubtotal;

    public void Bind(string name, int price, int count, int sum)
    {
        if (txtName)     txtName.text     = name;
        if (txtPrice)    txtPrice.text    = price.ToString();
        if (txtCount)    txtCount.text    = count.ToString();
        if (txtSubtotal) txtSubtotal.text = sum.ToString();
    }
}