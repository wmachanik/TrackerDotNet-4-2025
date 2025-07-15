// Decompiled with JetBrains decompiler
// Type: TrackerDotNet.control.ReoccuringOrderExtData
// Assembly: TrackerDotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2B5ACBFB-45EE-46B9-81D2-DBD1194F39CE
// Assembly location: C:\SRC\Apps\qtracker\bin\TrackerDotNet.dll

//- only form later versions #nullable disable
namespace TrackerDotNet.Controls
{
    public class ReoccuringOrderExtData : ReoccuringOrderTbl
    {
        private string _CompanyName;
        private string _ItemTypeDesc;
        private string _ReoccuranceTypeDesc;

        public ReoccuringOrderExtData()
        {
            this._CompanyName = this._ItemTypeDesc = this._ReoccuranceTypeDesc = string.Empty;
        }

        public string CompanyName
        {
            get => this._CompanyName;
            set => this._CompanyName = value;
        }

        public string ItemTypeDesc
        {
            get => this._ItemTypeDesc;
            set => this._ItemTypeDesc = value;
        }

        public string ReoccuranceTypeDesc
        {
            get => this._ReoccuranceTypeDesc;
            set => this._ReoccuranceTypeDesc = value;
        }
    }
}