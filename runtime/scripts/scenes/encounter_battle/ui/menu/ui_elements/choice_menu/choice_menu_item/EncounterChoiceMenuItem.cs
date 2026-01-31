using Godot;
using System;

[GlobalClass]
public partial class EncounterChoiceMenuItem : HBoxContainer
{
	[Export]
	public string Text
	{
		get => text;
		set
		{
			text = value;
			DisplayTextTyper.Start($"[instant=true]* {value}");
		}
	}
	[Export]
	public bool ProgressVisible
	{
		get => ValueProgressBarMarginContainer.Visible;
		set
		{
			ValueProgressBarMarginContainer.Visible = value;
		}
	}

	[Export]
	public double ProgressMaxValue
	{
		get => ValueProgressBar.MaxValue;
		set
		{
			ValueProgressBar.MaxValue = value;
		}
	}

	[Export]
	public double ProgressValue
	{
		get => ValueProgressBar.Value;
		set
		{
			ValueProgressBar.Value = value;
		}
	}

	[Export]
	TextTyper DisplayTextTyper;
	[Export]
	MarginContainer ValueProgressBarMarginContainer;
	[Export]
	ProgressBar ValueProgressBar;
	[Export]
	Marker2D soulMarker;

	private string text = "";

	public Transform2D GetSoulTransform()
	{
		return soulMarker.GlobalTransform;
	}
}
