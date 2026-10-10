namespace Wizard5.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface _mainSurface;
    private Game WizardGame;

    public RootScreen()
    {
        SadConsole.Components.Cursor cursor = new();
        _mainSurface = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

        _mainSurface.UseKeyboard = true;
        _mainSurface.SadComponents.Add(cursor);
        cursor.IsEnabled = true;

        Children.Add(_mainSurface);

        WizardGame = new Game();

    }

    public override void Update(TimeSpan delta)
    {
        // Base update processes keyboard, mouse, and child components
        base.Update(delta);

        _mainSurface.Clear();
        WizardGame.Draw(_mainSurface);
        
    }
}
