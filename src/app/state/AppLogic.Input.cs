namespace Eggsploration;

public partial class AppLogic
{
    public static class Input
    {
        public record struct NewGameClick;

        public record struct RequestQuitGame;

        public record struct QuitClick;
    }
}
