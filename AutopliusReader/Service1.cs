using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AutopliusReader.Services;

namespace AutopliusReader
{
    public partial class Service1 : ServiceBase
    {
        private bool _stateRunning;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (!_stateRunning)
            {
                Debug.WriteLine("OnStart()");
                _stateRunning = true;

                try
                {
                    ServiceStart();
                    Debug.WriteLine("Task Started");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("OnStart - Exception: {0}", e.GetType().Name);
                }
                finally
                {
                    Debug.WriteLine("OnStart - finally");
                }
            }
        }

        protected override void OnStop()
        {
            Debug.WriteLine("OnStop()");

            if (_stateRunning)
            {
                try
                {
                    ServiceStop();

                    Debug.WriteLine("Task stopped");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("OnStop - Exception: {0}", e.GetType().Name);
                }
                finally
                {
                    Debug.WriteLine("OnStop - finally");
                }
            }
        }


        public void ServiceStart()
        {

            Timer t = new Timer(5000); // 1 sec = 1000, 60 sec = 60000
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Start();
           
        }

        private static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           Class1. CheckCars();
        }

        public void ServiceStop()
        {
           
        }
    }
}
