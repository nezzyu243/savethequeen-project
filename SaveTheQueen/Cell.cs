using System.Runtime.CompilerServices;

namespace SaveTheQueen;

public class Cell
{
    public char Visuals;
    public Item? Item { get; private set; }

   public void Display()
    {
        if (Item != null)
        {
            Console.Write(Item.GetAvatar());
        }
        else
        {
            Console.Write(Visuals);
        }
    }

    public bool HasItem() => Item != null;

    public void PutItem(Item item)
    {
        Item = item;
    }

    public Item? TakeItem()
    {
        Item? item = Item;
        Item = null;
        return item;
    }

   public bool IsLockedDoor() => Visuals == '(' || Visuals == ')';

    public bool TryUnlock(Inventory inventory)
    {
        if (!IsLockedDoor()) return true;

        Item? key = inventory.FindItemWithEffect(ItemEffect.Key);
        if (key == null) return false;

        inventory.Remove(key);
        Visuals = '.';
        return true;
    }
    public void Leave()
{
    Item = null;
    Visuals = '.';
    Console.Write(Visuals);
}
}