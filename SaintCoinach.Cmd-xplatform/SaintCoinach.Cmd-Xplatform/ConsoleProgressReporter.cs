namespace SaintCoinach.Cmd_Xplatform;
public class ConsoleProgressReporter : IProgress<Ex.Relational.Update.UpdateProgress> {

    #region IProgress<UpdateProgress> Members

    public void Report(Ex.Relational.Update.UpdateProgress value) {
        Console.WriteLine(value);
    }

    #endregion
}