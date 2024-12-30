namespace Eggsploration;

using Chickensoft.LogicBlocks;

public partial class AppLogic
{
    public abstract partial record State
    {
        public record ClosingApplication : State
        {
            public ClosingApplication()
            {
                this.OnEnter(() => Output(new Output.QuitApp()));
            }
        }
    }
}
