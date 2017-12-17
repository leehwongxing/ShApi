using System;
using System.IO;

namespace API.Configs
{
    public class Resource
    {
        public string Folder { get; set; }

        public Resource()
        {
            Folder = "UploadedResources";

            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
        }

        public FileStream CreateWriteStream(string FileName)
        {
            if (string.IsNullOrWhiteSpace(FileName))
            {
                throw new Exception("FileName can't empty");
            }

            if (File.Exists(Path.Combine(Folder, FileName)))
            {
                throw new Exception("Can't write on existed file");
            }

            return new FileStream(Path.Combine(Folder, FileName), FileMode.Create, FileAccess.Write, FileShare.None, 10240);
        }

        public FileStream CreateReadStream(string FileName)
        {
            if (string.IsNullOrWhiteSpace(FileName))
            {
                throw new Exception("FileName can't empty");
            }

            if (!File.Exists(Path.Combine(Folder, FileName)))
            {
                throw new Exception("Can't read on unexisted file");
            }

            return new FileStream(Path.Combine(Folder, FileName), FileMode.Open, FileAccess.Read, FileShare.Read, 10240);
        }

        public bool Exist(string FileName)
        {
            if (string.IsNullOrWhiteSpace(FileName))
            {
                return false;
            }

            if (!File.Exists(Path.Combine(Folder, FileName)))
            {
                return false;
            }

            return true;
        }

        public bool Delete(string FileName)
        {
            try
            {
                File.Delete(Path.Combine(Folder, FileName));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}