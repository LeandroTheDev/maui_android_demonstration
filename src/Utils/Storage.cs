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
        static async public Task<string> Save_ImageSource_To_Directory(string directory, ImageSource imageSource)
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            //Permission Treatment
            if (status != PermissionStatus.Granted) { throw new Exception("Permission Denied"); }
            //Creating the directory to add the image
            var save_directory = Create_Directory(directory);
            //Error Treatment
            if (save_directory == "operational_system_incompatible") { throw new Exception("Operational System Incompatible"); }
            //Getting image bytes
            var bytes = await Get_Image_Bytes_From_ImageSource(imageSource) ?? throw new Exception("Image Corrupted");
            //Saving the file into directory            
            await File.WriteAllBytesAsync(save_directory, bytes);
            Console.WriteLine("[Storage]: Image Saved");
            return save_directory;
        }

        /// <summary>
        /// Creates the directory in device
        /// <para></para>You cannot place dots "." in directory
        /// only place in file extensions!!!
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        static public string Create_Directory(string directory)
        {
            var formated_directory = "operational_system_incompatible";
#if ANDROID
            formated_directory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string[] directory_parts = directory.Split('/');
            var directory_with_file = false;
            //Formatting the directory
            foreach (string part in directory_parts)
            {
                // If contains a dot this means is a file
                if (part.Contains('.'))
                {
                    //If is a file so create directory before combining the path
                    Directory.CreateDirectory(formated_directory);
                    Console.WriteLine("[Storage]: Directory created with file name provided");
                    directory_with_file = true;
                    formated_directory = System.IO.Path.Combine(formated_directory, part);
                    continue;
                }
                formated_directory = System.IO.Path.Combine(formated_directory, part);
            }
            Console.WriteLine("[Storage]: Directory formated: " + formated_directory);

            //Check if directory has file
            //That means the directory is not yet created
            if (!directory_with_file)
            {
                Directory.CreateDirectory(formated_directory);
                Console.WriteLine("[Storage]: Directory created without file name provided");
            }
#endif
            //Returning the directory
            return formated_directory;
        }

        static public string Remove_File(string directory)
        {
            var formated_directory = "operational_system_incompatible";
#if ANDROID
            formated_directory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string[] directory_parts = directory.Split('/');
            //Adding the path
            foreach (string part in directory_parts)
            {
                formated_directory = System.IO.Path.Combine(formated_directory, part);
            }
            try
            {
                // Check if exist
                if (File.Exists(formated_directory))
                {
                    // Removing the file
                    File.Delete(formated_directory);
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
                formated_directory = "error_on_deleting";
            }
#endif
            return formated_directory;
        }

        /// <summary>
        /// Converts the ImageSource into Bytes,
        /// returns null if any error occurs
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        static public async Task<byte[]> Get_Image_Bytes_From_ImageSource(ImageSource imageSource)
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
