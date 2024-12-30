namespace Eggsploration;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public record InGame : State, IGet<Input.RequestQuitGame> {
      public InGame() {
        this.OnEnter(() => {
          Output(new Output.StartNewGame());
          Output(new Output.FadeIn());
        });
        this.OnExit(() => Output(new Output.RemoveGame()));
      }

      public Transition On(in Input.RequestQuitGame input) =>
        To<FadingOut>()
          .With(state =>
            ((FadingOut)state).FadeOutFinishedAction = EFadeOutFinishedAction.BackToMainMenu
          );
    }
  }
}
