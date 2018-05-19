using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Media;

namespace screen_off
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            Window_Loaded();
        }

        #region Global Variables

        DispatcherTimer disTimerChecker = new DispatcherTimer();

        int MonitorStateOff = 2;
        int MonitorStateWake = -1;

        string strSleep;
        string strWake;
        int intWait;

        List<string> timeHH;
        List<string> timeMM;
        List<string> timeSS;

        string CurrentTime;

        #endregion

        #region Window Load Methods

        private void Window_Loaded()
        {
            NativeMethods monInt = new NativeMethods();
            monInt.sessionSwitch();

            load_DateTime();

            timeHH = new List<string>();
            timeMM = new List<string>();
            timeSS = new List<string>();

            for (int i = 0; i <= 24; i++)
            {
                if (i.ToString().Length == 1)
                {
                    timeHH.Add("0" + i.ToString());
                }
                else
                {
                    timeHH.Add(i.ToString());

                }
            }

            for (int i = 0; i <= 60; i++)
            {
                if (i.ToString().Length == 1)
                {
                    timeMM.Add("0" + i.ToString());
                    timeSS.Add("0" + i.ToString());
                }
                else
                {
                    timeMM.Add(i.ToString());
                    timeSS.Add(i.ToString());
                }
            }

            cbSleephh.ItemsSource = timeHH;
            cbSleepmm.ItemsSource = timeMM;
            cbSleepss.ItemsSource = timeSS;
            cbWakehh.ItemsSource = timeHH;
            cbWakemm.ItemsSource = timeMM;
            cbWakess.ItemsSource = timeSS;

        }

        #endregion

        #region Time Dispatcher

        private void load_DateTime()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CurrentTime = (DateTimeOffset.Now.ToString("HH:mm:ss"));
            lblTime.Content = ("Current Time: " + CurrentTime);
        }

        #endregion

        #region Start button click method

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (disTimerChecker.IsEnabled) //is the time checker dispatcher already running
            {
                disTimerChecker.Stop(); //if so stop the current dispatcher
                lbStatus.Items.Add(DateTime.Now + " - Time Checker now: Stopped"); //update the status box

                if (strWake != null || strSleep != null) //if a sleep or wake time is set
                {
                    if (strWake != null) //sets wake time if value is not null
                    {
                        lbStatus.Items.Add(DateTime.Now + " - Wake up time checker now: Active");
                        lblStatusWake.Content = ("Current: " + strWake);
                    }

                    if (strSleep != null) //sets sleep time if value is not null
                    {
                        lbStatus.Items.Add(DateTime.Now + " - Sleep time checker now: Active");
                        lblStatusSleep.Content = ("Current: " + strSleep);
                    }

                    wakesleepCheck(); //starts the dispatcher
                }
                else //if neither a wake or sleep time has been set
                {
                    if (intWait != 0) //if a delay time has been set
                    {
                        lbStatus.Items.Add(DateTime.Now + " - Monitor will turn off in: " + intWait.ToString());
                        NativeMethods monInt = new NativeMethods();
                        monInt.SetMonitorInState(MonitorStateOff, intWait);
                    }
                    else //if no delay time has been set
                    {
                        lbStatus.Items.Add(DateTime.Now + " - Monitor turned off.");
                        NativeMethods monInt = new NativeMethods();
                        monInt.SetMonitorInState(MonitorStateOff, intWait);
                    }
                }
            }
            else //if the dispatch timer is currently not active
            {
                if (strWake != null || strSleep != null) //if a sleep or wake time is set
                {
                    if (strWake != null) //sets wake time if value is not null
                    {
                        lbStatus.Items.Add(DateTime.Now + " - Wake up time checker now: Active");
                        lblStatusWake.Content = ("Current: " + strWake);
                    }

                    if (strSleep != null) //sets sleep time if value is not null
                    {
                        lbStatus.Items.Add(DateTime.Now + " - Sleep time checker now: Active");
                        lblStatusSleep.Content = ("Current: " + strSleep);
                    }

                    wakesleepCheck(); //starts the dispatcher
                }
                else //if neither a wake or sleep time has been set
                {
                    if (intWait != 0) //if a delay time has been set
                    {
                        lbStatus.Items.Add(DateTime.Now + " - Monitor will turn off in: " + intWait.ToString());
                        NativeMethods monInt = new NativeMethods();
                        monInt.SetMonitorInState(MonitorStateOff, intWait);
                    }
                    else //if no delay time has been set
                    {
                        lbStatus.Items.Add(DateTime.Now + " - Monitor turned off.");
                        NativeMethods monInt = new NativeMethods();
                        monInt.SetMonitorInState(MonitorStateOff, intWait);
                    }
                }
            }
        }

        #endregion

        #region stop button click method

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            disTimerChecker.Stop();
            lbStatus.Items.Add(DateTime.Now + " - Sleep/Wake time checker is now: In-active");
        }

        #endregion

        #region set and clear button click methods

        private void btnSetSleep_Click(object sender, RoutedEventArgs e)
        {
            if (cbSleephh.SelectedValue == null || cbSleepmm.SelectedValue == null || cbSleepss.SelectedValue == null)
            {
                MessageBox.Show("Please ensure that all drop down boxes contain a value.");
            }
            else if (cbSleephh.SelectedValue != null && cbSleepmm.SelectedValue != null && cbSleepss.SelectedValue != null)
            {
                strSleep = (cbSleephh.Text + ":" + cbSleepmm.Text + ":" + cbSleepss.Text);
                lblStatusSleep.Content = ("Current: " + strSleep);
                tbWait.IsReadOnly = true;
                tbWait.Background = Brushes.DarkGray;
                tbWait.ToolTip = "Disabled when sleep or wake time is set";
            }
        }

        private void btnSetWake_Click(object sender, RoutedEventArgs e)
        {
            if (cbWakehh.SelectedValue == null || cbWakemm.SelectedValue == null || cbWakess.SelectedValue == null)
            {
                MessageBox.Show("Please ensure that all drop down boxes contain a value.");
            }
            else if (cbWakehh.SelectedValue != null && cbWakemm.SelectedValue != null && cbWakess.SelectedValue != null)
            {
                strWake = (cbWakehh.Text + ":" + cbWakemm.Text + ":" + cbWakess.Text);
                lblStatusWake.Content = ("Current: " + strWake);
                tbWait.IsReadOnly = true;
                tbWait.Background = Brushes.DarkGray;
                tbWait.ToolTip = "Disabled when sleep or wake time is set";
            }
        }

        private void btnSetWait_Click(object sender, RoutedEventArgs e)
        {
            int intOut;

            if (tbWait.Text.Trim().Length != 0)
            {
                bool blWait = int.TryParse(tbWait.Text, out intOut);

                if (blWait == true)
                {
                    intWait = intOut;
                }
                if (blWait != true)
                {
                    MessageBox.Show("Please only enter a number value.");
                }

                lblStatusWait.Content = ("Current: " + intWait + " /ms");
            }
            else if (tbWait.Text.Trim().Length == 0)
            {
                MessageBox.Show("The field is currently blank, please enter a time in /ms.");
            }
        }

        private void btnClearSleep_Click(object sender, RoutedEventArgs e)
        {
            strSleep = "";
            lblStatusSleep.Content = ("Current: ");
            cbSleephh.SelectedIndex = -1;
            cbSleepmm.SelectedIndex = -1;
            cbSleepss.SelectedIndex = -1;
            tbWait.IsReadOnly = false;
            tbWait.Background = Brushes.White;
            tbWait.ToolTip = "Sets delay before monitor off is triggered";
        }

        private void btnClearWake_Click(object sender, RoutedEventArgs e)
        {
            strWake = "";
            lblStatusWake.Content = ("Current: ");
            cbWakehh.SelectedIndex = -1;
            cbWakemm.SelectedIndex = -1;
            cbWakess.SelectedIndex = -1;
            tbWait.IsReadOnly = false;
            tbWait.Background =  Brushes.White;
            tbWait.ToolTip = "Sets delay before monitor off is triggered";
        }

        private void btnClearWait_Click(object sender, RoutedEventArgs e)
        {
            intWait = 0;
            lblStatusWait.Content = ("Current: ");
            tbWait.Text = "";
        }

        #endregion

        #region Dispatcher method for checking wake/sleep times against current time

        private void wakesleepCheck()
        {
            disTimerChecker.Tick += new EventHandler(dispatcherTimer2_Tick);
            disTimerChecker.Interval = new TimeSpan(0, 0, 1);
            disTimerChecker.Start();
        }

        private void dispatcherTimer2_Tick(object sender, EventArgs e)
        {
            if (strWake == CurrentTime)
            {
                NativeMethods monInt = new NativeMethods();
                monInt.SetMonitorInState(MonitorStateWake, 0);
            }
            if (strSleep == CurrentTime)
            {
                NativeMethods monInt = new NativeMethods();
                monInt.SetMonitorInState(MonitorStateOff, 0);
            }
        }

        #endregion

    }
}
