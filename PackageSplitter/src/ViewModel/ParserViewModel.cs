using DataBaseRepository.Model;
using OracleParser;
using PackageSplitter.Model.Parser;
using PackageSplitter.Model.Split;
using PackageSplitter.src.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PackageSplitter.ViewModel
{
    public class ParserViewModel :PropertyChangedBase
    {
        private const int TIMER_INTERVAL_MS = 250;
        private readonly TimeSpan TIMER_INTERVAL = TimeSpan.FromMilliseconds(TIMER_INTERVAL_MS);
        private Timer timer = new Timer(TIMER_INTERVAL_MS);


        public Action CloseAction;
        public RelayCommand CloseCommand { get; private set; }

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                _ErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ParserDataViewModel> ParserData { get; private set; }

        public ParserViewModel(RepositoryPackage repositoryPackage)
        {
            CloseCommand = new RelayCommand((x) => { CloseAction(); });

            ParserData = new ObservableCollection<ParserDataViewModel>();
            ParserData.Add(new ParserDataViewModel(repositoryPackage.SpecRepFullPath));
            ParserData.Add(new ParserDataViewModel(repositoryPackage.BodyRepFullPath));

            timer.Elapsed += Timer_Elapsed;
  
            RunParse(repositoryPackage);
        }



        public ParserViewModel()
        {

        }

        public async void RunParse(RepositoryPackage repositoryPackage)
        {
            OraParser.Instance().ObjectWasParsed += ParserViewModel_ObjectWasParsed;
            ParserData[0].Status = eParseStatus.InProgress;
            ParserData[1].Status = eParseStatus.Wait;

            try
            {
                timer.Start();
                var package = await OraParser.Instance().GetPackage(repositoryPackage);
                timer.Stop();

                SplitManager.Instance().LoadOracleParsedPackage(package);
            }
            catch(Exception ex)
            {
                timer.Stop();
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
