using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using Newtonsoft.Json;
using System.Data;
using CsvHelper;

namespace CSVHelperTester
{
	class Program
	{

		private const string NUGET_CMD = "Install-Package CSVHelper"; // Install this for future use in other projects via the Package Manager Console.

		private static DataTable contactsviewerTestTable;

		[STAThreadAttribute]
		public static void Main(string[] args)
		{
			// TestFileDialog();
			// TestRecruiterFunctions();
			// TestContactFunctions();
			// TestContactsViewerUpload();
			// TestJSONSerializationOfRecruiter();
			// TestJSONSerializationOfContact();
			// TestNewFileDialog();
			TestCSVReaderWithFile();
			wait();
		}

		private static void TestCSVReaderWithFile()
		{
			write("Testing CSV Reader functions with a legitimate file.");
			wait("Initializing the ContactsViewer program.");

			ContactsViewer.Initialize();

			bool fileIsValid = false;
			while (!fileIsValid)
			{
				using (var dialog = new CommonFileDialog(new string[] { "*.csv" }))
				{
					waitByTime("Open a valid *.csv file please.", 1);

					try
					{
						// Handle file.
						if (dialog.FileConfirmation() == true)
						{
							TestHandleFileViaMethod(dialog, out fileIsValid);
						}
						else
						{
							write("Please choose a file to test out the functions.");
						}
					}
					catch (FileNotFoundException fnfe)
					{
						write("The file could not be found.");
						waitByTime("Error: " + fnfe.Message, 0.7);
						waitByTime("Source: " + fnfe.Source, 0.7);
						waitByTime("Stack Trace: " + fnfe.StackTrace, 0.7);
						fileIsValid = false;
					}
					catch (IOException ioe)
					{
						write("The file could not be accessed.");
						waitByTime("Error: " + ioe.Message, 0.7);
						waitByTime("Source: " + ioe.Source, 0.7);
						waitByTime("Stack Trace: " + ioe.StackTrace, 0.7);
						fileIsValid = false;
					}
					catch (Exception e) when (e is NullReferenceException || e is ArgumentNullException)
					{
						write("A null pointer reference has a appeared.");
						waitByTime("Error: " + e.Message, 0.7);
						waitByTime("Source: " + e.Source, 0.7);
						waitByTime("Stack Trace: " + e.StackTrace, 0.7);
						fileIsValid = false;
					}
					catch (Exception e)
					{
						write("An unknown error has occured.");
						waitByTime("Error: " + e.Message, 0.7);
						waitByTime("Source: " + e.Source, 0.7);
						waitByTime("Stack Trace: " + e.StackTrace, 0.7);
						fileIsValid = false;
					}
					finally
					{
						// Dispose.
					}
				}
			}
		}

