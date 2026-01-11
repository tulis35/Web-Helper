using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHelper.Models.Enums;

namespace WebHelper.Models
{
    public class Result
    {
        public ResultState ResultState { get; private set; }
        public string Message { get; private set; }
        public string Class { get; private set; }
        public string Icon { get; private set; }

        public Result(ResultState resultState, string msg)
        {
            ResultState = resultState;
            Message = msg;

            switch (ResultState)
            {
                case ResultState.Success:
                    Class = "result-success";
                    Icon = "icon-success";
                    break;
                case ResultState.Error:
                    Class = "result-error";
                    Icon = "icon-error";
                    break;
                case ResultState.Info:
                    Class = "result-info";
                    Icon = "icon-info";
                    break;
            }
        }
    }
}

