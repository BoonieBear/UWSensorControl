﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.ViewModel;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.Structure;
using USBLDC.Helpers;
using System.Windows.Controls;

namespace USBLDC.ViewModel
{
    public class HomePageViewModel : ViewModelBase, IHandleMessage<ShowStructureInfo>
    {
        public override void Initialize()
        {
            Heading = 45;
            Pitch = -30;
            Roll = 30;
            Heave = 100;
            StartCMD = RegisterCommand(ExecuteStartCMD, CanExecuteStartCMD, true);
            StopCMD = RegisterCommand(ExecuteStopCMD, CanExecuteStopCMD, true);
            HeadingChartTitle = "艏向角=" + Heading.ToString("F02") + "°";
            PRHTtitle = "纵倾=" + Pitch.ToString("F02") + "°" + " " + "横摇=" + Roll.ToString("F02") + "°" + " " + "升沉=" +
                        Heave.ToString("F02") + "m";
            ShowCmd = true;
            CurrentModel = null;
        }

        public override void InitializePage(object extraData)
        {
            if (CurrentModel == null && UnitCore.Instance.CurrentModel != null)
                CurrentModel = UnitCore.Instance.CurrentModel;
            var pos = new AjustPositionInfo();
            pos.Status = 1;
            pos.XAjust = 1000;
            pos.YAjust = 3040;
            pos.ZAjust = 2300;
            pos.Noise = 71;
            UpdatePositionView(new ShowStructureInfo(pos,(int)TypeId.AjustPos));
            
        }
        public uint SonarStatus
        {
            get { return GetPropertyValue(() => SonarStatus); }
            set { SetPropertyValue(() => SonarStatus, value); }
        }

        public float coordinateX
        {
            get { return GetPropertyValue(() => coordinateX); }
            set { SetPropertyValue(() => coordinateX, value); }
        }

        public float coordinateY
        {
            get { return GetPropertyValue(() => coordinateY); }
            set { SetPropertyValue(() => coordinateY, value); }
        }
        
        public float coordinateZ
        {
            get { return GetPropertyValue(() => coordinateZ); }
            set { SetPropertyValue(() => coordinateZ, value); }
        }

        public float targetLong
        {
            get { return GetPropertyValue(() => targetLong); }
            set { SetPropertyValue(() => targetLong, value); }
        }

        public float targetLat
        {
            get { return GetPropertyValue(() => targetLat); }
            set { SetPropertyValue(() => targetLat, value); }
        }

        public float Noise
        {
            get { return GetPropertyValue(() => Noise); }
            set { SetPropertyValue(() => Noise, value); }
        }

        public string gpsTime
        {
            get { return GetPropertyValue(() => gpsTime); }
            set { SetPropertyValue(() => gpsTime, value); }
        }

        public float shipLong
        {
            get { return GetPropertyValue(() => shipLong); }
            set { SetPropertyValue(() => shipLong, value); }
        }

        public float shipLat
        {
            get { return GetPropertyValue(() => shipLat); }
            set { SetPropertyValue(() => shipLat, value); }
        }

        public uint Satelites
        {
            get { return GetPropertyValue(() => Satelites); }
            set { SetPropertyValue(() => Satelites, value); }
        }

        public uint gpsSpeed
        {
            get { return GetPropertyValue(() => gpsSpeed); }
            set { SetPropertyValue(() => gpsSpeed, value); }
        }

        public uint gpsStatus
        {
            get { return GetPropertyValue(() => gpsStatus); }
            set { SetPropertyValue(() => gpsStatus, value); }
        }

        public float Heading
        {
            get { return GetPropertyValue(() => Heading); }
            set { SetPropertyValue(() => Heading, value); }
        }

        public float Pitch
        {
            get { return GetPropertyValue(() => Pitch); }
            set { SetPropertyValue(() => Pitch, value); }
        }

        public float Roll
        {
            get { return GetPropertyValue(() => Roll); }
            set { SetPropertyValue(() => Roll, value); }
        }

        public float Heave
        {
            get { return GetPropertyValue(() => Heave); }
            set { SetPropertyValue(() => Heave, value); }
        }
        private uint poseStatus = 0;
        
        public Model3D CurrentModel
        {
            get { return GetPropertyValue(() => CurrentModel); }
            set { SetPropertyValue(() => CurrentModel, value); }
        }
        public string HeadingChartTitle
        {
            get { return GetPropertyValue(() => HeadingChartTitle); }
            set { SetPropertyValue(() => HeadingChartTitle, value); }
        }

