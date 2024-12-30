namespace Eggsploration;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public enum EFadeOutFinishedAction {
    StartGame,
    BackToMainMenu,
    QuitApp,
  }

  public abstract partial record State {
    public EFadeOutFinishedAction FadeOutFinishedAction { get; set; }

    public record FadingOut : State, IGet<Input.FadeOutFinished> {
      public FadingOut() {
        this.OnEnter(() => Output(new Output.FadeOut()));
      }

      public Transition On(in Input.FadeOutFinished input) =>
        FadeOutFinishedAction switch {
          EFadeOutFinishedAction.StartGame => To<InGame>(),
          EFadeOutFinishedAction.BackToMainMenu => To<InMainMenu>(),
          EFadeOutFinishedAction.QuitApp => To<ClosingApplication>(),
          _ => throw new System.NotImplementedException(),
        };
    }
  }
}
