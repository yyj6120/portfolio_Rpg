using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Handler
{
    public Transform defaultHandler;
    public List<Transform> customHandlers;
    public Handler()
    {
        customHandlers = new List<Transform>();
    }
}