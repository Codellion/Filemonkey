using System;
using System.ComponentModel.DataAnnotations;

namespace FileMonkey.Pandora.dal.entities
{
    public abstract class RuleFile
    {
        public enum TypeFileRule
        {
            Date,
            Extension,
            FileName
        }

        public int RuleFileId { get; set; }
        public String Name { get; set; }
        public int RuleType { get; set; }

        public virtual Inspector Inspector { get; set; }

        [NotMapped]
        public String ImagePath { get; set; }
    }
}