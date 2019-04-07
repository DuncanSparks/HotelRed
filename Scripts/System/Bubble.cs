using Godot;
using System;

public class Bubble : Sprite
{
    public void PlayAnimation(string animation)
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Play(animation);
	}

	// ================================================================

	private void AnimFinished(string anim_name)
	{
		QueueFree();
	}
}
