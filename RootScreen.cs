using SadConsole.Input;

namespace Wizard5.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface _gameSurface;
    private ScreenSurface _uiSurface;
    private Game WizardGame;

    public RootScreen()
    {
        _gameSurface = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        _gameSurface.Position = new Point((GameSettings.SCREEN_WIDTH / 2) - GameSettings.GAME_WIDTH / 2,0 );

        _gameSurface.UseKeyboard = true;
        _gameSurface.UseMouse = true;
        _gameSurface.IsFocused = true;

        // Hook into the surface's own mouse processing
        _gameSurface.MouseMove += (sender, args) =>
        {
            if (_gameSurface.Surface.Area.Contains(args.SurfaceCellPosition))
            {
                WizardGame.ProcessMouse(args);
            }
     
        };

        _uiSurface = new(GameSettings.SCREEN_WIDTH, GameSettings.SCREEN_HEIGHT - GameSettings.GAME_HEIGHT);

        _uiSurface.Position = new Point(0, GameSettings.GAME_HEIGHT);

        _uiSurface.MouseMove += (sender, args) =>
        {
            if (_uiSurface.Surface.Area.Contains(args.SurfaceCellPosition))
            {
                WizardGame.ProcessUIMouse(args);
            }

        };

        Children.Add(_uiSurface);
        Children.Add(_gameSurface);

        WizardGame = new Game();

    }

    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        var mainSurfaceMouseState = new MouseScreenObjectState(_gameSurface, state.Mouse);

        WizardGame.ProcessMouse(mainSurfaceMouseState);
        return base.ProcessMouse(state);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {

        WizardGame.ProcessKeyboard(keyboard);
        return base.ProcessKeyboard(keyboard);
    }

    public override void Update(TimeSpan delta)
    {
        // Base update processes keyboard, mouse, and child components
        base.Update(delta);

        WizardGame.Draw(_gameSurface, _uiSurface);
        
    }
}
