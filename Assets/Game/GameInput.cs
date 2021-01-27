using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInput
{
	public enum Action
	{
		None,
		Left,
		Right,
		Up,
		Down,
		Confirm,
		Cancel
	}

	public static Action GetAction()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad1))
			return Action.Left;
		else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad3))
			return Action.Right;
		else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad5))
			return Action.Up;
		else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2))
			return Action.Down;
		else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			return Action.Confirm;
		else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Keypad0))
			return Action.Cancel;
		else
			return Action.None;
	}
	public static bool TryGetAction(out Action action)
	{
		action = GetAction();
		return action != Action.None;
	}
}
