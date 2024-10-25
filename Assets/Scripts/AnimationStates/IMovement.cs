using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    public void Walk(Vector2 direction);
    public void Run(Vector2 direction);
    public void Jump();
    public void Roll(Vector2 direction);
    public void Stay();
}