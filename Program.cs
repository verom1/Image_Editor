namespace ImageEditor;

using ImageEditor.Core.Facade;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(new ImageEditorFacade()));
    }
}