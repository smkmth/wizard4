
namespace Wizard5.Functionality.Component
{
    public class Renderer : IGameComponent
    {
        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Renderer";

        private Color color;
        private Color backgroundColor;
        private char symbol;

        public Renderer(GameEntity owner, Color color, Color backgroundColor, char symbol)
        {
            Owner = owner;
            this.color = color;
            this.backgroundColor = backgroundColor;
            this.symbol = symbol;
        }

        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Renderer(newOwner, color,backgroundColor,symbol);   
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_Draw draw)
            {
                Draw(draw.Surface);
            }
            if (gameEvent is GE_ChangeRenderSymbol symbol)
            {
                this.symbol = symbol.symbol;
            }
            if (gameEvent is GE_ChangeRenderColor color)
            {
                this.color = color.color;
                this.backgroundColor= color.bgcolor;
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
