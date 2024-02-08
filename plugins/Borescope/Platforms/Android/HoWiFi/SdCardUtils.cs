using AndroidX.Core.App;
using Java.IO;
using Java.Lang;
using Java.Util;
namespace HoWiFi;
public class SdCardUtils
{
    public static string getSecondExternPath()
    {
        ArrayList allExterSdcardPath = getAllExterSdcardPath();
        if (allExterSdcardPath.Size() != 2)
        {
            return null;
        }
        for (int i = 0; i < allExterSdcardPath.Size(); i++)
        {
            string str = allExterSdcardPath.Get(i).ToString();
            if (str != null && !str.Equals(getFirstExternPath()))
            {
                return str;
            }
        }
        return null;
    }

    public static string getFirstExternPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    }

    public static ArrayList getAllExterSdcardPath()
    {
        ArrayList arrayList = new ArrayList();
        string firstExternPath = getFirstExternPath();
        try
        {
            BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(Runtime.GetRuntime().Exec("mount").InputStream));
            while (true)
            {
                string readLine = bufferedReader.ReadLine();
                if (readLine == null)
                {
                    break;
                }
                else if (!(readLine.Contains("secure") || readLine.Contains("asec") || readLine.Contains("media") || readLine.Contains("system") || readLine.Contains(NotificationCompat.CategorySystem) || readLine.Contains("data") || readLine.Contains("tmpfs") || readLine.Contains("shell") || readLine.Contains("root") || readLine.Contains("acct") || readLine.Contains("proc") || readLine.Contains("misc") || readLine.Contains("obb")))
                {
                    if (readLine.Contains("fat") || readLine.Contains("fuse") || readLine.Contains("ntfs"))
                    {
                        string[] split = readLine.Split(' ');
                        if (split != null && split.Length > 1)
                        {
                            string toLowerCase = split[1].ToLower();
                            if (!(toLowerCase == null || arrayList.Contains(toLowerCase) || !toLowerCase.Contains("sd")))
                            {
                                arrayList.Add(split[1]);
                            }
                        }
                    }
                }
            }
        }
        catch (Java.Lang.Exception e)
        {
            e.PrintStackTrace();
        }
        if (!arrayList.Contains(firstExternPath))
        {
            arrayList.Add(firstExternPath);
        }
        return arrayList;
    }
}