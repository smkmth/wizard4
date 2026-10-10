using SadConsole.Configuration;

Settings.WindowTitle = "My SadConsole Game";

Builder
    .GetBuilder()
    .SetWindowSizeInCells(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
    .SetStartingScreen<Wizard5.Scenes.RootScreen>()
    .IsStartingScreenFocused(true)
    .ConfigureFonts("Fonts/Cheepicus12.font")
    .Run();

