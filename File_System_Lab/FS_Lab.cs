using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DokanNet;


namespace File_System_Lab
{


    class FS_Lab : IDokanOperations
    {
        private NTree_Lab NTFSLikeTree = new NTree_Lab(new F_Lab("", new byte[0], FileAttributes.Directory));
        int totalSpace = 512000000;
        int freeSpace = 512000000;


        public FS_Lab(char diskLetter)
        {
            this.Mount(diskLetter + ":\\", DokanOptions.DebugMode | DokanOptions.StderrOutput);
        }


        public void Cleanup(string fileName, DokanFileInfo info)
        {
            //
        }

        public void CloseFile(string fileName, DokanFileInfo info)
        {
            //
        }

        public NtStatus CreateFile(string fileName, DokanNet.FileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes, DokanFileInfo info)
        {
            if (fileName == "\\")
                return NtStatus.Success;
            String[] name = fileName.Split("\\\\".ToCharArray());
            if (name[name.Length - 1].Length > 25) return NtStatus.Error;
            if (name.Length > 12) return NtStatus.Error;

            


            if (access == DokanNet.FileAccess.ReadAttributes && mode == FileMode.Open)
                return NtStatus.Success;

            if (mode == FileMode.CreateNew)
            {
                if (attributes == FileAttributes.Directory || info.IsDirectory)
                {
                    String[] path = fileName.Split("\\\\".ToCharArray());
                    String folderPath = fileName.Remove(fileName.Length - path[path.Length - 1].Length);

                    if (folderPath != "\\")
                        folderPath = folderPath.Remove(folderPath.Length - 1, 1);

                    NTree_Lab currentFolder = NTFSLikeTree.FindChild(folderPath, NTFSLikeTree);

                    F_Lab newItem = new F_Lab(path[path.Length - 1], new byte[0], FileAttributes.Directory);
                    newItem.DateCreated = newItem.DateModified = DateTime.Now;
                    newItem.Size = 0;

                    currentFolder.AddChild(newItem);
                }
                else
                {
                    if (getExstension(fileName).Length > 3) return NtStatus.Error;
                }

            }

            return NtStatus.Success;
        }

