using System;
using System.Collections.Generic;
using FileMonkey.Pandora.dal.utils;
using System.ComponentModel.DataAnnotations;
using Pandora.dpl;

namespace FileMonkey.Pandora.dal.entities
{
    public class Inspector
    {
        public enum TypeActions
        {
            MoveSubDir,
            DeleteFiles
        }

        public int InspectorId { get; set; }
        public String Name { get; set; }
        public String Path { get; set; }
        public int CheckPeriod { get; set; }
        public Boolean Enable { get; set; }
        public int Action { get; set; }
        public String SubDirAction { get; set; }

        public virtual ICollection<RuleFile> Rules { get; set; }

        public Inspector()
        {
            Enable = true;

            if (Rules == null)
            {
                Rules = new HashSet<RuleFile>();
            }
        }

        [NotMapped]
        public String CheckPeriodText { get; set; }
        [NotMapped]
        public String ImageEnable { get; set; }
        [NotMapped]
        public IList<InspectorHelper> RulesAux { get; set; }
    }
}
