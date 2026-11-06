using System;
using UnityEngine;

namespace UNodes.Project
{
    public class HolderRegistrator : BaseHolderRegistrator
    {
        public override void RegisterItems(Action onRegistered)
        {
            
            onRegistered?.Invoke();
        }
    }
}
