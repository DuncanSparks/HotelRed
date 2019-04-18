using Godot;
using System;

public class Item : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // refs
    [Export]
    private Sprite image = null;
    [Export]
    private string itemName = "Item Name";
    [Export]
    private string itemDescription = "Item Description";
    // Called when the node enters the scene tree for the first time.
    public Sprite Image { get { return image; } set { image = value; } }
    public string ItemName { get { return itemName; } set { itemName = value; } }
    public string ItemDescription { get { return itemDescription; } set { itemDescription = value; } }
    public override void _Ready()
    {
        GetNode<Sprite>("Image").SetTexture(image.Texture);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
