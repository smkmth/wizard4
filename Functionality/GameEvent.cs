using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Wizard5;
using Wizard5.Functionality;

namespace Wizard5
{
    public class GameEvent
    {
        public EventResponse? Response { get; set; }
    }

    public class EventResponse
    {
    }

    public class GE_FinishAction : GameEvent
    {
        public Point oldPos;
        public Point newPos;

        public GE_FinishAction(Point oldPos, Point newPos)
        {
            this.oldPos = oldPos;
            this.newPos = newPos;
        }

        public bool moved
        {
            get
            {
                return oldPos != newPos;
            }
        }
    }

    public class GE_TurnEnd : GameEvent
    {
    }

    public class GE_RequestPosition : GameEvent
    {
    }

    public class R_RequestPosition : EventResponse
    {
        public int X { get; set; }
        public int Y { get; set; }

    }

    public class GE_SetPosition : GameEvent
    {
        public int X;
        public int Y;

        public GE_SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class GE_Move : GameEvent
    {
        public int DeltaX;
        public int DeltaY;

        public GE_Move(int deltaX, int deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }

    public class GE_Draw : GameEvent
    {
        public ScreenSurface Surface;

        public float Light;

        public GE_Draw(ScreenSurface surface)
        {
            Surface = surface;
        }
    }
    public class GE_ChangeRenderSymbol : GameEvent
    {
        public char symbol;

        public GE_ChangeRenderSymbol(char symbol)
        {
            this.symbol = symbol;
        }
    }

    public class GE_UnlockDoor : GameEvent
    {
    }

    public class R_UnlockDoor : EventResponse
    {
        public bool Success;
        public R_UnlockDoor(bool success)
        {
            Success = success;
        }
    }

    public class GE_ChangeRenderColor : GameEvent
    {
        public Color color;
        public Color bgcolor;

        public GE_ChangeRenderColor(Color color, Color bgcolor)
        {
            this.color = color;
            this.bgcolor = bgcolor;
        }
    }

    public class R_ChangeRenderColor : EventResponse
    {
        public Color oldColor;

        public R_ChangeRenderColor(Color oldColor)
        {
            this.oldColor = oldColor;
        }
    }

    public class GE_PlayerUnPossess : GameEvent
    {

    }

    public class GE_Collide : GameEvent
    {
        public GameEntity movingEntity; //all entity collisions have an entity that is moving into the collision

        public GE_Collide(GameEntity movingEntity)
        {
            this.movingEntity = movingEntity;
        }
    }

    public class R_CollideResult : EventResponse
    {
        public bool canMove = false;

        public R_CollideResult(bool canMove)
        {
            this.canMove = canMove;
        }
    }

    public class GE_TakeDamage : GameEvent
    {
        public int damageAmount;

        public GE_TakeDamage(int _damageAmount)
        {
            this.damageAmount = _damageAmount;
        }
    }

    public class GE_FormAttack : GameEvent
    {
        public GameEntity Target;

        public GE_FormAttack(GameEntity target)
        {
            Target = target;
        }
    }

    public class R_FormAttackResult : EventResponse
    {
        public int damage;

        public R_FormAttackResult(int damage)
        {
            this.damage = damage;
        }
    }

    public class GE_GameStart : GameEvent
    {

    }

    public class GE_PlayerPossess : GameEvent
    {
        public GameEntity Player;
        public GE_PlayerPossess(GameEntity player)
        {
            Player = player;
        }
    }

    public class R_RegisterAbility : EventResponse
    {
        public List<(string, AbilityType)> AbilityContainers = new List<(string, AbilityType)>();

    }

    public class GE_RegisterAbilities : GameEvent
    {
        public GameEvent AbilityTriggerEvent;
    }

    public class GE_TriggerAbilityKey : GameEvent
    {
        public char AbilityKey;

        //we dont know what the ability needs - just add everything
        public GameEntity? Target;
        public Point TargetPoint;

        public GE_TriggerAbilityKey(char abilityKey)
        {
            AbilityKey = abilityKey;
        }
    }

    public class GE_TriggerAbility : GameEvent
    {
        public string AbilityName;

        public GE_TriggerAbility(string abilityName)
        {
            AbilityName = abilityName;
        }
    }


    public class GE_TriggerAbilityTargetEntity : GE_TriggerAbility
    {
        public string AbilityName;

        public GameEntity Target;

        public GE_TriggerAbilityTargetEntity(GameEntity target, string abilityName) : base(abilityName)
        {
            Target = target;
        }
    }


    public class GE_TriggerMindControl : GameEvent
    {

    }

    public class GE_PossessEntity : GameEvent
    {
        public GameEntity? Controller;
        public GE_PossessEntity(GameEntity? controller)
        {
            Controller = controller;
        }
    }

    public class GE_Death : GameEvent
    {

    }
    public class GE_SetOpaque : GameEvent
    {
        public bool Transparent;

        public GE_SetOpaque(bool value)
        {
            Transparent = value;
        }
    }


    public class GE_CheckOpaque : GameEvent
    {
        public bool StaticCheck;

        public GE_CheckOpaque(bool staticCheck)
        {
            StaticCheck = staticCheck;
        }
    }

    public class R_Opaque : EventResponse
    {
        public bool isOpaque;

        public R_Opaque(bool isOpaque)
        {
            this.isOpaque = isOpaque;
        }
    }

    public class GE_SomthingMoved : GameEvent
    {
        public GameEntity Mover;

        public GE_SomthingMoved(GameEntity mover)
        {
            Mover = mover;
        }
    }

    public class GE_CheckSight : GameEvent
    {

    }

}