        public string ObjectCenter
        {
            get { return GetPropertyValue(() => ObjectCenter); }
            set { SetPropertyValue(() => ObjectCenter, value); }
        }
        public uint PoseStatus
        {
            get { return GetPropertyValue(() => PoseStatus); }
            set { SetPropertyValue(() => PoseStatus, value); }
        }
        public string PoseTime
        {
            get { return GetPropertyValue(() => PoseTime); }
            set { SetPropertyValue(() => PoseTime, value); }
        }
        public string xyzTtitle
        {
            get { return GetPropertyValue(() => xyzTtitle); }
            set { SetPropertyValue(() => xyzTtitle, value); }
        }
        public string PRHTtitle
        {
            get { return GetPropertyValue(() => PRHTtitle); }
            set { SetPropertyValue(() => PRHTtitle, value); }
        }

        public bool ShowCmd
        {
            get { return GetPropertyValue(() => ShowCmd); }
            set { SetPropertyValue(() => ShowCmd, value); }
        }
        public ICommand StartCMD
        {
            get { return GetPropertyValue(() => StartCMD); }
            set { SetPropertyValue(() => StartCMD, value); }
        }
        private void CanExecuteStartCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteStartCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            
            if(!UnitCore.Instance.NetCore.IsWorking)
                return;
            Task<bool> ret;
            UnitCore.Instance.SonarConfiguration.Cmd = 1;
            ret =  UnitCore.Instance.NetCore.SendCMD(UnitCore.Instance.SonarConfiguration.SavePakage());
            await ret;
            if (ret.Result == false)
            {
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "关闭";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "开始工作",
                    UnitCore.Instance.NetCore.Error, MessageDialogStyle.Affirmative, md);
            }
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "开始工作";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "命令发送成功！";

                await TaskEx.Delay(2000);
                ShowCmd = false;
                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            }
        }
        public ICommand StopCMD
        {
            get { return GetPropertyValue(() => StopCMD); }
            set { SetPropertyValue(() => StopCMD, value); }
        }

        

        private void CanExecuteStopCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteStopCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if(!UnitCore.Instance.NetCore.IsWorking || ShowCmd)
                return;
            Task<bool> ret;
            UnitCore.Instance.SonarConfiguration.Cmd = 0;
            ret = UnitCore.Instance.NetCore.SendCMD(UnitCore.Instance.SonarConfiguration.SavePakage());
            await ret;
            if (ret.Result == false)
            {
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "关闭";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "停止工作",
                    UnitCore.Instance.NetCore.Error, MessageDialogStyle.Affirmative, md);
            }
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "停止工作";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "命令发送成功！";

                await TaskEx.Delay(2000);
                ShowCmd = true;
                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            }
        }
        public void Handle(ShowStructureInfo message)
        {
            switch (message.Id)
            {
                case (int)TypeId.GPS:
                    UpdateGpsView(message);
                    break;
                case (int)TypeId.Pose:
                    UpdatePoseView(message);
                    break;
                case (int)TypeId.AjustPos:
                    UpdatePositionView(message);
                    break;

                default:
                    break;
            }
        }

        private void UpdatePoseView(ShowStructureInfo message)
        {
            var info = message.Info as PosetureInfo;
            //PoseTime = info.EpochSecond;
            Heading = info.Heading;
            Pitch = info.Pitch;
            Roll = -info.Roll;//坐标轴x相反，反方向旋转
            Heave = info.Heave;
            PoseStatus = info.Status;
            HeadingChartTitle = "艏向角=" + Heading.ToString("F02") + "°";
            PRHTtitle = "纵倾=" + Pitch.ToString("F02") + "°" + " " + "横摇=" + Roll.ToString("F02") + "°" + " " + "升沉=" +
                        Heave.ToString("F02") + "m";
        }

        private void UpdatePositionView(ShowStructureInfo message)
        {
            var info = message.Info as AjustPositionInfo;
            coordinateX = info.XAjust;
            coordinateY = info.YAjust;
            coordinateZ = info.ZAjust;
            SonarStatus = info.Status;
            targetLat = info.AjustLat;
            targetLong = info.AjustLong;
            Noise = info.Noise;
            var x = -coordinateX;//坐标轴x相反，取反,5是画图系数
            var y = -coordinateY;//坐标轴y相反，取反
            var z = -coordinateY;//坐标轴z相反，取反
            ObjectCenter = x.ToString("F02") +","+ y.ToString("F02")+"," + z.ToString("F02");
            xyzTtitle = "X = "+coordinateX.ToString("F02")+" Y = "+coordinateY.ToString("F02")+" Z = "+
            coordinateZ.ToString("F02");
        }

        private void UpdateGpsView(ShowStructureInfo message)
        {
            throw new NotImplementedException();
        }
    }
}