        public NtStatus DeleteDirectory(string fileName, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus DeleteFile(string fileName, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, DokanFileInfo info)
        {
            files = new List<FileInformation>();
            NTree_Lab currentFolder = NTFSLikeTree.FindChild(fileName, NTFSLikeTree);
            if (currentFolder == null) return NtStatus.Error;

            for (int i = 0; i < currentFolder.Children.Count; i++)

                files.Add(new FileInformation()
                {
                    FileName = currentFolder.GetChild(i + 1).Data.Name,
                    Attributes = currentFolder.GetChild(i + 1).Data.Attributes,
                    CreationTime = currentFolder.GetChild(i + 1).Data.DateCreated,
                    LastWriteTime = currentFolder.GetChild(i + 1).Data.DateModified
                });

            return NtStatus.Success;
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, DokanFileInfo info)
        {
            files = new FileInformation[0];
            return DokanResult.NotImplemented;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, DokanFileInfo info)
        {
            streams = new FileInformation[0];
            return DokanResult.NotImplemented;
        }

        public NtStatus FlushFileBuffers(string fileName, DokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, DokanFileInfo info)
        {
            freeBytesAvailable = freeSpace;
            totalNumberOfBytes = totalSpace;
            totalNumberOfFreeBytes = freeSpace;

            return NtStatus.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, DokanFileInfo info)
        {
            FileInformation temp = new FileInformation();
            if (fileName == "\\")
            {
                temp = new FileInformation()
                {
                    FileName = NTFSLikeTree.Data.Name,
                    Attributes = NTFSLikeTree.Data.Attributes,
                    CreationTime = NTFSLikeTree.Data.DateCreated,
                    LastWriteTime = NTFSLikeTree.Data.DateModified,
                    Length = NTFSLikeTree.Data.Size
                };
            }

            else
            {
                NTree_Lab currentFolder = NTFSLikeTree.FindChild(fileName, NTFSLikeTree);

                if (currentFolder == null)
                {
                    fileInfo = default(FileInformation);
                    return NtStatus.NoSuchFile;
                }

                F_Lab item = currentFolder.Data;

                temp = new FileInformation()
                {
                    FileName = item.Name,
                    Attributes = item.Attributes,
                    CreationTime = item.DateCreated,
                    LastWriteTime = item.DateModified,
                    Length = item.Size
                };
            }
            fileInfo = temp;

            return NtStatus.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, DokanFileInfo info)
        {
            security = null;
            return NtStatus.Error;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, DokanFileInfo info)
        {
            volumeLabel = "BicbaDrajv :v";
            features = FileSystemFeatures.None;
            fileSystemName = String.Empty;
            return NtStatus.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus Mounted(DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, DokanFileInfo info)
        {
            if (oldName == newName)
                return NtStatus.Error;

            String[] name = newName.Split("\\\\".ToCharArray());
            if (name[name.Length - 1].Length > 25) return NtStatus.Error;
            if (name.Length > 12) return NtStatus.Error;
            if(NTFSLikeTree.FindChild(oldName,NTFSLikeTree).Data.Attributes==FileAttributes.Normal)
            {
                if (getExstension(newName).Length > 3) return NtStatus.Error;
            }

            String[] path = oldName.Split("\\\\".ToCharArray());
            String folderPath = oldName.Remove(oldName.Length - path[path.Length - 1].Length);

            if (folderPath != "\\")
                folderPath = folderPath.Remove(folderPath.Length - 1, 1);

            NTree_Lab currentFolder = NTFSLikeTree.FindChild(folderPath, NTFSLikeTree);

            String[] newpath = newName.Split("\\\\".ToCharArray());
            String newfolderPath = newName.Remove(newName.Length - newpath[newpath.Length - 1].Length);

            if (newfolderPath != "\\")
                newfolderPath = newfolderPath.Remove(newfolderPath.Length - 1, 1);

            NTree_Lab newFolder = NTFSLikeTree.FindChild(newfolderPath, NTFSLikeTree);


            if (!replace)
                for (int i = 0; i < newFolder.Children.Count; i++)
                    if (newpath[newpath.Length - 1] == newFolder.GetChild(i + 1).Data.Name)
                        return NtStatus.Error;


            NTree_Lab toMove = null;

            for (int i = 0; i < currentFolder.Children.Count; i++)

                if (currentFolder.GetChild(i + 1).Data.Name == path[path.Length - 1])
                {
                    toMove = new NTree_Lab(currentFolder.GetChild(i + 1));
                    toMove.Data.Name = newpath[newpath.Length - 1];
                    currentFolder.RemoveChild(i + 1);
                    break;
                }

            newFolder.AddChildTree(toMove);

            return NtStatus.Success;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, DokanFileInfo info)
        {
            String[] path = fileName.Split("\\\\".ToCharArray());
            String folderPath = fileName.Remove(fileName.Length - path[path.Length - 1].Length);

            if (folderPath != "\\")
                folderPath = folderPath.Remove(folderPath.Length - 1, 1);

            NTree_Lab currentFolder = NTFSLikeTree.FindChild(folderPath, NTFSLikeTree);
            F_Lab toRead = null;
            if ( currentFolder==null) { bytesRead = 0; return NtStatus.Error; }

            for (int i = 0; i < currentFolder.Children.Count; i++)
                if (currentFolder.GetChild(i + 1).Data.Name == path[path.Length - 1])
                    toRead = currentFolder.GetChild(i + 1).Data;

            int l = 0;

            if (toRead == null) { bytesRead = 0; return NtStatus.Error; }

            for (; ((l + (int)offset) < toRead.Size) && (l < buffer.Length); l++)
                buffer[l] = toRead.Data[l + (int)offset];

            bytesRead = l;
            return NtStatus.Success;
        }

        public NtStatus SetAllocationSize(string fileName, long length, DokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus SetEndOfFile(string fileName, long length, DokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, DokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, DokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            return NtStatus.Error;
        }

        public NtStatus Unmounted(DokanFileInfo info)
        {
            return NtStatus.Success;
        }


        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, DokanFileInfo info)
        {

            String[] path = fileName.Split("\\\\".ToCharArray());
            String folderPath = fileName.Remove(fileName.Length - path[path.Length - 1].Length);

            if (folderPath != "\\")
                folderPath = folderPath.Remove(folderPath.Length - 1, 1);

            NTree_Lab currentFolder = NTFSLikeTree.FindChild(folderPath, NTFSLikeTree);
            F_Lab newItem = null;

            int childCounter = 0;

            bool fileExists = false;

            for (; childCounter < currentFolder.Children.Count; childCounter++)
                if (currentFolder.GetChild(childCounter + 1).Data.Name == path[path.Length - 1])
                {
                    fileExists = true;
                    break;
                }

            if (offset > 32000000) { bytesWritten = 0; currentFolder.RemoveChild(childCounter + 1); return NtStatus.Error; }


            if (fileExists)
            {
                newItem = currentFolder.GetChild(childCounter + 1).Data;
                if (newItem.DataSet == null)
                {
                    bytesWritten = 0;
                    return NtStatus.Error;
                }

                for (int i = 0; i < buffer.Length; i++)
                    newItem.DataSet.Add(buffer[i]);

                newItem.Size = newItem.DataSet.Count;
                bytesWritten = newItem.Size;

                currentFolder.GetChild(childCounter + 1).Data = newItem;
            }

            else
            {
                newItem = new F_Lab(path[path.Length - 1], new byte[buffer.Length], FileAttributes.Normal);
                newItem.DateCreated = newItem.DateModified = DateTime.Now;

                for (int i = 0; i < buffer.Length; i++)
                    newItem.DataSet.Add(buffer[i]);

                newItem.BufferSize = buffer.Length;

                newItem.Size = newItem.DataSet.Count;
                bytesWritten = newItem.Size;

                currentFolder.AddChild(newItem);
            }

            if (buffer.Length != currentFolder.GetChild(childCounter + 1).Data.BufferSize)
            {
                List<byte> temp = currentFolder.GetChild(childCounter + 1).Data.DataSet;
                currentFolder.GetChild(childCounter + 1).Data.Data = new byte[temp.Count];

                for (int i = 0; i < temp.Count; i++)
                    currentFolder.GetChild(childCounter + 1).Data.Data[i] = temp[i];

                currentFolder.GetChild(childCounter + 1).Data.Size = temp.Count;

                currentFolder.GetChild(childCounter + 1).Data.DataSet = null;
            }


            freeSpace -= buffer.Length;
            return NtStatus.Success;
        }
        string getExstension(string txt)
        {
            string[] exstension= txt.Split(".".ToCharArray());
            return exstension[exstension.Length - 1];
        }
    }
}
