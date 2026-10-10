
namespace Wizard5.Functionality.Component
{
    public class Renderer : IGameComponent
    {
        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Renderer";

        private Color _color;
        private Color _backgroundColor;
        private char _symbol;

        public Renderer(GameEntity owner, Color color, Color backgroundColor, char symbol)
        {
            Owner = owner;
            _color = color;
            _backgroundColor = backgroundColor;
            _symbol = symbol;
        }

        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Renderer(Owner,_color,_backgroundColor,_symbol);   
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
            mainSurface.SetForeground(position.X, position.Y, _color );
            mainSurface.SetBackground(position.X, position.Y, _backgroundColor);
            mainSurface.SetGlyph(position.X, position.Y, _symbol);

        }
    }
}
