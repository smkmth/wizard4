using SadConsole.Input;

namespace Wizard5.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface _mainSurface;
    private Game WizardGame;

    public RootScreen()
    {
        _mainSurface = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

        _mainSurface.UseKeyboard = true;
 

        Children.Add(_mainSurface);

        WizardGame = new Game();

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

        _mainSurface.Clear();
        WizardGame.Draw(_mainSurface);
        
    }
}
