using System;
using System.Numerics;

namespace Metroidarium;

public class Command<T>(Action<T> pressedPressedCallback, Action<T> notPressedCallback = null)
{
    public Action<T> NotPressedCallback = notPressedCallback;
    public Action<T> PressedCallback = pressedPressedCallback;
}
