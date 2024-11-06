using System;
using System.Collections.Generic;
using In.ProjectEKA.OtpService.Common;

namespace In.ProjectEKA.OtpService.Otp
{
    public class OtpGenerationDetail
    {
        private static Dictionary<Action, String> templateIDs;
        static OtpGenerationDetail()
        {
            templateIDs = new Dictionary<Action, string>();
            templateIDs.Add(Action.LINK_PATIENT_CARECONTEXT, "1207173028863410217");
            templateIDs.Add(Action.FORGOT_PIN, "1207161856899774071");
        }
        public string SystemName { get; set; }
        public Action Action { get; set; }

        public OtpGenerationDetail(string systemName, string action)
        {
            SystemName = systemName;
            Action = EnumUtil.ParseEnum<Action>(action);
        }
        
        public String GetTemplateID()
        {
            return templateIDs[Action];
        }
    }
}