		private static void TestHandleFileViaMethod(CommonFileDialog dialog, out bool isFileValid)
		{
			// Open the selected file.
			using (FileStream fileStream = dialog.OpenFile(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				using (StreamReader sr = new StreamReader(fileStream))
				{
					// Handling takes place here.
					TestHandleFile(sr, out isFileValid);
				}

				fileStream.Close();
			}
		}

		public static void TestHandleFile(StreamReader sr, out bool isFileValid)
		{
			isFileValid = false;
			if (sr == null)
			{
				throw new FileNotFoundException("The input stream was empty!");
			}
			else
			{
				if (!ContactsViewer.IsInitialized) { ContactsViewer.Initialize(); }
				using (CsvReader csv = new CsvReader(sr))
				{
					Recruiter testRecruiter = new Recruiter(1, "Ian", "Alexander", "Effendi", "effendiian@gmail.com");
					Recruiter recruiter = testRecruiter;

					DataTable dt = new DataTable("Imported Contacts", "ContactsViewer");

					DataColumn dc = new DataColumn(ContactsMap.COL_ID, Type.GetType("System.Int32"));
					dt.Columns.Add(dc);

					dc = new DataColumn(ContactsMap.COL_FIRST_NAME, Type.GetType("System.String"));
					dt.Columns.Add(dc);


					dc = new DataColumn(ContactsMap.COL_MIDDLE_NAME, Type.GetType("System.String"));
					dt.Columns.Add(dc);


					dc = new DataColumn(ContactsMap.COL_LAST_NAME, Type.GetType("System.String"));
					dt.Columns.Add(dc);


					dc = new DataColumn(ContactsMap.COL_EMAIL_ADDRESS, Type.GetType("System.String"));
					dt.Columns.Add(dc);


					dc = new DataColumn(ContactsMap.COL_COMPANY, Type.GetType("System.String"));
					dt.Columns.Add(dc);
					
					dc = new DataColumn(ContactsMap.COL_JOB_TITLE, Type.GetType("System.String"));
					dt.Columns.Add(dc);

					dc = new DataColumn(ContactsMap.COL_RECRUITER_ID, Type.GetType("System.Int32"));
					dt.Columns.Add(dc);

					dc = new DataColumn(ContactsMap.COL_RECRUITER, Type.GetType("System.Object"));
					dt.Columns.Add(dc);

					int contactId = 1;

					foreach (DataColumn col in dt.Columns)
					{
						if (col.ColumnName != ContactsMap.COL_RECRUITER)
						{
							write(col.ColumnName, false);
							write("\t", false);
						}
					}

					write();

					while (csv.Read())
					{
						string rowStr = "";
						var row = dt.NewRow();
						foreach (DataColumn col in dt.Columns)
						{
							switch (col.ColumnName)
							{
								case ContactsMap.COL_ID:
									row[col.ColumnName] = contactId;
									rowStr += "" + row[col.ColumnName] + "\t";
									break;
								case ContactsMap.COL_RECRUITER_ID:
									row[col.ColumnName] = recruiter.Id;
									rowStr += "" + row[col.ColumnName] + "\t";
									break;
								case ContactsMap.COL_RECRUITER:
									row[col.ColumnName] = recruiter;
									break;
								default:
									row[col.ColumnName] = csv.GetField(col.DataType, col.ColumnName);
									rowStr += "" + row[col.ColumnName] + "\t";
									break;
							}
						}
						waitByTime(rowStr, 0.1);
						dt.Rows.Add(row);
						++contactId;
					}

					isFileValid = true;
				}
			}
		}

		private static void TestNewFileDialog()
		{
			wait();

			// Allow the user to collect a custom file.
			// And open that file.
			wait("Thank you for using this console!");
			write();

			// Open a file.
			wait("You will now open a file.");
					

			using (var dialog = new CommonFileDialog(new string[] {"*.csv"}))
			{
				try
				{
					// File handling.
					// Call the ShowDialog method to show the dialog box.
					if (dialog.FileConfirmation() == true)
					{
						write("File successfully chosen. Printing first 5 lines file:");
						write();

						// Open the selected file.
						using (FileStream fileStream = dialog.OpenFile(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
						{
							using (StreamReader sr = new StreamReader(fileStream))
							{
								// Handling takes place here.
								string line;
								int count = 0;
								int countMax = 5;
								while (((line = sr.ReadLine()) != null) && (count < countMax))
								{
									write(line);
									count++;
								}
							}

							fileStream.Close();
						}
					}
					else
					{
						write("File not chosen.");
					}

				}
				catch (IOException e)
				{
					// Error handling
					write("There was an error opening the file.");
					write("This could be due to the file being used by another program.");
					wait("Please close the file in any other programs before accessing it.");

					write("Error: " + e.Message);
					write("Stack Trace: " + e.StackTrace);
					wait("Source: " + e.Source);
				}
				catch (Exception e)
				{
					// Error handling
					wait("There was an unknown error. Please contact the developer with the following information:");

					write("Error: " + e.Message);
					write("Stack Trace: " + e.StackTrace);
					write("Source: " + e.Source);
				}
				finally
				{
					// Dispose
					write();
					wait("Exiting program");
					System.Environment.ExitCode = 0;
					System.Environment.Exit(System.Environment.ExitCode);
				}
			}
		}

		private static void TestJSONSerializationOfRecruiter()
		{
			//string jsonObject;
			//Recruiter testJSON, testBuild;
			//testJSON = new Recruiter(1, );
			//testBuild = new Recruiter(2, );

		}

		private static void TestContactFunctions()
		{
			// Variables and information
			double delay = 0.5;
			string emailA = "effendiian@gmail.com"; // Valid email.
			string emailB = "thisemailshouldbeinvalid"; // Forcibly invalid email via client validation.
			string emailC = "e@kdjfj.cskdjflskdjflsjd"; // Forcibly invalid email via API query.
			Contact testA, testB, testC;
			Recruiter testProfile = new Recruiter(1, "Test", "ing", "Recruiter", "fakeemail@example.com");

			int id = 0;

			// Build 3 contacts; two equal, one similar but different.
			// Create contact 1.
			wait("Loading contacts...");

			// Contact A
			++id;
			testA = new Contact(id, "Ian", "Alexander", "Effendi", emailA, "Goldsmith and Co.", "Technology Intern", testProfile);
			waitByTime("Contact #" + id + " created...", delay);

			// Contact B
			++id;
			testB = new Contact(id, "Ian", "", "Effendi", emailB, "", "", testProfile);
			waitByTime("Contact #" + id + " created...", delay);

			// Contact C
			++id;
			testC = new Contact(id, "Nai", "", "Idneffe", emailC, "Fake Google", "Professional Fake Whisperer", testProfile);
			waitByTime("Contact #" + id + " created...", delay);

			// Test information storage.
			write();
			write("Contacts created.");
			wait("Testing contact email validation.");

			// Test email validation.
			Contact[] contacts = { testA, testB, testC };

			foreach (Contact test in contacts)
			{
				string test_email = test.EmailAddress;
				write("Testing " + test.FirstName + "'s email address: " + test.EmailAddress);
				waitByTime("> " +  ContactsViewer.ValidateEmailAddress(test_email), 2);
				write("Email validation test completed for " + test.FirstName);
				write();
			}

			// Print Recruiter.
			write("Recruiter profile:" );
			waitByTime(testProfile.ToString(), delay);
			write("Information printed for this profile completed successfully.");
			wait();

			// Test comparision between three contacts.
			write("Comparing contacts.");
			int testNum = 0;

			++testNum; // TestA with TestB.
			write("#" + testNum + ") Comparing A to B.");
			waitByTime("> " + testA.Equals(testB), 0.5);
			write();

			++testNum; // TestA with TestC.
			write("#" + testNum + ") Comparing A to C.");
			waitByTime("> " + testA.Equals(testC), 0.5);
			write();

			++testNum; // TestC with TestB.
			write("#" + testNum + ")Comparing C to B.");
			waitByTime("> " + testC.Equals(testB), 0.5);
			write();

			++testNum; // TestB with TestA.
			write("#" + testNum + ")Comparing B to A.");
			waitByTime("> " + testB.Equals(testA), 0.5);
			write();

			++testNum; // TestB with TestC.
			write("#" + testNum + ")Comparing B to C.");
			waitByTime("> " + testB.Equals(testC), 0.5);
			write();

			++testNum; // TestA with TestA.
			write("#" + testNum + ")Comparing A to A.");
			waitByTime("> " + testA.Equals(testA), 0.5);
			write();

			++testNum; // TestB with TestB.
			write("#" + testNum + ")Comparing B to B.");
			waitByTime("> " + testB.Equals(testB), 0.5);
			write();

			++testNum; // TestC with TestC.
			write("#" + testNum + ")Comparing C to C.");
			waitByTime("> " + testC.Equals(testC), 0.5);
			waitByTime("", 2);
			wait();

			// Print contacts.
			foreach (Contact test in contacts)
			{
				write("Contact: " + test.Id);
				waitByTime(test.ToString(), delay);
				write("Information printed for this contact successfully.");
				wait();
			}

			// End program.
			wait("Exiting program");
			System.Environment.ExitCode = 0;
			System.Environment.Exit(System.Environment.ExitCode);
		}

		private static void TestRecruiterFunctions()
		{
			double delay = 0.2;

			int test_id = 1;
			int test_id2 = 2;
			string test_fname = "Bob";
			string test_lname = "Jones";
			string test_mname = "";
			string test_mname2 = "Kyle";
			string test_email = "effendiian@gmail.com";

			Recruiter rTest, bTest;
			rTest = new Recruiter(test_id, test_fname, test_mname, test_lname, test_email);
			bTest = new Recruiter(test_id2, test_fname, test_mname2, test_lname, test_email);
			wait("Loading recruiters...");


			write("Testing email address: " + test_email);
			waitByTime("> " + ContactsViewer.ValidateEmailAddress(test_email), 2);
			write();

			write("Recruiter: " + rTest.Id);
			waitByTime(rTest.ToString(), delay);

			write("Recruiter: " + bTest.Id);
			waitByTime(bTest.ToString(), delay);

			write("Comparing recruiters.");
			waitByTime("> " + rTest.Equals(bTest), 2);
			write();

			wait("Exiting program");
			System.Environment.ExitCode = 0;
			System.Environment.Exit(System.Environment.ExitCode);
		}

		private static void TestFileDialog()
		{
			wait("Thank you for using this console!");
			write();
			write("Testing the use of CSVHelper NuGet package.");
			wait("Install via the cmd: " + NUGET_CMD);

			// Read the directory files.
			DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

			foreach (FileInfo file in dirInfo.GetFiles("*.*"))
			{
				Console.WriteLine("{0}, {1}", file.Name, file.Length);
			}

			// Open a file.
			wait("You will now open a file.");

			// Create OpenFileDialog box instance.
			OpenFileDialog fileDialog = new OpenFileDialog();

			// Set filter options and filter index.
			fileDialog.Filter = "CSV Files (.csv)|*.csv|All Files (*.*)|*.*";
			fileDialog.FilterIndex = 1;

			fileDialog.Multiselect = false;

			// Call the ShowDialog method to show the dialog box.
			bool? confirmation = (fileDialog.ShowDialog() == DialogResult.OK);

			if (confirmation == true)
			{
				write("File successfully chosen. Printing first 5 lines file:");
				write();

				// Open the selected file.
				using (FileStream fileStream = OpenFile(fileDialog))
				{
					using (StreamReader sr = new StreamReader(fileStream))
					{
						string line;
						int count = 0;
						int countMax = 500000;
						while (((line = sr.ReadLine()) != null) && (count < countMax))
						{
							write(line);
							count++;
						}
					}

					fileStream.Close();
				}
			}
			else
			{
				write("File not chosen.");
			}

			write();
			wait("Exiting program");
			System.Environment.ExitCode = 0;
			System.Environment.Exit(System.Environment.ExitCode);
		}

		private static FileStream OpenFile(FileDialog dialog)
		{
			return new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}

		private static void write()
		{
			Console.WriteLine("");
		}

		private static void write(string message, bool newline)
		{
			if (newline)
			{
				Console.WriteLine(message);
			}
			else
			{
				Console.Write(message);
			}
		}

		private static void write(string message)
		{
			Console.WriteLine(message);
		}

		private static void wait()
		{
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(true);
		}

		private static void wait(string message)
		{
			write(message);
			wait();
		}

		private static void waitByTime(string message, double time)
		{
			TimeSpan span = TimeSpan.FromSeconds(time);
			Thread.Sleep((int)span.TotalMilliseconds);
			write(message);
		}

	}
}
