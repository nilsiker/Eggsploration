namespace Eggsploration;

public partial class AppLogic {
  public static class Output {
    public record struct StartNewGame;

    public record struct RemoveGame;

    public record struct FadeOut;

    public record struct FadeIn;

    public record struct QuitApp;

    public record struct UpdateMainMenuVisibility(bool Visible);
  }
}
