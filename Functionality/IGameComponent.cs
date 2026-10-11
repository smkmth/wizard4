using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5
{
    public interface IGameComponent
    {
        public GameEntity? Owner { get; set; }

        public string ComponentName { get; } 

        public void ReceiveEvent(GameEvent gameEvent);

        public IGameComponent Copy(GameEntity newOwner);

    }
}
