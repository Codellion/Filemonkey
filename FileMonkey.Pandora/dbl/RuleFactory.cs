
using System;
using FileMonkey.Pandora.dal.entities;

namespace Pandora.dbl
{
    public class RuleFactory
    {
        public static RuleFile GetFileRule(RuleFile.TypeFileRule type)
        {
            RuleFile res = null;

            switch(type)
            {
                case RuleFile.TypeFileRule.Date:
                    res = new RuleFileDate();
                    break;
                case RuleFile.TypeFileRule.Extension:
                    res = new RuleFileExtension();
                    break;
                case RuleFile.TypeFileRule.FileName:
                    res = new RuleFileName();
                    break;
            }

            if(res != null)
            {
                res.RuleType = (int) type;
            }

            return res;
        }
    }
}
