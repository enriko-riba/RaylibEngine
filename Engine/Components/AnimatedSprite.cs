namespace RaylibEngine.Components;

using RaylibEngine.Core;
using Raylib_CsLo;

/// <summary>
/// 2D sprite with textures atlas containing multiple frames drawn in a sequence.
/// </summary>
public class AnimatedSprite : Sprite, IUpdateable
{
	private float accumulator;
	private int frameIndex = -1;
	private string? currentSequence;
	private readonly Action? onUpdateAction;
	private readonly Action? onCompleteAction;
	private readonly Dictionary<string, Rectangle[]> animationSequences = new();

	public AnimatedSprite(Texture texture) : this(texture, null, null) { }
	public AnimatedSprite(Texture texture, Action? onComplete) : this(texture, onComplete, null) { }
	public AnimatedSprite(Texture texture, Action? onComplete, Action? onUpdate) : base(texture)
	{
		onUpdateAction = onUpdate;
		onCompleteAction = onComplete;
	}

	public int Fps { get; set; } = 4;

	public bool IsLooping { get; set; }

	public bool IsPlaying => !string.IsNullOrEmpty(currentSequence);

	public void Update(float ellapsedSeconds)
	{
		if (IsPlaying)
		{
			accumulator += ellapsedSeconds;
			var secForFrame = 1f / Fps;
			if (accumulator >= secForFrame)
			{
				accumulator -= secForFrame;
				frameIndex++;
				var frames = animationSequences[currentSequence!];
				if (onUpdateAction is not null) onUpdateAction();
				if (frameIndex == frames.Length)
				{
					frameIndex = 0;

					//  end the animation if not looping
					if (!IsLooping)
					{
						currentSequence = null;
						if (onCompleteAction is not null) onCompleteAction();
					}
				}
			}
		}
	}

	public override void Draw()
	{
		if (!string.IsNullOrWhiteSpace(currentSequence))
		{
			var frames = animationSequences[currentSequence];
			Frame = frames[Math.Max(0, frameIndex)];
			base.Draw();
		}
	}

	public void AddAnimation(string animationName, Rectangle[] frames)
	{
		animationSequences.Add(animationName, frames);
	}

	public void Stop()
	{
		currentSequence = null;
		accumulator = 0;
		frameIndex = -1;
	}

	public void Play(string animationName, int? fps, bool loop = true)
	{
		if (!string.IsNullOrWhiteSpace(currentSequence) || currentSequence != animationName)
		{
			Stop();
		}

		if (!string.IsNullOrWhiteSpace(animationName) && animationSequences.ContainsKey(animationName))
		{
			currentSequence = animationName;
			Fps = fps ?? Fps;
			IsLooping = loop;
		}
	}
}
