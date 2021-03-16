using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using ProgramB.Model;
using ProgramB.Repositories;
using ProgramB.Services;

namespace ProgramB.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IHandRepository _handRepository;
        private readonly IHandFromFileService _handFromFileService;
        private bool _runned;
        private string _filePathA;
        private string _filePathB;

        private Thread _threadForFileA;
        private Thread _threadForFileB;

        public MainViewModel(IHandRepository handRepository,
            IHandFromFileService handFromFileService)
        {
            _handRepository = handRepository;
            _handFromFileService = handFromFileService;

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

        public async void Load()
        {
            FilePathA = Properties.Settings.Default.LastFilePathA;
            FilePathB = Properties.Settings.Default.LastFilePathB;

            MessageBox.Show("Total hands in db: " + (await _handRepository.GetAllAsync()).Count());
        }

        private void OnRunCommandExecute()
        {
            if (!File.Exists(FilePathA))
            {
                MessageBox.Show("File A not exist.");
                return;
            }

            if (!File.Exists(FilePathB))
            {
                MessageBox.Show("File B not exist.");
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

            _handFromFileService.ResetSource();

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
                    mutex.WaitOne();

                    var hand = _handFromFileService.GetHandFromFileA(FilePathA);

                    mutex.ReleaseMutex();

                    if (string.IsNullOrEmpty(hand.Id))
                    {
                        Thread.Sleep(15000);
                        continue;
                    }

                    _handRepository.Add(new Hand
                    {
                        HandId = hand.Id,
                        HandText = hand.Hand
                    });

                    _handRepository.Save();

                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => HandsA.Add(hand.Hand.Substring(0, 50))));
                }
            }
        }

        private void FileBParsingInSeparateThread()
        {
            using (var mutex = new Mutex(false, Properties.Settings.Default.MutexNameB))
            {
                while (true)
                {
                    mutex.WaitOne();

                    var hand = _handFromFileService.GetHandFromFileB(FilePathB);

                    mutex.ReleaseMutex();

                    if (string.IsNullOrEmpty(hand.Id))
                    {
                        Thread.Sleep(15000);
                        continue;
                    }

                    _handRepository.Add(new Hand
                    {
                        HandId = hand.Id,
                        HandText = hand.Hand
                    });

                    _handRepository.Save();

                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => HandsB.Add(hand.Hand.Substring(0, 50))));
                }
            }
        }
    }
}
