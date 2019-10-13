namespace Raven.Tests.Commands
{
    [Command("subcommandstest", "", "")]
    public class SubcommandsTestCommand
    {
        public string Arg1;
        public int Arg2;
        public string Arg3;

        [Subcommand("sub1", "", "")]
        public void Sub1(string arg1, [DefaultValue("")] string arg2)
        {
            Arg1 = arg1;
            Arg3 = arg2;
        }

        [Subcommand("sub2", "", "")]
        public void Sub2(string arg1, [DefaultValue(5)] int arg2, [DefaultValue("")] string arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }
    }
}