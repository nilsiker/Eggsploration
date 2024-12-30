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
  [Export]
  private PackedScene _gameScene = default!;
  private Button _newGameButton = default!;
  private Button _quitButton = default!;
  private Game? _game;
  #endregion

  public override void _Ready() {
    Logic = new AppLogic();
    Binding = Logic.Bind();

    _newGameButton = GetNode<Button>("%NewGameButton");
    _quitButton = GetNode<Button>("%QuitButton");
    _newGameButton.Pressed += OnNewGameButtonPressed;
    _quitButton.Pressed += OnQuitButtonPressed;

    Binding
      .Handle((in AppLogic.Output.StartNewGame output) => OnOutputStartNewGame())
      .Handle((in AppLogic.Output.RemoveGame output) => OnOutputRemoveGame())
      .Handle((in AppLogic.Output.QuitApp output) => OnOutputQuitApp())
      .Handle(
        (in AppLogic.Output.UpdateMainMenuVisibility output) =>
          OnOutputUpdateMainMenuVisibility(output.Visible)
      );
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed("ui_cancel")) {
      Logic.Input(new AppLogic.Input.RequestQuitGame());
    }
  }

  #region Input Handlers
  private void OnNewGameButtonPressed() => Logic.Input(new AppLogic.Input.NewGameClick());

  private void OnQuitButtonPressed() => Logic.Input(new AppLogic.Input.QuitClick());
  #endregion

  #region Output Handlers
  private void OnOutputStartNewGame() {
    _game = _gameScene.Instantiate<Game>();
    AddChild(_game);
  }

  private void OnOutputRemoveGame() {
    _game?.QueueFree();
    _game = default;
  }

  private void OnOutputQuitApp() => GetTree().Quit();

  private void OnOutputUpdateMainMenuVisibility(bool visible) =>
    GetNode<Control>("UI/MainMenu").Visible = visible;
  #endregion
}

[Meta, LogicBlock(typeof(State), Diagram = true)]
public partial class AppLogic : LogicBlock<AppLogic.State> {
  public override Transition GetInitialState() => To<State.InMainMenu>();

  public static class Input {
    public record struct NewGameClick;

    public record struct RequestQuitGame;

    public record struct QuitClick;
  }

  public static class Output {
    public record struct StartNewGame;

    public record struct RemoveGame;

    public record struct QuitApp;

    public record struct UpdateMainMenuVisibility(bool Visible);
  }

  public abstract record State : StateLogic<State> {
    public record InMainMenu : State, IGet<Input.NewGameClick>, IGet<Input.QuitClick> {
      public InMainMenu() {
        this.OnEnter(() => Output(new Output.UpdateMainMenuVisibility(true)));
        this.OnExit(() => Output(new Output.UpdateMainMenuVisibility(false)));
      }

      public Transition On(in Input.NewGameClick input) => To<InGame>();

      public Transition On(in Input.QuitClick input) => To<ClosingApplication>();
    }

    public record InGame : State, IGet<Input.RequestQuitGame> {
      public InGame() {
        this.OnEnter(() => Output(new Output.StartNewGame()));
        this.OnExit(() => Output(new Output.RemoveGame()));
      }

      public Transition On(in Input.RequestQuitGame input) => To<InMainMenu>();
    }

    public record ClosingApplication : State {
      public ClosingApplication() {
        this.OnEnter(() => Output(new Output.QuitApp()));
      }
    }
  }
}
