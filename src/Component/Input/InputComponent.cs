using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public class InputComponent : Component
{
    enum VectorIndexes
    {
        NegativeX=0,
        PositiveX=1,
        NegativeY=2,
        PositiveY=3,
    }
    
    private Dictionary<string, Command<object>> commands = new Dictionary<string, Command<object>>();
    private Dictionary<List<string>, Command<Vector2>> vectorCommands = new Dictionary<List<string>, Command<Vector2>>();
    public bool InputEnabled = true;
    
    public void AddCommand(string name, Action<object> pressedCallback, Action<object> notPressedCallback = null)
    {
        commands.Add(name, new Command<object>(pressedCallback, notPressedCallback));
    }

    public void AddVectorCommand(string negativeX, string positiveX, string negativeY, string positiveY, Action<Vector2> pressedCallback, Action<Vector2> notPressedCallback = null)
    {
        vectorCommands.Add([negativeX, positiveX, negativeY, positiveY], new Command<Vector2>(pressedCallback, notPressedCallback));
    }

    public void Update()
    {
        foreach (var command in commands)
        {
            if (InputEnabled && Input.IsActionPressed(command.Key))
            {
                GD.Print(command.Key);
                command.Value.PressedCallback.Invoke(null);
            }
            else
            {
                command.Value.NotPressedCallback?.Invoke(null);
            }
        }
        
        foreach (var command in vectorCommands)
        {
            Vector2 input = Input.GetVector(
                command.Key[(int)VectorIndexes.NegativeX],
                command.Key[(int)VectorIndexes.PositiveX],
                command.Key[(int)VectorIndexes.NegativeY],
                command.Key[(int)VectorIndexes.PositiveY]);

            if (InputEnabled && input != Vector2.Zero)
            {
                command.Value.PressedCallback.Invoke(input);
            }
            else
            {
                command.Value.NotPressedCallback?.Invoke(input);
            }
            
        }
    }
}