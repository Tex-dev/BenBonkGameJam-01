public static class OpenInFileBrowser
{
    [UnityEditor.MenuItem("Window/Test OpenInFileBrowser")]
    public static void Test()
    {
        Open(UnityEngine.Application.dataPath);
    }

    public static System.Diagnostics.Process OpenInWin(string path)
    {
        bool openInsidesOfFolder = false;

        // try windows
        string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

        if (System.IO.Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }

        try
        {
            return System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
        }
        catch (System.ComponentModel.Win32Exception e)
        {
            // tried to open win explorer in mac
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }

        return null;
    }

    public static System.Diagnostics.Process Open(string path)
    {
        return OpenInWin(path);
    }
}