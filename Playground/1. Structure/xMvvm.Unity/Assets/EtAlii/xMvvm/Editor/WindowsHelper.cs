namespace EtAlii.xMvvm
{
    using UnityEditor;

    public class WindowsHelper 
    {
        public static void GiveConsoleWindowFocus()
        {
            var consoleWindowType = typeof(SceneView).Assembly.GetType("UnityEditor.ConsoleWindow", throwOnError: false);
            if (consoleWindowType == null) return;
            
            var consoleWindow = EditorWindow.GetWindow(consoleWindowType);
            consoleWindow.Focus();
        }
    }
}
