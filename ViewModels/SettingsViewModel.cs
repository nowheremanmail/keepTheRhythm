using nowhereman;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Navigation;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        string _Mode = default(string);
        public string Mode { get { return _Mode; } set { Set(ref _Mode, value); } }


        bool _PauseOnObscured = default(bool);
        public bool PauseOnObscured { get { return _PauseOnObscured; } set { Set(ref _PauseOnObscured, value); } }

        bool _TimeGPSaccuracy = default(bool);
        public bool TimeGPSaccuracy { get { return _TimeGPSaccuracy; } set { Set(ref _TimeGPSaccuracy, value); } }

        bool _HasLoops = default(bool);
        public bool HasLoops { get { return _HasLoops; } set { Set(ref _HasLoops, value); } }


        bool _Intelligence = default(bool);
        public bool Intelligence { get { return _Intelligence; } set { Set(ref _Intelligence, value); } }


        bool _IntelligencePace = default(bool);
        public bool IntelligencePace { get { return _IntelligencePace; } set { Set(ref _IntelligencePace, value); } }


        bool _IntelligenceRotatio = default(bool);
        public bool IntelligenceRotation { get { return _IntelligenceRotatio; } set { Set(ref _IntelligenceRotatio, value); } }


        int _TimeToDetect = default(int);
        public int TimeToDetectP { get { return _TimeToDetect; } set { Set(ref _TimeToDetect, value); } }


        int _TimeToDetectR = default(int);
        public int TimeToDetectR { get { return _TimeToDetectR; } set { Set(ref _TimeToDetectR, value); } }

        double _voiceAdvicesDistance = 2.0;
        public double VoiceAdvicesDistance { get { return _voiceAdvicesDistance; } set { Set(ref _voiceAdvicesDistance, value); } }

        TimeSpan _voiceAdvicesTime = default(TimeSpan);
        public TimeSpan VoiceAdvicesTime { get { return _voiceAdvicesTime; } set { Set(ref _voiceAdvicesTime, value); } }

        double _accuracy = 2.0;
        public double GPSaccuracy { get { return _accuracy; } set { Set(ref _accuracy, value); } }

        double _GPSaccuracyMax = 50.0;
        public double GPSaccuracyMax { get { return _GPSaccuracyMax; } set { Set(ref _GPSaccuracyMax, value); } }


        double _LoopDistance = 1000.0;
        public double LoopDistance { get { return _LoopDistance; } set { Set(ref _LoopDistance, value); } }



        List<PairCodeDesc> _LoopList = default(List<PairCodeDesc>);
        public List<PairCodeDesc> LoopList { get { return _LoopList; } set { Set(ref _LoopList, value); } }
        PairCodeDesc _LoopCurrent = default(PairCodeDesc);
        public PairCodeDesc LoopCurrent { get { return _LoopCurrent; } set { Set(ref _LoopCurrent, value); } }

        List<PairCodeDesc> _ActionOnTabList = default(List<PairCodeDesc>);
        public List<PairCodeDesc> ActionOnTabList { get { return _ActionOnTabList; } set { Set(ref _ActionOnTabList, value); } }
        PairCodeDesc _ActionOnTabCurrent = default(PairCodeDesc);
        public PairCodeDesc ActionOnTabCurrent { get { return _ActionOnTabCurrent; } set { Set(ref _ActionOnTabCurrent, value); } }


        List<PairCodeDesc> _ActionOnDoubleTabList = default(List<PairCodeDesc>);
        public List<PairCodeDesc> ActionOnDoubleTabList { get { return _ActionOnDoubleTabList; } set { Set(ref _ActionOnDoubleTabList, value); } }
        PairCodeDesc _ActionOnDoubleTabCurrent = default(PairCodeDesc);
        public PairCodeDesc ActionOnDoubleTabCurrent { get { return _ActionOnDoubleTabCurrent; } set { Set(ref _ActionOnDoubleTabCurrent, value); } }

        List<PairCodeDesc> _ActionHoldTabList = default(List<PairCodeDesc>);
        public List<PairCodeDesc> ActionHoldTabList { get { return _ActionHoldTabList; } set { Set(ref _ActionHoldTabList, value); } }
        PairCodeDesc _ActionHoldTabCurrent = default(PairCodeDesc);
        public PairCodeDesc ActionHoldTabCurrent { get { return _ActionHoldTabCurrent; } set { Set(ref _ActionHoldTabCurrent, value); } }

        List<PairCodeDesc> _ActionVerticalFlickList = default(List<PairCodeDesc>);
        public List<PairCodeDesc> ActionVerticalFlickList { get { return _ActionVerticalFlickList; } set { Set(ref _ActionVerticalFlickList, value); } }
        PairCodeDesc _ActionVerticalFlickCurrent = default(PairCodeDesc);
        public PairCodeDesc ActionVerticalFlickCurrent { get { return _ActionVerticalFlickCurrent; } set { Set(ref _ActionVerticalFlickCurrent, value); } }

        List<PairCodeDesc> _Action_VerticalFlickList = default(List<PairCodeDesc>);
        public List<PairCodeDesc> Action_VerticalFlickList { get { return _Action_VerticalFlickList; } set { Set(ref _Action_VerticalFlickList, value); } }
        PairCodeDesc _Action_VerticalFlickCurrent = default(PairCodeDesc);
        public PairCodeDesc Action_VerticalFlickCurrent { get { return _Action_VerticalFlickCurrent; } set { Set(ref _Action_VerticalFlickCurrent, value); } }

        List<PairCodeDesc> _ActionHorizontalFlickList = default(List<PairCodeDesc>);
        public List<PairCodeDesc> ActionHorizontalFlickList { get { return _ActionHorizontalFlickList; } set { Set(ref _ActionHorizontalFlickList, value); } }
        PairCodeDesc _ActionHorizontalFlickCurrent = default(PairCodeDesc);
        public PairCodeDesc ActionHorizontalFlickCurrent { get { return _ActionHorizontalFlickCurrent; } set { Set(ref _ActionHorizontalFlickCurrent, value); } }

        List<PairCodeDesc> _Action_HorizontalFlickList = default(List<PairCodeDesc>);
        public List<PairCodeDesc> Action_HorizontalFlickList { get { return _Action_HorizontalFlickList; } set { Set(ref _Action_HorizontalFlickList, value); } }
        PairCodeDesc _Action_HorizontalFlickCurrent = default(PairCodeDesc);
        public PairCodeDesc Action_HorizontalFlickCurrent { get { return _Action_HorizontalFlickCurrent; } set { Set(ref _Action_HorizontalFlickCurrent, value); } }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            //nowhereman.Properties.setDoubleProperty("frequencyMetronom" + Mode, frecuency.Value);
            //nowhereman.Properties.setDoubleProperty("amplitudeMetronom", amplitude.Value);

            nowhereman.Properties.setProperty("Tab" + Mode, ActionOnTabCurrent.code);
            //// DoubleTab, Tab, Hold, VerticalFlick, HorizontalFlick, -VerticalFlick, -HorizontalFlick
            nowhereman.Properties.setProperty("DoubleTab" + Mode, ActionOnDoubleTabCurrent.code);
            nowhereman.Properties.setProperty("Hold" + Mode, ActionHoldTabCurrent.code);

            nowhereman.Properties.setProperty("VerticalFlick" + Mode, ActionVerticalFlickCurrent.code);
            nowhereman.Properties.setProperty("_VerticalFlick" + Mode, Action_VerticalFlickCurrent.code);
            nowhereman.Properties.setProperty("HorizontalFlick" + Mode, ActionHorizontalFlickCurrent.code);
            nowhereman.Properties.setProperty("_HorizontalFlick" + Mode, Action_HorizontalFlickCurrent.code);

            nowhereman.Properties.setBoolProperty("intelligence" + Mode, Intelligence);
            nowhereman.Properties.setBoolProperty("intelligencePace" + Mode, IntelligencePace);
            nowhereman.Properties.setBoolProperty("intelligenceRotation" + Mode, IntelligenceRotation);
            
            nowhereman.Properties.setIntProperty("timeAdvicesEveryMinute" + Mode, VoiceAdvicesTime.Minutes);
            nowhereman.Properties.setIntProperty("distanceAdvicesEveryMeter" + Mode, (int)Math.Round(VoiceAdvicesDistance));

            nowhereman.Properties.setDoubleProperty("GPSaccuracy" + Mode, GPSaccuracy);
            nowhereman.Properties.setDoubleProperty("GPSaccuracyMax" + Mode, GPSaccuracyMax);

            nowhereman.Properties.setBoolProperty("hasLoops" + Mode, HasLoops);
            nowhereman.Properties.setDoubleProperty("loopDistance" + Mode, LoopDistance);

            nowhereman.Properties.setBoolProperty("PauseOnObscured" + Mode, PauseOnObscured);

            nowhereman.Properties.setIntProperty("timeToPause" + Mode, TimeToDetectP);

            nowhereman.Properties.setIntProperty("timeToRun" + Mode, TimeToDetectR);

            return Task.CompletedTask;
        }
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Mode = (string)parameter;

            //    ctltitle.Text = string.Format(resourceLoader.GetString("SettingsSpec,") resourceLoader.GetString("ResourceManager.GetString(mode")));

            //             SineMediaStreamSource mss = new SineMediaStreamSource(nowhereman.Properties.getDoubleProperty("frequencyMetronom", 500.0), nowhereman.Properties.getDoubleProperty("amplitudeMetronom", 1.0), TimeSpan.FromMilliseconds(nowhereman.Properties.getIntProperty("lengthMetronom", 50)));

            PauseOnObscured = nowhereman.Properties.getBoolProperty("PauseOnObscured" + Mode, false);
            //frecuency.Value = nowhereman.Properties.getDoubleProperty("frequencyMetronom" + Mode, 500);
            //amplitude.Value = nowhereman.Properties.getDoubleProperty("amplitudeMetronom", 1.0);


            TimeGPSaccuracy = nowhereman.Properties.getBoolProperty("timeGPSaccuracy" + Mode, false);

            GPSaccuracy = nowhereman.Properties.getDoubleProperty("GPSaccuracy" + Mode, 2.0);
         
            GPSaccuracyMax = nowhereman.Properties.getDoubleProperty("GPSaccuracyMax" + Mode, 50.0);

            HasLoops = nowhereman.Properties.getBoolProperty("hasLoops" + Mode, false);
            LoopDistance = nowhereman.Properties.getDoubleProperty("loopDistance" + Mode, 1000.0);

            Intelligence = nowhereman.Properties.getBoolProperty("intelligence" + Mode, true);
            IntelligencePace = nowhereman.Properties.getBoolProperty("intelligencePace" + Mode, true);
            IntelligenceRotation = nowhereman.Properties.getBoolProperty("intelligenceRotation" + Mode, false);

            TimeToDetectP = nowhereman.Properties.getIntProperty("timeToPause" + Mode, 5);
             TimeToDetectR = nowhereman.Properties.getIntProperty("timeToRun" + Mode, 3);
 
            VoiceAdvicesTime = TimeSpan.FromMinutes(nowhereman.Properties.getIntProperty("timeAdvicesEveryMinute" + Mode, 15));

            VoiceAdvicesDistance = nowhereman.Properties.getIntProperty("distanceAdvicesEveryMeter" + Mode, 1000);

            //// DoubleTab, Tab, Hold, VerticalFlick, HorizontalFlick, -VerticalFlick, -HorizontalFlick
            List<PairCodeDesc> listActions = new List<PairCodeDesc>();
            listActions.Add(new PairCodeDesc("NONE", resourceLoader.GetString("Nothing")));
            listActions.Add(new PairCodeDesc(Constants.PAUSE, resourceLoader.GetString("PauseAction")));
            listActions.Add(new PairCodeDesc(Constants.MOVE_NEXT, resourceLoader.GetString("MoveNextAction")));
            listActions.Add(new PairCodeDesc(Constants.MOVE_PREVIOUS, resourceLoader.GetString("MovePreviousAction")));
            listActions.Add(new PairCodeDesc(Constants.STOP, resourceLoader.GetString("StopAction")));
            listActions.Add(new PairCodeDesc(Constants.DISABLE_ENABLE_ADVICES, resourceLoader.GetString("DisableEnableAdvices")));
            listActions.Add(new PairCodeDesc(Constants.DISABLE_ENABLE_MUSIC, resourceLoader.GetString("DisableEnableMusic")));
            listActions.Add(new PairCodeDesc(Constants.DISABLE_START_STOP, resourceLoader.GetString("DisableStartAndStop")));
            listActions.Add(new PairCodeDesc(Constants.ENABLE_START_STOP, resourceLoader.GetString("EnableStartAndStop")));
            listActions.Add(new PairCodeDesc(Constants.SAY_INFO, resourceLoader.GetString("SayInfo")));

            ActionOnTabList = listActions;
            ActionOnTabCurrent = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("Tab" + Mode, Constants.MOVE_NEXT), ""))];

            ActionOnDoubleTabList = listActions;
            ActionOnDoubleTabCurrent = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("DoubleTab" + Mode, Constants.PAUSE), ""))];

            ActionHoldTabList = listActions;
            ActionHoldTabCurrent = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("Hold" + Mode, "NONE"), ""))];

            ActionVerticalFlickList = listActions;
            ActionVerticalFlickCurrent = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("VerticalFlick" + Mode, Constants.MOVE_NEXT), ""))];

            Action_VerticalFlickList = listActions;
            Action_VerticalFlickCurrent = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("_VerticalFlick" + Mode, Constants.MOVE_PREVIOUS), ""))];

            ActionHorizontalFlickList = listActions;
            ActionHorizontalFlickCurrent = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("HorizontalFlick" + Mode, Constants.MOVE_NEXT), ""))];

            Action_HorizontalFlickList = listActions;
            Action_HorizontalFlickCurrent = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("_HorizontalFlick" + Mode, Constants.MOVE_PREVIOUS), ""))];

            return Task.CompletedTask;
        }
    }
}
