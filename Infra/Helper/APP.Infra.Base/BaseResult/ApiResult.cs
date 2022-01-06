using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Infra.Base.BaseResult
{
   public class ApiResult
    {
        public ApiResult() { }

        public ApiResult(bool _Result, string _Message)
        {
            Result = _Result;
            Message = _Message;
        }

        public ApiResult(bool _Result, string _Message, int _HttpStatusCode)
        {
            Result = _Result;
            Message = _Message;
            HttpStatusCode = _HttpStatusCode;
        }

        public ApiResult(bool _Result, string _Message, int _HttpStatusCode, string _ExceptionCode, object _Data = null)
        {
            Result = _Result;
            Message = _Message;
            HttpStatusCode = _HttpStatusCode;
            ExceptionCode = _ExceptionCode;
            Data = _Data;
        }

        public ApiResult(bool _Result, string _Message, int _HttpStatusCode, string _ExceptionCode, object _Data = null, object _PaginationMetaData = null)
        {
            Result = _Result;
            Message = _Message;
            HttpStatusCode = _HttpStatusCode;
            ExceptionCode = _ExceptionCode;
            Data = _Data;
        }

        public bool Result { get; set; }
        public string Message { get; set; }
        public int? HttpStatusCode { get; set; }
        public string ExceptionCode { get; set; }
        public object Data { get; set; }

        public List<string> Errors { get; set; }
    }

    public class ApiResult<T> : ApiResult
    {
        public ApiResult() { }

        public ApiResult(bool _Result, string _Message, T _Data = default(T), PaginationMetaData _PaginationMetaData = default(PaginationMetaData))
        {
            Result = _Result;
            Message = _Message;
            Data = _Data;
            PaginationMetaData = _PaginationMetaData;
        }

        public ApiResult(bool _Result, string _Message, T _Data = default(T))
        {
            Result = _Result;
            Message = _Message;
            Data = _Data;
        }

        public ApiResult(bool _Result, T _Data = default(T))
        {
            Result = _Result;
            Data = _Data;
        }

        public PaginationMetaData PaginationMetaData { get; set; }

        public T Data { get; set; }
    }

    public class PaginationMetaData
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
    

