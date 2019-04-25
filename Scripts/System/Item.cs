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

}
