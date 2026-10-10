
namespace Wizard5.Functionality.Component
{
    public class Renderer : IGameComponent
    {
        public GameEntity? Owner { get; set; }

        public string ComponentName { get; } = "Renderer";

        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Renderer { Owner = newOwner };   
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {

        }

        public void Draw(ScreenSurface mainSurface)
        {
            var position = Owner.ResolveGameEvent(new GE_RequestPosition());

            mainSurface.SetForeground(Owner.transform.x, Owner.transform.y, _color * lightVal);
            mainSurface.SetBackground(Owner.transform.x, Owner.transform.y, BackgroundColor * lightVal);
            mainSurface.SetGlyph(Owner.transform.x, Owner.transform.y, Symbol);

        }
    }
}
