static class GameSettings
{
    public const int SCREEN_HEIGHT = 50;
    public const int SCREEN_WIDTH = 50;


    public const int GAME_WIDTH = 20;
    public const int GAME_HEIGHT = 20;

    public static readonly Dictionary<string, Color> GameColors = new Dictionary<string, Color>()
    {
        { "Wall", Color.Black},
        { "Shadow", Color.Navy },
        { "Light", Color.LightYellow },
        { "Player", Color.OliveDrab },
        { "Knight", Color.Black}
    };



}
