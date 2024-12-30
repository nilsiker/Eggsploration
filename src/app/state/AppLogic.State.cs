namespace Eggsploration;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State : StateLogic<State> { }
}
