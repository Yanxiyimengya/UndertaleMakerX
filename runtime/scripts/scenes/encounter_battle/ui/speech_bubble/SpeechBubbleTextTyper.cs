using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class SpeechBubbleTextTyper : TextTyper
{
	private SpeechBubble _GetSpeechBubble()
	{
		return GetParent() as SpeechBubble;
	}

	public override bool _ProcessCmd(string cmd, Dictionary<string, string> args)
	{
		SpeechBubble bubble = _GetSpeechBubble();
		if (bubble != null)
		{
			if (cmd.Equals("spikeVisible", StringComparison.OrdinalIgnoreCase))
			{
				if ((args.TryGetValue("value", out string visibleValue)
					|| args.TryGetValue("visible", out visibleValue))
					&& bool.TryParse(visibleValue, out bool visible))
				{
					bubble.HideSpike = !visible;
				}
				else
				{
					bubble.HideSpike = !bubble.HideSpike;
				}
				return true;
			}

			if (cmd.Equals("dir", StringComparison.OrdinalIgnoreCase))
			{
				if ((args.TryGetValue("value", out string dirValue)
					|| args.TryGetValue("dir", out dirValue))
					&& TryParseDirection(dirValue, out int direction))
				{
					bubble.Dir = direction;
				}
				else
				{
					bubble.Dir = (bubble.Dir + 1) % 4;
				}
				return true;
			}
		}

		return base._ProcessCmd(cmd, args);
	}

	private static bool TryParseDirection(string value, out int dir)
	{
		dir = 0;
		if (string.IsNullOrWhiteSpace(value))
			return false;

		if (int.TryParse(value, out int parsed))
		{
			dir = ((parsed % 4) + 4) % 4;
			return true;
		}

		switch (value.Trim().ToLowerInvariant())
		{
			case "top":
				dir = 0;
				return true;
			case "bottom":
				dir = 1;
				return true;
			case "left":
				dir = 2;
				return true;
			case "right":
				dir = 3;
				return true;
			default:
				return false;
		}
	}

	public override void Destroy()
	{
		SpeechBubble bubble = _GetSpeechBubble();
		if (bubble != null)
		{
			bubble.Destroy();
			return;
		}

		base.Destroy();
	}
}
