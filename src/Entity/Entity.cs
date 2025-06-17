using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public partial class Entity : CharacterBody2D
{
    private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();

    public void AddComponent(Component component)
    {
        components.Add(component.GetType(), component);
    }
    
    public T GetComponent<T>() where T : Component {
        return (T) components[typeof(T)];
    }
}