namespace Eggsploration;

using System;
using Godot;

public partial class App : Node {
  #region State
  private AppLogic Logic { get; set; } = default!;
  private AppLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Nodes
  [Export]
  private PackedScene _gameScene = default!;
  private AnimationPlayer _animPlayer = default!;
  private Button _newGameButton = default!;
  private Button _quitButton = default!;
  private Game? _game;
  #endregion

  public override void _Ready() {
    Logic = new AppLogic();
    Binding = Logic.Bind();

    _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    _newGameButton = GetNode<Button>("%NewGameButton");
    _quitButton = GetNode<Button>("%QuitButton");

    _animPlayer.AnimationFinished += OnAnimationPlayerAnimationFinished;
    _newGameButton.Pressed += OnNewGameButtonPressed;
    _quitButton.Pressed += OnQuitButtonPressed;

    Binding
      .Handle((in AppLogic.Output.StartNewGame output) => OnOutputStartNewGame())
      .Handle((in AppLogic.Output.RemoveGame output) => OnOutputRemoveGame())
      .Handle((in AppLogic.Output.QuitApp output) => OnOutputQuitApp())
      .Handle(
        (in AppLogic.Output.UpdateMainMenuVisibility output) =>
          OnOutputUpdateMainMenuVisibility(output.Visible)
      )
      .Handle((in AppLogic.Output.FadeOut output) => OnOutputFadeOut())
      .Handle((in AppLogic.Output.FadeIn output) => OnOutputFadeIn());
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed("ui_cancel")) {
      Logic.Input(new AppLogic.Input.RequestQuitGame());
    }
  }

  #region Input Handlers
  private void OnAnimationPlayerAnimationFinished(StringName animation) {
    if (animation == "fade_out") {
      Logic.Input(new AppLogic.Input.FadeOutFinished());
    }
  }

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

  private void OnOutputFadeOut() => _animPlayer.Play("fade_out");

  private void OnOutputFadeIn() => _animPlayer.Play("fade_in");

  #endregion
}
