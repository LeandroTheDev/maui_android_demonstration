namespace Android_Native_Demonstration.Utils
{
    public class Storage
    {
        /// <summary>
        /// Save a ImageSource into a directory in device storage and,
        /// the directory needs to contain the file name
        /// return a string containing the result
        /// <para></para>Exception Permission Denied = the device is not allowed to save files in storage
        /// <para></para>Exception Operational System Incompatible =  the actual OS running is not compatible
        /// <para></para>Exception Image Corrupted = directory problem, probably a character incompatible
        /// <para></para>success = sucessfully saved the file in device
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        static async public Task<string> SaveImageSourceToDirectory(string directory, ImageSource imageSource)
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            //Permission Treatment
            if (status != PermissionStatus.Granted) { throw new Exception("Permission Denied"); }
            //Creating the directory to add the image
            var saveDirectory = CreateDirectory(directory);
            //Error Treatment
            if (saveDirectory == "operational_system_incompatible") { throw new Exception("Operational System Incompatible"); }
            //Getting image bytes
            var bytes = await GetImageBytesFromImageSource(imageSource) ?? throw new Exception("Image Corrupted");
            //Saving the file into directory            
            await File.WriteAllBytesAsync(saveDirectory, bytes);
            Console.WriteLine("[Storage]: Image Saved");
            return saveDirectory;
        }

        /// <summary>
        /// Creates the directory in device
        /// <para></para>You cannot place dots "." in directory
        /// only place in file extensions!!!
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        static public string CreateDirectory(string directory)
        {
            var formatedDirectory = "operational_system_incompatible";
#if ANDROID
            formatedDirectory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string[] directoryParts = directory.Split('/');
            var directoryWithFile = false;
            //Formatting the directory
            foreach (string part in directoryParts)
            {
                // If contains a dot this means is a file
                if (part.Contains('.'))
                {
                    //If is a file so create directory before combining the path
                    Directory.CreateDirectory(formatedDirectory);
                    Console.WriteLine("[Storage]: Directory created with file name provided");
                    directoryWithFile = true;
                    formatedDirectory = System.IO.Path.Combine(formatedDirectory, part);
                    continue;
                }
                formatedDirectory = System.IO.Path.Combine(formatedDirectory, part);
            }
            Console.WriteLine("[Storage]: Directory formated: " + formatedDirectory);

            //Check if directory has file
            //That means the directory is not yet created
            if (!directoryWithFile)
            {
                Directory.CreateDirectory(formatedDirectory);
                Console.WriteLine("[Storage]: Directory created without file name provided");
            }
#endif
            //Returning the directory
            return formatedDirectory;
        }

        static public string RemoveFile(string directory)
        {
            var formatedDirectory = "operational_system_incompatible";
#if ANDROID
            formatedDirectory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string[] directoryParts = directory.Split('/');
            //Adding the path
            foreach (string part in directoryParts)
            {
                formatedDirectory = System.IO.Path.Combine(formatedDirectory, part);
            }
            try
            {
                // Check if exist
                if (File.Exists(formatedDirectory))
                {
                    // Removing the file
                    File.Delete(formatedDirectory);
                    return "success";
                }
                else
                {
                    return "no_file_found";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[File] error while deleting the file: {ex}");
                formatedDirectory = "error_on_deleting";
            }
#endif
            return formatedDirectory;
        }

        /// <summary>
        /// Converts the ImageSource into Bytes,
        /// returns null if any error occurs
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        static public async Task<byte[]> GetImageBytesFromImageSource(ImageSource imageSource)
        {
            //ImageSource from a Stream
            if (imageSource is IStreamImageSource)
            {
                Stream stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
                byte[] bytesAvailable = new byte[stream.Length];
                stream.Read(bytesAvailable, 0, bytesAvailable.Length);

                return bytesAvailable;
            }
            //ImageSource from a File
            else if (imageSource is FileImageSource fileImageSource)
            {
                var filePath = fileImageSource.File;
                if (!string.IsNullOrEmpty(filePath))
                {
                    return await File.ReadAllBytesAsync(filePath);
                }
            }
            //ImageSource from a Path
            else if (imageSource is UriImageSource uriImageSource)
            {
                var uri = uriImageSource.Uri;
                if (uri != null)
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        return await client.GetByteArrayAsync(uri);
                    }
                }
            }
            throw new Exception("Invalid Type");
        }
    }
}
