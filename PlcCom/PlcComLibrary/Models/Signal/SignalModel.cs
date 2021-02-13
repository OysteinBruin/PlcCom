using PlcComLibrary.Common;

namespace PlcComLibrary.Models.Signal
{
    public abstract class SignalModel
    {
        protected SignalModel(ISignalModelContext ctx)
        {
            Index = ctx.Index;
            Name = ctx.Name;
            Address = ctx.Address;
            Db = ctx.DbNumber;
            Description = ctx.Description;
        }
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