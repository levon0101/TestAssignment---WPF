using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using ProgramA.Services;

namespace ProgramA.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEmbeddedFileService _embeddedFileService;
        private string _filePathA;
        private string _filePathB;
        private bool _runned;

        private Thread _threadForFileA;
        private Thread _threadForFileB;

        public MainViewModel(IEmbeddedFileService embeddedFileService)
        {
            _embeddedFileService = embeddedFileService;

            RunCommand = new DelegateCommand(OnRunCommandExecute, OnRunCommandCanExecute);
            StopCommand = new DelegateCommand(OnStopCommandExecute, OnStopCommandCanExecute);

            HandsA = new ObservableCollection<string>();
            HandsB = new ObservableCollection<string>();
        }

        public bool Runned
        {
            get => _runned;
            set
            {
                _runned = value;

                ((DelegateCommand)RunCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)StopCommand).RaiseCanExecuteChanged();
            }
        }

        public string FilePathA
        {
            get => _filePathA;
            set
            {
                _filePathA = value;
                Properties.Settings.Default.LastFilePathA = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public string FilePathB
        {
            get => _filePathB;
            set
            {
                _filePathB = value;
                Properties.Settings.Default.LastFilePathB = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> HandsA { get; set; }
        public ObservableCollection<string> HandsB { get; set; }

        public ICommand RunCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public void Load()
        {
            FilePathA = Properties.Settings.Default.LastFilePathA;
            FilePathB = Properties.Settings.Default.LastFilePathB;

        }

        private void OnRunCommandExecute()
        {
            try
            {

                if (File.Exists(FilePathA))
                {
                    File.Delete(FilePathA);
                }

                if (File.Exists(FilePathB))
                {
                    File.Delete(FilePathB);
                }

                using (File.Create(FilePathA))
                {

                }

                using (File.Create(FilePathB))
                {

                }
            }
            catch
            {
                MessageBox.Show("Can't create file");
                Runned = false;

                return;
            }

            _threadForFileA = new Thread(FileAParsingInSeparateThread);
            _threadForFileB = new Thread(FileBParsingInSeparateThread);

            _threadForFileA.IsBackground = true;
            _threadForFileB.IsBackground = true;

            HandsA.Clear();
            HandsB.Clear();

            _threadForFileA.Start();
            _threadForFileB.Start();


            Runned = true;
        }

        private bool OnRunCommandCanExecute()
        {
            return !Runned;
        }

        private void OnStopCommandExecute()
        {
            _threadForFileA.Abort();
            _threadForFileB.Abort();

            _embeddedFileService.ResetSource();

            Runned = false;

        }

        private bool OnStopCommandCanExecute()
        {
            return Runned;
        }

        private void FileAParsingInSeparateThread()
        {
            using (var mutex = new Mutex(false, Properties.Settings.Default.MutexNameA))
            {

                while (true)
                {

                    var hand = _embeddedFileService.GetHandFromFileA();

                    if (string.IsNullOrEmpty(hand))
                        break; 

                    mutex.WaitOne();

                    using (StreamWriter sw = File.AppendText(FilePathA))
                    {
                        sw.WriteLine(hand);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                    }

                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => HandsA.Add(hand.Substring(0, 50))));

                    mutex.ReleaseMutex();
                    Thread.Sleep(15000);
                }
            }
        }

        private void FileBParsingInSeparateThread()
        {
            using (var mutex = new Mutex(false, Properties.Settings.Default.MutexNameB))
            {
                while (true)
                {
                    var hand = _embeddedFileService.GetHandFromFileB();

                    if (string.IsNullOrEmpty(hand))
                        break;
                     
                    mutex.WaitOne();

                    using (StreamWriter sw = File.AppendText(FilePathB))
                    {
                        sw.WriteLine(hand);
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                    }

                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => HandsB.Add(hand.Substring(0, 50))));

                    mutex.ReleaseMutex();
                    Thread.Sleep(15000);

                }
            }
        }
    }
}
