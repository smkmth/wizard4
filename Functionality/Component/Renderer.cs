
namespace Wizard5.Functionality.Component
{
    public class Renderer : IGameComponent
    {
        public GameEntity Owner { get; }

        public string ComponentName { get; } = "Renderer";

        private Color color;
        private Color backgroundColor;
        private char symbol;

        public Renderer(GameEntity owner, Color _color, Color _backgroundColor, char _symbol)
        {
            Owner = owner;
            this.color = _color;
            this.backgroundColor = _backgroundColor;
            this.symbol = _symbol;
        }

        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Renderer(Owner,color,backgroundColor,symbol);   
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_Draw draw)
            {
                Draw(draw.Surface);
            }
        }

        public void Draw(ScreenSurface mainSurface)
        {
            var position = (R_RequestPosition)Owner?.ResolveGameEvent(new GE_RequestPosition());
            mainSurface.SetForeground(position.X, position.Y, color );
            mainSurface.SetBackground(position.X, position.Y, backgroundColor);
            mainSurface.SetGlyph(position.X, position.Y, symbol);

        }
    }
}
