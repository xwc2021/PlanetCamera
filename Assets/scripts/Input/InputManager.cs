public class InputManager
{
    static InputProxy inputProxy;
    public static InputProxy getInputProxy()
    {
        if (inputProxy != null)
            return inputProxy;

        inputProxy = new PCInput();
        return inputProxy;
    }
}