namespace Eggsploration;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public record InMainMenu : State, IGet<Input.NewGameClick>, IGet<Input.QuitClick> {
      public InMainMenu() {
        this.OnEnter(() => {
          Output(new Output.UpdateMainMenuVisibility(true));
          Output(new Output.FadeIn());
        });
        this.OnExit(() => Output(new Output.UpdateMainMenuVisibility(false)));
      }

      public Transition On(in Input.NewGameClick input) =>
        To<FadingOut>()
          .With(state =>
            ((FadingOut)state).FadeOutFinishedAction = EFadeOutFinishedAction.StartGame
          );

      public Transition On(in Input.QuitClick input) =>
        To<FadingOut>()
          .With(state => ((FadingOut)state).FadeOutFinishedAction = EFadeOutFinishedAction.QuitApp);
    }
  }
}
