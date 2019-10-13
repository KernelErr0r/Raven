namespace Raven.Tests.Commands
{
    [Command("optionalparameterstest", "", "")]
    public class OptionalParametersTestCommand
    {
        [Subcommand("sub1", "", "")]
        public void Sub1(string arg1, [DefaultValue("")] string arg2)
        {
            
        }

        [Subcommand("sub2", "", "")]
        public void Sub2(string arg1, [DefaultValue(5)] int arg2, [DefaultValue("")] string arg3)
        {
            
        }
    }
}