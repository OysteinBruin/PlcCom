using PlcComLibrary.Common;

namespace PlcComLibrary.Models.Signal
{
    public abstract class SignalModel
    {
        protected SignalModel()
        {

        }
        protected SignalModel(SignalModelContext ctx)
        {
            Index = ctx.Index;
            DbIndex = ctx.DbIndex;
            CpuIndex = ctx.CpuIndex;
            Name = ctx.Name;
            Address = ctx.Address;
            Description = ctx.Description;
            DataTypeStr = ctx.DataTypeStr;
            Db = ctx.DbNumber;
            
        }
        public int CpuIndex { get; set; }
        public int DbIndex { get; set; }
        public int Index { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int Db { get; set; }
        public object Value { get; set; }
        public string DataTypeStr { get; set; }
        public abstract int ByteCount { get; }
        public abstract object BytesToValue(byte[] bytes);
    }
}