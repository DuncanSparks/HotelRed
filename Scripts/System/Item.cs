using Godot;
using System;

public class Item : TextureButton
{
    // [Export]
    // private Texture itemTexture;

    [Export]
    private string itemName = "Item Name";

    [Export]
    private string itemDescription = "Item Description";

    public string ItemName { get { return itemName; } set { itemName = value; } }
    public string ItemDescription { get { return itemDescription; } set { itemDescription = value; } }

    public override void _Ready()
    {
      // SetNormalTexture(itemTexture);
    }

    public void _on_Item_mouse_entered()
    {
      Player.Inventory.GetNode<Label>("CurrentItem").Text = itemName;
      Player.Inventory.GetNode<Label>("CurrentDescription").Text = itemDescription;
    }

    public void _on_Item_mouse_exited()
    {
      Player.Inventory.GetNode<Label>("CurrentItem").Text = string.Empty;
      Player.Inventory.GetNode<Label>("CurrentDescription").Text = string.Empty;
    }
}
