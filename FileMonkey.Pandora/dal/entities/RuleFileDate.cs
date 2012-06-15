using System;

namespace FileMonkey.Pandora.dal.entities
{
    public class RuleFileDate : RuleFile
    {
        #region TypeRuleFilaDate enum

        public enum TypeRuleFilaDate
        {
            Period,
            Dfirst,
            AfterDfirst,
            BeforeDfirst,
            Dlast,
            AfterDlast,
            BeforeDlast
        }

        #endregion

        public DateTime DateFirst { get; set; }
        public DateTime DateLast { get; set; }
    }
}