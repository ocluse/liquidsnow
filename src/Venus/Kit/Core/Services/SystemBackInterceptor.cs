namespace Ocluse.LiquidSnow.Venus.Kit.Services;

public class SystemBackInterceptor
{
    private readonly List<ISystemBackReceiver> _receivers = [];

    public bool OnBackButtonPressed()
    {
        if (_receivers.Count == 0)
        {
            return false;
        }
        var receiver = _receivers[^1];
        return receiver.HandleBackPressed();
    }

    public void Bind(ISystemBackReceiver receiver)
    {
        _receivers.Add(receiver);
    }

    public void Unbind(ISystemBackReceiver receiver)
    {
        _receivers.Remove(receiver);
    }
}