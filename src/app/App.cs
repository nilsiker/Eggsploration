namespace Eggsploration;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public partial class App : Node {
  #region State
  private AppLogic Logic { get; set; } = default!;
  private AppLogic.IBinding Binding { get; set; } = default!;
  #endregion
  #region Nodes
  private Button _newGameButton = default!;
  private Button _quitButton = default!;
  private Game _game = default!;
  #endregion

  public override void _Ready() {
    Logic = new AppLogic();
    Binding = Logic.Bind();
    _newGameButton = GetNode<Button>("%NewGameButton");
    _quitButton = GetNode<Button>("%QuitButton");
    _newGameButton.Pressed += OnNewGameButtonPressed;
    _quitButton.Pressed += OnQuitButtonPressed;
    // NEW BOILERPLATE UNLOCKED
    Binding
      .Handle((in AppLogic.Output.StartNewGame output) => OnOutputStartNewGame())
      .Handle((in AppLogic.Output.QuitApp output) => OnOutputQuitApp());
  }

  #region Input Handlers
  private void OnNewGameButtonPressed() => Logic.Input(new AppLogic.Input.NewGameClick());

  private void OnQuitButtonPressed() => Logic.Input(new AppLogic.Input.QuitClick());
  #endregion

  #region Output Handlers
  private void OnOutputQuitApp() => throw new NotImplementedException();

  private void OnOutputStartNewGame() => throw new NotImplementedException();
  #endregion
}

[Meta, LogicBlock(typeof(State), Diagram = true)]
public partial class AppLogic : LogicBlock<AppLogic.State> {
  public override Transition GetInitialState() => To<State.InMainMenu>();

  public static class Input {
    public record struct NewGameClick;

    public record struct QuitClick;
  }

  public static class Output {
    public record struct StartNewGame;

    public record struct QuitApp;
  }

  public abstract record State : StateLogic<State> {
    public record InMainMenu : State, IGet<Input.NewGameClick>, IGet<Input.QuitClick> {
      public Transition On(in Input.NewGameClick input) => To<InGame>();

      public Transition On(in Input.QuitClick input) => To<ClosingApplication>();
    }

    public record InGame : State {
      public InGame() {
        this.OnEnter(() => Output(new Output.StartNewGame()));
      }
    }

    public record ClosingApplication : State {
      public ClosingApplication() {
        this.OnEnter(() => Output(new Output.QuitApp()));
      }
    }
  }
}
