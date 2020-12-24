using DBRXFSample.Interfaces;
using System;
using System.Timers;
using Xamarin.Forms;

namespace DBRXFSample.ViewModels
{
    class CaptureViewModel : ViewModelBase
    {
        public CaptureViewModel() 
        {

        }
        /// <summary>
        /// Begins a Capture Sequence, preparing the UI and the Timer.
        /// </summary>
        public void StartCaptureSequence()
        {
            CaptureHandler = App.CurrentCaptureUI;
            Timer t = new Timer(100);
            t.Elapsed += new ElapsedEventHandler(Timer_Tick);
            t.AutoReset = true;
            t.Enabled = true;
        }
        public void Timer_Tick(object source, ElapsedEventArgs e)
        {
            Instruction = CaptureHandler.GetResults();
        }

        /// <summary>
        /// The current Instruction to the User.
        /// </summary>
        public string Instruction
        {
            get { return _Instruction; }
            set
            {
                _Instruction = value;
                UpdateProperty();
            }
        }
        /// <summary>
        /// Backing Field for <see cref="Instruction"/>.
        /// </summary>
        private string _Instruction;

        private ICaptureUI CaptureHandler { get; set; }
    }
}
