using DataBaseRepository.Model;
using OracleParser;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace PackageSplitter.ViewModel
{
    public class ParserViewModel :PropertyChangedBase
    {
        private const int TIMER_INTERVAL_MS = 1000;
        private readonly TimeSpan TIMER_INTERVAL = TimeSpan.FromMilliseconds(TIMER_INTERVAL_MS);
        private Timer _timer = new Timer(TIMER_INTERVAL_MS);
        private IOraParser _OraParser = OraParser.Instance();
        private IOracleParsedPackageSetter _oracleParsedPackageSetter;

        public RelayCommand CloseCommand { get; private set; }

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                _ErrorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged("HasError");
            }
        }

        public Visibility HasError => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

        public ObservableCollection<ParserDataViewModel> ParserData { get; private set; }

        public ParserViewModel(RepositoryPackage repositoryPackage, Action<object> CloseAction, IOracleParsedPackageSetter oracleParsedPackageSetter, IOraParser oraParser)
        {
            CloseCommand = new RelayCommand(CloseAction);
            _oracleParsedPackageSetter = oracleParsedPackageSetter;
            _OraParser = oraParser;

            ParserData = new ObservableCollection<ParserDataViewModel>();
            ParserData.Add(new ParserDataViewModel(repositoryPackage.SpecRepFullPath));
            ParserData.Add(new ParserDataViewModel(repositoryPackage.BodyRepFullPath));

            _timer.Elapsed += Timer_Elapsed;
  
            RunParse(repositoryPackage);
        }

        public ParserViewModel()
        {

        }

        public async void RunParse(RepositoryPackage repositoryPackage)
        {
            try
            {
                var package = _OraParser.GetSavedPackage(repositoryPackage);
                var HasSavedPackage = package != null;

                if (!HasSavedPackage)
                {
                    ParserData[0].Status = eParseStatus.InProgress;
                    ParserData[1].Status = eParseStatus.Wait;
                    _OraParser.ObjectWasParsed += ParserViewModel_ObjectWasParsed;
                    _timer.Start();
                    package = await _OraParser.ParsePackage(repositoryPackage, Config.Instanse().AllowNationalChars);
                }

                _oracleParsedPackageSetter.SetOracleParsedPackage(package);

                if (!HasSavedPackage)
                    await Task.Delay(1000);

                CloseCommand.Execute(null);
            }
            catch (Exception ex)
            {
                _timer.Stop();
                ErrorMessage = ex.Message;
                if (ParserData[0].InProgress)
                    ParserData[0].Status = eParseStatus.Fail;
                else
                    ParserData[1].Status = eParseStatus.Fail;
            }
        }

        private void ParserViewModel_ObjectWasParsed(eRepositoryObjectType obj)
        {
            switch (obj)
            {
                case eRepositoryObjectType.Package_Spec:
                    ParserData[0].Status = eParseStatus.Done;
                    ParserData[1].Status = eParseStatus.InProgress;
                    break;
                case eRepositoryObjectType.Package_Body:
                    ParserData[1].Status = eParseStatus.Done;
                    _timer.Stop();
                    break;
            }
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (ParserData[0].InProgress)
                    ParserData[0].LoadTime = ParserData[0].LoadTime.Add(TIMER_INTERVAL);
                else
                    ParserData[1].LoadTime = ParserData[1].LoadTime.Add(TIMER_INTERVAL);
            });
        }   
    }
}
