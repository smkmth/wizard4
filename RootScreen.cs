using SadConsole.Input;

namespace Wizard5.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface _mapSurface;
    private ScreenSurface _uiSurface;
    private ScreenSurface _overlaySurface;
    private ScreenSurface _underlaySurface;
    private Game WizardGame;

    public RootScreen()
    {
        _mapSurface = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        _mapSurface.Position = new Point((GameSettings.SCREEN_WIDTH / 2) - GameSettings.GAME_WIDTH / 2,0 );

        _mapSurface.UseKeyboard = true;
        _mapSurface.UseMouse = true;
        _mapSurface.IsFocused = true;

        // Hook into the surface's own mouse processing
        _mapSurface.MouseMove += (sender, args) =>
        {
            if (_mapSurface.Surface.Area.Contains(args.SurfaceCellPosition))
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


        _underlaySurface = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        _underlaySurface.Position = new Point((GameSettings.SCREEN_WIDTH / 2) - GameSettings.GAME_WIDTH / 2, 0);

        _underlaySurface.Surface.Clear();

        _underlaySurface.UseMouse = false;
        _underlaySurface.UseKeyboard = false;



        _overlaySurface = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        _overlaySurface.Position = new Point((GameSettings.SCREEN_WIDTH / 2) - GameSettings.GAME_WIDTH / 2, 0);

        _overlaySurface.Surface.Clear();

        _overlaySurface.UseMouse = false;
        _overlaySurface.UseKeyboard = false;

        _underlaySurface.SortOrder = 1;
        _uiSurface.SortOrder = 1;
        _mapSurface.SortOrder = 2;
        _overlaySurface.SortOrder = 3;

        Children.Add(_underlaySurface);
        Children.Add(_uiSurface);
        Children.Add(_mapSurface);
        Children.Add(_overlaySurface);

        WizardGame = new Game(_mapSurface, _uiSurface, _overlaySurface, _underlaySurface);

    }

    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        var mainSurfaceMouseState = new MouseScreenObjectState(_mapSurface, state.Mouse);

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

        WizardGame.Draw(delta);
        
    }
}
