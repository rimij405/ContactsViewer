using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace CSVHelperTester
{
	public class CommonFileDialog : IDisposable
	{
		// Fields
		private static bool OPEN_DIALOG;
		private static bool MULTI_SELECT;
		private const string EMPTY = "![EMPTY]";

		// Attributes
		private OpenFileDialog fileDialog;
		private DirectoryInfo directoryInfo;
		private FileInfo fileInfo;
		private string rootDirectory;
		private string currDirectory;
		private string fileName;
		private string fileFilter;
		
		// Properties
		public OpenFileDialog FileDialog
		{
			get { return this.fileDialog; }
		}

		public string Root
		{
			get { return this.rootDirectory; }
			set { this.fileName = value; }
		}

		public string CurrentFolder
		{
			get { return this.currDirectory; }
			set { this.fileName = value; }
		}

		public string FileName
		{
			get { return this.fileName; }
		}

		public string FileFilter
		{
			get { return this.fileFilter; }
		}

		public bool IsDialogOpen
		{
			get { return OPEN_DIALOG; }
		}

		// Constructor
		public CommonFileDialog(params string[] extensions)
		{
			// Set defaults.
			OPEN_DIALOG = false;
			MULTI_SELECT = false;

			// Assign parameters.
			this.fileName = EMPTY;
			this.currDirectory = Directory.GetCurrentDirectory();
			this.rootDirectory = Directory.GetDirectoryRoot(currDirectory);

			// Create objects.
			fileDialog = new OpenFileDialog();
			directoryInfo = new DirectoryInfo(currDirectory);
			SetExtensions(extensions);

			// Set basis.
			Initialize();
		}

		public CommonFileDialog(string name, params string[] extensions)
		{
			// Set defaults.
			OPEN_DIALOG = false;
			MULTI_SELECT = false;

			// Assign parameters.
			this.fileName = name;
			this.currDirectory = Directory.GetCurrentDirectory();
			this.rootDirectory = Directory.GetDirectoryRoot(currDirectory);

			// Create objects.
			fileDialog = new OpenFileDialog();
			directoryInfo = new DirectoryInfo(currDirectory);
			SetExtensions(extensions);

			// Set basis.
			Initialize();
		}

		// Methods
		public void Initialize()
		{
			// Set basis.
			fileDialog.InitialDirectory = currDirectory;
			fileDialog.Multiselect = MULTI_SELECT;
			fileDialog.CheckPathExists = true;
			fileDialog.CheckFileExists = true;
		}

		public bool? FileConfirmation()
		{
			if (!OPEN_DIALOG) { OPEN_DIALOG = true; } else { return null; }

			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				OPEN_DIALOG = false;
				if (fileDialog.FileName != null && fileDialog.FileName.Length > 0)
				{
					fileName = fileDialog.FileName;
					fileInfo = new FileInfo(fileName);
					directoryInfo = fileInfo.Directory;
					currDirectory = fileInfo.DirectoryName;
					rootDirectory = Directory.GetDirectoryRoot(currDirectory);
					fileDialog.InitialDirectory = currDirectory;

					if (fileInfo.IsReadOnly)
					{
						throw new FileAccessException("The file " + fileInfo.Name + " in {" + fileInfo.DirectoryName + "} is read only.");
					}

					return true;
				}

				return false;
			}

			return null;
		}
		
		public FileStream OpenFile(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			if(fileName == EMPTY) { return null; }

			if (File.Exists(fileName))
			{
				FileStream toReturn;
				if (OpenFile(fileInfo, fileMode, fileAccess, fileShare, out toReturn))
				{
					return toReturn;
				}
				return null;
			}

			return null;
		}

		public bool OpenFile(FileInfo file, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, TimeSpan timeOut, out FileStream stream)
		{
			var endTime = DateTime.Now + timeOut;

			while (DateTime.Now < endTime)
			{
				if (OpenFile(file, fileMode, fileAccess, fileShare, out stream))
				{
					return true;
				}
			}

			stream = null;
			return false;
		}

		public bool OpenFile(FileInfo file,	FileMode fileMode,	FileAccess fileAccess,	FileShare fileShare, out FileStream stream)
		{
			try
			{
				stream = file.Open(fileMode, fileAccess, fileShare);
				return true;
			}
			catch (IOException e)
			{
				if (!FileIsLocked(e))
				{
					throw;
				}
				else
				{
					throw new IOException("The file is currently in use. " + e.Message);
				}
			}
		}

		public FileInfo[] GetFilesInCurrentDirectory(bool hiddenFiles)
		{
			if (hiddenFiles)
			{
				return directoryInfo.GetFiles("*");
			}
			else
			{
				return GetFilesInCurrentDirectory();
			}
		}

		public FileInfo[] GetFilesInCurrentDirectory()
		{
			return directoryInfo.GetFiles("*.*");
		}
		
		public FileInfo[] GetFilesInCurrentDirectory(string ext)
		{
			// Each input argument should have the format of ".csv" or "*.csv"
			// If the first character is not a period,
			// Or if the first character is an asterisk and the second is not a period,
			// It isn't a valid extension argument.
			char asterisk = '*';
			char period = '.';

			// Check extension length.
			if (ext.Length > 2 // Minimum ".c" is two characters.
				&& ext[0] == period // And the first character is a "."
				&& ext[1] != period // And the second character is not a "."
				&& !ext.Substring(1).Contains(asterisk)) // And the string from the second character onwards does not contain "*".
			{
				return directoryInfo.GetFiles("*" + ext);
			}
			else if (ext.Length > 3) // Minimum with asterisk is three characters. "*.c"
			{
				if (ext[0] == asterisk // If the first character is an asterisk.
					&& ext[1] == period // And the second character is a period.
					&& !ext.Substring(2).Contains(asterisk) // And the string from the 3rd character onwards
					&& ext[2] != period)  // // contains no asterisk or period.
				{
					return directoryInfo.GetFiles(ext);
				}
				else if (ext[0] == period // If the first character is a period.
					&& !ext.Substring(1).Contains(asterisk) // And the rest of the string does not
					&& ext[1] != period) // // contain any asterisks or periods in inappropriate places.
				{
					return directoryInfo.GetFiles("*" + ext);
				}
			}

			return GetFilesInCurrentDirectory();	
		}
		
		public void SetExtensions(string[] extensions)
		{
			string defaultFilter = "All Files (*.*)|*.*";

			if (extensions == null || extensions.Length <= 0)
			{
				this.fileFilter = defaultFilter;
				fileDialog.FilterIndex = 0;
				return;
			}

			string filterPipe = "|";
			string filter = "" + defaultFilter;

			foreach (string extension in extensions)
			{
				// Each input argument should have the format of ".csv" or "*.csv"
				// If the first character is not a period,
				// Or if the first character is an asterisk and the second is not a period,
				// It isn't a valid extension argument.
				char asterisk = '*';
				char period = '.';
				string pipeFiltered = "";

				// Check extension length.
				if (extension.Length > 2 // Minimum ".c" is two characters.
					&& extension[0] == period // And the first character is a "."
					&& extension[1] != period // And the second character is not a "."
					&& !extension.Substring(1).Contains(asterisk)) // And the string from the second character onwards does not contain "*".
				{
					pipeFiltered = extension.Substring(1).ToUpper() + " Files (" + extension + ")|*" + extension;
				}
				else if (extension.Length > 3) // Minimum with asterisk is three characters. "*.c"
				{
					if (extension[0] == asterisk // If the first character is an asterisk.
						&& extension[1] == period // And the second character is a period.
						&& !extension.Substring(2).Contains(asterisk) // And the string from the 3rd character onwards
						&& extension[2] != period)  // // contains no asterisk or period.
					{
						pipeFiltered = extension.Substring(2).ToUpper() + " Files (" + extension.Substring(1) + ")|" + extension;
					}
					else if(extension[0] == period // If the first character is a period.
						&& !extension.Substring(1).Contains(asterisk) // And the rest of the string does not
						&& extension[1] != period) // // contain any asterisks or periods in inappropriate places.
					{
						pipeFiltered = extension.Substring(1).ToUpper() + " Files (" + extension + ")|*" + extension;
					}
				}

				filter = pipeFiltered + filterPipe + filter; // Inserts the extension to the filter list at the front of the array.
			}

			this.fileFilter = filter;
			fileDialog.Filter = fileFilter;

			if(fileFilter.Length <= defaultFilter.Length)
			{
				fileDialog.FilterIndex = 0;
			}
			fileDialog.FilterIndex = 1;
		}
		
		private const uint HRFileLocked = 0x80070020;
		private const uint HRPortionOfFileLocked = 0x80070021;

		private bool FileIsLocked(IOException ioException)
		{
			var errorCode = (uint)Marshal.GetHRForException(ioException);
			return ((errorCode == HRFileLocked) || (errorCode == HRPortionOfFileLocked));
		}

		public void Dispose()
		{
			fileDialog.Dispose();
		}
	}

	internal class FileAccessException : Exception
	{
		public FileAccessException(string message)
			:base(message)
		{
		}

	}

}
