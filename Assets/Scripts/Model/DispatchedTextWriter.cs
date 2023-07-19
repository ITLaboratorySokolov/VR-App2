using System.IO;
using System.Text;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Utility;

public class DispatchedTextWriter : TextWriter
{
    private readonly ConsoleController _writer;
    private readonly bool _err;

    public override Encoding Encoding => Encoding.UTF8;

    public DispatchedTextWriter(ConsoleController writer, bool err)
    {
        _writer = writer;
        _err = err;
    }

    public override void WriteLine(string value)
    {
        UnityDispatcher.ExecuteOnUnityThread(() =>
        {
            if (_err)
            {
                _writer.AppendErrorTextToConsole(value + "\n");
            }
            else
            {
                _writer.AppendTextToConsole(value + "\n");
            }
        });
    }

    public override void Write(string value)
    {
        UnityDispatcher.ExecuteOnUnityThread(() =>
        {
            if (_err)
            {
                _writer.AppendErrorTextToConsole(value);
            }
            else
            {
                _writer.AppendTextToConsole(value);
            }
        });
    }
}