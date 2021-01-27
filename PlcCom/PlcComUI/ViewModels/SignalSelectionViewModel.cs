using Caliburn.Micro;
using PlcComUI.EventModels;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Windows.Input;

namespace PlcComUI.ViewModels
{
    public class SignalSelectionViewModel : Screen//, IDragSource
    {
        private IEventAggregator _events;
        private List<CpuDisplayModel> _cpuList;
        private List<SignalDisplayModel> _signals;
        private bool _escapeTreeViewEventPropagation = false;

        public SignalSelectionViewModel(IEventAggregator events)
        {
            _events = events;
        }

        public event EventHandler DatablockSelected;
        public event EventHandler SignalSelected;
        public event EventHandler GraphViewSelected;

        public void CreateMultiGraphView() 
        {
            GraphViewSelected?.Invoke(this, 
                new OpenGraphViewEvent(OpenGraphViewEvent.GraphViewType.MultiGraph));
        }

        public void CreateSingleGraphsView() 
        {
            GraphViewSelected?.Invoke(this, 
                new OpenGraphViewEvent(OpenGraphViewEvent.GraphViewType.SingleGraphs));
        }

        public List<CpuDisplayModel> CpuList
        {
            get => _cpuList;
            set
            {
                _cpuList = value;
                if (_cpuList != null && _cpuList.Count > 0 && _cpuList[0].Datablocks.Count > 0)
                {
                    Signals = _cpuList[0].Datablocks[0].Signals;
                }
                
                NotifyOfPropertyChange(() => CpuList);
            }
        }

        public List<SignalDisplayModel> Signals
        { 
            get => _signals; 
            set
            {
                _signals = value;
                NotifyOfPropertyChange(() => Signals);
            }
        }

        public void TreeViewItemDoubleClicked(object treeViewItemObj, MouseEventArgs e)
        {
            if (treeViewItemObj is SignalDisplayModel sdm)
            {
                _escapeTreeViewEventPropagation = true;
                e.Handled = true;
                SignalSelected?.Invoke(this, new SignalSelectedEvent(sdm));
            }

            if (treeViewItemObj is DatablockDisplayModel ddm && !_escapeTreeViewEventPropagation)
            {
                e.Handled = true;
                DatablockSelected?.Invoke(this, new NewDatablockTabEvent(ddm));
            }

            if (treeViewItemObj is CpuDisplayModel cdm)
            {
                _escapeTreeViewEventPropagation = false;
            }
        }

        //public void TreeViewRootDoubleClicked(object treeViewItemObj, MouseEventArgs e)
        //{
        //    Console.WriteLine("TreeViewRootDoubleClicked");
        //    e.Handled = true;
        //}

        //public void SignalDoubleClicked(object treeViewItemObj, MouseEventArgs e)
        //{
        //    Console.WriteLine("SignalDoubleClicked");
        //    e.Handled = true;
        //}

        //public void DatablockDoubleClicked(object treeViewItemObj, MouseEventArgs e)
        //{
        //    Console.WriteLine("DatablockDoubleClicked");
        //    e.Handled = true;
        //}


        //public void StartDrag(IDragInfo dragInfo)
        //{
        //    Console.WriteLine("StartDrag");
        //}

        //public bool CanStartDrag(IDragInfo dragInfo)
        //{
        //    Console.WriteLine("CanStartDrag");
        //    return true;
        //}

        //public void Dropped(IDropInfo dropInfo)
        //{

        //}

        //public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        //{

        //}

        //public void DragCancelled()
        //{

        //}

        //public bool TryCatchOccurredException(Exception exception)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
