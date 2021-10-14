public class InputManager
{
    static InputProxy inputProxy;
    public static InputProxy getInputProxy()
    {
#if (UNITY_ANDROID)
        inputProxy=new AndroidInput();
#else
        inputProxy = new PCInput();
#endif
        return inputProxy;
    }
}