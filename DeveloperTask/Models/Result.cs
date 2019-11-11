using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public enum ResultEnum
    {
        Success = 1,
        Fail = 0,
    }

    public class Result
    {
        public ResultEnum result { get; set; }
        public object data { get; set; }
        public string message { get; set; }


        public Result()
        {
        }

        public Result(ResultEnum _result)
        {
            result = _result;
            data = null;
            message = null;
        }
        public Result(ResultEnum _result, string _message)
        {
            result = _result;
            data = null;
            message = _message;
        }
        public Result(ResultEnum _result, object _data, string _message)
        {
            result = _result;
            data = _data;
            message = _message;
        }
    }
}