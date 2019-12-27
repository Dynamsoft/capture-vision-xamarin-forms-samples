
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DBRXFSample.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Updates a UI Property.
        /// </summary>
        protected void UpdateProperty([CallerMemberName]string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /// <summary>
        /// The Property Changed Event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
