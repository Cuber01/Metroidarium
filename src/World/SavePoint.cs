using System;
using Godot;

namespace Metroidarium;

public partial class SavePoint : Area2D
{
    private void _onPlayerEntered(Node player)
    {
        ((SnakeHead)player).OnInteractedEvent += rest;
    }

    private void _onPlayerExited(Node player)
    {
        ((SnakeHead)player).OnInteractedEvent -= rest;
    }

    private void rest(SnakeHead player)
    {
        GD.Print(player.AmountOfTail - player.CurrentAmountOfTail);
        player.growTail(player.AmountOfTail - player.CurrentAmountOfTail);
    }
}